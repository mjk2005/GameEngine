using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요합니다.

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // --- 이동 관련 변수들 ---
    public float baseMoveSpeed = 5.0f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 12.0f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    // --- 점수 및 아이템 관련 변수들 ---
    public GameObject floatingScorePrefab; // 생성할 플로팅 스코어 프리팹을 연결할 변수
    public int scorePerCherry = 1;        // 체리 하나당 오를 점수
    private int currentScore = 0;          // 현재 점수를 저장할 변수

    // --- 컴포넌트 및 상태 변수들 ---
    private Rigidbody2D rb;
    private Animator animator;
    private bool isCrouching = false;
    private bool isGrounded = false;
    private float moveInput;

    void Start()
    {
        // 컴포넌트 가져오기
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // 컴포넌트 null 체크
        if (animator == null) Debug.LogError("Animator 컴포넌트가 없습니다!");
        if (rb == null) Debug.LogError("Rigidbody2D 컴포넌트가 없습니다!");
        if (groundCheck == null) Debug.LogError("GroundCheck 오브젝트가 할당되지 않았습니다!");
        if (floatingScorePrefab == null) Debug.LogError("Floating Score Prefab이 할당되지 않았습니다!");

        // 점수 초기화
        currentScore = 0;
    }

    void Update()
    {
        // 입력 처리
        isCrouching = Input.GetKey(KeyCode.S);
        moveInput = Input.GetAxis("Horizontal");

        // 점프 입력
        if (!isCrouching && Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            Jump();
        }

        // 애니메이션 및 방향 전환 처리
        UpdateAnimation();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        // 땅 감지
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 이동 처리
        if (!isCrouching)
        {
            HandleMovement();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // 아이템과 부딪혔을 때 호출되는 함수
private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Cherry"))
    {
        currentScore += scorePerCherry;
        Debug.Log("Current Score: " + currentScore);

        if (floatingScorePrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
            
            // 1. 프리팹을 생성하고, 생성된 오브젝트를 변수에 저장합니다.
            GameObject scoreObject = Instantiate(floatingScorePrefab, spawnPosition, Quaternion.identity);
            
            // 2. 생성된 오브젝트에서 FloatingScore 스크립트를 가져옵니다.
            FloatingScore floatingScore = scoreObject.GetComponent<FloatingScore>();
            
            // 3. 스크립트의 SetText 함수를 호출해서 실제 점수를 전달합니다!
            if (floatingScore != null)
            {
                floatingScore.SetText("+" + scorePerCherry); // "+10" 과 같은 형태로 표시
            }
        }

        Destroy(other.gameObject);
    }
}

    // --- 이동 및 애니메이션 관련 함수들 ---
    private void HandleMovement()
    {
        float currentMoveSpeed = baseMoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMoveSpeed *= sprintMultiplier;
        }
        rb.linearVelocity = new Vector2(moveInput * currentMoveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void UpdateAnimation()
    {
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsJumping", !isGrounded);

        if (isGrounded && !isCrouching)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void FlipSprite()
    {
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동 및 점프 설정 변수들
    public float baseMoveSpeed = 5.0f;
    public float sprintMultiplier = 1.5f;
    public float jumpHeight = 1.7f;
    public float jumpDuration = 0.6f;

    private Animator animator;

    // 상태 관리 변수
    private bool isJumping = false;
    private bool isCrouching = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 없습니다! 플레이어 오브젝트에 Animator를 추가해주세요.");
        }
    }

    void Update()
    {
        // 1. 웅크리기 상태 감지
        isCrouching = Input.GetKey(KeyCode.S);
        animator.SetBool("IsCrouching", isCrouching);

        // 웅크리기가 아닐 때만 이동과 점프 허용
        if (!isCrouching)
        {
            // 2. 스프린트 속도 계산
            float currentMoveSpeed = baseMoveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentMoveSpeed *= sprintMultiplier;
            }

            // 3. 좌우 이동 및 애니메이션 처리
            float moveInput = Input.GetAxis("Horizontal");
            Vector3 movement = new Vector3(moveInput, 0, 0);

            transform.Translate(movement * currentMoveSpeed * Time.deltaTime);

            // 점프 중에는 Speed를 0으로 고정하여 Run 모션이 재생되지 않게 합니다.
            if (!isJumping)
            {
                animator.SetFloat("Speed", Mathf.Abs(moveInput * currentMoveSpeed));
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }
            
            // 스프라이트 방향 뒤집기
            if (moveInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveInput < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            // 4. 점프
            if (Input.GetKeyDown(KeyCode.W) && !isJumping)
            {
                StartCoroutine(JumpRoutine());
            }
        }
    }

    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        animator.SetBool("IsJumping", true);

        Vector3 startPos = transform.position;
        Vector3 peakPos = startPos + new Vector3(0, jumpHeight, 0);

        float upTime = 0;
        while (upTime < jumpDuration / 2)
        {
            float newY = Mathf.Lerp(startPos.y, peakPos.y, upTime / (jumpDuration / 2));
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            upTime += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, peakPos.y, transform.position.z);

        float downTime = 0;
        while (downTime < jumpDuration / 2)
        {
            float newY = Mathf.Lerp(peakPos.y, startPos.y, downTime / (jumpDuration / 2));
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            downTime += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, startPos.y, startPos.z);

        isJumping = false;
        animator.SetBool("IsJumping", false);
    }
}
using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float lifeTime = 1.0f;

    private TextMeshProUGUI textMesh;
    private Color startColor;
    private float timeElapsed = 0.0f;

    // ★★★ 이 Awake 함수가 카메라를 자동으로 찾고, 텍스트 컴포넌트를 미리 준비합니다. ★★★
    void Awake()
    {
        // 캔버스 카메라 자동 설정
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
        
        // 텍스트 컴포넌트 찾아두기
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime); // 생명 시간 후 파괴
        if (textMesh != null)
        {
            startColor = textMesh.color;
        }
    }
    
    // ★★★ 다른 스크립트에서 호출할 수 있도록 점수 텍스트를 설정하는 함수 추가 ★★★
    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }

    void Update()
    {
        // 위로 이동 및 페이드 아웃
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;
        if (textMesh != null)
        {
            float alpha = 1.0f - (timeElapsed / lifeTime);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }
    }
}
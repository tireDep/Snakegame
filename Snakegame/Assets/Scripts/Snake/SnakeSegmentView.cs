using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SnakeSegmentView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;           // 세그먼트 스프라이트 렌더러
    private bool hasValidReferences;    // 필수 컴포넌트 검증 완료 여부
    public SpriteRenderer SpriteRenderer => spriteRenderer;    // 세그먼트 스프라이트 렌더러
    
    // 세그먼트 렌더러와 필수 컴포넌트를 준비하는 함수
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hasValidReferences = ValidateReferences();
    }

    // 세그먼트 표시에 필요한 컴포넌트를 검증하는 함수
    private bool ValidateReferences()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SnakeSegmentView::ValidateReferences SpriteRenderer is required.", this);
            return false;
        }

        return true;
    }

    // 세그먼트의 월드 위치를 설정하는 함수
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    // 세그먼트에 표시할 스프라이트를 설정하는 함수
    public void SetSprite(Sprite sprite)
    {
        if (!hasValidReferences || sprite == null)
        {
            return;   
        }
        
        spriteRenderer.sprite = sprite;
    }

    // 세그먼트의 회전 각도를 설정하는 함수
    public void SetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }
}

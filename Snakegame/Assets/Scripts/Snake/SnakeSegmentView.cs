using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SnakeSegmentView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool hasValidReferences;    // 필수 컴포넌트 검증 완료 여부
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    
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

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetSprite(Sprite sprite)
    {
        if (!hasValidReferences || sprite == null)
        {
            return;   
        }
        
        spriteRenderer.sprite = sprite;
    }

    public void SetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }
}

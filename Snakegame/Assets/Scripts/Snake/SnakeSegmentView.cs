using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SnakeSegmentView : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetSprite(Sprite sprite)
    {
        if (sprite == null || spriteRenderer == null)
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

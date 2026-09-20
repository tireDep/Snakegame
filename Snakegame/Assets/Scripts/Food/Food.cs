using UnityEngine;

public class Food : MonoBehaviour
{
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Food::Awake Animator not found!");
            return;
        }   
    }
    
    public void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    public void SetShow(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    public void SetPlayAnim(bool isPlay)
    {
        if (animator == null)
        {
            return;
        }
        
        animator.speed = isPlay ? 1.0f : 0.0f; 
    }
}

using UnityEngine;

public class Food : MonoBehaviour
{
    private Animator animator;
    private bool isInitialized;    // 필수 컴포넌트 검증 완료 여부
    public bool IsInitialized => isInitialized;    // 음식 표시 준비 완료 여부
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        isInitialized = ValidateReferences();
    }

    // 음식 표시에 필요한 컴포넌트를 검증하는 함수
    private bool ValidateReferences()
    {
        if (animator == null)
        {
            Debug.LogError("Food::ValidateReferences Animator is required.", this);
            return false;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Food::ValidateReferences AnimatorController is required.", this);
            return false;
        }

        return true;
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
        if (!isInitialized)
        {
            return;
        }
        
        animator.speed = isPlay ? 1.0f : 0.0f; 
    }
}

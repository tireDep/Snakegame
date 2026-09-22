using UnityEngine;

public class Food : MonoBehaviour
{
    private Animator animator;                  // 음식 애니메이터
    private bool hasValidReferences;    // 필수 컴포넌트 검증 완료 여부
    public bool IsInitialized => hasValidReferences;    // 음식 표시 준비 완료 여부
    
    // 음식 애니메이터와 필수 컴포넌트를 준비하는 함수
    private void Awake()
    {
        animator = GetComponent<Animator>();
        hasValidReferences = ValidateReferences();
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
    
    // 음식의 월드 위치를 설정하는 함수
    public void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    // 음식 오브젝트의 표시 여부를 설정하는 함수
    public void SetShow(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    // 음식 애니메이션의 재생 여부를 설정하는 함수
    public void SetPlayAnim(bool isPlay)
    {
        if (!hasValidReferences)
        {
            return;
        }
        
        animator.speed = isPlay ? 1.0f : 0.0f; 
    }
}

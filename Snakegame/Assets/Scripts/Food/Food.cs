using UnityEngine;

public class Food : MonoBehaviour
{
    public void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    public void SetShow(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}

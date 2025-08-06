using UnityEngine;

public class Double : MonoBehaviour
{
    public Animator animator;
    public bool isDoubleActivated = false;

    public void Activate()
    {
        if (!isDoubleActivated)
        {
            gameObject.SetActive(true);
            animator.Rebind();
            animator.Update(0f);
            animator.SetBool("Double", true);
            isDoubleActivated = true;
        }
    }

    public void OnAnimationEnd()
    {
        Debug.Log("애니메이션 끝났어요!");
        animator.SetBool("Double", false);
        gameObject.SetActive(false);
        isDoubleActivated = false; // 다시 활성화 가능하도록
    }
}

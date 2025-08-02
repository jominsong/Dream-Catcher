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
        animator.SetBool("Double", false);
        gameObject.SetActive(false);
        isDoubleActivated = false; 
    }
}

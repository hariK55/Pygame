using UnityEngine;

public class PyAnimation : MonoBehaviour
{
    public static PyAnimation Instance { get; private set; }

    private Animator pyAnimator;

    private void Awake()
    {
        Instance = this;
        pyAnimator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
      
        pyAnimator.SetBool("slowSlither", PyMovement.Instance.IsSlithering());
        pyAnimator.SetBool("fastSlither", PyMovement.Instance.IsFast());
       
    }

    public void Attack()
    {
        
        pyAnimator.Play("attack");
    }
    public void Jump()
    {
        
        pyAnimator.Play("jmp");
    }

}

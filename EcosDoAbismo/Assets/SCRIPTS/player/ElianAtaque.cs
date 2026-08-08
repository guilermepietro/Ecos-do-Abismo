using UnityEngine;

public class ElianAtaque : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && animator.GetBool("estaNoChao"))
            {
            animator.SetTrigger("Atacar");
            }
    }
}
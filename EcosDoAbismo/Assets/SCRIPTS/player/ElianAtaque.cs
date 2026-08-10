using UnityEngine;

public class ElianAtaque : MonoBehaviour
{
    private Animator animator;

    private bool atacando = false;
    private bool comboAtaque2 = false;
    private bool comboAtaque3 = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && animator.GetBool("estaNoChao"))
        {
            if (!atacando)
            {
                animator.ResetTrigger("Ataque2");
                animator.ResetTrigger("Ataque3");

                animator.SetTrigger("Atacar");
                atacando = true;
            }
            else if (!comboAtaque2)
            {
                comboAtaque2 = true;
                animator.SetTrigger("Ataque2");
            }
            else if (!comboAtaque3)
            {
                comboAtaque3 = true;
                animator.SetTrigger("Ataque3");
            }
        }
    }

    public void FinalizarAtaque1()
    {
        if (!comboAtaque2)
        {
            atacando = false;
        }
    }

    public void FinalizarAtaque2()
    {
        if (!comboAtaque3)
        {
            atacando = false;
            comboAtaque2 = false;
        }
    }

    public void FinalizarAtaque3()
    {
        atacando = false;
        comboAtaque2 = false;
        comboAtaque3 = false;

        animator.ResetTrigger("Ataque2");
        animator.ResetTrigger("Ataque3");
    }
}
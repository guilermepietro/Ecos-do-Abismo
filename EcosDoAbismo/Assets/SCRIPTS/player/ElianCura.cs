using UnityEngine;

public class ElianCura : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && animator.GetBool("estaNoChao"))
{
    movimento.podeMover = false;
    animator.SetFloat("velocidade", 0);
    animator.SetTrigger("Cura");
}
    }

    public void ExecutarCura()
    {
        GetComponent<ElianVida>().Curar(30);
    }

    public void FinalizarCura()
    {
        movimento.podeMover = true;
    }
}
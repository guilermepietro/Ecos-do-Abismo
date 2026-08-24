using UnityEngine;

public class SalenthraVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaximaFase1 = 100;
    private int vidaAtualFase1;

    [Header("Referências")]
    public Animator animator;

    private SalenthraMovimento movimento;
    private bool transformando = false;

    private void Awake()
    {
        vidaAtualFase1 = vidaMaximaFase1;
        movimento = GetComponent<SalenthraMovimento>();
    }

    public void ReceberDano(int dano)
    {
        if (transformando)
            return;

        vidaAtualFase1 -= dano;

        if (vidaAtualFase1 <= 0)
        {
            vidaAtualFase1 = 0;
            IniciarTransformacao();
            return;
        }

        if (movimento != null)
        {
            movimento.podeMover = false;
        }

        animator.SetTrigger("Dano");
    }

    private void IniciarTransformacao()
    {
        transformando = true;

        if (movimento != null)
        {
            movimento.podeMover = false;
        }

        animator.SetBool("Andando", false);
        animator.SetTrigger("Transformar");
    }

    public void FinalizarDano()
    {
        if (transformando)
            return;

        if (movimento != null)
        {
            movimento.podeMover = true;
        }
    }
}
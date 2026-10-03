using UnityEngine;

public class SalenthraVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaximaFase1 = 100;
    public int vidaMaximaFase2 = 150;

    private int vidaAtualFase1;
    private int vidaAtualFase2;

    [Header("Referências")]
    public Animator animator;

    private SalenthraMovimento movimento;

    private int faseAtual = 1;
    private bool transformando = false;
    private bool morta = false;

    public bool EstaMorta
    {
        get { return morta; }
    }

    private void Awake()
    {
        vidaAtualFase1 = vidaMaximaFase1;
        vidaAtualFase2 = vidaMaximaFase2;

        movimento = GetComponent<SalenthraMovimento>();
    }

    private void Update()
    {
        if (
            transformando &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Fase 2")
        )
        {
            IniciarFase2();
        }
    }

    public void ReceberDano(int dano)
    {
        if (transformando || morta)
            return;

        if (faseAtual == 1)
        {
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
        else if (faseAtual == 2)
        {
            vidaAtualFase2 -= dano;

            if (vidaAtualFase2 <= 0)
            {
                vidaAtualFase2 = 0;
                MorrerFase2();
                return;
            }

            if (movimento != null)
            {
                movimento.podeMover = false;
            }

            animator.SetTrigger("DanoFase2");
        }
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

    public void IniciarFase2()
    {
        faseAtual = 2;
        transformando = false;
        vidaAtualFase2 = vidaMaximaFase2;

        if (movimento != null)
        {
            movimento.EntrarFase2();
            movimento.podeMover = true;
        }
    }

    public void FinalizarDano()
    {
        if (transformando || morta)
            return;

        if (movimento != null)
        {
            movimento.podeMover = true;
        }
    }

    public void FinalizarDanoFase2()
    {
        if (morta)
            return;

        if (movimento != null)
        {
            movimento.podeMover = true;
        }
    }

    private void MorrerFase2()
    {
        morta = true;

        if (movimento != null)
        {
            movimento.podeMover = false;
        }

        animator.SetBool("Andando", false);
        animator.SetTrigger("MorteFase2");
    }

    public int CalcularDano(int danoBase)
    {
        float porcentagemVida;

        if (faseAtual == 1)
        {
            porcentagemVida =
                (float)vidaAtualFase1 / vidaMaximaFase1;
        }
        else
        {
            porcentagemVida =
                (float)vidaAtualFase2 / vidaMaximaFase2;
        }

        float multiplicador = 1f;

        if (porcentagemVida <= 0.30f)
        {
            multiplicador = 1.4f;
        }
        else if (porcentagemVida <= 0.60f)
        {
            multiplicador = 1.2f;
        }

        return Mathf.RoundToInt(
            danoBase * multiplicador
        );
    }

    public float ObterPorcentagemVidaAtual()
    {
        if (faseAtual == 1)
        {
            return (float)vidaAtualFase1 /
                   vidaMaximaFase1;
        }

        return (float)vidaAtualFase2 /
               vidaMaximaFase2;
    }
}
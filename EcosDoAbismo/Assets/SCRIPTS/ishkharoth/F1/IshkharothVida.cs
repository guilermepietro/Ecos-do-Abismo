
using UnityEngine;

public class IshkharothVida : MonoBehaviour
{
    [Header("Vida da Primeira Fase")]
    [SerializeField, Min(1)] private int vidaMaxima = 400;
    [SerializeField] private int vidaAtual;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D corpo;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private IshkharothMovimento movimento;
    [SerializeField] private IshkharothIA ia;

    [Header("Ataques da Primeira Fase")]
    [SerializeField] private IshkharothAtaque1 ataque1;
    [SerializeField] private IshkharothAtaque2 ataque2;
    [SerializeField] private IshkharothAtaque3 ataque3;

    [Header("Efeito da Transformacao")]
    [SerializeField] private IshkharothPiscarTela efeitoTela;

    [Header("Visual da Fase 2")]
    [SerializeField] private Sprite spriteFinalFase2;
    [SerializeField] private string estadoIdleFase2 =
        "Base Layer.Idle_F2";

    private bool transicaoIniciada;
    private bool claraoFinalIniciado;
    private bool fase2Iniciada;

    public int VidaAtual => vidaAtual;

    public float PorcentagemVida =>
        vidaMaxima > 0
            ? (float)vidaAtual / vidaMaxima
            : 0f;

    public bool EstaNaFase1 =>
        !transicaoIniciada &&
        !fase2Iniciada &&
        vidaAtual > 0;

    public bool EmTransicao => transicaoIniciada;
    public bool Fase2Iniciada => fase2Iniciada;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (corpo == null)
            corpo = GetComponent<Rigidbody2D>();

        if (visual == null)
            visual = GetComponent<SpriteRenderer>();

        if (movimento == null)
            movimento = GetComponent<IshkharothMovimento>();

        if (ia == null)
            ia = GetComponent<IshkharothIA>();

        if (ataque1 == null)
            ataque1 = GetComponent<IshkharothAtaque1>();

        if (ataque2 == null)
            ataque2 = GetComponent<IshkharothAtaque2>();

        if (ataque3 == null)
            ataque3 = GetComponent<IshkharothAtaque3>();

        if (efeitoTela == null)
            efeitoTela = GetComponent<IshkharothPiscarTela>();

        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int dano)
    {
        if (dano <= 0 || !EstaNaFase1)
            return;

        vidaAtual = Mathf.Max(0, vidaAtual - dano);

        Debug.Log(
            "Ishkharoth - Vida: " +
            vidaAtual + "/" + vidaMaxima
        );

        if (vidaAtual <= 0)
            IniciarTransicaoFase2();
    }

    private void IniciarTransicaoFase2()
    {
        if (transicaoIniciada || fase2Iniciada)
            return;

        transicaoIniciada = true;
        claraoFinalIniciado = false;

        // Interrompe a IA.
        if (ia != null)
            ia.enabled = false;

        // Bloqueia o movimento.
        if (movimento != null)
            movimento.BloquearMovimento();

        // Interrompe os ataques da primeira fase.
        if (ataque1 != null)
            ataque1.enabled = false;

        if (ataque2 != null)
            ataque2.enabled = false;

        if (ataque3 != null)
            ataque3.enabled = false;

        if (corpo != null)
        {
            corpo.linearVelocity = new Vector2(
                0f,
                corpo.linearVelocity.y
            );
        }

        if (animator != null)
        {
            animator.SetBool("Andando", false);

            animator.ResetTrigger("Ataque1");
            animator.ResetTrigger("Ataque2");
            animator.ResetTrigger("Ataque3");

            animator.ResetTrigger("EntrarLoopAtaque3");
            animator.ResetTrigger("EncerrarAtaque3");

            animator.SetTrigger("TransicaoFase2");
        }

        // Comeca a piscar a tela.
        if (efeitoTela != null)
            efeitoTela.IniciarPiscadas();

        Debug.Log(
            "Ishkharoth: transformacao iniciada!"
        );
    }

    // Animation Event na ultima key da transformacao.
    public void IniciarClaraoFinal()
    {
        if (!transicaoIniciada || claraoFinalIniciado)
            return;

        claraoFinalIniciado = true;

        if (efeitoTela != null)
        {
            efeitoTela.ExecutarClaraoFinal(
                FinalizarTransicaoFase2
            );
        }
        else
        {
            FinalizarTransicaoFase2();
        }
    }

    // Chamado quando a tela estiver totalmente coberta.
    public void FinalizarTransicaoFase2()
    {
        if (!transicaoIniciada || fase2Iniciada)
            return;

        // Se Idle_F2 existir no Animator,
        // passa imediatamente para esse estado.
        bool encontrouIdleFase2 = false;

        if (animator != null &&
            !string.IsNullOrEmpty(estadoIdleFase2))
        {
            int hashEstado = Animator.StringToHash(
                estadoIdleFase2
            );

            if (animator.HasState(0, hashEstado))
            {
                animator.Play(
                    hashEstado,
                    0,
                    0f
                );

                encontrouIdleFase2 = true;
            }
        }

        // Alternativa enquanto o Animator da fase 2
        // ainda nao estiver implementado.
        if (!encontrouIdleFase2 &&
            spriteFinalFase2 != null &&
            visual != null)
        {
            if (animator != null)
                animator.enabled = false;

            visual.sprite = spriteFinalFase2;
        }

        // Sem Idle_F2 e sem sprite alternativo,
        // permanece no ultimo frame da transformacao.

        transicaoIniciada = false;
        fase2Iniciada = true;

        Debug.Log(
            "Ishkharoth: transformacao concluida!"
        );
    }

    [ContextMenu("TESTE - Perder 100 de vida")]
    private void TestarDano()
    {
        if (Application.isPlaying)
            ReceberDano(100);
    }
}

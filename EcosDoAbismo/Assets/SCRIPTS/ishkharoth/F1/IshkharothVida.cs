using UnityEngine;

public class IshkharothVida : MonoBehaviour
{
    [Header("Vida da Primeira Fase")]
    [SerializeField, Min(1)] private int vidaMaxima = 400;
    [SerializeField] private int vidaAtual;

    [Header("Vida da Segunda Fase")]
    [SerializeField, Min(1)] private int vidaMaximaFase2 = 500;
    [SerializeField] private int vidaAtualFase2;

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

    [Header("Segunda Fase")]
    [SerializeField] private IshkharothMovimentoFase2 movimentoFase2;
    [SerializeField] private IshkharothIAFase2 iaFase2;
    [SerializeField] private IshkharothAtaque1Fase2 ataque1Fase2;
    [SerializeField] private IshkharothAtaque2Fase2 ataque2Fase2;
    [SerializeField] private IshkharothAtaque3Fase2 ataque3Fase2;

    [Header("Final do Jogo")]
    [SerializeField] private FinalIshkharoth finalIshkharoth;

    private bool transicaoIniciada;
    private bool claraoFinalIniciado;
    private bool fase2Iniciada;

    private bool morto;
    private bool morteFinalizada;

    public int VidaAtual => vidaAtual;
    public int VidaAtualFase2 => vidaAtualFase2;

    public float PorcentagemVida =>
        vidaMaxima > 0
            ? (float)vidaAtual / vidaMaxima
            : 0f;

    public float PorcentagemVidaFase2 =>
        vidaMaximaFase2 > 0
            ? (float)vidaAtualFase2 / vidaMaximaFase2
            : 0f;

    public bool EstaNaFase1 =>
        !transicaoIniciada &&
        !fase2Iniciada &&
        !morto &&
        vidaAtual > 0;

    public bool EstaNaFase2 =>
        fase2Iniciada &&
        !transicaoIniciada &&
        !morto &&
        vidaAtualFase2 > 0;

    public bool PodeReceberDano =>
        EstaNaFase1 || EstaNaFase2;

    public bool EmTransicao => transicaoIniciada;
    public bool Fase2Iniciada => fase2Iniciada;
    public bool Morto => morto;
    public bool MorteFinalizada => morteFinalizada;

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

        if (movimentoFase2 == null)
        {
            movimentoFase2 =
                GetComponent<IshkharothMovimentoFase2>();
        }

        if (iaFase2 == null)
        {
            iaFase2 =
                GetComponent<IshkharothIAFase2>();
        }

        if (ataque1Fase2 == null)
        {
            ataque1Fase2 =
                GetComponent<IshkharothAtaque1Fase2>();
        }

        if (ataque2Fase2 == null)
        {
            ataque2Fase2 =
                GetComponent<IshkharothAtaque2Fase2>();
        }

        if (ataque3Fase2 == null)
        {
            ataque3Fase2 =
                GetComponent<IshkharothAtaque3Fase2>();
        }

        if (finalIshkharoth == null)
        {
            finalIshkharoth =
                GetComponent<FinalIshkharoth>();
        }

        vidaAtual = vidaMaxima;
        vidaAtualFase2 = 0;

        transicaoIniciada = false;
        claraoFinalIniciado = false;
        fase2Iniciada = false;

        morto = false;
        morteFinalizada = false;
    }

    public void ReceberDano(int dano)
    {
        if (
            dano <= 0 ||
            !PodeReceberDano
        )
        {
            return;
        }

        if (EstaNaFase1)
        {
            vidaAtual =
                Mathf.Max(
                    0,
                    vidaAtual - dano
                );

            Debug.Log(
                "Ishkharoth Fase 1 - Vida: " +
                vidaAtual + "/" + vidaMaxima
            );

            if (vidaAtual <= 0)
            {
                IniciarTransicaoFase2();
            }

            return;
        }

        if (EstaNaFase2)
        {
            vidaAtualFase2 =
                Mathf.Max(
                    0,
                    vidaAtualFase2 - dano
                );

            Debug.Log(
                "Ishkharoth Fase 2 - Vida: " +
                vidaAtualFase2 + "/" +
                vidaMaximaFase2
            );

            if (vidaAtualFase2 <= 0)
            {
                IniciarMorteDefinitiva();
            }
        }
    }

    // =========================
    // TRANSICAO PARA FASE 2
    // =========================

    private void IniciarTransicaoFase2()
    {
        if (
            transicaoIniciada ||
            fase2Iniciada ||
            morto
        )
        {
            return;
        }

        transicaoIniciada = true;
        claraoFinalIniciado = false;

        if (ia != null)
        {
            ia.enabled = false;
        }

        if (movimento != null)
        {
            movimento.BloquearMovimento();
        }

        if (ataque1 != null)
        {
            ataque1.enabled = false;
        }

        if (ataque2 != null)
        {
            ataque2.enabled = false;
        }

        if (ataque3 != null)
        {
            ataque3.enabled = false;
        }

        if (corpo != null)
        {
            corpo.linearVelocity =
                new Vector2(
                    0f,
                    corpo.linearVelocity.y
                );
        }

        if (animator != null)
        {
            animator.SetBool(
                "Andando",
                false
            );

            animator.ResetTrigger(
                "Ataque1"
            );

            animator.ResetTrigger(
                "Ataque2"
            );

            animator.ResetTrigger(
                "Ataque3"
            );

            animator.ResetTrigger(
                "EntrarLoopAtaque3"
            );

            animator.ResetTrigger(
                "EncerrarAtaque3"
            );

            animator.SetTrigger(
                "TransicaoFase2"
            );
        }

        if (efeitoTela != null)
        {
            efeitoTela.IniciarPiscadas();
        }

        Debug.Log(
            "Ishkharoth: transformacao iniciada!"
        );
    }

    // =========================
    // CLARAO DA TRANSFORMACAO
    // =========================

    public void IniciarClaraoFinal()
    {
        if (
            !transicaoIniciada ||
            claraoFinalIniciado
        )
        {
            return;
        }

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

    // =========================
    // INICIO DA FASE 2
    // =========================

    public void FinalizarTransicaoFase2()
    {
        if (
            !transicaoIniciada ||
            fase2Iniciada ||
            morto
        )
        {
            return;
        }

        bool encontrouIdleFase2 = false;

        if (
            animator != null &&
            !string.IsNullOrEmpty(
                estadoIdleFase2
            )
        )
        {
            int hashEstado =
                Animator.StringToHash(
                    estadoIdleFase2
                );

            if (
                animator.HasState(
                    0,
                    hashEstado
                )
            )
            {
                animator.Play(
                    hashEstado,
                    0,
                    0f
                );

                encontrouIdleFase2 = true;
            }
        }

        if (
            !encontrouIdleFase2 &&
            spriteFinalFase2 != null &&
            visual != null
        )
        {
            if (animator != null)
            {
                animator.enabled = false;
            }

            visual.sprite =
                spriteFinalFase2;
        }

        vidaAtualFase2 =
            vidaMaximaFase2;

        transicaoIniciada = false;
        fase2Iniciada = true;

        Debug.Log(
            "Ishkharoth: segunda fase iniciada! Vida: " +
            vidaAtualFase2 + "/" +
            vidaMaximaFase2
        );
    }

    // =========================
    // MORTE DEFINITIVA
    // =========================

    private void IniciarMorteDefinitiva()
    {
        if (
            morto ||
            !fase2Iniciada
        )
        {
            return;
        }

        morto = true;
        morteFinalizada = false;

        Debug.Log(
            "Ishkharoth: morte definitiva iniciada!"
        );

        // Interrompe a IA da segunda fase.
        if (iaFase2 != null)
        {
            iaFase2.enabled = false;
        }

        // Bloqueia completamente
        // o movimento da segunda fase.
        if (movimentoFase2 != null)
        {
            movimentoFase2.BloquearMovimento();
            movimentoFase2.enabled = false;
        }

        // Garante que a primeira fase
        // nao volte a executar.
        if (ia != null)
        {
            ia.enabled = false;
        }

        if (movimento != null)
        {
            movimento.enabled = false;
        }

        // Interrompe todos os ataques
        // da segunda fase.
        if (ataque1Fase2 != null)
        {
            ataque1Fase2.enabled = false;
        }

        if (ataque2Fase2 != null)
        {
            ataque2Fase2.enabled = false;
        }

        if (ataque3Fase2 != null)
        {
            ataque3Fase2.enabled = false;
        }

        // Congela completamente a fisica.
        // Assim Ishkharoth nao cai
        // durante a animacao de morte.
        if (corpo != null)
        {
            corpo.linearVelocity =
                Vector2.zero;

            corpo.angularVelocity =
                0f;

            corpo.simulated =
                false;
        }

        // Desativa corpo e hitboxes
        // durante a morte.
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (
            Collider2D collider
            in colliders
        )
        {
            collider.enabled = false;
        }

        // Prepara a animacao definitiva.
        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = 1f;

            animator.SetBool(
                "Andando",
                false
            );

            animator.ResetTrigger(
                "Ataque1"
            );

            animator.ResetTrigger(
                "Ataque2"
            );

            animator.ResetTrigger(
                "Ataque3"
            );

            animator.ResetTrigger(
                "Ataque1F2"
            );

            animator.ResetTrigger(
                "Ataque2F2"
            );

            animator.ResetTrigger(
                "Ataque3F2"
            );

            animator.ResetTrigger(
                "EntrarLoopAtaque3"
            );

            animator.ResetTrigger(
                "EncerrarAtaque3"
            );

            animator.ResetTrigger(
                "EncerrarAtaque3F2"
            );

            animator.SetTrigger(
                "MorteF2"
            );
        }
        else
        {
            FinalizarMorteF2();
        }
    }

    // =========================
    // FINAL DA MORTE
    // =========================

    // Chamado pelo Animation Event
    // no ultimo frame de Morte_F2.
    public void FinalizarMorteF2()
    {
        if (
            !morto ||
            morteFinalizada
        )
        {
            return;
        }

        morteFinalizada = true;

        Debug.Log(
            "Ishkharoth: morte definitiva concluida!"
        );

        // Depois da animacao de morte,
        // inicia a cutscene final do jogo.
        if (finalIshkharoth != null)
        {
            finalIshkharoth.IniciarFinalDoJogo();
        }
        else
        {
            Debug.LogWarning(
                "FinalIshkharoth nao foi encontrado."
            );
        }
    }

    // =========================
    // TESTE
    // =========================

    [ContextMenu("TESTE - Perder 100 de vida")]
    private void TestarDano()
    {
        if (Application.isPlaying)
        {
            ReceberDano(100);
        }
    }
}
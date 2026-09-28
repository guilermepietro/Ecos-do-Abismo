using UnityEngine;

public class RakthasMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    private RakthasVida vida;
    public Animator animator;

    private bool ativado = false;
    public bool EstaAtivado => ativado;

    private RakthasAtaques ataques;

    [Header("Movimento")]
    public float velocidadeNormal = 1.6f;
    public float velocidadeAgressiva = 4f;

    [Header("Arrancada")]
    public float duracaoArrancada = 0.8f;
    public float intervaloArrancadas = 1.2f;
    public float preparacaoArrancada = 0.15f;
    public float velocidadeAnimacaoArrancada = 1.55f;

    private bool emArrancada = false;
    private bool preparandoArrancada = false;

    private float fimArrancada;
    private float proximaArrancada;
    private float fimPreparacaoArrancada;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vida = GetComponent<RakthasVida>();
        ataques = GetComponent<RakthasAtaques>();
    }

    private void FixedUpdate()
    {
        if (vida != null && vida.EstaMorto)
        {
            PararMovimento();
            return;
        }

        if (vida != null && vida.EstaTomandoDano)
        {
            PararMovimento();
            return;
        }

        if (ataques != null && ataques.EstaAtacando)
        {
            PararMovimento();
            return;
        }

        if (ataques != null && ataques.EstaEmPausaDecisao)
{
    PararMovimento();
    return;
}

        if (!ativado)
        {
            PararMovimento();
            return;
        }

        if (elian == null)
            return;

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        float direcao = Mathf.Sign(
            elian.position.x - transform.position.x
        );

        // DIREÇÃO ORIGINAL
        if (direcao > 0)
            spriteRenderer.flipX = false;
        else if (direcao < 0)
            spriteRenderer.flipX = true;

        // SE JÁ ESTÁ NA DISTÂNCIA DO SOCO
        if (ataques != null &&
            distancia <= ataques.DistanciaSoco)
        {
            PararMovimento();
            return;
        }

        bool precisaPressionar = false;

        if (ataques != null)
        {
            // Perseguição agressiva após certos ataques
            if (ataques.PerseguicaoAgressivaObrigatoria)
                precisaPressionar = true;

            // Elian está longe demais
            else if (distancia > ataques.DistanciaMaximaRaio)
                precisaPressionar = true;
        }

        // =========================
        // PREPARAÇÃO DA ARRANCADA
        // =========================

        if (preparandoArrancada)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetBool("estaCorrendo", false);
            animator.speed = 1f;

            if (Time.time >= fimPreparacaoArrancada)
            {
                preparandoArrancada = false;
                emArrancada = true;

                fimArrancada =
                    Time.time + duracaoArrancada;
            }

            return;
        }

        // =========================
        // ARRANCADA
        // =========================

        if (emArrancada)
        {
            rb.linearVelocity = new Vector2(
                direcao * velocidadeAgressiva,
                rb.linearVelocity.y
            );

            animator.SetBool("estaCorrendo", true);
            animator.speed = velocidadeAnimacaoArrancada;

            if (Time.time >= fimArrancada)
            {
                emArrancada = false;

                proximaArrancada =
                    Time.time + intervaloArrancadas;

                animator.speed = 1f;
            }

            return;
        }

        // =========================
        // INICIAR NOVA ARRANCADA
        // =========================

        if (precisaPressionar &&
            Time.time >= proximaArrancada)
        {
            preparandoArrancada = true;

            fimPreparacaoArrancada =
                Time.time + preparacaoArrancada;

            PararMovimento();
            return;
        }

        

        rb.linearVelocity = new Vector2(
            direcao * velocidadeNormal,
            rb.linearVelocity.y
        );

        animator.SetBool("estaCorrendo", true);
        animator.speed = 1f;
    }

    private void PararMovimento()
    {
        rb.linearVelocity = new Vector2(
            0,
            rb.linearVelocity.y
        );

        animator.SetBool("estaCorrendo", false);
        animator.speed = 1f;
    }

    public void AtivarRakthas()
    {
        ativado = true;
    }
}
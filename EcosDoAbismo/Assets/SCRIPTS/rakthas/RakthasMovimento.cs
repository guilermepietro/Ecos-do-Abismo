
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
    public float velocidadeNormal = 3.5f;
    public float velocidadeAgressiva = 5.5f;

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
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (vida != null && vida.EstaTomandoDano)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (ataques != null && ataques.EstaAtacando)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("estaCorrendo", false);
            return;
        }

        if (!ativado)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("estaCorrendo", false);
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

        // PARA QUANDO ALCANÇA A DISTÂNCIA DO SOCO
        if (ataques != null &&
            distancia <= ataques.DistanciaSoco)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetBool("estaCorrendo", false);
            animator.speed = 1f;
            return;
        }

        // CORRIDA AGRESSIVA
        bool corridaAgressiva = false;

        if (ataques != null)
        {
            if (ataques.PerseguicaoAgressivaObrigatoria)
            {
                corridaAgressiva = true;
            }
            else if (distancia > ataques.DistanciaMaximaRaio)
            {
                corridaAgressiva = true;
            }
        }

        float velocidadeAtual = corridaAgressiva
            ? velocidadeAgressiva
            : velocidadeNormal;

        rb.linearVelocity = new Vector2(
            direcao * velocidadeAtual,
            rb.linearVelocity.y
        );

        animator.SetBool("estaCorrendo", true);

        if (corridaAgressiva)
        {
            animator.speed =
                velocidadeAgressiva / velocidadeNormal;
        }
        else
        {
            animator.speed = 1f;
        }
    }

    public void AtivarRakthas()
    {
        ativado = true;
    }
}

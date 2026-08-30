using UnityEngine;

public class GosmaHumanoideAtaque : MonoBehaviour
{
    public float distanciaAtaque = 4f;
    public float velocidadeAtaque = 6f;
    public float tempoEntreAtaques = 1.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform elian;
    private ElianVida vidaElian;

    private float proximoAtaque;
    private float direcaoAtaque;
    public Transform hitboxAtaque;
    private GosmaHumanoideVida vida;

    public bool EstaAtacando { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vida = GetComponent<GosmaHumanoideVida>();

        GameObject jogador = GameObject.Find("elian");

        if (jogador != null)
        {
            elian = jogador.transform;
            vidaElian = jogador.GetComponent<ElianVida>();
        }
    }

    void Update()
    {
        if (elian == null)
            return;
            if (vida != null && (vida.EstaMorto || vida.EstaTomandoDano))
    return;

        if (vidaElian != null && vidaElian.EstaMorto)
            return;

        if (EstaAtacando)
            return;

        float distancia = Mathf.Abs(elian.position.x - transform.position.x);

        if (distancia <= distanciaAtaque && Time.time >= proximoAtaque)
        {
            direcaoAtaque = Mathf.Sign(elian.position.x - transform.position.x);

            spriteRenderer.flipX = direcaoAtaque < 0;
            Vector3 posicaoHitbox = hitboxAtaque.localPosition;

if (direcaoAtaque < 0)
{
    posicaoHitbox.x = -0.02f;
}
else
{
    posicaoHitbox.x = 0.68f;
}

hitboxAtaque.localPosition = posicaoHitbox;

            EstaAtacando = true;

            animator.SetTrigger("Atacar");

            proximoAtaque = Time.time + tempoEntreAtaques;
        }
    }

    public void PararAoAcertar()
{
    EstaAtacando = false;

    rb.linearVelocity = new Vector2(
        0f,
        rb.linearVelocity.y
    );

    animator.Play("Idle");
}

    void FixedUpdate()
    {
        if (!EstaAtacando)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            return;
        }

        rb.linearVelocity = new Vector2(
            direcaoAtaque * velocidadeAtaque,
            rb.linearVelocity.y
        );
    }

    public void FinalizarAtaque()
    {
        EstaAtacando = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }
    public void CancelarAtaque()
{
    EstaAtacando = false;

    rb.linearVelocity = new Vector2(
        0f,
        rb.linearVelocity.y
    );
}
}
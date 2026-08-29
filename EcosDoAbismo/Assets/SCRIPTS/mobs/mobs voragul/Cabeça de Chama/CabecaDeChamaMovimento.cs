using UnityEngine;

public class CabecaDeChamaMovimento : MonoBehaviour
{
    public float velocidade = 2.5f;
    public float distanciaAtaque = 1.5f;
    public float tempoEntreAtaques = 1.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform elian;
    public Transform hitboxAtaque;
    private ElianVida vidaElian;

    private float proximoAtaque;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject jogador = GameObject.Find("elian");
        if (jogador != null)
{
    elian = jogador.transform;
    vidaElian = jogador.GetComponent<ElianVida>();
}


    }

    void FixedUpdate()
    {
        if (vidaElian != null && vidaElian.EstaMorto)
{
    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

    animator.SetBool("Andando", false);
    animator.ResetTrigger("Atacar");

    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
    {
        animator.Play("Idle");
    }

    return;
}
            

        AnimatorStateInfo estado = animator.GetCurrentAnimatorStateInfo(0);

        if (estado.IsName("Damage") ||
            estado.IsName("Death") ||
            estado.IsName("Attack"))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("Andando", false);
            return;
        }

        float distancia = Mathf.Abs(elian.position.x - transform.position.x);
        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        spriteRenderer.flipX = direcao < 0;
        Vector3 posicaoHitbox = hitboxAtaque.localPosition;
        posicaoHitbox.x = Mathf.Abs(posicaoHitbox.x) * (direcao < 0 ? -1 : 1);
        hitboxAtaque.localPosition = posicaoHitbox;

        if (distancia <= distanciaAtaque)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("Andando", false);

            if (Time.time >= proximoAtaque)
            {
                animator.SetTrigger("Atacar");
                proximoAtaque = Time.time + tempoEntreAtaques;
            }

            return;
        }

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );

        animator.SetBool("Andando", true);
    }

    public void AcertarAtaque()
{
    CabecaDeChamaAtaque ataque = GetComponentInChildren<CabecaDeChamaAtaque>();

    if (ataque != null)
        ataque.VerificarAcerto();
}
}
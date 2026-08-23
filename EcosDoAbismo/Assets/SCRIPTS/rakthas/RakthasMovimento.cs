using UnityEngine;

public class RakthasMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    private RakthasVida vida;
    public Animator animator;
    private bool ativado = false;

    [Header("Movimento")]
    public float velocidadeNormal = 3.5f;
    public float velocidadeAgressiva = 5.5f;
    public float distanciaParaCorridaAgressiva = 6f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vida = GetComponent<RakthasVida>();
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

        if (!ativado)
{
    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    animator.SetBool("estaCorrendo", false);
    return;
}

        if (elian == null)
            return;

        float distancia = Mathf.Abs(elian.position.x - transform.position.x);

        bool corridaAgressiva = distancia >= distanciaParaCorridaAgressiva;

        float velocidadeAtual = corridaAgressiva
            ? velocidadeAgressiva
            : velocidadeNormal;

        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(
            direcao * velocidadeAtual,
            rb.linearVelocity.y
        );

        animator.SetBool("estaCorrendo", true);

        if (corridaAgressiva)
            animator.speed = velocidadeAgressiva / velocidadeNormal;
        else
            animator.speed = 1f;

        if (direcao > 0)
            spriteRenderer.flipX = false;
        else if (direcao < 0)
            spriteRenderer.flipX = true;
    }

    public void AtivarRakthas()
{
    ativado = true;
}
}
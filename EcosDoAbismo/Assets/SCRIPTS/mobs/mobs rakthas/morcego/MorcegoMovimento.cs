using UnityEngine;

public class MorcegoMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;

    [Header("Ataque")]
    public float distanciaDeteccao = 6f;
    public float velocidadeAtaque = 7f;
    public float tempoEntreAtaques = 1.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float tempoProximoAtaque;
    private bool atacando;
    private ElianVida vidaElian;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
{
    if (elian != null)
        vidaElian = elian.GetComponent<ElianVida>();
}

    private void Update()
    {
        if (elian == null)
            return;

            if (vidaElian != null && vidaElian.EstaMorto)
{
    rb.linearVelocity = new Vector2(
        0f,
        rb.linearVelocity.y
    );

    animator.ResetTrigger("Atacar");
    atacando = false;

    return;
}

        float distancia = Vector2.Distance(transform.position, elian.position);

        if (!atacando && distancia <= distanciaDeteccao)
        {
            TentarAtacar();
        }
    }

    private void TentarAtacar()
    {
        if (Time.time < tempoProximoAtaque)
            return;

        atacando = true;

        VirarParaElian();

        animator.SetTrigger("Atacar");

        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(
            direcao * velocidadeAtaque,
            rb.linearVelocity.y
        );

        tempoProximoAtaque = Time.time + tempoEntreAtaques;
    }

    private void VirarParaElian()
{
    if (elian.position.x < transform.position.x)
    {
        spriteRenderer.flipX = false;
    }
    else
    {
        spriteRenderer.flipX = true;
    }
}

    public void FinalizarAtaque()
    {
        atacando = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    public void CancelarAtaque()
{
    atacando = false;

    animator.ResetTrigger("Atacar");

    if (rb != null)
        rb.linearVelocity = Vector2.zero;
}

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (!atacando)
        return;

    if (collision.gameObject.CompareTag("Player"))
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        ElianVida vidaElian = collision.gameObject.GetComponent<ElianVida>();

        if (vidaElian != null)
        {
            vidaElian.ReceberDano(20);
        }
    }
}
}
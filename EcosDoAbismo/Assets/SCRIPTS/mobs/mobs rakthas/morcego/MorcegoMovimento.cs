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

    // Guarda a direção escolhida no início do dash
    private float direcaoAtaque;

    // Impede que o morcego continue avançando
    // depois de atingir o Elian
    private bool parouAtaque;

    private ElianVida vidaElian;

    private AudioMob audioMob;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioMob = GetComponentInParent<AudioMob>();
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
            CancelarAtaque();
            return;
        }

        float distancia = Vector2.Distance(
            transform.position,
            elian.position
        );

        if (!atacando && distancia <= distanciaDeteccao)
        {
            TentarAtacar();
        }
    }

    private void FixedUpdate()
    {
        if (!atacando)
            return;

        if (parouAtaque)
            return;

        // Mantém o dash enquanto a animação de ataque estiver acontecendo
        rb.linearVelocity = new Vector2(
            direcaoAtaque * velocidadeAtaque,
            rb.linearVelocity.y
        );
    }

    private void TentarAtacar()
    {
        if (Time.time < tempoProximoAtaque)
            return;

        atacando = true;
        parouAtaque = false;

        VirarParaElian();

        // Guarda a direção uma única vez.
        // O morcego não muda de direção no meio do dash.
        direcaoAtaque = Mathf.Sign(
            elian.position.x - transform.position.x
        );

        animator.SetTrigger("Atacar");

        if (audioMob != null)
        {
            audioMob.TocarAtaque();
        }

        // Já inicia o avanço imediatamente
        rb.linearVelocity = new Vector2(
            direcaoAtaque * velocidadeAtaque,
            rb.linearVelocity.y
        );

        tempoProximoAtaque =
            Time.time + tempoEntreAtaques;
    }

    private void VirarParaElian()
    {
        // O sprite original do morcego olha para a esquerda
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
        parouAtaque = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    public void CancelarAtaque()
    {
        atacando = false;
        parouAtaque = false;

        animator.ResetTrigger("Atacar");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!atacando)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Para o avanço, mas deixa a animação terminar
            parouAtaque = true;

            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            ElianVida vidaDoElian =
                collision.gameObject.GetComponent<ElianVida>();

            if (vidaDoElian != null)
            {
                vidaDoElian.ReceberDano(20);
            }
        }
    }
}
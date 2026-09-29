using UnityEngine;

public class SalenthraMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    public Animator animator;

    [Header("Movimento")]
    public float velocidade = 3f;
    public float distanciaParada = 1.5f;

    [Header("Ataque Dash")]
    public float distanciaDash = 4f;
    public float velocidadeDash = 8f;
    public float tempoEntreDashes = 2f;

    public bool podeMover = true;
    public bool ativada = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool atacandoDash = false;
    private float direcaoDash;
    private float proximoDash = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (!ativada || !podeMover)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            animator.SetBool("Andando", false);
            return;
        }

        if (elian == null)
            return;

        if (atacandoDash)
        {
            rb.linearVelocity = new Vector2(
                direcaoDash * velocidadeDash,
                rb.linearVelocity.y
            );

            animator.SetBool("Andando", false);
            return;
        }

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        if (distancia <= distanciaDash && Time.time >= proximoDash)
        {
            IniciarDash();
            return;
        }

        if (distancia <= distanciaParada)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            animator.SetBool("Andando", false);
            return;
        }

        float direcao = Mathf.Sign(
            elian.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );

        animator.SetBool("Andando", true);

        if (direcao > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direcao < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void IniciarDash()
    {
        if (atacandoDash)
            return;

        atacandoDash = true;

        direcaoDash = Mathf.Sign(
            elian.position.x - transform.position.x
        );

        if (direcaoDash > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direcaoDash < 0)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetBool("Andando", false);
        animator.SetTrigger("AtaqueDash");
    }

    public void FinalizarDash()
    {
        atacandoDash = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        proximoDash = Time.time + tempoEntreDashes;
    }

    public void Ativar()
    {
        ativada = true;
    }
}
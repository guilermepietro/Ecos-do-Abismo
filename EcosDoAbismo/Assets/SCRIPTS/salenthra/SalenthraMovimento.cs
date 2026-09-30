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

    [Header("Ataque Puxão")]
    public float distanciaPuxao = 6f;
    public float forcaPuxao = 8f;
    public float tempoEntrePuxoes = 3f;

    public bool podeMover = true;
    public bool ativada = false;
    private bool fase2 = false;

    private Rigidbody2D rb;
    private Rigidbody2D rbElian;
    private SpriteRenderer spriteRenderer;

    private bool atacandoDash = false;
    private float direcaoDash;
    private float proximoDash = 0f;
    private bool dashParadoAoAcertar = false;

    private bool atacandoPuxao = false;
    private float proximoPuxao = 0f;

    private bool movendoNoDash = false;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (elian != null)
        {
            rbElian = elian.GetComponent<Rigidbody2D>();
        }
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

        if (atacandoDash)
        {
            if (movendoNoDash && !dashParadoAoAcertar)
            {
                rb.linearVelocity = new Vector2(
                    direcaoDash * velocidadeDash,
                    rb.linearVelocity.y
                );
            }
            else
            {
                rb.linearVelocity = new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
            }

            animator.SetBool("Andando", false);
            return;
        }

        // Puxão em andamento
        if (atacandoPuxao)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            animator.SetBool("Andando", false);
            return;
        }

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        // ATAQUE 2 - PUXÃO
        // Só usa quando Elian está mais longe que a área do dash
        if (
    !fase2 &&
    distancia > distanciaDash &&
    distancia <= distanciaPuxao &&
    Time.time >= proximoPuxao
)
{
    IniciarPuxao();
    return;
}

        // ATAQUE 1 - DASH
        if (
    !fase2 &&
    distancia <= distanciaDash &&
    Time.time >= proximoDash
)
{
    IniciarDash();
    return;
}

        // Para quando está próxima do Elian
        if (distancia <= distanciaParada)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            animator.SetBool("Andando", false);
            return;
        }

        // Movimento normal
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

    // =========================
    // ATAQUE 1 - DASH
    // =========================

    private void IniciarDash()
    {
        if (atacandoDash || atacandoPuxao)
            return;

        atacandoDash = true;
        movendoNoDash = false;
        dashParadoAoAcertar = false;

        direcaoDash = Mathf.Sign(
            elian.position.x - transform.position.x
        );

        if (direcaoDash > 0)
            spriteRenderer.flipX = false;
        else if (direcaoDash < 0)
            spriteRenderer.flipX = true;

        animator.SetBool("Andando", false);
        animator.SetTrigger("AtaqueDash");
    }

    public void FinalizarDash()
{
    atacandoDash = false;
    movendoNoDash = false;
    dashParadoAoAcertar = false;

    rb.linearVelocity = new Vector2(
        0f,
        rb.linearVelocity.y
    );

    proximoDash = Time.time + tempoEntreDashes;
}

    public void ComecarMovimentoDash()
    {
        if (!atacandoDash)
            return;

        movendoNoDash = true;
    }



    public void PararDashAoAcertar()
    {
        if (!atacandoDash)
            return;

        dashParadoAoAcertar = true;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    public void PararMovimentoDash()
{
    movendoNoDash = false;

    rb.linearVelocity = new Vector2(
        0f,
        rb.linearVelocity.y
    );
}

    // =========================
    // ATAQUE 2 - PUXÃO
    // =========================

    private void IniciarPuxao()
    {
        if (atacandoPuxao || atacandoDash)
            return;

        atacandoPuxao = true;

        float direcao = Mathf.Sign(
            elian.position.x - transform.position.x
        );

        if (direcao > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direcao < 0)
        {
            spriteRenderer.flipX = true;
        }

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        animator.SetBool("Andando", false);
        animator.SetTrigger("AtaquePuxao");
    }

        public void PuxarElian()
    {
        if (!atacandoPuxao)
            return;

        if (elian == null)
            return;

        ElianMovimento movimentoElian = elian.GetComponent<ElianMovimento>();

        if (movimentoElian == null)
            return;

        float direcao = Mathf.Sign(
            transform.position.x - elian.position.x
        );

        movimentoElian.AplicarPuxao(
            direcao,
            forcaPuxao
        );
    }

    public void FinalizarPuxao()
    {
        atacandoPuxao = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        proximoPuxao = Time.time + tempoEntrePuxoes;
    }

    // =========================
    // ATIVAÇÃO
    // =========================

    public void Ativar()
    {
        ativada = true;
    }

    public void EntrarFase2()
{
    fase2 = true;

    atacandoDash = false;
    atacandoPuxao = false;
    movendoNoDash = false;
    dashParadoAoAcertar = false;

    rb.linearVelocity = new Vector2(
        0f,
        rb.linearVelocity.y
    );

    animator.SetBool("Andando", false);
}
}
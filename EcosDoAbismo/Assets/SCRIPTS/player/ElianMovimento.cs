using UnityEngine;
using System.Collections;

public class ElianMovimento : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
    public float velocidadeCorrida = 8f;
    public float velocidadeAnimacaoCorrida = 1.5f;
    public float forcaPulo = 10f;
    public float cooldownDash = 0.5f;
    private bool dashDisponivel = true;
    private bool dashAereoDisponivel = true;
    private Collider2D colisorElian;

    [Header("Dash")]
    public float velocidadeDash = 15f;
    public float tempoDash = 0.2f;

    [Header("Chao")]
    public Transform pontoDeChao;
    public float raioChao = 0.2f;
    public LayerMask camadaChao;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool estaNoChao;
    private bool estaDashando = false;

    public bool podeMover = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        colisorElian = GetComponent<Collider2D>();
    }

    void Update()
    {
        VerificarChao();

        if (estaDashando)
        {
            return;
        }

        Movimento();
        Pulo();
        Dash();
    }

    void VerificarChao()
    {
        if (estaNoChao)
{
    dashAereoDisponivel = true;
}
        estaNoChao = Physics2D.OverlapCircle(
            pontoDeChao.position,
            raioChao,
            camadaChao
        );

        animator.SetBool("estaNoChao", estaNoChao);

        
    }

    void Movimento()
    {
        if (!podeMover)
        {
            return;
        }

        float movimentoHorizontal = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("velocidade", Mathf.Abs(movimentoHorizontal));

        bool correndo =
            Input.GetKey(KeyCode.LeftShift) &&
            movimentoHorizontal != 0 &&
            estaNoChao;

        float velocidadeAtual = correndo ? velocidadeCorrida : velocidade;

        animator.SetFloat(
            "velocidadeAnimacao",
            correndo ? velocidadeAnimacaoCorrida : 1f
        );

        rb.linearVelocity = new Vector2(
            movimentoHorizontal * velocidadeAtual,
            rb.linearVelocity.y
        );

        // Virar o Elian
        if (movimentoHorizontal > 0)
        {
            spriteRenderer.flipX = false;
            GetComponent<HitboxControle>().AtualizarDirecao(false);
        }
        else if (movimentoHorizontal < 0)
        {
            spriteRenderer.flipX = true;
            GetComponent<HitboxControle>().AtualizarDirecao(true);
        }
    }

    void Pulo()
    {
        if (Input.GetKeyDown(KeyCode.Space) && estaNoChao)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                forcaPulo
            );
        }
    }

    void Dash()
{
    if (Input.GetKeyDown(KeyCode.LeftControl) &&
        podeMover &&
        dashDisponivel &&
        (estaNoChao || dashAereoDisponivel))
    {
        if (!estaNoChao)
        {
            dashAereoDisponivel = false;
        }

        StartCoroutine(ExecutarDash());
    }
}

void IgnorarColisaoInimigos(bool ignorar)
{
    GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Inimigo");

    foreach (GameObject inimigo in inimigos)
    {
        Collider2D[] colisores = inimigo.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D colisorInimigo in colisores)
        {
            if (!colisorInimigo.isTrigger)
            {
                Physics2D.IgnoreCollision(
                    colisorElian,
                    colisorInimigo,
                    ignorar
                );
            }
        }
    }
}

    IEnumerator ExecutarDash()
{
    estaDashando = true;
    dashDisponivel = false;

    GetComponent<ElianVida>().AtivarInvencibilidadeDash();

    IgnorarColisaoInimigos(true);

    animator.SetFloat("velocidade", 0);
    animator.SetTrigger("Dash");

    float direcao = spriteRenderer.flipX ? -1f : 1f;

    rb.linearVelocity = new Vector2(
        direcao * velocidadeDash,
        rb.linearVelocity.y
    );

    yield return new WaitForSeconds(tempoDash);

    GetComponent<ElianVida>().DesativarInvencibilidadeDash();

    IgnorarColisaoInimigos(false);

    rb.linearVelocity = new Vector2(
        0f,
        rb.linearVelocity.y
    );

    estaDashando = false;

    yield return new WaitForSeconds(cooldownDash);

    dashDisponivel = true;
}
}
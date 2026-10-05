using UnityEngine;
using System.Collections;

public class ElianMovimento : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
    public float velocidadeCorrida = 8f;
    public float velocidadeAnimacaoCorrida = 1.5f;
    public float forcaPulo = 10f;

    [Header("Dash")]
    public float velocidadeDash = 15f;
    public float tempoDash = 0.2f;
    public float cooldownDash = 0.5f;

    [Header("Chao")]
    public Transform pontoDeChao;
    public float raioChao = 0.2f;
    public LayerMask camadaChao;

    [Header("Movimento Externo")]
    public float tempoPuxao = 0.25f;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Collider2D colisorElian;

    private ElianAtaque ataque;

    private bool estaNoChao;

    private bool estaDashando = false;
    private bool dashDisponivel = true;
    private bool dashAereoDisponivel = true;

    private bool sendoPuxado = false;

    public bool podeMover = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        colisorElian = GetComponent<Collider2D>();

        ataque = GetComponent<ElianAtaque>();
    }

    void Update()
    {
        VerificarChao();

        if (estaDashando || sendoPuxado)
        {
            return;
        }

        Movimento();
        Pulo();
        Dash();
    }

    void VerificarChao()
    {
        estaNoChao = Physics2D.OverlapCircle(
            pontoDeChao.position,
            raioChao,
            camadaChao
        );

        if (estaNoChao)
        {
            dashAereoDisponivel = true;
        }

        animator.SetBool("estaNoChao", estaNoChao);
    }

    void Movimento()
    {
        if (!podeMover)
        {
            return;
        }

        float movimentoHorizontal =
            Input.GetAxisRaw("Horizontal");

        animator.SetFloat(
            "velocidade",
            Mathf.Abs(movimentoHorizontal)
        );

        bool correndo =
            Input.GetKey(KeyCode.LeftShift) &&
            movimentoHorizontal != 0 &&
            estaNoChao;

        float velocidadeAtual =
            correndo ? velocidadeCorrida : velocidade;

        animator.SetFloat(
            "velocidadeAnimacao",
            correndo ? velocidadeAnimacaoCorrida : 1f
        );

        rb.linearVelocity = new Vector2(
            movimentoHorizontal * velocidadeAtual,
            rb.linearVelocity.y
        );

        if (movimentoHorizontal > 0)
        {
            spriteRenderer.flipX = false;

            GetComponent<HitboxControle>()
                .AtualizarDirecao(false);
        }
        else if (movimentoHorizontal < 0)
        {
            spriteRenderer.flipX = true;

            GetComponent<HitboxControle>()
                .AtualizarDirecao(true);
        }
    }

    void Pulo()
    {
        if (!podeMover)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) &&
            estaNoChao)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                forcaPulo
            );
        }
    }

    void Dash()
    {
        if (!Input.GetKeyDown(KeyCode.LeftControl))
        {
            return;
        }

        if (!dashDisponivel)
        {
            return;
        }

        bool cancelandoCombo =
            ataque != null &&
            ataque.EstaNoCombo();

        if (!podeMover && !cancelandoCombo)
        {
            return;
        }

        if (!estaNoChao && !dashAereoDisponivel)
        {
            return;
        }

        if (cancelandoCombo)
        {
            ataque.CancelarComboPorDash();
        }

        if (!estaNoChao)
        {
            dashAereoDisponivel = false;
        }

        StartCoroutine(ExecutarDash());
    }

    public void AplicarPuxao(float direcao, float forca)
    {
        if (estaDashando)
            return;

        StopCoroutine(nameof(ExecutarPuxao));
        StartCoroutine(ExecutarPuxao(direcao, forca));
    }

    IEnumerator ExecutarPuxao(float direcao, float forca)
    {
        sendoPuxado = true;

        animator.SetFloat("velocidade", 0f);

        rb.linearVelocity = new Vector2(
            direcao * forca,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(tempoPuxao);

        sendoPuxado = false;
    }

    void IgnorarColisaoInimigos(bool ignorar)
    {
        GameObject[] inimigos =
            GameObject.FindGameObjectsWithTag("Inimigo");

        foreach (GameObject inimigo in inimigos)
        {
            Collider2D[] colisores =
                inimigo.GetComponentsInChildren<Collider2D>();

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

        bool dashNoAr = !estaNoChao;

        float gravidadeOriginal = rb.gravityScale;

        GetComponent<ElianVida>()
            .AtivarInvencibilidadeDash();

        IgnorarColisaoInimigos(true);

        animator.SetFloat("velocidade", 0);
        animator.SetTrigger("Dash");

        float direcao =
            spriteRenderer.flipX ? -1f : 1f;

        if (dashNoAr)
        {
            rb.gravityScale = 0f;
        }

        rb.linearVelocity = new Vector2(
            direcao * velocidadeDash,
            0f
        );

        yield return new WaitForSeconds(tempoDash);

        if (dashNoAr)
        {
            rb.gravityScale = gravidadeOriginal;
        }

        GetComponent<ElianVida>()
            .DesativarInvencibilidadeDash();

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
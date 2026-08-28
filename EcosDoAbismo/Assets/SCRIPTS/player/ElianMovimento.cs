using UnityEngine;

public class ElianMovimento : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
    public float velocidadeCorrida = 8f;
    public float velocidadeAnimacaoCorrida = 1.5f;
    public float forcaPulo = 10f;

    [Header("Chao")]
    public Transform pontoDeChao;
    public float raioChao = 0.2f;
    public LayerMask camadaChao;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool estaNoChao;

    public bool podeMover = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        VerificarChao();
        Movimento();
        Pulo();
    }

    void VerificarChao()
    {
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
}
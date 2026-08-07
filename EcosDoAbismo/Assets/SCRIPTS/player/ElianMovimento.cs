using UnityEngine;

public class ElianMovimento : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
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

        rb.linearVelocity = new Vector2(
            movimentoHorizontal * velocidade,
            rb.linearVelocity.y
        );

        // Virar o Elian
        if (movimentoHorizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movimentoHorizontal < 0)
        {
            spriteRenderer.flipX = true;
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
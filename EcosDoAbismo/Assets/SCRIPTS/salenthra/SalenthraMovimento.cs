using UnityEngine;

public class SalenthraMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    public Animator animator;
    public float distanciaParada = 1.5f;

    [Header("Movimento")]
    public float velocidade = 3f;

    public bool podeMover = true;
    public bool ativada = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        float distancia = Mathf.Abs(elian.position.x - transform.position.x);

if (distancia <= distanciaParada)
{
    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    animator.SetBool("Andando", false);
    return;
}
        if (!ativada || !podeMover)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("Andando", false);
            return;
        }

        if (elian == null)
            return;

        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );

        animator.SetBool("Andando", true);

        if (direcao > 0)
            spriteRenderer.flipX = false;
        else if (direcao < 0)
            spriteRenderer.flipX = true;
    }

    public void Ativar()
    {
        ativada = true;
    }
}
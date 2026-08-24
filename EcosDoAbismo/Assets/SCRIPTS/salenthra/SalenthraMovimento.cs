using UnityEngine;

public class SalenthraMovimento : MonoBehaviour
{
    public bool podeMover = true;

    [Header("Referências")]
    public Transform elian;
    public Animator animator;

    [Header("Movimento")]
    public float velocidade = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (!podeMover)
{
    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    animator.SetBool("Andando", false);
    return;
}
        if (elian == null)
            return;

        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);

        animator.SetBool("Andando", true);

        if (direcao > 0)
            spriteRenderer.flipX = false;
        else if (direcao < 0)
            spriteRenderer.flipX = true;
    }
}
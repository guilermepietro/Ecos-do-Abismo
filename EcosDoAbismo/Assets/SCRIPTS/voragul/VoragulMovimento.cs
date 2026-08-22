using UnityEngine;

public class VoragulMovimento : MonoBehaviour
{
    public float velocidade = 2f;
    public Transform elian;
    public bool podeMover = true;
    public bool lutaIniciada = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {

        if (!lutaIniciada)
{
    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    animator.SetBool("andando", false);
    return;
}

        if (!podeMover)
{
    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    animator.SetBool("andando", false);
    return;
}

        if (elian == null)
            return;

        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(direcao * velocidade, rb.linearVelocity.y);

        animator.SetBool("andando", true);

        if (direcao > 0)
            spriteRenderer.flipX = false;
        else if (direcao < 0)
            spriteRenderer.flipX = true;
    }
}
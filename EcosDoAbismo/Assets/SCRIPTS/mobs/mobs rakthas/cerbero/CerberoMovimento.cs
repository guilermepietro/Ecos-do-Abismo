using UnityEngine;

public class CerberoMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    

    [Header("Hitbox")]
    public Transform hitboxTransform;
    public CerberoHitboxAtaque hitboxAtaque;
    public float hitboxDireitaX = 0.283f;
    public float hitboxEsquerdaX = -0.384f;

    [Header("Movimento")]
    public float velocidade = 3f;
    public float distanciaDeteccao = 6f;
    public float distanciaAtaque = 1.5f;

    [Header("Ataque")]
    public float tempoEntreAtaques = 1.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private ElianVida vidaElian;

    private float tempoProximoAtaque;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vidaElian = elian.GetComponent<ElianVida>();
    }

    private void Update()
{
    if (elian == null)
        return;

    if (vidaElian != null && vidaElian.EstaMorto)
    {
        Parar();
        animator.ResetTrigger("Atacar");
        return;
    }

    float distancia = Vector2.Distance(transform.position, elian.position);

    if (distancia > distanciaDeteccao)
    {
        Parar();
        return;
    }

    VirarParaElian();

    if (distancia <= distanciaAtaque)
    {
        Parar();
        TentarAtacar();
    }
    else
    {
        Perseguir();
    }
}

    public void AtivarHitbox()
    {
        hitboxAtaque.AtivarHitbox();
    }

    public void DesativarHitbox()
    {
        hitboxAtaque.DesativarHitbox();
    }

    private void Perseguir()
    {
        animator.SetBool("Correndo", true);

        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );
    }

    private void Parar()
    {
        animator.SetBool("Correndo", false);

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    private void TentarAtacar()
    {
        if (Time.time < tempoProximoAtaque)
            return;

        animator.SetTrigger("Atacar");

        tempoProximoAtaque = Time.time + tempoEntreAtaques;
    }

    private void VirarParaElian()
    {
        Vector3 posicaoHitbox = hitboxTransform.localPosition;

        if (elian.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
            posicaoHitbox.x = hitboxEsquerdaX;
        }
        else
        {
            spriteRenderer.flipX = false;
            posicaoHitbox.x = hitboxDireitaX;
        }

        hitboxTransform.localPosition = posicaoHitbox;
    }

    private void Start()
{
    if (elian != null)
        vidaElian = elian.GetComponent<ElianVida>();
}
}
using UnityEngine;

public class GolemMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;

    [Header("Movimento")]
    public float velocidade = 2f;
    public float distanciaDeteccao = 6f;
    public float distanciaAtaque = 1.6f;

    [Header("Posição da Hitbox")]
    public Transform hitboxTransform;
    public float hitboxDireitaX = 0f;
    public float hitboxEsquerdaX = -0.621f;

    [Header("Ataque")]
    public float tempoEntreAtaques = 1.8f;

    [Header("Hitbox")]
    public GolemHitboxAtaque hitboxAtaque;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float tempoProximoAtaque;
    private bool atacando;
    private ElianVida vidaElian;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
{
    if (elian != null)
        vidaElian = elian.GetComponent<ElianVida>();
}

    private void Update()
{
    if (elian == null)
        return;
        if (vidaElian != null && vidaElian.EstaMorto)
{
    CancelarAtaque();
    Parar();
    return;
}

    if (atacando)
    {
        Parar();
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

    public void CancelarAtaque()
{
    atacando = false;

    animator.ResetTrigger("Atacar");

    if (rb != null)
        rb.linearVelocity = Vector2.zero;

    if (hitboxAtaque != null)
        hitboxAtaque.DesativarHitbox();
}

    private void TentarAtacar()
{
    if (Time.time < tempoProximoAtaque)
        return;

    atacando = true;

    Parar();

    animator.SetTrigger("Atacar");

    tempoProximoAtaque = Time.time + tempoEntreAtaques;
}
public void FinalizarAtaque()
{
    atacando = false;
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
}
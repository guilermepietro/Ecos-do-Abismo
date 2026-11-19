using UnityEngine;

public class CabecaDeChamaMovimento : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2.5f;
    public float distanciaPerseguir = 6f;

    [Header("Ataque")]
    public float distanciaAtaque = 1.5f;
    public float tempoEntreAtaques = 1.2f;
    public Transform hitboxAtaque;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform elian;

    private ElianVida vidaElian;
    private KnockbackInimigo knockback;

    private float proximoAtaque;

    private AudioMob audioMob;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<KnockbackInimigo>();

        audioMob = GetComponentInParent<AudioMob>();

        GameObject jogador = GameObject.Find("elian");

        if (jogador != null)
        {
            elian = jogador.transform;
            vidaElian = jogador.GetComponent<ElianVida>();
        }
    }

    void FixedUpdate()
    {
        if (elian == null)
            return;

        // Elian morreu
        if (vidaElian != null && vidaElian.EstaMorto)
        {
            Parar();

            animator.ResetTrigger("Atacar");

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.Play("Idle");
            }

            return;
        }

        // Está sofrendo knockback
        if (knockback != null && knockback.EstaEmKnockback)
        {
            animator.SetBool("Andando", false);
            return;
        }

        AnimatorStateInfo estado =
            animator.GetCurrentAnimatorStateInfo(0);

        // Não se move durante essas animações
        if (estado.IsName("Damage") ||
            estado.IsName("Death") ||
            estado.IsName("Attack"))
        {
            Parar();
            return;
        }

        float distancia = Vector2.Distance(
            transform.position,
            elian.position
        );

        // ELian está perto o suficiente para atacar
        if (distancia <= distanciaAtaque)
        {
            VirarParaElian();

            Parar();

            if (Time.time >= proximoAtaque)
            {
                animator.SetTrigger("Atacar");

                if (audioMob != null)
                {
                    audioMob.TocarAtaque();
                }

                proximoAtaque =
                    Time.time + tempoEntreAtaques;
            }

            return;
        }

        // Elian entrou na área de perseguição
        if (distancia <= distanciaPerseguir)
        {
            SeguirElian();
            return;
        }

        // Elian está longe
        Parar();
    }

    void SeguirElian()
    {
        float direcao =
            Mathf.Sign(elian.position.x - transform.position.x);

        Virar(direcao);

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );

        animator.SetBool("Andando", true);
    }

    void VirarParaElian()
    {
        float direcao =
            Mathf.Sign(elian.position.x - transform.position.x);

        Virar(direcao);
    }

    void Virar(float direcao)
    {
        spriteRenderer.flipX = direcao < 0;

        if (hitboxAtaque != null)
        {
            Vector3 posicaoHitbox =
                hitboxAtaque.localPosition;

            posicaoHitbox.x =
                Mathf.Abs(posicaoHitbox.x) *
                (direcao < 0 ? -1 : 1);

            hitboxAtaque.localPosition =
                posicaoHitbox;
        }
    }

    void Parar()
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        animator.SetBool("Andando", false);
    }

    public void AcertarAtaque()
    {
        CabecaDeChamaAtaque ataque =
            GetComponentInChildren<CabecaDeChamaAtaque>();

        if (ataque != null)
        {
            ataque.VerificarAcerto();
        }
    }
}
using UnityEngine;
using System.Collections;

public class ElianAtaque : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool atacando = false;
    private bool comboAtaque2 = false;
    private bool comboAtaque3 = false;

    private bool ataqueAreaDisponivel = true;
    private bool ataqueAreaEmExecucao = false;

    private Coroutine avancoAtual;

    [Header("Ataque em Área")]
    public float tempoCooldownArea = 5f;

    [Header("Avanço do Combo")]
    public float velocidadeAvancoAtaque1 = 2f;
    public float velocidadeAvancoAtaque2 = 2.5f;
    public float velocidadeAvancoAtaque3 = 3.5f;

    public float tempoAvanco = 0.08f;

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ATAQUE EM ÁREA
        if (Input.GetKeyDown(KeyCode.K) &&
            animator.GetBool("estaNoChao") &&
            !atacando &&
            ataqueAreaDisponivel)
        {
            animator.SetFloat("velocidade", 0);

            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            movimento.podeMover = false;

            animator.SetTrigger("AtaqueArea");

            ataqueAreaDisponivel = false;
            ataqueAreaEmExecucao = true;
        }

        // COMBO NORMAL
        if (Input.GetKeyDown(KeyCode.J) &&
            animator.GetBool("estaNoChao"))
        {
            if (!atacando)
            {
                animator.ResetTrigger("Ataque2");
                animator.ResetTrigger("Ataque3");

                animator.SetFloat("velocidade", 0);

                rb.linearVelocity = new Vector2(
                    0f,
                    rb.linearVelocity.y
                );

                movimento.podeMover = false;

                animator.SetTrigger("Atacar");

                atacando = true;
            }
            else if (!comboAtaque2)
            {
                comboAtaque2 = true;

                animator.SetTrigger("Ataque2");
            }
            else if (!comboAtaque3)
            {
                comboAtaque3 = true;

                animator.SetTrigger("Ataque3");
            }
        }
    }

    // ======================================================
    // INFORMA SE O ELIAN ESTÁ NO COMBO
    // ======================================================

    public bool EstaNoCombo()
    {
        return atacando;
    }

    // ======================================================
    // CANCELA COMBO PARA DAR DASH
    // ======================================================

    public void CancelarComboPorDash()
    {
        if (!atacando)
        {
            return;
        }

        atacando = false;
        comboAtaque2 = false;
        comboAtaque3 = false;

        animator.ResetTrigger("Atacar");
        animator.ResetTrigger("Ataque2");
        animator.ResetTrigger("Ataque3");

        // CANCELA AVANÇO DO GOLPE
        if (avancoAtual != null)
        {
            StopCoroutine(avancoAtual);
            avancoAtual = null;
        }

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        // DESLIGA HITBOX
        HitboxControle hitboxControle =
            GetComponent<HitboxControle>();

        if (hitboxControle != null)
        {
            hitboxControle.DesativarHitbox();
        }

        movimento.podeMover = true;
    }

    // ======================================================
    // AVANÇO DOS ATAQUES
    // ======================================================

    public void AvancoAtaque1()
    {
        IniciarAvanco(velocidadeAvancoAtaque1);
    }

    public void AvancoAtaque2()
    {
        IniciarAvanco(velocidadeAvancoAtaque2);
    }

    public void AvancoAtaque3()
    {
        IniciarAvanco(velocidadeAvancoAtaque3);
    }

    void IniciarAvanco(float velocidadeAvanco)
    {
        if (avancoAtual != null)
        {
            StopCoroutine(avancoAtual);
        }

        avancoAtual = StartCoroutine(
            ExecutarAvanco(velocidadeAvanco)
        );
    }

    IEnumerator ExecutarAvanco(float velocidadeAvanco)
    {
        float direcao = spriteRenderer.flipX ? -1f : 1f;

        rb.linearVelocity = new Vector2(
            direcao * velocidadeAvanco,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(tempoAvanco);

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        avancoAtual = null;
    }

    // ======================================================
    // ATAQUE EM ÁREA
    // ======================================================

    public void AtivarHitboxArea()
    {
        Transform hitbox = transform.Find("HitboxArea");

        if (hitbox != null)
        {
            hitbox.gameObject.SetActive(true);
        }
    }

    public void DesativarHitboxArea()
    {
        Transform hitbox = transform.Find("HitboxArea");

        if (hitbox != null)
        {
            hitbox.gameObject.SetActive(false);
        }
    }

    public void VerificarDanoArea()
    {
        Transform hitbox = transform.Find("HitboxArea");

        if (hitbox != null)
        {
            HitboxArea area = hitbox.GetComponent<HitboxArea>();

            if (area != null)
            {
                area.VerificarAcerto();
            }
        }
    }

    public void VerificarAcertoArea()
    {
        Transform hitbox = transform.Find("HitboxArea");

        if (hitbox != null)
        {
            HitboxArea area = hitbox.GetComponent<HitboxArea>();

            if (area != null)
            {
                area.VerificarAcerto();
            }
        }
    }

    // ======================================================
    // FINALIZAÇÃO DO COMBO
    // ======================================================

    public void FinalizarAtaque1()
    {
        if (!comboAtaque2)
        {
            atacando = false;

            movimento.podeMover = true;
        }
    }

    public void FinalizarAtaque2()
    {
        if (!comboAtaque3)
        {
            atacando = false;
            comboAtaque2 = false;

            movimento.podeMover = true;
        }
    }

    public void FinalizarAtaque3()
    {
        atacando = false;
        comboAtaque2 = false;
        comboAtaque3 = false;

        animator.ResetTrigger("Ataque2");
        animator.ResetTrigger("Ataque3");

        movimento.podeMover = true;
    }

    // ======================================================
    // COOLDOWN DO ATAQUE EM ÁREA
    // ======================================================

    private void ReativarAtaqueArea()
    {
        ataqueAreaDisponivel = true;
    }

    public void IniciarCooldownArea()
    {
        ataqueAreaEmExecucao = false;

        movimento.podeMover = true;

        CancelInvoke(nameof(ReativarAtaqueArea));

        Invoke(
            nameof(ReativarAtaqueArea),
            tempoCooldownArea
        );
    }

    // ======================================================
    // CANCELAMENTO POR DANO
    // ======================================================

    public void CancelarAtaquePorDano()
    {
        atacando = false;
        comboAtaque2 = false;
        comboAtaque3 = false;

        animator.ResetTrigger("Atacar");
        animator.ResetTrigger("Ataque2");
        animator.ResetTrigger("Ataque3");

        movimento.podeMover = true;

        if (avancoAtual != null)
        {
            StopCoroutine(avancoAtual);
            avancoAtual = null;
        }

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        HitboxControle hitboxControle =
            GetComponent<HitboxControle>();

        if (hitboxControle != null)
        {
            hitboxControle.DesativarHitbox();
        }

        if (ataqueAreaEmExecucao)
        {
            ataqueAreaEmExecucao = false;

            animator.ResetTrigger("AtaqueArea");

            DesativarHitboxArea();

            CancelInvoke(nameof(ReativarAtaqueArea));

            Invoke(
                nameof(ReativarAtaqueArea),
                tempoCooldownArea
            );
        }
    }
}
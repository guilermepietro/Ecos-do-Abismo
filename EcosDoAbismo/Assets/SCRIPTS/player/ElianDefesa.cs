
using UnityEngine;
using System.Collections;

public class ElianDefesa : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;

    private bool defendendo = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("Impacto do Escudo")]
    public float forcaKnockbackEscudo = 3f;

    [Header("Escudo")]
    public int vidaMaximaEscudo = 50;
    private int vidaEscudo;
    [Header("Quebra do Escudo")]
public int danoAoQuebrar = 5;

    [Header("Recarga do Escudo")]
    public float tempoRecargaEscudo = 3f;
    private bool escudoDisponivel = true;

    [Header("Audio - Escudo")]
    public AudioSource audioEscudo;
    public AudioClip somBloqueio;

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vidaEscudo = vidaMaximaEscudo;
    }

    void Update()
    {
        if (ControlesElian.Pressionou(ComandoElian.Defender))
        {
            IniciarDefesa();
        }

        // Finaliza quando a tecla configurada é solta.
        if (defendendo &&
            !ControlesElian.Segurando(ComandoElian.Defender))
        {
            FinalizarDefesa();
        }
    }

    void IniciarDefesa()
    {
        if (!escudoDisponivel)
        {
            return;
        }

        defendendo = true;

        movimento.podeMover = false;

        animator.SetFloat("velocidade", 0);
        animator.SetBool("Defendendo", true);
    }

    void FinalizarDefesa()
    {
        defendendo = false;

        animator.SetBool("Defendendo", false);

        movimento.podeMover = true;
    }

    public int ReceberDanoEscudo(int dano)
    {
        if (!defendendo)
        {
            return dano;
        }

        if (audioEscudo != null && somBloqueio != null)
        {
            audioEscudo.PlayOneShot(somBloqueio);
        }

        ReceberImpactoEscudo();

        vidaEscudo -= dano;

        if (vidaEscudo > 0)
        {
            animator.SetTrigger("ImpactoEscudo");

            Debug.Log("Vida do escudo: " + vidaEscudo);

            return 0;
        }

        vidaEscudo = 0;

        Debug.Log("Escudo quebrado!");

        QuebrarDefesa();

        return danoAoQuebrar;
    }

    public void ReceberImpactoEscudo()
    {
        float direcao = spriteRenderer.flipX ? 1f : -1f;

        rb.linearVelocity = new Vector2(
            direcao * forcaKnockbackEscudo,
            rb.linearVelocity.y
        );
    }

    public void QuebrarDefesa()
    {
        defendendo = false;
        escudoDisponivel = false;

        animator.SetBool("Defendendo", false);

        movimento.podeMover = true;

        StartCoroutine(RecarregarEscudo());
    }

    IEnumerator RecarregarEscudo()
    {
        yield return new WaitForSeconds(tempoRecargaEscudo);

        vidaEscudo = vidaMaximaEscudo;
        escudoDisponivel = true;

        Debug.Log("Escudo recuperado!");
    }

    public bool EstaDefendendo()
    {
        return defendendo;
    }
}

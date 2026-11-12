
using System.Collections;
using UnityEngine;

public class MorcegoVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;

    [Header("Dano")]
    public float tempoDano = 0.5f;

    private int vidaAtual;
    private Animator animator;
    private Rigidbody2D rb;
    private MorcegoMovimento movimento;

    private bool morreu;
    private bool tomandoDano;

    // NOVO
    private AudioMob audioMob;

    private void Awake()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movimento = GetComponent<MorcegoMovimento>();

        // NOVO
        audioMob = GetComponentInParent<AudioMob>();
    }

    public void ReceberDano(int dano)
    {
        if (morreu || tomandoDano)
            return;

        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        // NOVO: som de dano não fatal
        if (audioMob != null)
        {
            audioMob.TocarDano();
        }

        StartCoroutine(Dano());
    }

    private IEnumerator Dano()
    {
        tomandoDano = true;

        if (movimento != null)
        {
            movimento.CancelarAtaque();
            movimento.enabled = false;
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        animator.ResetTrigger("Atacar");
        animator.SetTrigger("Dano");

        yield return new WaitForSeconds(tempoDano);

        if (!morreu && movimento != null)
            movimento.enabled = true;

        tomandoDano = false;
    }

    private void Morrer()
    {
        morreu = true;

        StopAllCoroutines();

        if (movimento != null)
        {
            movimento.CancelarAtaque();
            movimento.enabled = false;
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        animator.ResetTrigger("Atacar");
        animator.ResetTrigger("Dano");
        animator.SetTrigger("Morrer");

        // NOVO: som de morte
        if (audioMob != null)
        {
            audioMob.TocarMorte();
        }
    }
}

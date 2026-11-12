
using UnityEngine;
using System.Collections;

public class GosmaHumanoideVida : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Animator animator;
    private Rigidbody2D rb;
    private GosmaHumanoideAtaque ataque;

    public bool EstaMorto { get; private set; }
    public bool EstaTomandoDano { get; private set; }

    // NOVO: áudio universal
    private AudioMob audioMob;

    void Start()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ataque = GetComponent<GosmaHumanoideAtaque>();

        // NOVO
        audioMob = GetComponentInParent<AudioMob>();
    }

    public void ReceberDano(int dano)
    {
        if (EstaMorto)
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

        StartCoroutine(TomarDano());
    }

    IEnumerator TomarDano()
    {
        EstaTomandoDano = true;

        ataque.CancelarAtaque();

        animator.ResetTrigger("Atacar");
        animator.SetTrigger("Dano");

        yield return new WaitForSeconds(0.35f);

        EstaTomandoDano = false;
    }

    void Morrer()
    {
        EstaMorto = true;
        EstaTomandoDano = false;

        ataque.CancelarAtaque();

        rb.linearVelocity = Vector2.zero;

        animator.ResetTrigger("Atacar");
        animator.SetTrigger("Morrer");

        // NOVO: som de morte
        if (audioMob != null)
        {
            audioMob.TocarMorte();
        }
    }
}

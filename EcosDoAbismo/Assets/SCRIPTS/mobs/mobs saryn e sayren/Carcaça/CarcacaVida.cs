using UnityEngine;
using System.Collections;

public class CarcacaVida : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Animator animator;
    private Rigidbody2D rb;
    private CarcacaAtaque ataque;

    public bool EstaMorto { get; private set; }
    public bool EstaTomandoDano { get; private set; }

    void Start()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ataque = GetComponent<CarcacaAtaque>();
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

        StartCoroutine(TomarDano());
    }

    IEnumerator TomarDano()
{
    EstaTomandoDano = true;

    ataque.CancelarAtaque();

    rb.linearVelocity = new Vector2(
        0f,
        rb.linearVelocity.y
    );

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
    }
}
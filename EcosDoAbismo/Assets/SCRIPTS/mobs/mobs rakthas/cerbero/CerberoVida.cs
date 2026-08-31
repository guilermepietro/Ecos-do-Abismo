using System.Collections;
using UnityEngine;

public class CerberoVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;

    [Header("Dano")]
    public float tempoDano = 0.5f;

    private int vidaAtual;
    private Animator animator;
    private CerberoMovimento movimento;
    private Rigidbody2D rb;
    private CerberoHitboxAtaque hitboxAtaque;

    private bool morreu;
    private bool tomandoDano;

    private void Awake()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
        movimento = GetComponent<CerberoMovimento>();
        rb = GetComponent<Rigidbody2D>();
        hitboxAtaque = GetComponentInChildren<CerberoHitboxAtaque>();
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

        StartCoroutine(Dano());
    }

    private IEnumerator Dano()
    {
        tomandoDano = true;

        if (movimento != null)
            movimento.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }

        animator.SetBool("Correndo", false);
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

    animator.SetBool("Correndo", false);
    animator.ResetTrigger("Atacar");
    animator.ResetTrigger("Dano");
    animator.SetTrigger("Morrer");

    if (movimento != null)
        movimento.enabled = false;

    if (rb != null)
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    if (hitboxAtaque != null)
    hitboxAtaque.DesativarHitbox();
}
}
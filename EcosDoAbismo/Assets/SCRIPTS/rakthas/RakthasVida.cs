using UnityEngine;

public class RakthasVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 300;

    private int vidaAtual;
    private bool morto = false;

    private bool tomandoDano = false;

    public bool EstaTomandoDano => tomandoDano;

    [Header("Referências")]
    public Animator animator;

    public bool EstaMorto => morto;

    private void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int dano)
{
    if (morto)
        return;

    vidaAtual -= dano;

    if (vidaAtual <= 0)
    {
        Morrer();
        return;
    }

    tomandoDano = true;

    Rigidbody2D rb = GetComponent<Rigidbody2D>();

    if (rb != null)
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

    animator.SetBool("estaCorrendo", false);
    animator.speed = 1f;
    animator.SetTrigger("Dano");
}

    private void Morrer()
    {
        morto = true;

        animator.speed = 1f;
        animator.SetBool("estaCorrendo", false);
        animator.SetTrigger("Morte");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public void FinalizarDano()
{
    tomandoDano = false;
}
}
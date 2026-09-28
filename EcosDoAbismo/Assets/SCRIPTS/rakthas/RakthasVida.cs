using UnityEngine;

public class RakthasVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 300;

    private int vidaAtual;
    private bool morto = false;
    private bool tomandoDano = false;

    private RakthasAtaques ataques;

    [Header("Referências")]
    public Animator animator;

    public bool EstaTomandoDano => tomandoDano;
    public bool EstaMorto => morto;

    private void Start()
    {
        vidaAtual = vidaMaxima;
        ataques = GetComponent<RakthasAtaques>();
    }

    public void ReceberDano(int dano)
    {
        if (morto)
            return;

        vidaAtual -= dano;

        // MORTE
        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        // SE ESTÁ ATACANDO:
        // recebe dano, mas não interrompe o ataque
        if (ataques != null && ataques.EstaAtacando)
        {
            return;
        }

        // DANO NORMAL
        tomandoDano = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }

        animator.SetBool("estaCorrendo", false);
        animator.speed = 1f;
        animator.SetTrigger("Dano");
    }

    private void Morrer()
    {
        morto = true;
        tomandoDano = false;

        // NA MORTE, CANCELA QUALQUER ATAQUE
        if (ataques != null)
        {
            ataques.CancelarAtaque();
        }

        animator.speed = 1f;
        animator.SetBool("estaCorrendo", false);
        animator.SetTrigger("Morte");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }
    }

    public void FinalizarDano()
    {
        tomandoDano = false;
    }
}
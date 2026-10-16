using UnityEngine;

public class VoragulVida : MonoBehaviour
{
    public int vidaMaxima = 500;

    private int vidaAtual;
    private bool morto = false;
    private bool levandoDano = false;

    private Animator animator;

    private void Awake()
    {
        vidaAtual = vidaMaxima;
        animator = GetComponent<Animator>();
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

        // Se estiver executando o Ataque 1,
        // recebe o dano, mas não interrompe a animação.
        VoragulAtaque1 ataque1 = GetComponent<VoragulAtaque1>();

        if (ataque1 != null && ataque1.EstaAtacando())
        {
            return;
        }

        // Se estiver executando o Ataque 2,
        // recebe o dano, mas não interrompe a animação.
        VoragulAtaque2 ataque2 = GetComponent<VoragulAtaque2>();

        if (ataque2 != null && ataque2.EstaAtacando())
        {
            return;
        }

        if (!levandoDano)
        {
            levandoDano = true;

            VoragulMovimento movimento = GetComponent<VoragulMovimento>();

            if (movimento != null)
            {
                movimento.podeMover = false;
            }

            animator.SetTrigger("Dano");
        }
    }

    private void Morrer()
    {
        morto = true;

        CancelInvoke(nameof(LiberarMovimento));

        VoragulMovimento movimento = GetComponent<VoragulMovimento>();

        if (movimento != null)
        {
            movimento.podeMover = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }

        animator.SetTrigger("Morte");
    }

    public void FinalizarDano()
    {
        levandoDano = false;

        Invoke(nameof(LiberarMovimento), 1.1f);
    }

    private void LiberarMovimento()
    {
        if (morto)
            return;

        VoragulMovimento movimento = GetComponent<VoragulMovimento>();

        if (movimento != null)
        {
            movimento.podeMover = true;
        }
    }

    public bool EstaMorto()
    {
        return morto;
    }
}
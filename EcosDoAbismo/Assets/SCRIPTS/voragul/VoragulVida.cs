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
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    animator.SetTrigger("Morte");
}

    public void FinalizarDano()
    {
        levandoDano = false;
        Invoke(nameof(LiberarMovimento), 2.5f);
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
using UnityEngine;

public class VoragulVida : MonoBehaviour
{
    public int vidaMaxima = 500;

    private int vidaAtual;
    private Animator animator;

    private void Awake()
    {
        vidaAtual = vidaMaxima;
        animator = GetComponent<Animator>();
    }

    public void ReceberDano(int dano)
    {
        vidaAtual -= dano;

        VoragulMovimento movimento = GetComponent<VoragulMovimento>();

if (movimento != null)
{
    movimento.podeMover = false;
}

        animator.SetTrigger("Dano");

        Debug.Log("Voragul recebeu " + dano + " de dano. Vida atual: " + vidaAtual);
    }

    public void FinalizarDano()
{
    Invoke(nameof(LiberarMovimento), 1.1f);
}

private void LiberarMovimento()
{
    VoragulMovimento movimento = GetComponent<VoragulMovimento>();

    if (movimento != null)
    {
        movimento.podeMover = true;
    }
}
}
using UnityEngine;

public class SombraVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;

    [Header("Referências")]
    public Animator animator;
    public SombraMovimento movimento;

    private int vidaAtual;
    private bool morto = false;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int dano)
    {
        if (morto)
            return;

        vidaAtual -= dano;

        movimento.PararPorDano();

        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        animator.SetTrigger("Damage");
    }

    void Morrer()
    {
        morto = true;

        movimento.Morrer();

        animator.SetBool("Run", false);
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Death");
    }

    public void Desaparecer()
    {
        Destroy(gameObject);
    }
}
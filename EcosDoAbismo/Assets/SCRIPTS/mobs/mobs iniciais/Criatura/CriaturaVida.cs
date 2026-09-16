using UnityEngine;

public class CriaturaVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;

    [Header("Referências")]
    public Animator animator;
    public CriaturaAtaque ataque;

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

        ataque.Morrer();

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Death");
    }

    public void Desaparecer()
    {
        Destroy(gameObject);
    }
}
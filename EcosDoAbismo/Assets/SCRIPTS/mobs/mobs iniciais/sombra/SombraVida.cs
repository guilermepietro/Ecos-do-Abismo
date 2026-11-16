
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

    // Permite identificar quando a Sombra morreu
    public bool EstaMorto => morto;

    private AudioMob audioMob;

    void Start()
    {
        vidaAtual = vidaMaxima;

        audioMob = GetComponentInParent<AudioMob>();
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

        if (audioMob != null)
        {
            audioMob.TocarDano();
        }
    }

    void Morrer()
    {
        morto = true;

        movimento.Morrer();

        animator.SetBool("Run", false);
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Death");

        if (audioMob != null)
        {
            audioMob.TocarMorte();
        }
    }

    public void Desaparecer()
    {
        Destroy(gameObject);
    }
}

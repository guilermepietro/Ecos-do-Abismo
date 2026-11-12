
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

    // NOVO
    private AudioMob audioMob;

    void Start()
    {
        vidaAtual = vidaMaxima;

        // NOVO
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

        // NOVO: som de dano
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

        // NOVO: som de morte
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


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

        ataque.Morrer();

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

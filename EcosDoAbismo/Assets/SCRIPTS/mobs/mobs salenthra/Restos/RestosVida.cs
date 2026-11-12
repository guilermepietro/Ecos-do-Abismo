
using UnityEngine;

public class RestosVida : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Restos restos;
    private bool morto;

    // NOVO: áudio universal
    private AudioMob audioMob;

    void Start()
    {
        vidaAtual = vidaMaxima;
        restos = GetComponent<Restos>();

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
            vidaAtual = 0;
            morto = true;

            restos.animator.SetBool("Andando", false);
            restos.animator.SetTrigger("Morrer");
            restos.Morrer();

            RestosDanoContato danoContato =
                GetComponent<RestosDanoContato>();

            if (danoContato != null)
            {
                danoContato.Morrer();
            }

            // NOVO: som de morte
            if (audioMob != null)
            {
                audioMob.TocarMorte();
            }

            return;
        }

        restos.animator.SetTrigger("Dano");

        // NOVO: som de dano não fatal
        if (audioMob != null)
        {
            audioMob.TocarDano();
        }
    }
}

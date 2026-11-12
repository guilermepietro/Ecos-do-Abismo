
using UnityEngine;

public class EstatuaVida : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Estatua estatua;

    // NOVO: áudio universal
    private AudioMob audioMob;

    void Start()
    {
        vidaAtual = vidaMaxima;
        estatua = GetComponent<Estatua>();

        // NOVO
        audioMob = GetComponentInParent<AudioMob>();
    }

    public void ReceberDano(int dano)
    {
        if (vidaAtual <= 0)
            return;

        vidaAtual -= dano;

        if (vidaAtual > 0)
        {
            estatua.animator.SetTrigger("Dano");

            // NOVO: som de dano não fatal
            if (audioMob != null)
            {
                audioMob.TocarDano();
            }
        }
        else
        {
            vidaAtual = 0;

            estatua.animator.SetTrigger("Morrer");
            estatua.Morrer();

            // NOVO: som de morte
            if (audioMob != null)
            {
                audioMob.TocarMorte();
            }

            DesaparecerAoMorrer desaparecer =
                GetComponent<DesaparecerAoMorrer>();

            if (desaparecer != null)
            {
                desaparecer.IniciarMorte();
            }
        }
    }
}

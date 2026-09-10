using UnityEngine;

public class EstatuaVida : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Estatua estatua;

    void Start()
    {
        vidaAtual = vidaMaxima;
        estatua = GetComponent<Estatua>();
    }

    public void ReceberDano(int dano)
    {
        if (vidaAtual <= 0)
            return;

        vidaAtual -= dano;

        if (vidaAtual > 0)
        {
            estatua.animator.SetTrigger("Dano");
        }
        else
{
    vidaAtual = 0;

    estatua.animator.SetTrigger("Morrer");
    estatua.Morrer();

    DesaparecerAoMorrer desaparecer = GetComponent<DesaparecerAoMorrer>();

    if (desaparecer != null)
    {
        desaparecer.IniciarMorte();
    }
}
    }
}
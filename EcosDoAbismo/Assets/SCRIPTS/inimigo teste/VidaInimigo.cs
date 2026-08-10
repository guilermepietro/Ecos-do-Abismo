using UnityEngine;

public class VidaInimigo : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int dano)
    {
        vidaAtual -= dano;

        Debug.Log("Vida do inimigo: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        Debug.Log("INIMIGO MORREU!");
    }

    
}
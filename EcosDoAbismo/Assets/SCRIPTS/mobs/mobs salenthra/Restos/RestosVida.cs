using UnityEngine;

public class RestosVida : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Restos restos;
    private bool morto;

    void Start()
    {
        vidaAtual = vidaMaxima;
        restos = GetComponent<Restos>();
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

            RestosDanoContato danoContato = GetComponent<RestosDanoContato>();

            if (danoContato != null)
            {
                danoContato.Morrer();
            }

            return;
        }

        restos.animator.SetTrigger("Dano");
    }
}
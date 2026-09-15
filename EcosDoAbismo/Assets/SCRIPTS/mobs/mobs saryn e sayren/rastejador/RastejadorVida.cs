using UnityEngine;
using System.Collections;

public class RastejadorVida : MonoBehaviour
{
    public int vidaMaxima = 100;
    public float tempoParadoAposDano = 1f;

    private int vidaAtual;
    private Animator animator;
    private RastejadorMovimento movimento;

    private Coroutine coroutineDano;

    public bool EstaMorto { get; private set; }
    public bool EstaTomandoDano { get; private set; }

    void Start()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
        movimento = GetComponent<RastejadorMovimento>();
    }

    public void ReceberDano(int dano)
    {
        if (EstaMorto)
            return;

        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        if (coroutineDano != null)
            StopCoroutine(coroutineDano);

        coroutineDano = StartCoroutine(TomarDano());
    }

    IEnumerator TomarDano()
    {
        EstaTomandoDano = true;

        if (movimento != null)
            movimento.PodeMover = false;

        animator.SetBool("Correndo", false);

        animator.ResetTrigger("Dano");
        animator.SetTrigger("Dano");

        yield return new WaitForSeconds(tempoParadoAposDano);

        EstaTomandoDano = false;

        if (movimento != null)
            movimento.PodeMover = true;

        coroutineDano = null;
    }

    void Morrer()
    {
        EstaMorto = true;
        EstaTomandoDano = false;

        if (coroutineDano != null)
        {
            StopCoroutine(coroutineDano);
            coroutineDano = null;
        }

        if (movimento != null)
            movimento.PodeMover = false;

        animator.SetBool("Correndo", false);
        animator.ResetTrigger("Dano");
        animator.SetTrigger("Morrer");
    }
}
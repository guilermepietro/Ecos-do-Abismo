using UnityEngine;
using System.Collections;

public class EsqueleticoVida : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Animator animator;

    public bool EstaMorto { get; private set; }
    public bool EstaTomandoDano { get; private set; }

    void Start()
    {
        vidaAtual = vidaMaxima;
        animator = GetComponent<Animator>();
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

        StartCoroutine(TomarDano());
    }

    IEnumerator TomarDano()
    {
        EstaTomandoDano = true;

        animator.ResetTrigger("Atacar");
        animator.SetTrigger("Dano");

        yield return new WaitForSeconds(0.35f);

        EstaTomandoDano = false;
    }

    void Morrer()
    {
        EstaMorto = true;
        EstaTomandoDano = false;

        animator.ResetTrigger("Atacar");
        animator.SetTrigger("Morrer");
    }
}
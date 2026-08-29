using System.Collections;
using UnityEngine;

public class CabecaDeChamaVida : MonoBehaviour
{
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Animator animator;
    private Rigidbody2D rb;

    public bool EstaTomandoDano { get; private set; }
    public bool EstaMorto { get; private set; }

    void Start()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void ReceberDano(int dano)
    {
        if (EstaMorto)
            return;

    Debug.Log("Cabeça de Chama recebeu dano: " + dano);

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

    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    animator.SetTrigger("Dano");

    yield return new WaitForSeconds(0.35f);

    EstaTomandoDano = false;
}

    void Morrer()
    {
        EstaMorto = true;
        EstaTomandoDano = false;

        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("Morrer");
    }
}
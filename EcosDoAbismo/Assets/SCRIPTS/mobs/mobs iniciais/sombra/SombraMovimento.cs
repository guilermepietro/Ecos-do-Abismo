using UnityEngine;

public class SombraMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Movimento")]
    public float velocidade = 2.5f;
    public float distanciaPerseguir = 6f;
    public float distanciaAtaque = 3.5f;

    [Header("Ataque")]
    public float tempoEntreAtaques = 1.5f;

    [Header("Dano")]
    public float tempoParadoAposDano = 0.5f;

    private float proximoAtaque = 0f;
    private float tempoLiberarMovimento = 0f;

    private bool atacando = false;
    private bool paradoPorDano = false;
    private bool morto = false;

    void Update()
    {
        if (morto || elian == null)
            return;

        if (paradoPorDano)
        {
            Parar();

            if (Time.time >= tempoLiberarMovimento)
            {
                paradoPorDano = false;
            }

            return;
        }

        if (atacando)
        {
            Parar();
            return;
        }

        float distancia = Vector2.Distance(transform.position, elian.position);

        if (distancia <= distanciaAtaque)
        {
            Parar();

            if (Time.time >= proximoAtaque)
            {
                Atacar();
            }
        }
        else if (distancia <= distanciaPerseguir)
        {
            SeguirElian();
        }
        else
        {
            Parar();
        }
    }

    void SeguirElian()
    {
        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );

        animator.SetBool("Run", true);

        if (direcao > 0)
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        else
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
    }

    void Parar()
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        animator.SetBool("Run", false);
    }

    void Atacar()
    {
        atacando = true;

        Parar();

        animator.SetTrigger("Attack");

        proximoAtaque = Time.time + tempoEntreAtaques;
    }

    public void FinalizarAtaque()
    {
        atacando = false;
    }

    public void PararPorDano()
    {
        paradoPorDano = true;

        tempoLiberarMovimento =
            Time.time + tempoParadoAposDano;

        atacando = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        animator.SetBool("Run", false);
        animator.ResetTrigger("Attack");
    }

    public void Morrer()
{
    morto = true;
    atacando = false;
    paradoPorDano = false;

    rb.linearVelocity = Vector2.zero;

    animator.SetBool("Run", false);
    animator.ResetTrigger("Attack");
}
}
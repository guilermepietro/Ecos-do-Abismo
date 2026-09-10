using UnityEngine;

public class Restos : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    public Animator animator;
    public Transform pontoA;
    public Transform pontoB;

    [Header("Movimento")]
    public float velocidade = 2.5f;

    private bool indoParaB = true;
    private bool morto;
    private bool recebendoDano;

    void Update()
    {
        if (morto || recebendoDano || elian == null)
            return;

        float limiteEsquerdo = Mathf.Min(pontoA.position.x, pontoB.position.x);
        float limiteDireito = Mathf.Max(pontoA.position.x, pontoB.position.x);

        bool elianDentroDaArea =
            elian.position.x >= limiteEsquerdo &&
            elian.position.x <= limiteDireito;

        if (elianDentroDaArea)
        {
            SeguirElian();
        }
        else
        {
            Patrulhar();
        }
    }

    void Patrulhar()
    {
        animator.SetBool("Andando", true);

        Transform destino = indoParaB ? pontoB : pontoA;

        float direcao = destino.position.x > transform.position.x ? 1f : -1f;

        transform.position += new Vector3(
            direcao * velocidade * Time.deltaTime,
            0f,
            0f
        );

        Virar(direcao);

        if (Mathf.Abs(transform.position.x - destino.position.x) < 0.1f)
        {
            indoParaB = !indoParaB;
        }
    }

    void SeguirElian()
    {
        animator.SetBool("Andando", true);

        float direcao = elian.position.x > transform.position.x ? 1f : -1f;

        transform.position += new Vector3(
            direcao * velocidade * Time.deltaTime,
            0f,
            0f
        );

        Virar(direcao);
    }

    void Virar(float direcao)
{
    SpriteRenderer sprite = GetComponent<SpriteRenderer>();

    if (sprite != null)
    {
        sprite.flipX = direcao > 0;
    }
}

public void IniciarDano()
{
    recebendoDano = true;
    animator.SetBool("Andando", false);
}

public void FinalizarDano()
{
    recebendoDano = false;
}

public void Morrer()
{
    morto = true;
    animator.SetBool("Andando", false);
}
}
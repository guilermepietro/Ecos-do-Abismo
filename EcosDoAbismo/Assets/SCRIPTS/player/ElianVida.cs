using UnityEngine;
using System.Collections;

public class ElianVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Animator animator;
    private ElianDefesa defesa;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    public bool EstaMorto { get; private set; }

    [Header("Dano")]
public float forcaEmpurrao = 5f;
public float forcaEmpurraoVertical = 4f;

    private bool invencivel = false;
    private bool invencivelDash = false;

public float tempoInvencibilidade = 1f;

private ElianMovimento movimento;

    void Start()
{
    vidaAtual = vidaMaxima;

    animator = GetComponent<Animator>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    rb = GetComponent<Rigidbody2D>();
    movimento = GetComponent<ElianMovimento>();
    defesa = GetComponent<ElianDefesa>();
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ReceberDano(20);
        }
    }

    public void ReceberDano(int dano)
{
    if (EstaMorto)
{
    return;
}
    if (invencivel || invencivelDash)
{
    return;
}

if (defesa != null && defesa.EstaDefendendo())
{
    dano = defesa.ReceberDanoEscudo(dano);

    if (dano <= 0)
    {
        return;
    }
}

    vidaAtual -= dano;

    Debug.Log("Vida do Elian: " + vidaAtual);

    HitStop.instancia.Executar(0.05f);
    animator.SetTrigger("RecebeuDano");
    AplicarEmpurrao();

    StartCoroutine(TempoInvencibilidade());

    if (vidaAtual <= 0)
    {
        Morrer();
    }
}

    void Morrer()
{
    EstaMorto = true;

    animator.SetTrigger("Morreu");

    Debug.Log("Elian morreu!");
}

public void AtivarInvencibilidadeDash()
{
    invencivelDash = true;
}

public void DesativarInvencibilidadeDash()
{
    invencivelDash = false;
}

    IEnumerator TempoInvencibilidade()
{
    invencivel = true;

    float tempoPiscada = 0.1f;

    float tempoPassado = 0;

    while (tempoPassado < tempoInvencibilidade)
    {
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(tempoPiscada);

        spriteRenderer.enabled = true;

        yield return new WaitForSeconds(tempoPiscada);

        tempoPassado += tempoPiscada * 2;
    }

    spriteRenderer.enabled = true;

    invencivel = false;
}

void AplicarEmpurrao()
{
    movimento.podeMover = false;

    float direcao = spriteRenderer.flipX ? 1 : -1;

    rb.linearVelocity = new Vector2(
    direcao * forcaEmpurrao,
    forcaEmpurraoVertical
);

    StartCoroutine(LiberarMovimento());
}

IEnumerator LiberarMovimento()
{
    yield return new WaitForSeconds(0.2f);

    movimento.podeMover = true;
}

public void Curar(int quantidade)
{
    vidaAtual += quantidade;

    if (vidaAtual > vidaMaxima)
    {
        vidaAtual = vidaMaxima;
    }

    Debug.Log("Elian foi curado! Vida atual: " + vidaAtual);
}
}

using UnityEngine;
using System.Collections;

public class ElianVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;

    private int vidaAtual;
    private Animator animator;
    
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    [Header("Dano")]
public float forcaEmpurrao = 5f;
public float forcaEmpurraoVertical = 4f;

    private bool invencivel = false;

public float tempoInvencibilidade = 1f;

private ElianMovimento movimento;

    void Start()
{
    vidaAtual = vidaMaxima;

    animator = GetComponent<Animator>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    rb = GetComponent<Rigidbody2D>();
    movimento = GetComponent<ElianMovimento>();
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
    if (invencivel)
    {
        return;
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
        animator.SetTrigger("Morreu");

        Debug.Log("Elian morreu!");
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
}

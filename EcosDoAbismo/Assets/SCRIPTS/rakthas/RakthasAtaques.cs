using UnityEngine;

public class RakthasAtaques : MonoBehaviour
{
    public GameObject hitboxRaio;
    

    private bool atacando = false;
    private SpriteRenderer spriteRenderer;

    [Header("Posição da Hitbox do Raio")]
    public float posicaoXDireita = 0.352f;
    public float posicaoXEsquerda = -1.054f;

    [Header("Hitboxes da Onda")]
    public BoxCollider2D hitboxOndaDireita;
    public BoxCollider2D hitboxOndaEsquerda;

    [Header("Hitboxes do Soco")]
    public BoxCollider2D hitboxSocoDireita;
    public BoxCollider2D hitboxSocoEsquerda;

    [Header("Velocidade do Soco")]
    public float velocidadeInicialSoco = 2f;
    public float velocidadeFinalSoco = 0.6f;

    public bool EstaAtacando => atacando;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    

private void Update()
{
    AtualizarPosicaoHitboxRaio();

    // TESTE DO RAIO
    if (Input.GetKeyDown(KeyCode.M) && !atacando)
    {
        atacando = true;

        Animator animator = GetComponent<Animator>();

        animator.speed = 1f;
        animator.SetBool("estaCorrendo", false);
        animator.SetTrigger("Raio");
    }

    // TESTE DA ONDA DE FOGO
    if (Input.GetKeyDown(KeyCode.N) && !atacando)
    {
        atacando = true;

        Animator animator = GetComponent<Animator>();

        animator.speed = 1f;
        animator.SetBool("estaCorrendo", false);
        animator.SetTrigger("Ataque1");
    }

    // TESTE DO SOCO DE FOGO
    if (Input.GetKeyDown(KeyCode.B) && !atacando)
    {
    atacando = true;

    Animator animator = GetComponent<Animator>();

    animator.speed = velocidadeInicialSoco;
    animator.SetBool("estaCorrendo", false);
    animator.SetTrigger("Ataque3");
    }

    // DESACELERAÇÃO PROGRESSIVA DO SOCO
    if (atacando)
    {
        Animator animator = GetComponent<Animator>();

        AnimatorStateInfo estado =
            animator.GetCurrentAnimatorStateInfo(0);

             if (estado.IsName("Attack3"))
            {
                float progresso = Mathf.Clamp01(
                estado.normalizedTime
                );

                animator.speed = Mathf.Lerp(
                velocidadeInicialSoco,
                velocidadeFinalSoco,
                progresso
                );
            }
    }
}


    private void AtualizarPosicaoHitboxRaio()
    {
        if (hitboxRaio == null || spriteRenderer == null)
            return;

        Vector3 posicao = hitboxRaio.transform.localPosition;

        if (spriteRenderer.flipX)
            posicao.x = posicaoXEsquerda;
        else
            posicao.x = posicaoXDireita;

        hitboxRaio.transform.localPosition = posicao;
    }

    public void AtivarHitboxRaio()
{
    if (hitboxRaio != null)
    {
        hitboxRaio.SetActive(true);

        RakthasRaioHitbox raio = hitboxRaio.GetComponent<RakthasRaioHitbox>();

        if (raio != null)
            raio.VerificarDano();
    }
}

    public void DesativarHitboxRaio()
    {
        if (hitboxRaio != null)
            hitboxRaio.SetActive(false);
    }

    public void FinalizarRaio()
    {
        atacando = false;
    }

    

public void AtivarHitboxOnda()
{
    if (hitboxOndaDireita == null || hitboxOndaEsquerda == null)
        return;

    hitboxOndaDireita.enabled = false;
    hitboxOndaEsquerda.enabled = false;

    BoxCollider2D hitboxAtual = spriteRenderer.flipX
        ? hitboxOndaEsquerda
        : hitboxOndaDireita;

    RakthasOndaHitbox onda =
        hitboxAtual.GetComponent<RakthasOndaHitbox>();

    if (onda != null)
        onda.ReiniciarAcertos();

    hitboxAtual.enabled = true;

    if (onda != null)
        onda.VerificarDano();
}

public void DesativarHitboxOnda()
{
    if (hitboxOndaDireita != null)
        hitboxOndaDireita.enabled = false;

    if (hitboxOndaEsquerda != null)
        hitboxOndaEsquerda.enabled = false;
}

public void FinalizarAtaque1()
{
    atacando = false;
}


public void AtivarHitboxSoco()
{
    if (hitboxSocoDireita == null || hitboxSocoEsquerda == null)
        return;

    hitboxSocoDireita.enabled = false;
    hitboxSocoEsquerda.enabled = false;

    BoxCollider2D hitboxAtual = spriteRenderer.flipX
        ? hitboxSocoEsquerda
        : hitboxSocoDireita;

    RakthasSocoHitbox soco =
        hitboxAtual.GetComponent<RakthasSocoHitbox>();

    if (soco != null)
        soco.ReiniciarAcertos();

    hitboxAtual.enabled = true;

    if (soco != null)
        soco.VerificarDano();
}

public void DesativarHitboxSoco()
{
    if (hitboxSocoDireita != null)
        hitboxSocoDireita.enabled = false;

    if (hitboxSocoEsquerda != null)
        hitboxSocoEsquerda.enabled = false;
}

public void FinalizarAtaque3()
{
    atacando = false;

    Animator animator = GetComponent<Animator>();
    animator.speed = 1f;
}

}
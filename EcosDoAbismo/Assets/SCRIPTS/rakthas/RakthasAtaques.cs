
using UnityEngine;

public class RakthasAtaques : MonoBehaviour
{
    [Header("Hitbox do Raio")]
    public GameObject hitboxRaio;

    private bool atacando = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

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
    public AnimationCurve curvaVelocidadeSoco =
        new AnimationCurve(
            new Keyframe(0f, 1.25f),
            new Keyframe(0.25f, 1.25f),
            new Keyframe(0.40f, 1.70f),
            new Keyframe(0.58f, 1f),
            new Keyframe(0.80f, 0.70f),
            new Keyframe(1f, 0.70f)
        );

    [Header("Inteligência Artificial")]
    public float distanciaSoco = 2.2f;
    public float distanciaCorrente = 5f;
    public float distanciaMaximaRaio = 8f;

    [Header("Recuperação dos Ataques")]
    public float recuperacaoRaio = 0.8f;
    public float recuperacaoCorrente = 1.2f;
    public float recuperacaoSoco = 1.5f;

public float esperaInicial = 1f;

    private float proximoAtaque;
    private bool iniciouCombate = false;

    private RakthasMovimento movimento;
    private RakthasVida vida;

    // NOVA MECÂNICA
    private bool perseguicaoAgressivaObrigatoria = false;

    // MEMÓRIA DO ÚLTIMO ATAQUE
    private string ultimoAtaque = "";

    public bool EstaAtacando => atacando;

    public bool PerseguicaoAgressivaObrigatoria
        => perseguicaoAgressivaObrigatoria;

    public float DistanciaSoco => distanciaSoco;

    public float DistanciaMaximaRaio => distanciaMaximaRaio;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movimento = GetComponent<RakthasMovimento>();
        vida = GetComponent<RakthasVida>();
    }

    private void Update()
    {
        AtualizarPosicaoHitboxRaio();

        // VELOCIDADE VARIÁVEL DO SOCO
        if (atacando)
        {
            AnimatorStateInfo estado =
                animator.GetCurrentAnimatorStateInfo(0);

            if (estado.IsName("Attack3"))
            {
                float progresso = Mathf.Clamp01(
                    estado.normalizedTime
                );

                animator.speed =
                    curvaVelocidadeSoco.Evaluate(progresso);
            }
        }

        // TECLAS TEMPORÁRIAS
        if (PodeAtacar())
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                IniciarAtaque("Raio");
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                IniciarAtaque("Ataque1");
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                IniciarAtaque("Ataque3");
            }
        }

        EscolherAtaque();
    }

    private bool PodeAtacar()
    {
        if (atacando)
            return false;

        if (vida != null &&
            (vida.EstaMorto || vida.EstaTomandoDano))
            return false;

        return true;
    }

    private void EscolherAtaque()
    {
        if (movimento == null || !movimento.EstaAtivado)
        {
            iniciouCombate = false;
            return;
        }

        if (!iniciouCombate)
        {
            iniciouCombate = true;
            proximoAtaque = Time.time + esperaInicial;
            return;
        }

        if (!PodeAtacar())
            return;

        if (movimento.elian == null)
            return;

        float distancia = Mathf.Abs(
            movimento.elian.position.x -
            transform.position.x
        );

        // PERSEGUIÇÃO AGRESSIVA OBRIGATÓRIA
        if (perseguicaoAgressivaObrigatoria)
        {
            // Aguarda o cooldown e persegue
            // até alcançar a distância do soco.
            if (distancia <= distanciaSoco &&
                Time.time >= proximoAtaque)
            {
                IniciarAtaque("Ataque3");
            }

            return;
        }

        // FORA DO ALCANCE MÁXIMO
        if (distancia > distanciaMaximaRaio)
            return;

        if (Time.time < proximoAtaque)
            return;

        // ELIAN PRÓXIMO
        if (distancia <= distanciaSoco)
        {
            // Se acabou de usar o soco,
            // utiliza a Corrente de Fogo.
            if (ultimoAtaque == "Ataque3")
            {
                IniciarAtaque("Ataque1");
            }
            else
            {
                IniciarAtaque("Ataque3");
            }
        }

        // MÉDIO ALCANCE
        else if (distancia <= distanciaCorrente)
        {
            IniciarAtaque("Ataque1");
        }

        // LONGO ALCANCE
        else
        {
            IniciarAtaque("Raio");
        }
    }

    private void IniciarAtaque(string nomeAtaque)
    {
        if (!PodeAtacar())
            return;

        // DIREÇÃO ANTES DO ATAQUE
        if (movimento != null && movimento.elian != null)
        {
            float diferencaX =
                movimento.elian.position.x -
                transform.position.x;

            if (diferencaX > 0.1f)
                spriteRenderer.flipX = false;
            else if (diferencaX < -0.1f)
                spriteRenderer.flipX = true;
        }

        atacando = true;

        ultimoAtaque = nomeAtaque;

        animator.speed = nomeAtaque == "Ataque3"
            ? curvaVelocidadeSoco.Evaluate(0f)
            : 1f;

        animator.SetBool("estaCorrendo", false);
        animator.SetTrigger(nomeAtaque);
    }

    // =========================
    // RAIO
    // =========================

    private void AtualizarPosicaoHitboxRaio()
    {
        if (hitboxRaio == null || spriteRenderer == null)
            return;

        Vector3 posicao =
            hitboxRaio.transform.localPosition;

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

            RakthasRaioHitbox raio =
                hitboxRaio.GetComponent<RakthasRaioHitbox>();

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

    proximoAtaque = Time.time + recuperacaoRaio;

    perseguicaoAgressivaObrigatoria = true;
}

    // =========================
    // CORRENTE DE FOGO
    // =========================

    public void AtivarHitboxOnda()
    {
        if (hitboxOndaDireita == null ||
            hitboxOndaEsquerda == null)
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

    proximoAtaque = Time.time + recuperacaoCorrente;

    perseguicaoAgressivaObrigatoria = true;
}

    // =========================
    // SOCO DE FOGO
    // =========================

    public void AtivarHitboxSoco()
    {
        if (hitboxSocoDireita == null ||
            hitboxSocoEsquerda == null)
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

    animator.speed = 1f;

    proximoAtaque = Time.time + recuperacaoSoco;

    perseguicaoAgressivaObrigatoria = false;
}
}


using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class ElianMovimento : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
    public float velocidadeCorrida = 8f;
    public float velocidadeAnimacaoCorrida = 1.5f;
    public float forcaPulo = 10f;

    [Header("Dash")]
    public float velocidadeDash = 15f;
    public float tempoDash = 0.2f;
    public float cooldownDash = 0.5f;

    [Header("Chao")]
    public Transform pontoDeChao;
    public float raioChao = 0.2f;
    public LayerMask camadaChao;

    [Header("Movimento Externo")]
    public float tempoPuxao = 0.25f;

    [Header("Audio - Fontes")]
    public AudioSource audioPassos;
    public AudioSource audioMovimentos;

    [Header("Audio - Passos")]
    public AudioClip somPassosChaoDuro;
    public AudioClip somPassosVegetacao;

    [Header("Audio - Pulo, Aterrissagem e Dash")]
    public AudioClip somPulo;
    public AudioClip somAterrissagem;
    public AudioClip somDash;

    [Header("Audio - Identificacao do Piso")]
    public Tilemap[] tilemapsChao;
    public TileBase[] tilesVegetacao;

    [Min(0f)]
    public float distanciaLeituraPiso = 0.1f;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Collider2D colisorElian;

    private ElianAtaque ataque;
    private ElianVida vida;
    private ElianColeta coleta;

    private bool estaNoChao;
    private bool verificouChaoInicialmente = false;
    private bool pulouNesteFrame = false;

    private bool estaDashando = false;
    private bool dashDisponivel = true;
    private bool dashAereoDisponivel = true;

    private bool sendoPuxado = false;

    public bool podeMover = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        colisorElian = GetComponent<Collider2D>();

        ataque = GetComponent<ElianAtaque>();
        vida = GetComponent<ElianVida>();
        coleta = GetComponent<ElianColeta>();

        if (audioPassos != null)
        {
            audioPassos.playOnAwake = false;
            audioPassos.loop = true;
        }

        if (audioMovimentos != null)
        {
            audioMovimentos.playOnAwake = false;
            audioMovimentos.loop = false;
        }
    }

    void Update()
    {
        pulouNesteFrame = false;

        // Bloqueia comandos de movimento após a morte
        if (vida != null && vida.EstaMorto)
        {
            podeMover = false;

            PararPassos();

            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            return;
        }

        VerificarChao();

        // Bloqueia movimento durante a interface de viagem
        if (coleta != null && coleta.MenuViagemAberto)
        {
            PararPassos();

            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            return;
        }

        if (estaDashando || sendoPuxado)
        {
            PararPassos();
            return;
        }

        Movimento();
        Pulo();
        Dash();

        AtualizarSomPassos();
    }

    void VerificarChao()
    {
        bool estavaNoChao = estaNoChao;

        estaNoChao = Physics2D.OverlapCircle(
            pontoDeChao.position,
            raioChao,
            camadaChao
        );

        if (estaNoChao)
        {
            dashAereoDisponivel = true;
        }

        animator.SetBool("estaNoChao", estaNoChao);

        if (verificouChaoInicialmente &&
            !estavaNoChao &&
            estaNoChao)
        {
            TocarEfeito(somAterrissagem);
        }

        verificouChaoInicialmente = true;
    }

    void Movimento()
    {
        if (!podeMover)
        {
            return;
        }

        float movimentoHorizontal =
            Input.GetAxisRaw("Horizontal");

        animator.SetFloat(
            "velocidade",
            Mathf.Abs(movimentoHorizontal)
        );

        bool correndo =
            Input.GetKey(KeyCode.LeftShift) &&
            movimentoHorizontal != 0 &&
            estaNoChao;

        float velocidadeAtual =
            correndo ? velocidadeCorrida : velocidade;

        animator.SetFloat(
            "velocidadeAnimacao",
            correndo ? velocidadeAnimacaoCorrida : 1f
        );

        rb.linearVelocity = new Vector2(
            movimentoHorizontal * velocidadeAtual,
            rb.linearVelocity.y
        );

        if (movimentoHorizontal > 0)
        {
            spriteRenderer.flipX = false;

            GetComponent<HitboxControle>()
                .AtualizarDirecao(false);
        }
        else if (movimentoHorizontal < 0)
        {
            spriteRenderer.flipX = true;

            GetComponent<HitboxControle>()
                .AtualizarDirecao(true);
        }
    }

    void Pulo()
    {
        if (!podeMover)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) &&
            estaNoChao)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                forcaPulo
            );

            pulouNesteFrame = true;
            PararPassos();
            TocarEfeito(somPulo);
        }
    }

    void Dash()
    {
        if (coleta != null && coleta.MenuViagemAberto)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.LeftControl))
        {
            return;
        }

        if (!dashDisponivel)
        {
            return;
        }

        bool cancelandoCombo =
            ataque != null &&
            ataque.EstaNoCombo();

        if (!podeMover && !cancelandoCombo)
        {
            return;
        }

        if (!estaNoChao && !dashAereoDisponivel)
        {
            return;
        }

        if (cancelandoCombo)
        {
            ataque.CancelarComboPorDash();
        }

        if (!estaNoChao)
        {
            dashAereoDisponivel = false;
        }

        StartCoroutine(ExecutarDash());
    }

    public void AplicarPuxao(float direcao, float forca)
    {
        if (vida != null && vida.EstaMorto)
            return;

        if (estaDashando)
            return;

        StopCoroutine(nameof(ExecutarPuxao));
        StartCoroutine(ExecutarPuxao(direcao, forca));
    }

    IEnumerator ExecutarPuxao(float direcao, float forca)
    {
        sendoPuxado = true;

        PararPassos();

        animator.SetFloat("velocidade", 0f);

        rb.linearVelocity = new Vector2(
            direcao * forca,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(tempoPuxao);

        sendoPuxado = false;
    }

    void IgnorarColisaoInimigos(bool ignorar)
    {
        GameObject[] inimigos =
            GameObject.FindGameObjectsWithTag("Inimigo");

        foreach (GameObject inimigo in inimigos)
        {
            Collider2D[] colisores =
                inimigo.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D colisorInimigo in colisores)
            {
                if (!colisorInimigo.isTrigger)
                {
                    Physics2D.IgnoreCollision(
                        colisorElian,
                        colisorInimigo,
                        ignorar
                    );
                }
            }
        }
    }

    IEnumerator ExecutarDash()
    {
        estaDashando = true;
        dashDisponivel = false;

        PararPassos();
        TocarEfeito(somDash);

        bool dashNoAr = !estaNoChao;

        float gravidadeOriginal = rb.gravityScale;

        GetComponent<ElianVida>()
            .AtivarInvencibilidadeDash();

        IgnorarColisaoInimigos(true);

        animator.SetFloat("velocidade", 0);
        animator.SetTrigger("Dash");

        float direcao =
            spriteRenderer.flipX ? -1f : 1f;

        if (dashNoAr)
        {
            rb.gravityScale = 0f;
        }

        rb.linearVelocity = new Vector2(
            direcao * velocidadeDash,
            0f
        );

        yield return new WaitForSeconds(tempoDash);

        if (dashNoAr)
        {
            rb.gravityScale = gravidadeOriginal;
        }

        GetComponent<ElianVida>()
            .DesativarInvencibilidadeDash();

        IgnorarColisaoInimigos(false);

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        estaDashando = false;

        yield return new WaitForSeconds(cooldownDash);

        dashDisponivel = true;
    }

    void AtualizarSomPassos()
    {
        if (audioPassos == null)
            return;

        // ALTERADO: passos independentes da tecla Shift.
        bool movendoNoChao =
            podeMover &&
            estaNoChao &&
            !pulouNesteFrame &&
            !estaDashando &&
            !sendoPuxado &&
            Input.GetAxisRaw("Horizontal") != 0f;

        if (!movendoNoChao)
        {
            PararPassos();
            return;
        }

        bool pisandoNaVegetacao = EstaPisandoNaVegetacao();

        AudioClip somEscolhido =
            pisandoNaVegetacao
                ? somPassosVegetacao
                : somPassosChaoDuro;

        if (somEscolhido == null)
        {
            PararPassos();
            return;
        }

        if (audioPassos.clip != somEscolhido)
        {
            audioPassos.Stop();
            audioPassos.clip = somEscolhido;
        }

        if (!audioPassos.isPlaying)
        {
            audioPassos.Play();
        }
    }

    bool EstaPisandoNaVegetacao()
    {
        if (pontoDeChao == null ||
            tilemapsChao == null ||
            tilesVegetacao == null)
        {
            return false;
        }

        Vector3 posicaoPiso =
            pontoDeChao.position +
            Vector3.down * distanciaLeituraPiso;

        foreach (Tilemap tilemap in tilemapsChao)
        {
            if (tilemap == null)
                continue;

            Vector3Int celula =
                tilemap.WorldToCell(posicaoPiso);

            TileBase tileAtual =
                tilemap.GetTile(celula);

            if (tileAtual == null)
                continue;

            foreach (TileBase tileVegetacao in tilesVegetacao)
            {
                if (tileVegetacao != null &&
                    tileAtual == tileVegetacao)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void PararPassos()
    {
        if (audioPassos != null && audioPassos.isPlaying)
        {
            audioPassos.Stop();
        }
    }

    void TocarEfeito(AudioClip som)
    {
        if (audioMovimentos != null && som != null)
        {
            audioMovimentos.PlayOneShot(som);
        }
    }
}

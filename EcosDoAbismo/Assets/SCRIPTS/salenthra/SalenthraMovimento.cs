using UnityEngine;

public class SalenthraMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    public Animator animator;
    public SalenthraVida vidaSalenthra;

    [Header("Colliders")]
    public Collider2D colliderSalenthra;
    public Collider2D colliderElian;

    [Header("Movimento")]
    public float velocidade = 3f;
    public float distanciaParada = 1.5f;

    [Header("IA")]
    public float tempoEntreDecisoes = 0.4f;

    private float proximaDecisao = 0f;
    private float proximaDecisaoFase2 = 0f;

    // 0 = nenhum
    // 1 = dash
    // 2 = puxão
    // 3 = chuva
    private int ultimoAtaque = 0;
    private int ataqueAnterior = 0;

    // FASE 2
    // 0 = nenhum
    // 1 = espada
    // 2 = dash
    // 3 = lua
    // 4 = raio
    private int ultimoAtaqueFase2 = 0;
    private int ataqueAnteriorFase2 = 0;

    // =========================
    // FASE 1 - DASH
    // =========================

    [Header("Ataque Dash")]
    public float distanciaDash = 4f;
    public float velocidadeDash = 8f;
    public float tempoEntreDashes = 2f;

    // =========================
    // FASE 1 - PUXÃO
    // =========================

    [Header("Ataque Puxão")]
    public float distanciaPuxao = 6f;
    public float forcaPuxao = 8f;
    public float tempoEntrePuxoes = 3f;

    // =========================
    // FASE 1 - CHUVA
    // =========================

    [Header("Ataque Chuva")]
    public ChuvaEspadas chuvaEspadas;
    public float tempoEntreChuvas = 5f;

    // =========================
    // FASE 2 - ATAQUE 1
    // =========================

    [Header("Ataque 1 Fase 2")]
    public SalenthraHitboxAtaque1Fase2 hitboxAtaque1Fase2;
    public Transform transformHitboxAtaque1Fase2;

    public float distanciaAtaque1Fase2 = 2f;
    public float tempoEntreAtaques1Fase2 = 1.5f;

    public float posicaoHitboxDireita = 1f;
    public float posicaoHitboxEsquerda = -1f;

    // =========================
    // FASE 2 - ATAQUE 2
    // =========================

    [Header("Ataque 2 Fase 2 - Dash")]
    public SalenthraHitboxAtaque2Fase2 hitboxAtaque2Fase2;

    public float distanciaAtaque2Fase2 = 7f;
    public float velocidadeAtaque2Fase2 = 22f;

    [Header("Cooldown Dash Fase 2")]
    public float cooldownDashVidaAlta = 3.2f;
    public float cooldownDashVidaMedia = 2.7f;
    public float cooldownDashVidaBaixa = 2.2f;

    // =========================
    // FASE 2 - ATAQUE 3
    // =========================

    [Header("Ataque 3 Fase 2 - Lua")]
    public GameObject projetilLuaPrefab;
    public Transform pontoDisparoLua;

    public float distanciaAtaque3Fase2 = 12f;
    public float tempoEntreAtaques3Fase2 = 2.2f;

    public float posicaoPontoLuaDireita = 1f;
    public float posicaoPontoLuaEsquerda = -1f;

    // =========================
    // FASE 2 - ATAQUE 4
    // =========================

    [Header("Ataque 4 Fase 2 - Raio")]
    public GameObject raioFase2;
    public Transform transformRaioFase2;
    public Animator animatorRaioFase2;

    public float distanciaAtaque4Fase2 = 6f;
    public float tempoEntreAtaques4Fase2 = 5f;

    public float posicaoRaioDireita = 1f;
    public float posicaoRaioEsquerda = -1f;

    public bool podeMover = true;
    public bool ativada = false;

    private bool fase2 = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // =========================
    // DASH FASE 1
    // =========================

    private bool atacandoDash = false;
    private float direcaoDash;
    private float proximoDash = 0f;

    private bool dashParadoAoAcertar = false;
    private bool movendoNoDash = false;

    // =========================
    // PUXÃO FASE 1
    // =========================

    private bool atacandoPuxao = false;
    private float proximoPuxao = 0f;

    // =========================
    // CHUVA FASE 1
    // =========================

    private bool atacandoChuva = false;
    private float proximaChuva = 0f;

    // =========================
    // ATAQUE 1 FASE 2
    // =========================

    private bool atacandoAtaque1Fase2 = false;
    private float proximoAtaque1Fase2 = 0f;

    // =========================
    // ATAQUE 2 FASE 2
    // =========================

    private bool atacandoAtaque2Fase2 = false;
    private bool movendoAtaque2Fase2 = false;
    private float direcaoAtaque2Fase2;
    private float proximoAtaque2Fase2 = 0f;

    private int dashesRestantesFase2 = 0;

    // =========================
    // ATAQUE 3 FASE 2
    // =========================

    private bool atacandoAtaque3Fase2 = false;
    private float proximoAtaque3Fase2 = 0f;

    // =========================
    // ATAQUE 4 FASE 2
    // =========================

    private bool atacandoAtaque4Fase2 = false;
    private float proximoAtaque4Fase2 = 0f;

    // =========================
    // ESTADO PÚBLICO FASE 2
    // =========================

    public bool EstaAtacandoFase2
    {
        get
        {
            return
                atacandoAtaque1Fase2 ||
                atacandoAtaque2Fase2 ||
                atacandoAtaque3Fase2 ||
                atacandoAtaque4Fase2;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (raioFase2 != null)
        {
            raioFase2.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!ativada || !podeMover)
        {
            PararMovimento();
            return;
        }

        if (elian == null)
            return;

        // =========================
        // ATAQUES FASE 1
        // =========================

        if (atacandoDash)
        {
            if (movendoNoDash && !dashParadoAoAcertar)
            {
                rb.linearVelocity = new Vector2(
                    direcaoDash * velocidadeDash,
                    rb.linearVelocity.y
                );
            }
            else
            {
                rb.linearVelocity = new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
            }

            animator.SetBool("Andando", false);
            return;
        }

        if (atacandoPuxao)
        {
            PararMovimento();
            return;
        }

        if (atacandoChuva)
        {
            PararMovimento();
            return;
        }

        // =========================
        // ATAQUE 1 FASE 2
        // =========================

        if (atacandoAtaque1Fase2)
        {
            PararMovimento();
            return;
        }

        // =========================
        // ATAQUE 2 FASE 2
        // =========================

        if (atacandoAtaque2Fase2)
        {
            animator.SetBool("Andando", false);

            if (movendoAtaque2Fase2)
            {
                rb.linearVelocity = new Vector2(
                    direcaoAtaque2Fase2 *
                    velocidadeAtaque2Fase2,
                    rb.linearVelocity.y
                );

                if (hitboxAtaque2Fase2 != null)
                {
                    hitboxAtaque2Fase2.VerificarAcerto();
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(
                    0f,
                    rb.linearVelocity.y
                );
            }

            return;
        }

        // =========================
        // ATAQUE 3 FASE 2
        // =========================

        if (atacandoAtaque3Fase2)
        {
            PararMovimento();
            return;
        }

        // =========================
        // ATAQUE 4 FASE 2
        // =========================

        if (atacandoAtaque4Fase2)
        {
            PararMovimento();
            return;
        }

        float distancia = Mathf.Abs(
            elian.position.x -
            transform.position.x
        );

        // =========================
        // IA FASE 2
        // =========================

        if (
            fase2 &&
            Time.time >= proximaDecisaoFase2
        )
        {
            if (distancia <= distanciaAtaque3Fase2)
            {
                if (EscolherAtaqueFase2(distancia))
                {
                    return;
                }
            }

            proximaDecisaoFase2 =
                Time.time +
                ObterTempoDecisaoAtual();
        }

        // =========================
        // IA FASE 1
        // =========================

        if (
            !fase2 &&
            Time.time >= proximaDecisao
        )
        {
            if (EscolherAtaqueFase1(distancia))
            {
                proximaDecisao =
                    Time.time +
                    ObterTempoDecisaoAtual();

                return;
            }

            proximaDecisao =
                Time.time +
                ObterTempoDecisaoAtual();
        }

        // =========================
        // MOVIMENTO NORMAL
        // =========================

        if (distancia <= distanciaParada)
        {
            PararMovimento();
            return;
        }

        float direcao = Mathf.Sign(
            elian.position.x -
            transform.position.x
        );

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );

        animator.SetBool("Andando", true);

        if (direcao > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direcao < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    // =========================
    // CANCELAR ATAQUES AO TOMAR DANO
    // =========================

    public void CancelarAtaquesPorDano()
    {
        atacandoDash = false;
        atacandoPuxao = false;
        atacandoChuva = false;

        movendoNoDash = false;
        dashParadoAoAcertar = false;

        atacandoAtaque1Fase2 = false;
        atacandoAtaque2Fase2 = false;
        atacandoAtaque3Fase2 = false;
        atacandoAtaque4Fase2 = false;

        movendoAtaque2Fase2 = false;
        dashesRestantesFase2 = 0;

        IgnorarColisaoDashFase2(false);

        if (raioFase2 != null)
        {
            raioFase2.SetActive(false);
        }

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        animator.SetBool("Andando", false);

        animator.ResetTrigger("AtaqueDash");
        animator.ResetTrigger("AtaquePuxao");
        animator.ResetTrigger("AtaqueChuva");

        animator.ResetTrigger("Ataque1Fase2");
        animator.ResetTrigger("Ataque2Fase2");
        animator.ResetTrigger("Ataque3Fase2");
        animator.ResetTrigger("Ataque4Fase2");
    }

    // =========================
    // AGRESSIVIDADE
    // =========================

    private float ObterTempoDecisaoAtual()
    {
        if (vidaSalenthra == null)
            return tempoEntreDecisoes;

        float porcentagem =
            vidaSalenthra.ObterPorcentagemVidaAtual();

        if (porcentagem <= 0.30f)
            return tempoEntreDecisoes * 0.5f;

        if (porcentagem <= 0.60f)
            return tempoEntreDecisoes * 0.75f;

        return tempoEntreDecisoes;
    }

    private float AjustarCooldownPorVida(
        float cooldownBase
    )
    {
        if (vidaSalenthra == null)
            return cooldownBase;

        float porcentagem =
            vidaSalenthra.ObterPorcentagemVidaAtual();

        if (porcentagem <= 0.30f)
            return cooldownBase * 0.70f;

        if (porcentagem <= 0.60f)
            return cooldownBase * 0.85f;

        return cooldownBase;
    }

    private float ObterCooldownDashFase2()
    {
        if (vidaSalenthra == null)
            return cooldownDashVidaAlta;

        float porcentagem =
            vidaSalenthra.ObterPorcentagemVidaAtual();

        if (porcentagem <= 0.40f)
        {
            return cooldownDashVidaBaixa;
        }

        if (porcentagem <= 0.70f)
        {
            return cooldownDashVidaMedia;
        }

        return cooldownDashVidaAlta;
    }

    // =========================
    // IA FASE 1
    // =========================

    private bool EscolherAtaqueFase1(
        float distancia
    )
    {
        float pesoDash = 0f;
        float pesoPuxao = 0f;
        float pesoChuva = 0f;

        if (
            distancia <= distanciaDash &&
            Time.time >= proximoDash
        )
        {
            pesoDash = 60f;
        }

        if (
            distancia > distanciaDash &&
            distancia <= distanciaPuxao &&
            Time.time >= proximoPuxao
        )
        {
            pesoPuxao = 60f;
        }

        if (Time.time >= proximaChuva)
        {
            if (distancia <= distanciaDash)
                pesoChuva = 30f;
            else if (distancia <= distanciaPuxao)
                pesoChuva = 40f;
            else
                pesoChuva = 70f;
        }

        float porcentagemVida = 1f;

        if (vidaSalenthra != null)
        {
            porcentagemVida =
                vidaSalenthra.ObterPorcentagemVidaAtual();
        }

        if (
            ultimoAtaque == 2 &&
            pesoDash > 0f
        )
        {
            if (porcentagemVida <= 0.30f)
                pesoDash *= 4f;
            else if (porcentagemVida <= 0.60f)
                pesoDash *= 3f;
            else
                pesoDash *= 2.5f;
        }

        if (ultimoAtaque == 3)
        {
            if (pesoDash > 0f)
            {
                if (porcentagemVida <= 0.30f)
                    pesoDash *= 2.2f;
                else
                    pesoDash *= 1.7f;
            }

            if (pesoPuxao > 0f)
            {
                if (porcentagemVida <= 0.30f)
                    pesoPuxao *= 1.8f;
                else
                    pesoPuxao *= 1.4f;
            }
        }

        if (
            ataqueAnterior == 1 &&
            pesoDash > 0f
        )
        {
            pesoDash *= 0.35f;
        }

        if (
            ataqueAnterior == 2 &&
            pesoPuxao > 0f
        )
        {
            pesoPuxao *= 0.35f;
        }

        if (
            ataqueAnterior == 3 &&
            pesoChuva > 0f
        )
        {
            pesoChuva *= 0.35f;
        }

        if (ultimoAtaque == 1)
            pesoDash = 0f;

        if (ultimoAtaque == 2)
            pesoPuxao = 0f;

        if (ultimoAtaque == 3)
            pesoChuva = 0f;

        float pesoTotal =
            pesoDash +
            pesoPuxao +
            pesoChuva;

        if (pesoTotal <= 0f)
            return false;

        float escolha =
            Random.Range(
                0f,
                pesoTotal
            );

        if (escolha < pesoDash)
        {
            ataqueAnterior = ultimoAtaque;
            ultimoAtaque = 1;

            IniciarDash();

            return true;
        }

        escolha -= pesoDash;

        if (escolha < pesoPuxao)
        {
            ataqueAnterior = ultimoAtaque;
            ultimoAtaque = 2;

            IniciarPuxao();

            return true;
        }

        ataqueAnterior = ultimoAtaque;
        ultimoAtaque = 3;

        IniciarAtaqueChuva();

        return true;
    }

    // =========================
    // IA FASE 2
    // =========================

    private bool EscolherAtaqueFase2(
        float distancia
    )
    {
        float pesoEspada = 0f;
        float pesoDash = 0f;
        float pesoLua = 0f;
        float pesoRaio = 0f;

        // =========================
        // DISTÂNCIA PERTO
        // =========================

        if (distancia <= distanciaAtaque1Fase2)
        {
            pesoEspada = 80f;
            pesoDash = 45f;
            pesoLua = 20f;
            pesoRaio = 25f;
        }

        // =========================
        // DISTÂNCIA MÉDIA
        // =========================

        else if (distancia <= distanciaAtaque2Fase2)
        {
            pesoEspada = 5f;
            pesoDash = 80f;
            pesoLua = 55f;
            pesoRaio = 70f;
        }

        // =========================
        // DISTÂNCIA LONGE
        // =========================

        else
        {
            pesoEspada = 0f;
            pesoDash = 60f;
            pesoLua = 80f;
            pesoRaio = 0f;
        }

        // O raio só pode ser escolhido se
        // o Elian estiver dentro do alcance dele.
        if (distancia > distanciaAtaque4Fase2)
        {
            pesoRaio = 0f;
        }

        // =========================
        // COOLDOWNS
        // =========================

        if (Time.time < proximoAtaque1Fase2)
        {
            pesoEspada = 0f;
        }

        if (Time.time < proximoAtaque2Fase2)
        {
            pesoDash = 0f;
        }

        if (Time.time < proximoAtaque3Fase2)
        {
            pesoLua = 0f;
        }

        if (Time.time < proximoAtaque4Fase2)
        {
            pesoRaio = 0f;
        }

        // =========================
        // LUA -> DASH
        // =========================

        if (
            ultimoAtaqueFase2 == 3 &&
            pesoDash > 0f
        )
        {
            pesoDash *= 2.2f;
        }

        // =========================
        // DASH -> ESPADA
        // =========================

        if (
            ultimoAtaqueFase2 == 2 &&
            distancia <= distanciaAtaque1Fase2 &&
            pesoEspada > 0f
        )
        {
            pesoEspada *= 4f;
        }

        // =========================
        // EVITAR A -> B -> A
        // =========================

        if (
            ataqueAnteriorFase2 == 1 &&
            pesoEspada > 0f
        )
        {
            pesoEspada *= 0.35f;
        }

        if (
            ataqueAnteriorFase2 == 2 &&
            pesoDash > 0f
        )
        {
            pesoDash *= 0.35f;
        }

        if (
            ataqueAnteriorFase2 == 3 &&
            pesoLua > 0f
        )
        {
            pesoLua *= 0.35f;
        }

        if (
            ataqueAnteriorFase2 == 4 &&
            pesoRaio > 0f
        )
        {
            pesoRaio *= 0.35f;
        }

        // =========================
        // NÃO REPETIR
        // =========================

        if (ultimoAtaqueFase2 == 1)
        {
            pesoEspada = 0f;
        }

        if (ultimoAtaqueFase2 == 2)
        {
            pesoDash = 0f;
        }

        if (ultimoAtaqueFase2 == 3)
        {
            pesoLua = 0f;
        }

        if (ultimoAtaqueFase2 == 4)
        {
            pesoRaio = 0f;
        }

        float pesoTotal =
            pesoEspada +
            pesoDash +
            pesoLua +
            pesoRaio;

        if (pesoTotal <= 0f)
            return false;

        float escolha =
            Random.Range(
                0f,
                pesoTotal
            );

        // =========================
        // ESPADA
        // =========================

        if (escolha < pesoEspada)
        {
            ataqueAnteriorFase2 =
                ultimoAtaqueFase2;

            ultimoAtaqueFase2 = 1;

            IniciarAtaque1Fase2();

            return true;
        }

        escolha -= pesoEspada;

        // =========================
        // DASH
        // =========================

        if (escolha < pesoDash)
        {
            ataqueAnteriorFase2 =
                ultimoAtaqueFase2;

            ultimoAtaqueFase2 = 2;

            IniciarAtaque2Fase2();

            return true;
        }

        escolha -= pesoDash;

        // =========================
        // LUA
        // =========================

        if (escolha < pesoLua)
        {
            ataqueAnteriorFase2 =
                ultimoAtaqueFase2;

            ultimoAtaqueFase2 = 3;

            IniciarAtaque3Fase2();

            return true;
        }

        // =========================
        // RAIO
        // =========================

        ataqueAnteriorFase2 =
            ultimoAtaqueFase2;

        ultimoAtaqueFase2 = 4;

        IniciarAtaque4Fase2();

        return true;
    }

    private void PararMovimento()
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        animator.SetBool(
            "Andando",
            false
        );
    }

    // =========================
    // DASH FASE 1
    // =========================

    private void IniciarDash()
    {
        if (
            atacandoDash ||
            atacandoPuxao ||
            atacandoChuva
        )
            return;

        atacandoDash = true;

        movendoNoDash = false;
        dashParadoAoAcertar = false;

        direcaoDash = Mathf.Sign(
            elian.position.x -
            transform.position.x
        );

        if (direcaoDash > 0)
            spriteRenderer.flipX = false;
        else if (direcaoDash < 0)
            spriteRenderer.flipX = true;

        animator.SetBool(
            "Andando",
            false
        );

        animator.SetTrigger(
            "AtaqueDash"
        );
    }

    public void ComecarMovimentoDash()
    {
        if (!atacandoDash)
            return;

        movendoNoDash = true;
    }

    public void PararMovimentoDash()
    {
        movendoNoDash = false;

        PararMovimento();
    }

    public void PararDashAoAcertar()
    {
        if (!atacandoDash)
            return;

        dashParadoAoAcertar = true;

        PararMovimento();
    }

    public void FinalizarDash()
    {
        atacandoDash = false;

        movendoNoDash = false;
        dashParadoAoAcertar = false;

        PararMovimento();

        proximoDash =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntreDashes
            );

        proximaDecisao =
            Time.time +
            ObterTempoDecisaoAtual();
    }

    // =========================
    // PUXÃO FASE 1
    // =========================

    private void IniciarPuxao()
    {
        if (
            atacandoPuxao ||
            atacandoDash ||
            atacandoChuva
        )
            return;

        atacandoPuxao = true;

        float direcao = Mathf.Sign(
            elian.position.x -
            transform.position.x
        );

        if (direcao > 0)
            spriteRenderer.flipX = false;
        else if (direcao < 0)
            spriteRenderer.flipX = true;

        PararMovimento();

        animator.SetTrigger(
            "AtaquePuxao"
        );
    }

    public void PuxarElian()
    {
        if (!atacandoPuxao)
            return;

        if (elian == null)
            return;

        ElianMovimento movimentoElian =
            elian.GetComponent<ElianMovimento>();

        if (movimentoElian == null)
            return;

        float direcao = Mathf.Sign(
            transform.position.x -
            elian.position.x
        );

        movimentoElian.AplicarPuxao(
            direcao,
            forcaPuxao
        );
    }

    public void FinalizarPuxao()
    {
        atacandoPuxao = false;

        PararMovimento();

        proximoPuxao =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntrePuxoes
            );

        proximaDecisao =
            Time.time +
            ObterTempoDecisaoAtual();
    }

    // =========================
    // CHUVA FASE 1
    // =========================

    private void IniciarAtaqueChuva()
    {
        if (
            atacandoDash ||
            atacandoPuxao ||
            atacandoChuva
        )
            return;

        atacandoChuva = true;

        PararMovimento();

        animator.SetTrigger(
            "AtaqueChuva"
        );
    }

    public void InvocarChuva()
    {
        if (!atacandoChuva)
            return;

        if (chuvaEspadas != null)
        {
            chuvaEspadas.IniciarChuva();
        }
    }

    public void FinalizarAtaqueChuva()
    {
        if (!atacandoChuva)
            return;

        atacandoChuva = false;

        proximaChuva =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntreChuvas
            );

        proximaDecisao =
            Time.time +
            ObterTempoDecisaoAtual();

        animator.SetTrigger(
            "FinalizarChuva"
        );
    }

    // =========================
    // ATAQUE 1 FASE 2
    // =========================

    private void IniciarAtaque1Fase2()
    {
        if (!fase2)
            return;

        if (
            atacandoAtaque1Fase2 ||
            atacandoAtaque2Fase2 ||
            atacandoAtaque3Fase2 ||
            atacandoAtaque4Fase2
        )
            return;

        atacandoAtaque1Fase2 = true;

        float direcao = Mathf.Sign(
            elian.position.x -
            transform.position.x
        );

        if (direcao > 0)
        {
            spriteRenderer.flipX = false;

            AtualizarHitboxAtaque1Fase2(
                false
            );
        }
        else if (direcao < 0)
        {
            spriteRenderer.flipX = true;

            AtualizarHitboxAtaque1Fase2(
                true
            );
        }

        PararMovimento();

        animator.SetTrigger(
            "Ataque1Fase2"
        );
    }

    private void AtualizarHitboxAtaque1Fase2(
        bool esquerda
    )
    {
        if (transformHitboxAtaque1Fase2 == null)
            return;

        Vector3 posicao =
            transformHitboxAtaque1Fase2.localPosition;

        if (esquerda)
        {
            posicao.x =
                posicaoHitboxEsquerda;
        }
        else
        {
            posicao.x =
                posicaoHitboxDireita;
        }

        transformHitboxAtaque1Fase2.localPosition =
            posicao;
    }

    public void AcertarAtaque1Fase2()
    {
        if (!atacandoAtaque1Fase2)
            return;

        if (hitboxAtaque1Fase2 != null)
        {
            hitboxAtaque1Fase2.VerificarAcerto();
        }
    }

    public void FinalizarAtaque1Fase2()
    {
        if (!atacandoAtaque1Fase2)
            return;

        atacandoAtaque1Fase2 = false;

        proximoAtaque1Fase2 =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntreAtaques1Fase2
            );

        proximaDecisaoFase2 =
            Time.time +
            ObterTempoDecisaoAtual();
    }

    // =========================
    // ATAQUE 2 FASE 2
    // DASH MÚLTIPLO
    // =========================

    private void IniciarAtaque2Fase2()
    {
        if (!fase2)
            return;

        if (
            atacandoAtaque1Fase2 ||
            atacandoAtaque2Fase2 ||
            atacandoAtaque3Fase2 ||
            atacandoAtaque4Fase2
        )
            return;

        atacandoAtaque2Fase2 = true;

        dashesRestantesFase2 =
            ObterQuantidadeDashesFase2();

        IgnorarColisaoDashFase2(true);

        PrepararDashFase2();
    }

    private int ObterQuantidadeDashesFase2()
    {
        if (vidaSalenthra == null)
            return 1;

        float porcentagemVida =
            vidaSalenthra.ObterPorcentagemVidaAtual();

        if (porcentagemVida <= 0.40f)
            return 3;

        if (porcentagemVida <= 0.70f)
            return 2;

        return 1;
    }

    private void IgnorarColisaoDashFase2(
        bool ignorar
    )
    {
        if (colliderSalenthra == null)
            return;

        if (colliderElian == null)
            return;

        Physics2D.IgnoreCollision(
            colliderSalenthra,
            colliderElian,
            ignorar
        );
    }

    private void PrepararDashFase2()
    {
        if (!fase2)
            return;

        if (elian == null)
            return;

        movendoAtaque2Fase2 = false;

        direcaoAtaque2Fase2 =
            Mathf.Sign(
                elian.position.x -
                transform.position.x
            );

        if (direcaoAtaque2Fase2 > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direcaoAtaque2Fase2 < 0)
        {
            spriteRenderer.flipX = true;
        }

        if (hitboxAtaque2Fase2 != null)
        {
            hitboxAtaque2Fase2.PrepararNovoDash();
        }

        PararMovimento();

        animator.ResetTrigger(
            "Ataque2Fase2"
        );

        animator.SetTrigger(
            "Ataque2Fase2"
        );
    }

    public void ComecarMovimentoAtaque2Fase2()
    {
        if (!atacandoAtaque2Fase2)
            return;

        movendoAtaque2Fase2 = true;
    }

    public void PararMovimentoAtaque2Fase2()
    {
        movendoAtaque2Fase2 = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    public void FinalizarAtaque2Fase2()
    {
        if (!atacandoAtaque2Fase2)
            return;

        movendoAtaque2Fase2 = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        dashesRestantesFase2--;

        if (dashesRestantesFase2 > 0)
        {
            PrepararDashFase2();
            return;
        }

        atacandoAtaque2Fase2 = false;
        dashesRestantesFase2 = 0;

        IgnorarColisaoDashFase2(false);

        proximoAtaque2Fase2 =
            Time.time +
            ObterCooldownDashFase2();

        proximaDecisaoFase2 =
            Time.time +
            ObterTempoDecisaoAtual();
    }

    // =========================
    // ATAQUE 3 FASE 2
    // PROJÉTIL LUA
    // =========================

    private void IniciarAtaque3Fase2()
    {
        if (!fase2)
            return;

        if (
            atacandoAtaque1Fase2 ||
            atacandoAtaque2Fase2 ||
            atacandoAtaque3Fase2 ||
            atacandoAtaque4Fase2
        )
            return;

        atacandoAtaque3Fase2 = true;

        float direcao = Mathf.Sign(
            elian.position.x -
            transform.position.x
        );

        if (direcao > 0)
        {
            spriteRenderer.flipX = false;

            AtualizarPontoDisparoLua(false);
        }
        else if (direcao < 0)
        {
            spriteRenderer.flipX = true;

            AtualizarPontoDisparoLua(true);
        }

        PararMovimento();

        animator.SetTrigger(
            "Ataque3Fase2"
        );
    }

    private void AtualizarPontoDisparoLua(
        bool esquerda
    )
    {
        if (pontoDisparoLua == null)
            return;

        Vector3 posicao =
            pontoDisparoLua.localPosition;

        if (esquerda)
        {
            posicao.x =
                posicaoPontoLuaEsquerda;
        }
        else
        {
            posicao.x =
                posicaoPontoLuaDireita;
        }

        pontoDisparoLua.localPosition =
            posicao;
    }

    public void DispararProjetilLua()
    {
        if (!atacandoAtaque3Fase2)
            return;

        if (projetilLuaPrefab == null)
            return;

        if (pontoDisparoLua == null)
            return;

        if (elian == null)
            return;

        float direcao = Mathf.Sign(
            elian.position.x -
            pontoDisparoLua.position.x
        );

        GameObject novaLua =
            Instantiate(
                projetilLuaPrefab,
                pontoDisparoLua.position,
                Quaternion.identity
            );

        ProjetilLua projetil =
            novaLua.GetComponent<ProjetilLua>();

        if (projetil != null)
        {
            projetil.direcao = direcao;

            projetil.vidaSalenthra =
                vidaSalenthra;
        }
    }

    public void FinalizarAtaque3Fase2()
    {
        if (!atacandoAtaque3Fase2)
            return;

        atacandoAtaque3Fase2 = false;

        proximoAtaque3Fase2 =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntreAtaques3Fase2
            );

        proximaDecisaoFase2 =
            Time.time +
            ObterTempoDecisaoAtual();
    }

    // =========================
    // ATAQUE 4 FASE 2
    // RAIO
    // =========================

    public void IniciarAtaque4Fase2()
    {
        if (!fase2)
            return;

        if (
            atacandoAtaque1Fase2 ||
            atacandoAtaque2Fase2 ||
            atacandoAtaque3Fase2 ||
            atacandoAtaque4Fase2
        )
            return;

        atacandoAtaque4Fase2 = true;

        float direcao = Mathf.Sign(
            elian.position.x -
            transform.position.x
        );

        if (direcao > 0)
        {
            spriteRenderer.flipX = false;

            AtualizarRaioFase2(false);
        }
        else if (direcao < 0)
        {
            spriteRenderer.flipX = true;

            AtualizarRaioFase2(true);
        }

        if (raioFase2 != null)
        {
            raioFase2.SetActive(false);
        }

        PararMovimento();

        animator.SetTrigger(
            "Ataque4Fase2"
        );
    }

    private void AtualizarRaioFase2(
        bool esquerda
    )
    {
        if (transformRaioFase2 == null)
            return;

        Vector3 posicao =
            transformRaioFase2.localPosition;

        Vector3 escala =
            transformRaioFase2.localScale;

        if (esquerda)
        {
            posicao.x =
                posicaoRaioEsquerda;

            escala.x =
                -Mathf.Abs(escala.x);
        }
        else
        {
            posicao.x =
                posicaoRaioDireita;

            escala.x =
                Mathf.Abs(escala.x);
        }

        transformRaioFase2.localPosition =
            posicao;

        transformRaioFase2.localScale =
            escala;
    }

    public void DispararRaioFase2()
    {
        if (!atacandoAtaque4Fase2)
            return;

        if (raioFase2 == null)
            return;

        raioFase2.SetActive(true);

        if (animatorRaioFase2 != null)
        {
            animatorRaioFase2.Rebind();
            animatorRaioFase2.Update(0f);
        }
    }

    public void FinalizarAtaque4Fase2()
    {
        if (!atacandoAtaque4Fase2)
            return;

        atacandoAtaque4Fase2 = false;

        if (raioFase2 != null)
        {
            raioFase2.SetActive(false);
        }

        proximoAtaque4Fase2 =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntreAtaques4Fase2
            );

        proximaDecisaoFase2 =
            Time.time +
            ObterTempoDecisaoAtual();
    }

    // =========================
    // ATIVAÇÃO
    // =========================

    public void Ativar()
    {
        ativada = true;

        proximaDecisao =
            Time.time +
            ObterTempoDecisaoAtual();
    }

    // =========================
    // FASE 2
    // =========================

    public void EntrarFase2()
    {
        fase2 = true;

        atacandoDash = false;
        atacandoPuxao = false;
        atacandoChuva = false;

        atacandoAtaque1Fase2 = false;
        atacandoAtaque2Fase2 = false;
        atacandoAtaque3Fase2 = false;
        atacandoAtaque4Fase2 = false;

        movendoNoDash = false;
        movendoAtaque2Fase2 = false;

        dashesRestantesFase2 = 0;

        dashParadoAoAcertar = false;

        ultimoAtaqueFase2 = 0;
        ataqueAnteriorFase2 = 0;

        IgnorarColisaoDashFase2(false);

        if (raioFase2 != null)
        {
            raioFase2.SetActive(false);
        }

        proximaDecisaoFase2 =
            Time.time +
            ObterTempoDecisaoAtual();

        PararMovimento();
    }
}
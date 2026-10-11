using UnityEngine;

public class SalenthraMovimento : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    public Animator animator;
    public SalenthraVida vidaSalenthra;

    [Header("Movimento")]
    public float velocidade = 3f;
    public float distanciaParada = 1.5f;

    [Header("IA")]
    public float tempoEntreDecisoes = 0.4f;

    private float proximaDecisao = 0f;

    // 0 = nenhum
    // 1 = dash
    // 2 = puxão
    // 3 = chuva
    private int ultimoAtaque = 0;
    private int ataqueAnterior = 0;

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
    public float tempoEntreAtaques2Fase2 = 2.5f;

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

    // =========================
    // ATAQUE 3 FASE 2
    // =========================

    private bool atacandoAtaque3Fase2 = false;
    private float proximoAtaque3Fase2 = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                    direcaoAtaque2Fase2 * velocidadeAtaque2Fase2,
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

        float distancia = Mathf.Abs(
            elian.position.x -
            transform.position.x
        );

        // =========================
        // FASE 2
        // =========================

        if (fase2)
        {
            if (
                distancia <= distanciaAtaque1Fase2 &&
                Time.time >= proximoAtaque1Fase2
            )
            {
                IniciarAtaque1Fase2();
                return;
            }

            if (
                distancia > distanciaAtaque1Fase2 &&
                distancia <= distanciaAtaque2Fase2 &&
                Time.time >= proximoAtaque2Fase2
            )
            {
                IniciarAtaque2Fase2();
                return;
            }

            if (
                distancia > distanciaAtaque2Fase2 &&
                distancia <= distanciaAtaque3Fase2 &&
                Time.time >= proximoAtaque3Fase2
            )
            {
                IniciarAtaque3Fase2();
                return;
            }
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

        float escolha = Random.Range(
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
            atacandoAtaque3Fase2
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
            posicao.x = posicaoHitboxEsquerda;
        else
            posicao.x = posicaoHitboxDireita;

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
        atacandoAtaque1Fase2 = false;

        proximoAtaque1Fase2 =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntreAtaques1Fase2
            );
    }

    // =========================
    // ATAQUE 2 FASE 2
    // =========================

    private void IniciarAtaque2Fase2()
    {
        if (!fase2)
            return;

        if (
            atacandoAtaque1Fase2 ||
            atacandoAtaque2Fase2 ||
            atacandoAtaque3Fase2
        )
            return;

        atacandoAtaque2Fase2 = true;

        movendoAtaque2Fase2 = false;

        direcaoAtaque2Fase2 =
            Mathf.Sign(
                elian.position.x -
                transform.position.x
            );

        if (direcaoAtaque2Fase2 > 0)
            spriteRenderer.flipX = false;
        else if (direcaoAtaque2Fase2 < 0)
            spriteRenderer.flipX = true;

        if (hitboxAtaque2Fase2 != null)
        {
            hitboxAtaque2Fase2.PrepararNovoDash();
        }

        PararMovimento();

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

        rb.linearVelocity =
            new Vector2(
                0f,
                rb.linearVelocity.y
            );
    }

    public void FinalizarAtaque2Fase2()
    {
        atacandoAtaque2Fase2 = false;

        movendoAtaque2Fase2 = false;

        rb.linearVelocity =
            new Vector2(
                0f,
                rb.linearVelocity.y
            );

        proximoAtaque2Fase2 =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntreAtaques2Fase2
            );
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
            atacandoAtaque3Fase2
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
        atacandoAtaque3Fase2 = false;

        proximoAtaque3Fase2 =
            Time.time +
            AjustarCooldownPorVida(
                tempoEntreAtaques3Fase2
            );
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

        movendoNoDash = false;
        movendoAtaque2Fase2 = false;

        dashParadoAoAcertar = false;

        PararMovimento();
    }
}
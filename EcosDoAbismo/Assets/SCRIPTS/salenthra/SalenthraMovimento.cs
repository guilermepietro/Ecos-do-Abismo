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

    [Header("Ataque Dash")]
    public float distanciaDash = 4f;
    public float velocidadeDash = 8f;
    public float tempoEntreDashes = 2f;

    [Header("Ataque Puxão")]
    public float distanciaPuxao = 6f;
    public float forcaPuxao = 8f;
    public float tempoEntrePuxoes = 3f;

    [Header("Ataque Chuva")]
    public ChuvaEspadas chuvaEspadas;
    public float tempoEntreChuvas = 5f;

    public bool podeMover = true;
    public bool ativada = false;

    private bool fase2 = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // =========================
    // DASH
    // =========================

    private bool atacandoDash = false;
    private float direcaoDash;
    private float proximoDash = 0f;

    private bool dashParadoAoAcertar = false;
    private bool movendoNoDash = false;

    // =========================
    // PUXÃO
    // =========================

    private bool atacandoPuxao = false;
    private float proximoPuxao = 0f;

    // =========================
    // CHUVA
    // =========================

    private bool atacandoChuva = false;
    private float proximaChuva = 0f;

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
        // ATAQUES EM ANDAMENTO
        // =========================

        if (atacandoDash)
        {
            if (
                movendoNoDash &&
                !dashParadoAoAcertar
            )
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

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        // =========================
        // IA DA FASE 1
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
            elian.position.x - transform.position.x
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
        {
            return tempoEntreDecisoes * 0.5f;
        }

        if (porcentagem <= 0.60f)
        {
            return tempoEntreDecisoes * 0.75f;
        }

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
        {
            return cooldownBase * 0.70f;
        }

        if (porcentagem <= 0.60f)
        {
            return cooldownBase * 0.85f;
        }

        return cooldownBase;
    }

    // =========================
    // IA - ESCOLHA DE ATAQUE
    // =========================

    private bool EscolherAtaqueFase1(
        float distancia
    )
    {
        float pesoDash = 0f;
        float pesoPuxao = 0f;
        float pesoChuva = 0f;

        // =========================
        // DASH
        // =========================

        if (
            distancia <= distanciaDash &&
            Time.time >= proximoDash
        )
        {
            pesoDash = 60f;
        }

        // =========================
        // PUXÃO
        // =========================

        if (
            distancia > distanciaDash &&
            distancia <= distanciaPuxao &&
            Time.time >= proximoPuxao
        )
        {
            pesoPuxao = 60f;
        }

        // =========================
        // CHUVA
        // =========================

        if (Time.time >= proximaChuva)
        {
            if (distancia <= distanciaDash)
            {
                pesoChuva = 30f;
            }
            else if (
                distancia <= distanciaPuxao
            )
            {
                pesoChuva = 40f;
            }
            else
            {
                pesoChuva = 70f;
            }
        }

        float porcentagemVida = 1f;

        if (vidaSalenthra != null)
        {
            porcentagemVida =
                vidaSalenthra
                .ObterPorcentagemVidaAtual();
        }

        // =========================
        // MEMÓRIA DE COMBOS
        // =========================

        // PUXÃO -> DASH
        if (
            ultimoAtaque == 2 &&
            pesoDash > 0f
        )
        {
            if (porcentagemVida <= 0.30f)
            {
                pesoDash *= 4f;
            }
            else if (
                porcentagemVida <= 0.60f
            )
            {
                pesoDash *= 3f;
            }
            else
            {
                pesoDash *= 2.5f;
            }
        }

        // CHUVA -> PRESSÃO
        if (ultimoAtaque == 3)
        {
            if (pesoDash > 0f)
            {
                if (porcentagemVida <= 0.30f)
                {
                    pesoDash *= 2.2f;
                }
                else
                {
                    pesoDash *= 1.7f;
                }
            }

            if (pesoPuxao > 0f)
            {
                if (porcentagemVida <= 0.30f)
                {
                    pesoPuxao *= 1.8f;
                }
                else
                {
                    pesoPuxao *= 1.4f;
                }
            }
        }

        // =========================
        // EVITAR PADRÕES REPETITIVOS
        // =========================

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

        // =========================
        // EVITAR REPETIÇÃO IMEDIATA
        // =========================

        if (ultimoAtaque == 1)
        {
            pesoDash = 0f;
        }

        if (ultimoAtaque == 2)
        {
            pesoPuxao = 0f;
        }

        if (ultimoAtaque == 3)
        {
            pesoChuva = 0f;
        }

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

        // =========================
        // DASH
        // =========================

        if (escolha < pesoDash)
        {
            ataqueAnterior = ultimoAtaque;
            ultimoAtaque = 1;

            IniciarDash();

            return true;
        }

        escolha -= pesoDash;

        // =========================
        // PUXÃO
        // =========================

        if (escolha < pesoPuxao)
        {
            ataqueAnterior = ultimoAtaque;
            ultimoAtaque = 2;

            IniciarPuxao();

            return true;
        }

        // =========================
        // CHUVA
        // =========================

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

        animator.SetBool("Andando", false);
    }

    // =========================
    // ATAQUE 1 - DASH
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
        {
            spriteRenderer.flipX = false;
        }
        else if (direcaoDash < 0)
        {
            spriteRenderer.flipX = true;
        }

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

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    public void PararDashAoAcertar()
    {
        if (!atacandoDash)
            return;

        dashParadoAoAcertar = true;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    public void FinalizarDash()
    {
        atacandoDash = false;

        movendoNoDash = false;
        dashParadoAoAcertar = false;

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

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
    // ATAQUE 2 - PUXÃO
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
        {
            spriteRenderer.flipX = false;
        }
        else if (direcao < 0)
        {
            spriteRenderer.flipX = true;
        }

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
    // ATAQUE 3 - CHUVA
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

        movendoNoDash = false;
        dashParadoAoAcertar = false;

        PararMovimento();
    }
}
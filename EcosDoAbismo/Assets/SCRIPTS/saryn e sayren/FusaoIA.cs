
using UnityEngine;

public class FusaoIA : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private FusaoAtaque1 ataque1;
    [SerializeField] private FusaoAtaque2 ataque2;
    [SerializeField] private FusaoAtaque3 ataque3;
    [SerializeField] private FusaoVida vidaFusao;
    [SerializeField] private Transform elian;

    [Header("Configuracoes")]
    [SerializeField] private float distanciaMaximaAtaque = 7f;
    [SerializeField] private float atrasoInicial = 1.5f;

    [Header("Distancias")]
    [SerializeField] private float distanciaCurta = 3f;
    [SerializeField] private float distanciaLonga = 5f;

    [Header("Anti-Repeticao")]
    [Range(0f, 1f)]
    [SerializeField] private float penalidadeRetorno = 0.4f;

    [Header("Chance de Sequencia Dupla")]
    [Range(0f, 1f)]
    [SerializeField] private float chanceSequenciaNormal = 0.20f;

    [Range(0f, 1f)]
    [SerializeField] private float chanceSequenciaPressao = 0.50f;

    [Range(0f, 1f)]
    [SerializeField] private float chanceSequenciaDesespero = 0.75f;

    [Header("Pausa Entre Golpes da Sequencia")]
    [SerializeField] private float pausaNormal = 0.45f;
    [SerializeField] private float pausaPressao = 0.30f;
    [SerializeField] private float pausaDesespero = 0.20f;

    [Header("Recuperacao Obrigatoria")]
    [SerializeField] private float recuperacaoNormal = 1.6f;
    [SerializeField] private float recuperacaoPressao = 1.3f;
    [SerializeField] private float recuperacaoDesespero = 1.1f;

    [Header("Recuperacao Especial")]
    [SerializeField] private float extraAposChuva = 0.85f;
    [SerializeField] private float extraAposSequenciaDupla = 0.25f;

    [Header("Depuracao")]
    [SerializeField] private bool mostrarLogs = false;

    private ElianVida vidaElian;

    private float proximoAtaque;
    private bool iniciouCombate;
    private bool aguardandoFimAtaque;
    private bool emRecuperacao;

    private int ultimoAtaque;
    private int penultimoAtaque;

    private int ataquesNaSequencia;
    private int quantidadeAtaquesSequencia;

    private bool sequenciaTeveChuva;

    public bool EmRecuperacao => emRecuperacao;

    private void Awake()
    {
        if (movimento == null)
            movimento = GetComponent<FusaoMovimento>();

        if (ataque1 == null)
            ataque1 = GetComponent<FusaoAtaque1>();

        if (ataque2 == null)
            ataque2 = GetComponent<FusaoAtaque2>();

        if (ataque3 == null)
            ataque3 = GetComponent<FusaoAtaque3>();

        if (vidaFusao == null)
            vidaFusao = GetComponent<FusaoVida>();

        if (elian == null)
        {
            GameObject objeto = GameObject.Find("elian");

            if (objeto != null)
                elian = objeto.transform;
        }

        if (elian != null)
            vidaElian = elian.GetComponent<ElianVida>();
    }

    private void Update()
    {
        if (movimento == null ||
            ataque1 == null ||
            ataque2 == null ||
            ataque3 == null ||
            elian == null)
        {
            return;
        }

        if (!movimento.CombateAtivo)
        {
            ReiniciarEstado();
            return;
        }

        if (vidaFusao != null && vidaFusao.Morto)
            return;

        if (!iniciouCombate)
        {
            iniciouCombate = true;
            proximoAtaque = Time.time + atrasoInicial;

            ataquesNaSequencia = 0;
            quantidadeAtaquesSequencia = 0;
            sequenciaTeveChuva = false;
        }

        if (vidaElian != null && vidaElian.EstaMorto)
            return;

        // Aguarda a conclusao real do ataque.
        if (aguardandoFimAtaque)
        {
            if (AlgumAtaqueAtivo())
                return;

            aguardandoFimAtaque = false;

            // A chuva de pedras sempre encerra
            // a sequencia, garantindo recuperacao.
            if (sequenciaTeveChuva ||
                ataquesNaSequencia >= quantidadeAtaquesSequencia)
            {
                IniciarRecuperacao();
            }
            else
            {
                // Intervalo curto para o segundo golpe.
                proximoAtaque =
                    Time.time + ObterPausaSequencia();
            }

            return;
        }

        if (AlgumAtaqueAtivo())
            return;

        // Durante a recuperacao, a Fusao nao
        // pode iniciar ataques nem perseguir.
        if (emRecuperacao)
        {
            if (Time.time < proximoAtaque)
                return;

            FinalizarRecuperacao();
        }

        if (Time.time < proximoAtaque)
            return;

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        // Se o jogador escapou de uma sequencia,
        // a Fusao entra em recuperacao.
        if (distancia > distanciaMaximaAtaque)
        {
            if (ataquesNaSequencia > 0)
                IniciarRecuperacao();

            return;
        }

        // Inicia uma nova sequencia.
        if (ataquesNaSequencia == 0)
            PrepararSequencia();

        EscolherAtaque(distancia);
    }

    // =========================
    // NIVEL DE AGRESSIVIDADE
    // =========================

    private float ObterPorcentagemVida()
    {
        if (vidaFusao == null)
            return 1f;

        return Mathf.Clamp01(
            vidaFusao.PorcentagemVida
        );
    }

    private float ObterChanceSequencia()
    {
        float vida = ObterPorcentagemVida();

        if (vida <= 0.30f)
            return chanceSequenciaDesespero;

        if (vida <= 0.60f)
            return chanceSequenciaPressao;

        return chanceSequenciaNormal;
    }

    private float ObterPausaSequencia()
    {
        float vida = ObterPorcentagemVida();

        if (vida <= 0.30f)
            return Mathf.Max(0f, pausaDesespero);

        if (vida <= 0.60f)
            return Mathf.Max(0f, pausaPressao);

        return Mathf.Max(0f, pausaNormal);
    }

    private float ObterTempoRecuperacao()
    {
        float vida = ObterPorcentagemVida();

        float tempo;

        if (vida <= 0.30f)
        {
            tempo = recuperacaoDesespero;
        }
        else if (vida <= 0.60f)
        {
            tempo = recuperacaoPressao;
        }
        else
        {
            tempo = recuperacaoNormal;
        }

        // A chuva e o golpe mais perigoso.
        // Sempre concede uma abertura maior.
        if (sequenciaTeveChuva)
            tempo += extraAposChuva;

        // Duas ofensivas seguidas exigem
        // uma recuperacao adicional.
        if (ataquesNaSequencia >= 2)
            tempo += extraAposSequenciaDupla;

        return Mathf.Max(0.2f, tempo);
    }

    // =========================
    // PREPARAR SEQUENCIA
    // =========================

    private void PrepararSequencia()
    {
        ataquesNaSequencia = 0;
        sequenciaTeveChuva = false;

        float chance = ObterChanceSequencia();

        bool sequenciaDupla =
            Random.value < chance;

        quantidadeAtaquesSequencia =
            sequenciaDupla ? 2 : 1;

        if (mostrarLogs)
        {
            Debug.Log(
                "Fusao IA - Sequencia planejada: " +
                quantidadeAtaquesSequencia + " ataque(s)."
            );
        }
    }

    // =========================
    // SELECAO DOS ATAQUES
    // =========================

    private void EscolherAtaque(float distancia)
    {
        float peso1;
        float peso2;
        float peso3;

        // Preserva os pesos originais
        // definidos pela distancia do Elian.
        if (distancia < distanciaCurta)
        {
            peso1 = 5f;
            peso2 = 5f;
            peso3 = 2f;
        }
        else if (distancia < distanciaLonga)
        {
            peso1 = 6f;
            peso2 = 4f;
            peso3 = 4f;
        }
        else
        {
            peso1 = 4f;
            peso2 = 4f;
            peso3 = 6f;
        }

        // Nunca repete imediatamente
        // o mesmo ataque.
        if (ultimoAtaque == 1)
            peso1 = 0f;

        if (ultimoAtaque == 2)
            peso2 = 0f;

        if (ultimoAtaque == 3)
            peso3 = 0f;

        // Reduz a chance de A-B-A.
        if (penultimoAtaque == 1)
            peso1 *= penalidadeRetorno;

        if (penultimoAtaque == 2)
            peso2 *= penalidadeRetorno;

        if (penultimoAtaque == 3)
            peso3 *= penalidadeRetorno;

        float pesoTotal =
            peso1 + peso2 + peso3;

        if (pesoTotal <= 0f)
            return;

        float sorteio =
            Random.Range(0f, pesoTotal);

        int escolhido;

        if (sorteio < peso1)
        {
            escolhido = 1;
        }
        else if (sorteio < peso1 + peso2)
        {
            escolhido = 2;
        }
        else
        {
            escolhido = 3;
        }

        // Executa exatamente os mesmos
        // metodos dos ataques originais.
        switch (escolhido)
        {
            case 1:
                ataque1.IniciarAtaque();
                break;

            case 2:
                ataque2.IniciarAtaque();
                break;

            case 3:
                ataque3.IniciarAtaque();
                break;
        }

        bool iniciou =
            (escolhido == 1 && ataque1.EmAtaque) ||
            (escolhido == 2 && ataque2.EmAtaque) ||
            (escolhido == 3 && ataque3.EmAtaque);

        if (!iniciou)
        {
            proximoAtaque = Time.time + 0.5f;
            return;
        }

        penultimoAtaque = ultimoAtaque;
        ultimoAtaque = escolhido;

        ataquesNaSequencia++;
        aguardandoFimAtaque = true;

        if (escolhido == 3)
            sequenciaTeveChuva = true;

        if (mostrarLogs)
        {
            Debug.Log(
                "Fusao IA - Ataque " + escolhido +
                " | Golpe " + ataquesNaSequencia +
                "/" + quantidadeAtaquesSequencia
            );
        }
    }

    // =========================
    // RECUPERACAO
    // =========================

    private void IniciarRecuperacao()
    {
        if (emRecuperacao)
            return;

        aguardandoFimAtaque = false;
        emRecuperacao = true;

        float tempo = ObterTempoRecuperacao();

        proximoAtaque = Time.time + tempo;

        // Impede a persecucao durante
        // a janela de contra-ataque.
        movimento.BloquearMovimento(true);

        if (mostrarLogs)
        {
            Debug.Log(
                "Fusao IA - Recuperando por " +
                tempo.ToString("F2") + " segundos."
            );
        }
    }

    private void FinalizarRecuperacao()
    {
        emRecuperacao = false;

        ataquesNaSequencia = 0;
        quantidadeAtaquesSequencia = 0;
        sequenciaTeveChuva = false;

        movimento.BloquearMovimento(false);

        proximoAtaque = Time.time;

        if (mostrarLogs)
        {
            Debug.Log(
                "Fusao IA - Recuperacao encerrada."
            );
        }
    }

    // =========================
    // CONTROLES AUXILIARES
    // =========================

    private bool AlgumAtaqueAtivo()
    {
        return
            ataque1.EmAtaque ||
            ataque2.EmAtaque ||
            ataque3.EmAtaque;
    }

    private void ReiniciarEstado()
    {
        if (emRecuperacao)
        {
            emRecuperacao = false;
            movimento.BloquearMovimento(false);
        }

        iniciouCombate = false;
        aguardandoFimAtaque = false;

        ultimoAtaque = 0;
        penultimoAtaque = 0;

        ataquesNaSequencia = 0;
        quantidadeAtaquesSequencia = 0;
        sequenciaTeveChuva = false;

        proximoAtaque = 0f;
    }
}

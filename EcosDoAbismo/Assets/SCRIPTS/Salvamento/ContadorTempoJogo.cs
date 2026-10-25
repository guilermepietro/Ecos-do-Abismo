
using UnityEngine;

public class ContadorTempoJogo : MonoBehaviour
{
    public static ContadorTempoJogo instancia;

    [Header("Salvamento Automático")]
    public float intervaloSalvamento = 30f;

    private float tempoTotal;
    private float tempoDesdeUltimoSalvamento;

    private int slotAtual;
    private bool contadorAtivo = false;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        slotAtual = MenuSelecaoSave.slotPartidaAtual;

        if (slotAtual < 1 || slotAtual > 3)
        {
            Debug.LogWarning(
                "Contador de tempo: nenhum slot selecionado."
            );
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slotAtual);

        if (dados == null)
        {
            Debug.LogError(
                "Não foi possível iniciar o contador de tempo."
            );
            return;
        }

        tempoTotal = Mathf.Max(
            0f,
            dados.tempoTotalJogado
        );

        tempoDesdeUltimoSalvamento = 0f;
        contadorAtivo = true;

        Debug.Log(
            "Contador iniciado no slot " + slotAtual +
            "\nTempo acumulado: " + FormatarTempo(tempoTotal)
        );
    }

    void Update()
    {
        if (!contadorAtivo)
            return;

        float tempoDecorrido = Time.deltaTime;

        tempoTotal += tempoDecorrido;
        tempoDesdeUltimoSalvamento += tempoDecorrido;

        if (tempoDesdeUltimoSalvamento >=
            Mathf.Max(1f, intervaloSalvamento))
        {
            SalvarTempo();
        }
    }

    // Atualiza o tempo nos dados antes de salvar
    public void AtualizarTempoNosDados(
        DadosSalvamento dados)
    {
        if (!contadorAtivo || dados == null)
            return;

        if (dados.numeroSlot != slotAtual)
            return;

        dados.tempoTotalJogado = Mathf.Max(
            tempoTotal,
            dados.tempoTotalJogado
        );

        tempoTotal = dados.tempoTotalJogado;
    }

    public void SalvarTempo()
    {
        if (!contadorAtivo)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slotAtual);

        if (dados == null)
        {
            Debug.LogError(
                "Não foi possível carregar os dados " +
                "para salvar o tempo."
            );
            return;
        }

        AtualizarTempoNosDados(dados);

        if (GerenciadorSalvamento.Salvar(slotAtual, dados))
        {
            tempoDesdeUltimoSalvamento = 0f;

            Debug.Log(
                "Tempo salvo no slot " + slotAtual +
                ": " + FormatarTempo(tempoTotal)
            );
        }
    }

    private string FormatarTempo(float segundos)
    {
        System.TimeSpan tempo =
            System.TimeSpan.FromSeconds(segundos);

        return string.Format(
            "{0:00}:{1:00}:{2:00}",
            (int)tempo.TotalHours,
            tempo.Minutes,
            tempo.Seconds
        );
    }

    void OnApplicationPause(bool pausado)
    {
        if (pausado)
            SalvarTempo();
    }

    void OnApplicationQuit()
    {
        SalvarTempo();
    }

    void OnDisable()
    {
        SalvarTempo();
    }

    void OnDestroy()
    {
        if (instancia == this)
            instancia = null;
    }
}

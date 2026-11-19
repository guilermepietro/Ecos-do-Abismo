
using System.Collections.Generic;
using UnityEngine;

public class ElianBencaos : MonoBehaviour
{
    [Header("Bencao da Ascensao")]
    [SerializeField, Min(1f)]
    private float multiplicadorPulo = 1.35f;

    [SerializeField]
    private KeyCode teclaAscensao = KeyCode.Alpha1;

    [Header("Investida Flamejante")]
    [SerializeField, Min(1)]
    private int danoInvestidaFlamejante = 20;

    [Header("Investida Celeste")]
    [SerializeField, Min(1f)]
    private float multiplicadorDistanciaDash = 1.4f;

    [SerializeField, Min(0f)]
    private float velocidadeDashCeleste = 10f;

    [SerializeField, Min(0f)]
    private float tempoMaximoDashCeleste = 0.7f;

    private ElianMovimento movimento;
    private ElianVida vida;

    private float forcaPuloOriginal;
    private float tempoDashOriginal;
    private float velocidadeDashOriginal;

    private bool possuiAscensao;
    private bool ascensaoAtiva;
    private bool possuiInvestidaFlamejante;
    private bool possuiInvestidaCeleste;
    private bool possuiSegundaExistencia;

    public bool PossuiAscensao => possuiAscensao;
    public bool AscensaoAtiva => ascensaoAtiva;

    public bool PossuiInvestidaFlamejante =>
        possuiInvestidaFlamejante;

    public int DanoInvestidaFlamejante =>
        danoInvestidaFlamejante;

    public bool PossuiInvestidaCeleste =>
        possuiInvestidaCeleste;

    public bool PossuiSegundaExistencia =>
        possuiSegundaExistencia;

    void Awake()
    {
        movimento = GetComponent<ElianMovimento>();
        vida = GetComponent<ElianVida>();

        if (movimento != null)
        {
            forcaPuloOriginal = movimento.forcaPulo;
            tempoDashOriginal = movimento.tempoDash;
            velocidadeDashOriginal = movimento.velocidadeDash;
        }
    }

    void Start()
    {
        CarregarBencaos();
    }

    void Update()
    {
        if (!possuiAscensao ||
            (vida != null && vida.EstaMorto) ||
            movimento == null ||
            !movimento.podeMover)
        {
            return;
        }

        if (Input.GetKeyDown(teclaAscensao))
        {
            ascensaoAtiva = !ascensaoAtiva;
            AplicarAscensao();

            Debug.Log(
                ascensaoAtiva
                    ? "Ascensao ATIVADA."
                    : "Ascensao DESATIVADA."
            );
        }
    }

    private void CarregarBencaos()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
            return;

        possuiAscensao =
            PossuiBencao(dados, "Ascensao");

        possuiInvestidaFlamejante =
            PossuiBencao(dados, "InvestidaFlamejante");

        possuiInvestidaCeleste =
            PossuiBencao(dados, "InvestidaCeleste");

        possuiSegundaExistencia =
            PossuiBencao(dados, "SegundaExistencia");

        ascensaoAtiva = false;

        AplicarAscensao();
        AplicarInvestidaCeleste();
    }

    private bool PossuiBencao(
        DadosSalvamento dados,
        string idBencao)
    {
        return dados.bencaosRecebidas != null &&
               dados.bencaosRecebidas.Contains(idBencao);
    }

    private void AplicarAscensao()
    {
        if (movimento == null)
            return;

        movimento.forcaPulo =
            possuiAscensao && ascensaoAtiva
                ? forcaPuloOriginal * multiplicadorPulo
                : forcaPuloOriginal;
    }

    private void AplicarInvestidaCeleste()
    {
        if (movimento == null)
            return;

        if (possuiInvestidaCeleste)
        {
            movimento.tempoDash = Mathf.Min(
                tempoDashOriginal *
                multiplicadorDistanciaDash,
                tempoMaximoDashCeleste
            );

            movimento.velocidadeDash =
                velocidadeDashCeleste;
        }
        else
        {
            movimento.tempoDash = tempoDashOriginal;
            movimento.velocidadeDash = velocidadeDashOriginal;
        }
    }

    public bool ConcederAscensao()
    {
        if (!RegistrarBencao("Ascensao"))
            return false;

        possuiAscensao = true;
        ascensaoAtiva = false;

        AplicarAscensao();

        Debug.Log("Bencao da Ascensao adquirida!");

        return true;
    }

    public bool ConcederInvestidaFlamejante()
    {
        if (!RegistrarBencao("InvestidaFlamejante"))
            return false;

        possuiInvestidaFlamejante = true;

        Debug.Log("Investida Flamejante adquirida!");

        return true;
    }

    public bool ConcederInvestidaCeleste()
    {
        if (!RegistrarBencao("InvestidaCeleste"))
            return false;

        possuiInvestidaCeleste = true;

        AplicarInvestidaCeleste();

        Debug.Log(
            "Investida Celeste adquirida! " +
            "Velocidade: " + movimento.velocidadeDash +
            " | Duracao: " + movimento.tempoDash
        );

        return true;
    }

    public bool ConcederSegundaExistencia()
    {
        if (!RegistrarBencao("SegundaExistencia"))
            return false;

        possuiSegundaExistencia = true;

        Debug.Log(
            "Segunda Existencia adquirida! " +
            "Uma morte fatal podera ser evitada por vida."
        );

        return true;
    }

    private bool RegistrarBencao(string idBencao)
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return false;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
            return false;

        if (dados.bencaosRecebidas == null)
        {
            dados.bencaosRecebidas =
                new List<string>();
        }

        if (!dados.bencaosRecebidas.Contains(idBencao))
        {
            dados.bencaosRecebidas.Add(idBencao);
        }

        if (ContadorTempoJogo.instancia != null)
        {
            ContadorTempoJogo.instancia
                .AtualizarTempoNosDados(dados);
        }

        if (!GerenciadorSalvamento.Salvar(slot, dados))
        {
            Debug.LogError(
                "Falha ao salvar a bencao: " + idBencao
            );

            return false;
        }

        return true;
    }
}

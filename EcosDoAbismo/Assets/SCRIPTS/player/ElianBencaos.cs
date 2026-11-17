
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
    private float multiplicadorDistanciaDash = 1.5f;

    private ElianMovimento movimento;
    private ElianVida vida;

    private float forcaPuloOriginal;
    private float tempoDashOriginal;

    private bool possuiAscensao;
    private bool ascensaoAtiva;

    private bool possuiInvestidaFlamejante;
    private bool possuiInvestidaCeleste;

    public bool PossuiAscensao => possuiAscensao;
    public bool AscensaoAtiva => ascensaoAtiva;

    public bool PossuiInvestidaFlamejante =>
        possuiInvestidaFlamejante;

    public int DanoInvestidaFlamejante =>
        danoInvestidaFlamejante;

    public bool PossuiInvestidaCeleste =>
        possuiInvestidaCeleste;

    void Awake()
    {
        movimento = GetComponent<ElianMovimento>();
        vida = GetComponent<ElianVida>();

        if (movimento != null)
        {
            forcaPuloOriginal = movimento.forcaPulo;
            tempoDashOriginal = movimento.tempoDash;
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
            dados.bencaosRecebidas != null &&
            dados.bencaosRecebidas.Contains("Ascensao");

        possuiInvestidaFlamejante =
            dados.bencaosRecebidas != null &&
            dados.bencaosRecebidas.Contains(
                "InvestidaFlamejante"
            );

        possuiInvestidaCeleste =
            dados.bencaosRecebidas != null &&
            dados.bencaosRecebidas.Contains(
                "InvestidaCeleste"
            );

        ascensaoAtiva = false;

        AplicarAscensao();
        AplicarInvestidaCeleste();
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

    

    public bool ConcederAscensao()
    {
        if (!RegistrarBencao("Ascensao"))
            return false;

        possuiAscensao = true;
        ascensaoAtiva = false;

        AplicarAscensao();

        Debug.Log(
            "Bencao da Ascensao adquirida!"
        );

        return true;
    }

    public bool ConcederInvestidaFlamejante()
    {
        if (!RegistrarBencao("InvestidaFlamejante"))
            return false;

        possuiInvestidaFlamejante = true;

        Debug.Log(
            "Investida Flamejante adquirida!"
        );

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
            "Nova duracao do dash: " + movimento.tempoDash
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

    private void AplicarInvestidaCeleste()
{
    if (movimento == null)
        return;

    movimento.tempoDash =
        possuiInvestidaCeleste
            ? Mathf.Min(
                tempoDashOriginal * multiplicadorDistanciaDash,
                0.6f
              )
            : tempoDashOriginal;
}
}

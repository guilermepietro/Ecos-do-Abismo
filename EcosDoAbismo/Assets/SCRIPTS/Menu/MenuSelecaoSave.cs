
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuSelecaoSave : MonoBehaviour
{
    [Header("Slots de Salvamento")]
    public Image[] slots;

    [Header("Imagens dos Slots")]
    public Sprite slotNormal;
    public Sprite slotSelecionado;

    [Header("Informações dos Salvamentos")]
    public TMP_Text[] textosLocal;
    public TMP_Text[] textosTempo;

    [Header("Aparência dos Slots")]
    public Image[] retratos;
    public TMP_Text[] textosNome;
    public TMP_Text[] textosStatus;
    public GameObject[] iconesExcluir;

    [Header("Menu Principal")]
    public MenuManager menuManager;

    private int indiceSelecionado = 0;
    private Color[] coresOriginaisRetratos;

    // Slot utilizado pela partida atual
    public static int slotPartidaAtual = 0;

    private bool iniciandoPartida = false;

    void Awake()
    {
        if (retratos == null)
            return;

        coresOriginaisRetratos = new Color[retratos.Length];

        for (int i = 0; i < retratos.Length; i++)
        {
            if (retratos[i] != null)
                coresOriginaisRetratos[i] = retratos[i].color;
        }
    }

    void OnEnable()
    {
        indiceSelecionado = 0;
        iniciandoPartida = false;

        AtualizarSelecao();
        AtualizarInformacoes();
    }

    void Update()
    {
        if (iniciandoPartida)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuManager != null)
                menuManager.VoltarAoMenu();

            return;
        }

        if (slots == null || slots.Length == 0)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            indiceSelecionado++;

            if (indiceSelecionado >= slots.Length)
                indiceSelecionado = 0;

            AtualizarSelecao();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            indiceSelecionado--;

            if (indiceSelecionado < 0)
                indiceSelecionado = slots.Length - 1;

            AtualizarSelecao();
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmarSlot();
        }
    }

    void ConfirmarSlot()
    {
        int numeroSlot = indiceSelecionado + 1;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(numeroSlot);

        if (dados == null)
        {
            if (GerenciadorSalvamento.ExisteSalvamento(numeroSlot))
            {
                Debug.LogError(
                    "O slot " + numeroSlot +
                    " possui um salvamento que não pôde ser carregado."
                );

                return;
            }

            dados =
                GerenciadorSalvamento.CriarNovoJogo(numeroSlot);

            if (dados == null)
            {
                Debug.LogError(
                    "Não foi possível criar a partida no slot " +
                    numeroSlot
                );

                return;
            }

            Debug.Log(
                "Nova partida criada no slot " + numeroSlot
            );
        }
        else
        {
            Debug.Log(
                "Continuando partida do slot " + numeroSlot +
                "\nÁrea: " + dados.nomeArea +
                "\nTempo: " + dados.tempoTotalJogado
            );
        }

        AtualizarInformacoes();

        if (menuManager == null)
        {
            Debug.LogError(
                "MenuManager não configurado!"
            );
            return;
        }

        // Guarda qual arquivo pertence à partida
        slotPartidaAtual = numeroSlot;

        iniciandoPartida = true;

        Debug.Log(
            "Iniciando Gameplay pelo slot " +
            slotPartidaAtual
        );

        menuManager.IniciarJogoSelecionado();
    }

    void AtualizarSelecao()
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (i == indiceSelecionado)
                slots[i].sprite = slotSelecionado;
            else
                slots[i].sprite = slotNormal;
        }
    }

    void AtualizarInformacoes()
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            DadosSalvamento dados =
                GerenciadorSalvamento.Carregar(i + 1);

            bool ocupado = dados != null;

            if (retratos != null &&
                i < retratos.Length &&
                retratos[i] != null)
            {
                if (ocupado)
                    retratos[i].color = coresOriginaisRetratos[i];
                else
                    retratos[i].color = Color.black;
            }

            if (textosNome != null &&
                i < textosNome.Length &&
                textosNome[i] != null)
            {
                textosNome[i].gameObject.SetActive(ocupado);

                if (ocupado)
                    textosNome[i].text = "ELIAN";
            }

            if (textosStatus != null &&
                i < textosStatus.Length &&
                textosStatus[i] != null)
            {
                textosStatus[i].gameObject.SetActive(!ocupado);

                if (!ocupado)
                    textosStatus[i].text = "VAZIO";
            }

            if (textosLocal != null &&
                i < textosLocal.Length &&
                textosLocal[i] != null)
            {
                textosLocal[i].gameObject.SetActive(ocupado);

                if (ocupado)
                    textosLocal[i].text = dados.nomeArea;
            }

            if (textosTempo != null &&
                i < textosTempo.Length &&
                textosTempo[i] != null)
            {
                textosTempo[i].gameObject.SetActive(ocupado);

                if (ocupado)
                {
                    System.TimeSpan tempo =
                        System.TimeSpan.FromSeconds(
                            Mathf.Max(0f, dados.tempoTotalJogado)
                        );

                    textosTempo[i].text =
                        string.Format(
                            "{0:00}:{1:00}:{2:00}",
                            (int)tempo.TotalHours,
                            tempo.Minutes,
                            tempo.Seconds
                        );
                }
            }

            if (iconesExcluir != null &&
                i < iconesExcluir.Length &&
                iconesExcluir[i] != null)
            {
                iconesExcluir[i].SetActive(ocupado);
            }
        }
    }
}

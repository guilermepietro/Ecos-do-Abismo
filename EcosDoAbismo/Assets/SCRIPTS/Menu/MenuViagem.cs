
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuViagem : MonoBehaviour
{
    [Header("Slots dos Destinos")]
    public Image[] slots;

    [Header("Sprites dos Slots")]
    public Sprite slotNormal;
    public Sprite slotSelecionado;

    [Header("Informações do Destino")]
    public TMP_Text textoDestino;

    [Header("Ícones dos Checkpoints")]
    public Image[] iconesCheckpoint;

    [Header("Teletransporte")]
    public Transform elian;
    public TransicaoTela transicaoTela;

    [Header("Tempo de Escuridão")]
    public float tempoTelaEscura = 3f;

    [Header("Frase da Viagem")]
    public FraseViagem fraseViagem;

    private int indiceSelecionado = -1;
    private string idArvoreOrigem = "";
    private bool viajando = false;

    private CanvasGroup grupoInterface;

    private readonly string[] idsArvores =
    {
        "Ecos_Floresta",
        "Ecos_RuinasDoSelo",
        "Ecos_CatedralDasSombras",
        "Ecos_FortalezaArdente"
    };

    private readonly string[] nomesAreas =
    {
        "FLORESTA",
        "RUÍNAS DO SELO",
        "CATEDRAL DAS SOMBRAS",
        "FORTALEZA ARDENTE"
    };

    private readonly string[] nomesAreasSalvamento =
    {
        "Floresta",
        "Ruínas do Selo",
        "Catedral das Sombras",
        "Fortaleza Ardente"
    };

    private bool[] destinosDesbloqueados;

    void Awake()
    {
        grupoInterface = GetComponent<CanvasGroup>();

        if (grupoInterface == null)
        {
            grupoInterface = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void DefinirOrigem(string idCheckpoint)
    {
        idArvoreOrigem = idCheckpoint;
    }

    void OnEnable()
    {
        viajando = false;

        if (grupoInterface != null)
        {
            grupoInterface.alpha = 1f;
            grupoInterface.interactable = true;
            grupoInterface.blocksRaycasts = true;
        }

        CarregarDestinos();

        indiceSelecionado = -1;

        for (int i = 0; i < destinosDesbloqueados.Length; i++)
        {
            if (destinosDesbloqueados[i])
            {
                indiceSelecionado = i;
                break;
            }
        }

        AtualizarSelecao();
    }

    void Update()
    {
        if (viajando)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FecharMenu();
            return;
        }

        if (slots == null || slots.Length == 0)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Navegar(1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Navegar(-1);
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmarDestino();
        }
    }

    void CarregarDestinos()
    {
        destinosDesbloqueados =
            new bool[idsArvores.Length];

        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot >= 1 && slot <= 3)
        {
            DadosSalvamento dados =
                GerenciadorSalvamento.Carregar(slot);

            if (dados != null &&
                dados.checkpointsAtivados != null)
            {
                for (int i = 0; i < idsArvores.Length; i++)
                {
                    destinosDesbloqueados[i] =
                        dados.checkpointsAtivados.Contains(
                            idsArvores[i]
                        );
                }
            }
        }

        AtualizarIcones();
    }

    void AtualizarIcones()
    {
        if (iconesCheckpoint == null)
            return;

        for (int i = 0; i < iconesCheckpoint.Length; i++)
        {
            if (iconesCheckpoint[i] == null)
                continue;

            bool desbloqueado =
                i < destinosDesbloqueados.Length &&
                destinosDesbloqueados[i];

            iconesCheckpoint[i].color =
                desbloqueado ? Color.white : Color.black;
        }
    }

    void Navegar(int direcao)
    {
        if (destinosDesbloqueados == null ||
            indiceSelecionado < 0 ||
            slots == null)
        {
            return;
        }

        int quantidade = Mathf.Min(
            slots.Length,
            destinosDesbloqueados.Length
        );

        for (int passo = 1; passo <= quantidade; passo++)
        {
            int proximoIndice =
                (indiceSelecionado +
                 direcao * passo +
                 quantidade * 2) % quantidade;

            if (destinosDesbloqueados[proximoIndice])
            {
                indiceSelecionado = proximoIndice;
                AtualizarSelecao();
                return;
            }
        }
    }

    void ConfirmarDestino()
    {
        if (destinosDesbloqueados == null ||
            indiceSelecionado < 0 ||
            indiceSelecionado >= idsArvores.Length)
        {
            Debug.LogWarning(
                "Nenhum destino disponível para viagem."
            );
            return;
        }

        if (!destinosDesbloqueados[indiceSelecionado])
        {
            Debug.LogWarning(
                "Destino ainda não descoberto."
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(idArvoreOrigem))
        {
            Debug.LogWarning(
                "Não foi possível identificar a árvore de origem."
            );
            return;
        }

        string idDestino = idsArvores[indiceSelecionado];

        if (idDestino == idArvoreOrigem)
        {
            Debug.Log(
                "Elian já está nesta Árvore dos Ecos: " +
                idDestino
            );
            return;
        }

        if (elian == null || transicaoTela == null ||
            transicaoTela.imagemFade == null)
        {
            Debug.LogError(
                "Elian ou TransicaoTela não configurados!"
            );
            return;
        }

        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogError(
                "Nenhum slot válido para realizar a viagem."
            );
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null ||
            dados.checkpointsAtivados == null ||
            !dados.checkpointsAtivados.Contains(idDestino))
        {
            Debug.LogError(
                "O destino não está desbloqueado no salvamento."
            );
            return;
        }

        CheckpointArvore[] arvores =
            FindObjectsByType<CheckpointArvore>(
                FindObjectsSortMode.None
            );

        Transform pontoDestino = null;

        foreach (CheckpointArvore arvore in arvores)
        {
            if (arvore.idCheckpoint != idDestino)
                continue;

            Transform raiz = arvore.transform.parent;

            if (raiz != null)
            {
                pontoDestino =
                    raiz.Find("PontoRenascimento");
            }

            break;
        }

        if (pontoDestino == null)
        {
            Debug.LogError(
                "PontoRenascimento não encontrado para " +
                idDestino
            );
            return;
        }

        StartCoroutine(
            ExecutarViagem(
                slot,
                dados,
                idDestino,
                nomesAreasSalvamento[indiceSelecionado],
                pontoDestino
            )
        );
    }

    IEnumerator ExecutarViagem(
        int slot,
        DadosSalvamento dados,
        string idDestino,
        string nomeArea,
        Transform pontoDestino)
    {
        viajando = true;

        Debug.Log(
            "Iniciando viagem para " + idDestino
        );

        // 1. Escurece rapidamente.
        yield return StartCoroutine(
            transicaoTela.Escurecer()
        );

        // 2. Atualiza o checkpoint.
        dados.idCheckpoint = idDestino;
        dados.cenaCheckpoint =
            SceneManager.GetActiveScene().name;
        dados.nomeArea = nomeArea;

        if (ContadorTempoJogo.instancia != null)
        {
            ContadorTempoJogo.instancia
                .AtualizarTempoNosDados(dados);
        }

        // 3. Salva o destino.
        if (!GerenciadorSalvamento.Salvar(slot, dados))
        {
            Debug.LogError(
                "Falha ao salvar o destino da viagem."
            );

            yield return StartCoroutine(
                transicaoTela.Clarear()
            );

            viajando = false;
            yield break;
        }

        // 4. Transporta Elian com a tela preta.
        Rigidbody2D rb =
            elian.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = pontoDestino.position;
        }

        elian.position = pontoDestino.position;

        Physics2D.SyncTransforms();

        GerenciadorArea.areaAtual = nomeArea;

        Debug.Log(
            "VIAGEM CONCLUÍDA!" +
            "\nDestino: " + idDestino +
            "\nÁrea: " + nomeArea +
            "\nNovo checkpoint de renascimento salvo."
        );

        // 5. Oculta visualmente o menu.
        if (grupoInterface != null)
        {
            grupoInterface.alpha = 0f;
            grupoInterface.interactable = false;
            grupoInterface.blocksRaycasts = false;
        }

        // 6. Exibe uma frase durante a escuridão.
        if (fraseViagem != null)
        {
            fraseViagem.MostrarFrase();
        }

        yield return new WaitForSecondsRealtime(
            Mathf.Max(0f, tempoTelaEscura)
        );

        // 7. Oculta a frase antes de clarear.
        if (fraseViagem != null)
        {
            fraseViagem.OcultarFrase();
        }

        // Permite atualização da câmera.
        yield return null;

        // 8. Revela a nova região.
        yield return StartCoroutine(
            transicaoTela.Clarear()
        );

        // 9. Fecha o menu.
        viajando = false;
        FecharMenu();
    }

    void FecharMenu()
    {
        gameObject.SetActive(false);
    }

    void AtualizarSelecao()
    {
        if (slots == null || slots.Length == 0)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            slots[i].sprite =
                i == indiceSelecionado
                ? slotSelecionado
                : slotNormal;
        }

        if (textoDestino != null)
        {
            if (indiceSelecionado >= 0 &&
                indiceSelecionado < nomesAreas.Length)
            {
                textoDestino.text =
                    nomesAreas[indiceSelecionado];
            }
            else
            {
                textoDestino.text = "";
            }
        }
    }
}


using UnityEngine;
using UnityEngine.UI;
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

    private int indiceSelecionado = -1;

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

    private bool[] destinosDesbloqueados;

    void OnEnable()
    {
        CarregarDestinos();

        // Seleciona a primeira árvore descoberta.
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
            indiceSelecionado < 0)
            return;

        int quantidade = Mathf.Min(
            slots.Length,
            destinosDesbloqueados.Length
        );

        // Procura o próximo destino desbloqueado.
        // Os bloqueados são ignorados.
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

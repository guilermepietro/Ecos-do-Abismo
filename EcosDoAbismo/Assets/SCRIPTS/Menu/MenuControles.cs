
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class MenuControles : MonoBehaviour
{
    [Header("Textos - Coluna Esquerda")]
    public TextMeshProUGUI[] textosEsquerda;

    [Header("Destaques - Coluna Esquerda")]
    public GameObject[] destaquesEsquerda;

    [Header("Textos - Coluna Direita")]
    public TextMeshProUGUI[] textosDireita;

    [Header("Destaques - Coluna Direita")]
    public GameObject[] destaquesDireita;

    [Header("Indicadores")]
    public GameObject teclaContinuar;
    public GameObject teclaAplicar;

    [Header("Remapeamento - Ataque")]
    public TextMeshProUGUI textoTeclaAtaque;
    public Image imagemTeclaAtaque;

    [Header("Remapeamento - Ataque em Area")]
    public TextMeshProUGUI textoTeclaAtaqueArea;
    public Image imagemTeclaAtaqueArea;

    [Header("Remapeamento - Defender")]
    public TextMeshProUGUI textoTeclaDefender;
    public Image imagemTeclaDefender;

    [Header("Remapeamento - Cura")]
    public TextMeshProUGUI textoTeclaCura;
    public Image imagemTeclaCura;

    [Header("Remapeamento - Interagir")]
    public TextMeshProUGUI textoTeclaInteragir;
    public Image imagemTeclaInteragir;

    [Header("Remapeamento - Correr")]
    public TextMeshProUGUI textoTeclaCorrer;
    public Image imagemTeclaCorrer;

    [Header("Remapeamento - Pular")]
    public TextMeshProUGUI textoTeclaPular;
    public Image imagemTeclaPular;

    [Header("Remapeamento - Dash")]
    public TextMeshProUGUI textoTeclaDash;
    public Image imagemTeclaDash;

    [Header("Sprites das Teclas")]
    public Sprite spriteQuadrado;
    public Sprite spriteMedio;
    public Sprite spriteLongo;

    [Header("Tamanhos das Teclas")]
    public Vector2 tamanhoQuadrado = new Vector2(120f, 120f);
    public Vector2 tamanhoMedio = new Vector2(220f, 300f);
    public Vector2 tamanhoLongo = new Vector2(290f, 190f);

    [Header("Posicoes X - Ataque")]
    public float posicaoXQuadrado = -396f;
    public float posicaoXMedio = -346f;
    public float posicaoXLongo = -311f;

    [Header("Posicoes X - Ataque em Area")]
    [FormerlySerializedAs("deslocamentoXAreaQuadrado")]
    public float posicaoXAreaQuadrado;

    [FormerlySerializedAs("deslocamentoXAreaMedio")]
    public float posicaoXAreaMedio;

    [FormerlySerializedAs("deslocamentoXAreaLongo")]
    public float posicaoXAreaLongo;

    [Header("Posicoes X - Defender")]
    [FormerlySerializedAs("deslocamentoXDefenderQuadrado")]
    public float posicaoXDefenderQuadrado;

    [FormerlySerializedAs("deslocamentoXDefenderMedio")]
    public float posicaoXDefenderMedio;

    [FormerlySerializedAs("deslocamentoXDefenderLongo")]
    public float posicaoXDefenderLongo;

    [Header("Posicoes X - Cura")]
    [FormerlySerializedAs("deslocamentoXCuraQuadrado")]
    public float posicaoXCuraQuadrado;

    [FormerlySerializedAs("deslocamentoXCuraMedio")]
    public float posicaoXCuraMedio;

    [FormerlySerializedAs("deslocamentoXCuraLongo")]
    public float posicaoXCuraLongo;

    [Header("Posicoes X - Interagir")]
    [FormerlySerializedAs("deslocamentoXInteragirQuadrado")]
    public float posicaoXInteragirQuadrado;

    [FormerlySerializedAs("deslocamentoXInteragirMedio")]
    public float posicaoXInteragirMedio;

    [FormerlySerializedAs("deslocamentoXInteragirLongo")]
    public float posicaoXInteragirLongo;

    [Header("Posicoes X - Correr")]
    [FormerlySerializedAs("deslocamentoXCorrerQuadrado")]
    public float posicaoXCorrerQuadrado;

    [FormerlySerializedAs("deslocamentoXCorrerMedio")]
    public float posicaoXCorrerMedio;

    [FormerlySerializedAs("deslocamentoXCorrerLongo")]
    public float posicaoXCorrerLongo;

    [Header("Posicoes X - Pular")]
    [FormerlySerializedAs("deslocamentoXPularQuadrado")]
    public float posicaoXPularQuadrado;

    [FormerlySerializedAs("deslocamentoXPularMedio")]
    public float posicaoXPularMedio;

    [FormerlySerializedAs("deslocamentoXPularLongo")]
    public float posicaoXPularLongo;

    [Header("Posicoes X - Dash")]
    [FormerlySerializedAs("deslocamentoXDashQuadrado")]
    public float posicaoXDashQuadrado;

    [FormerlySerializedAs("deslocamentoXDashMedio")]
    public float posicaoXDashMedio;

    [FormerlySerializedAs("deslocamentoXDashLongo")]
    public float posicaoXDashLongo;

    [Header("Cores")]
    public Color corNormal = Color.white;
    public Color corSelecionada = Color.yellow;

    private int colunaSelecionada = 0;
    private int linhaSelecionada = 0;

    private bool editandoTecla = false;
    private bool aguardandoNovaTecla = false;

    private KeyCode teclaAtaque = KeyCode.J;
    private KeyCode teclaAtaqueArea = KeyCode.K;
    private KeyCode teclaDefender = KeyCode.Q;
    private KeyCode teclaCura = KeyCode.L;
    private KeyCode teclaInteragir = KeyCode.E;
    private KeyCode teclaCorrer = KeyCode.LeftShift;
    private KeyCode teclaPular = KeyCode.Space;
    private KeyCode teclaDash = KeyCode.LeftControl;

    private KeyCode teclaAnterior;
    private int comandoEmEdicao = -1;
    private int quadroInicioEdicao;

    void OnEnable()
    {
        colunaSelecionada = 0;
        linhaSelecionada = 0;

        editandoTecla = false;
        aguardandoNovaTecla = false;
        comandoEmEdicao = -1;

        AtualizarSelecao();
        AtualizarIndicadores();
        AtualizarTeclas();
    }

    void Update()
    {
        if (editandoTecla)
        {
            AtualizarEdicao();
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoverVertical(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoverVertical(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoverHorizontal(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoverHorizontal(-1);
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (colunaSelecionada == 0)
            {
                if (linhaSelecionada == 0)
                    IniciarEdicao(0);
                else if (linhaSelecionada == 2)
                    IniciarEdicao(5);
                else if (linhaSelecionada == 3)
                    IniciarEdicao(6);
                else if (linhaSelecionada == 4)
                    IniciarEdicao(7);
            }
            else if (colunaSelecionada == 1)
            {
                if (linhaSelecionada == 0)
                    IniciarEdicao(1);
                else if (linhaSelecionada == 1)
                    IniciarEdicao(2);
                else if (linhaSelecionada == 2)
                    IniciarEdicao(3);
                else if (linhaSelecionada == 3)
                    IniciarEdicao(4);
            }
        }
    }

    void IniciarEdicao(int comando)
    {
        editandoTecla = true;
        aguardandoNovaTecla = true;

        comandoEmEdicao = comando;
        quadroInicioEdicao = Time.frameCount;

        switch (comandoEmEdicao)
        {
            case 0:
                teclaAnterior = teclaAtaque;
                if (textoTeclaAtaque != null)
                    textoTeclaAtaque.text = "";
                break;

            case 1:
                teclaAnterior = teclaAtaqueArea;
                if (textoTeclaAtaqueArea != null)
                    textoTeclaAtaqueArea.text = "";
                break;

            case 2:
                teclaAnterior = teclaDefender;
                if (textoTeclaDefender != null)
                    textoTeclaDefender.text = "";
                break;

            case 3:
                teclaAnterior = teclaCura;
                if (textoTeclaCura != null)
                    textoTeclaCura.text = "";
                break;

            case 4:
                teclaAnterior = teclaInteragir;
                if (textoTeclaInteragir != null)
                    textoTeclaInteragir.text = "";
                break;

            case 5:
                teclaAnterior = teclaCorrer;
                if (textoTeclaCorrer != null)
                    textoTeclaCorrer.text = "";
                break;

            case 6:
                teclaAnterior = teclaPular;
                if (textoTeclaPular != null)
                    textoTeclaPular.text = "";
                break;

            case 7:
                teclaAnterior = teclaDash;
                if (textoTeclaDash != null)
                    textoTeclaDash.text = "";
                break;
        }

        AtualizarIndicadores();
    }

    void AtualizarEdicao()
    {
        if (Time.frameCount == quadroInicioEdicao)
            return;

        if (aguardandoNovaTecla)
        {
            foreach (KeyCode tecla in System.Enum.GetValues(
                typeof(KeyCode)))
            {
                if (tecla == KeyCode.None ||
                    tecla == KeyCode.Escape ||
                    tecla == KeyCode.Return ||
                    tecla == KeyCode.KeypadEnter)
                    continue;

                if (Input.GetKeyDown(tecla))
                {
                    DefinirTeclaEmEdicao(tecla);

                    aguardandoNovaTecla = false;
                    AtualizarTeclas();
                    return;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                AplicarEdicao();
            }
        }
    }

    void DefinirTeclaEmEdicao(KeyCode tecla)
    {
        switch (comandoEmEdicao)
        {
            case 0:
                teclaAtaque = tecla;
                break;

            case 1:
                teclaAtaqueArea = tecla;
                break;

            case 2:
                teclaDefender = tecla;
                break;

            case 3:
                teclaCura = tecla;
                break;

            case 4:
                teclaInteragir = tecla;
                break;

            case 5:
                teclaCorrer = tecla;
                break;

            case 6:
                teclaPular = tecla;
                break;

            case 7:
                teclaDash = tecla;
                break;
        }
    }

    public bool CancelarEdicaoSeAtiva()
    {
        if (!editandoTecla)
            return false;

        CancelarEdicao();
        return true;
    }

    void AplicarEdicao()
    {
        editandoTecla = false;
        aguardandoNovaTecla = false;
        comandoEmEdicao = -1;

        AtualizarIndicadores();
    }

    void CancelarEdicao()
    {
        DefinirTeclaEmEdicao(teclaAnterior);

        editandoTecla = false;
        aguardandoNovaTecla = false;
        comandoEmEdicao = -1;

        AtualizarTeclas();
        AtualizarIndicadores();
    }

    void AtualizarTeclas()
    {
        string nomeAtaque = ObterNomeTecla(teclaAtaque);
        string nomeAtaqueArea = ObterNomeTecla(teclaAtaqueArea);
        string nomeDefender = ObterNomeTecla(teclaDefender);
        string nomeCura = ObterNomeTecla(teclaCura);
        string nomeInteragir = ObterNomeTecla(teclaInteragir);
        string nomeCorrer = ObterNomeTecla(teclaCorrer);
        string nomePular = ObterNomeTecla(teclaPular);
        string nomeDash = ObterNomeTecla(teclaDash);

        if (textoTeclaAtaque != null)
            textoTeclaAtaque.text = nomeAtaque;

        if (textoTeclaAtaqueArea != null)
            textoTeclaAtaqueArea.text = nomeAtaqueArea;

        if (textoTeclaDefender != null)
            textoTeclaDefender.text = nomeDefender;

        if (textoTeclaCura != null)
            textoTeclaCura.text = nomeCura;

        if (textoTeclaInteragir != null)
            textoTeclaInteragir.text = nomeInteragir;

        if (textoTeclaCorrer != null)
            textoTeclaCorrer.text = nomeCorrer;

        if (textoTeclaPular != null)
            textoTeclaPular.text = nomePular;

        if (textoTeclaDash != null)
            textoTeclaDash.text = nomeDash;

        AtualizarImagemTecla(
            imagemTeclaAtaque,
            nomeAtaque,
            posicaoXQuadrado,
            posicaoXMedio,
            posicaoXLongo
        );

        AtualizarImagemTecla(
            imagemTeclaAtaqueArea,
            nomeAtaqueArea,
            posicaoXAreaQuadrado,
            posicaoXAreaMedio,
            posicaoXAreaLongo
        );

        AtualizarImagemTecla(
            imagemTeclaDefender,
            nomeDefender,
            posicaoXDefenderQuadrado,
            posicaoXDefenderMedio,
            posicaoXDefenderLongo
        );

        AtualizarImagemTecla(
            imagemTeclaCura,
            nomeCura,
            posicaoXCuraQuadrado,
            posicaoXCuraMedio,
            posicaoXCuraLongo
        );

        AtualizarImagemTecla(
            imagemTeclaInteragir,
            nomeInteragir,
            posicaoXInteragirQuadrado,
            posicaoXInteragirMedio,
            posicaoXInteragirLongo
        );

        AtualizarImagemTecla(
            imagemTeclaCorrer,
            nomeCorrer,
            posicaoXCorrerQuadrado,
            posicaoXCorrerMedio,
            posicaoXCorrerLongo
        );

        AtualizarImagemTecla(
            imagemTeclaPular,
            nomePular,
            posicaoXPularQuadrado,
            posicaoXPularMedio,
            posicaoXPularLongo
        );

        AtualizarImagemTecla(
            imagemTeclaDash,
            nomeDash,
            posicaoXDashQuadrado,
            posicaoXDashMedio,
            posicaoXDashLongo
        );
    }

    string ObterNomeTecla(KeyCode tecla)
    {
        switch (tecla)
        {
            case KeyCode.Space:
                return "ESPAÇO";

            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                return "SHIFT";

            case KeyCode.LeftControl:
            case KeyCode.RightControl:
                return "CTRL";

            case KeyCode.LeftAlt:
            case KeyCode.RightAlt:
                return "ALT";

            case KeyCode.Backspace:
                return "BACKSPACE";

            case KeyCode.UpArrow:
                return "↑";

            case KeyCode.DownArrow:
                return "↓";

            case KeyCode.LeftArrow:
                return "←";

            case KeyCode.RightArrow:
                return "→";

            default:
                return tecla.ToString().ToUpper();
        }
    }

    void AtualizarImagemTecla(
        Image imagem,
        string nomeTecla,
        float xQuadrado,
        float xMedio,
        float xLongo)
    {
        if (imagem == null)
            return;

        Sprite novoSprite = spriteQuadrado;
        Vector2 novoTamanho = tamanhoQuadrado;
        float novaPosicaoX = xQuadrado;

        if (nomeTecla.Length > 5)
        {
            novoSprite = spriteLongo;
            novoTamanho = tamanhoLongo;
            novaPosicaoX = xLongo;
        }
        else if (nomeTecla.Length > 2)
        {
            novoSprite = spriteMedio;
            novoTamanho = tamanhoMedio;
            novaPosicaoX = xMedio;
        }

        if (novoSprite != null)
            imagem.sprite = novoSprite;

        RectTransform rect = imagem.rectTransform;

        rect.sizeDelta = novoTamanho;

        Vector2 posicao = rect.anchoredPosition;
        posicao.x = novaPosicaoX;
        rect.anchoredPosition = posicao;
    }

    void AtualizarIndicadores()
    {
        if (teclaContinuar != null)
            teclaContinuar.SetActive(!editandoTecla);

        if (teclaAplicar != null)
            teclaAplicar.SetActive(editandoTecla);
    }

    void MoverVertical(int direcao)
    {
        int quantidade = QuantidadeItensColuna();

        if (quantidade == 0)
            return;

        linhaSelecionada += direcao;

        if (linhaSelecionada >= quantidade)
            linhaSelecionada = 0;

        if (linhaSelecionada < 0)
            linhaSelecionada = quantidade - 1;

        AtualizarSelecao();
    }

    void MoverHorizontal(int direcao)
    {
        if (direcao > 0 && colunaSelecionada == 0)
        {
            if (textosDireita == null ||
                textosDireita.Length == 0)
                return;

            colunaSelecionada = 1;
        }
        else if (direcao < 0 && colunaSelecionada == 1)
        {
            if (textosEsquerda == null ||
                textosEsquerda.Length == 0)
                return;

            colunaSelecionada = 0;
        }
        else
        {
            return;
        }

        int quantidade = QuantidadeItensColuna();

        if (linhaSelecionada >= quantidade)
            linhaSelecionada = quantidade - 1;

        AtualizarSelecao();
    }

    int QuantidadeItensColuna()
    {
        if (colunaSelecionada == 0)
            return textosEsquerda != null
                ? textosEsquerda.Length : 0;

        return textosDireita != null
            ? textosDireita.Length : 0;
    }

    void AtualizarSelecao()
    {
        AtualizarColuna(
            textosEsquerda,
            destaquesEsquerda,
            0
        );

        AtualizarColuna(
            textosDireita,
            destaquesDireita,
            1
        );
    }

    void AtualizarColuna(
        TextMeshProUGUI[] textos,
        GameObject[] destaques,
        int coluna)
    {
        if (textos != null)
        {
            for (int i = 0; i < textos.Length; i++)
            {
                bool selecionado =
                    colunaSelecionada == coluna &&
                    linhaSelecionada == i;

                if (textos[i] != null)
                {
                    textos[i].color = selecionado
                        ? corSelecionada
                        : corNormal;
                }
            }
        }

        if (destaques != null)
        {
            for (int i = 0; i < destaques.Length; i++)
            {
                bool selecionado =
                    colunaSelecionada == coluna &&
                    linhaSelecionada == i;

                if (destaques[i] != null)
                    destaques[i].SetActive(selecionado);
            }
        }
    }
}

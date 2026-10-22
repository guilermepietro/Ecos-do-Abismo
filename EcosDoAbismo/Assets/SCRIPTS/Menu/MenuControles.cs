
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Deslocamentos X - Ataque em Area")]
    public float deslocamentoXAreaQuadrado = 0f;
    public float deslocamentoXAreaMedio = 50f;
    public float deslocamentoXAreaLongo = 85f;

    [Header("Deslocamentos X - Defender")]
    public float deslocamentoXDefenderQuadrado = 0f;
    public float deslocamentoXDefenderMedio = 50f;
    public float deslocamentoXDefenderLongo = 85f;

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

    private KeyCode teclaAnterior;

    // 0 = Ataque, 1 = Ataque em Area, 2 = Defender
    private int comandoEmEdicao = -1;
    private int quadroInicioEdicao;

    private float posicaoOriginalXArea;
    private float posicaoOriginalXDefender;

    void Awake()
    {
        if (imagemTeclaAtaqueArea != null)
        {
            posicaoOriginalXArea =
                imagemTeclaAtaqueArea.rectTransform.anchoredPosition.x;
        }

        if (imagemTeclaDefender != null)
        {
            posicaoOriginalXDefender =
                imagemTeclaDefender.rectTransform.anchoredPosition.x;
        }
    }

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
            if (colunaSelecionada == 0 && linhaSelecionada == 0)
            {
                IniciarEdicao(0);
            }
            else if (colunaSelecionada == 1 && linhaSelecionada == 0)
            {
                IniciarEdicao(1);
            }
            else if (colunaSelecionada == 1 && linhaSelecionada == 1)
            {
                IniciarEdicao(2);
            }
        }
    }

    void IniciarEdicao(int comando)
    {
        editandoTecla = true;
        aguardandoNovaTecla = true;

        comandoEmEdicao = comando;
        quadroInicioEdicao = Time.frameCount;

        if (comandoEmEdicao == 0)
        {
            teclaAnterior = teclaAtaque;

            if (textoTeclaAtaque != null)
                textoTeclaAtaque.text = "";
        }
        else if (comandoEmEdicao == 1)
        {
            teclaAnterior = teclaAtaqueArea;

            if (textoTeclaAtaqueArea != null)
                textoTeclaAtaqueArea.text = "";
        }
        else if (comandoEmEdicao == 2)
        {
            teclaAnterior = teclaDefender;

            if (textoTeclaDefender != null)
                textoTeclaDefender.text = "";
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
                    if (comandoEmEdicao == 0)
                        teclaAtaque = tecla;
                    else if (comandoEmEdicao == 1)
                        teclaAtaqueArea = tecla;
                    else if (comandoEmEdicao == 2)
                        teclaDefender = tecla;

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
        if (comandoEmEdicao == 0)
            teclaAtaque = teclaAnterior;
        else if (comandoEmEdicao == 1)
            teclaAtaqueArea = teclaAnterior;
        else if (comandoEmEdicao == 2)
            teclaDefender = teclaAnterior;

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

        if (textoTeclaAtaque != null)
            textoTeclaAtaque.text = nomeAtaque;

        if (textoTeclaAtaqueArea != null)
            textoTeclaAtaqueArea.text = nomeAtaqueArea;

        if (textoTeclaDefender != null)
            textoTeclaDefender.text = nomeDefender;

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
            posicaoOriginalXArea + deslocamentoXAreaQuadrado,
            posicaoOriginalXArea + deslocamentoXAreaMedio,
            posicaoOriginalXArea + deslocamentoXAreaLongo
        );

        AtualizarImagemTecla(
            imagemTeclaDefender,
            nomeDefender,
            posicaoOriginalXDefender + deslocamentoXDefenderQuadrado,
            posicaoOriginalXDefender + deslocamentoXDefenderMedio,
            posicaoOriginalXDefender + deslocamentoXDefenderLongo
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
        AtualizarColuna(textosEsquerda, destaquesEsquerda, 0);
        AtualizarColuna(textosDireita, destaquesDireita, 1);
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

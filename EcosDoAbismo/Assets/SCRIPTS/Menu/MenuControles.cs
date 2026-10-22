
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

    [Header("Cores")]
    public Color corNormal = Color.white;
    public Color corSelecionada = Color.yellow;

    private int colunaSelecionada = 0;
    private int linhaSelecionada = 0;

    private bool editandoTecla = false;
    private bool aguardandoNovaTecla = false;

    private KeyCode teclaAtaque = KeyCode.J;
    private KeyCode teclaAtaqueArea = KeyCode.K;

    private KeyCode teclaAnterior;
    private int colunaEmEdicao;
    private int quadroInicioEdicao;

    private float posicaoOriginalXArea;

    void Awake()
    {
        if (imagemTeclaAtaqueArea != null)
        {
            posicaoOriginalXArea =
                imagemTeclaAtaqueArea.rectTransform.anchoredPosition.x;
        }
    }

    void OnEnable()
    {
        colunaSelecionada = 0;
        linhaSelecionada = 0;

        editandoTecla = false;
        aguardandoNovaTecla = false;

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
            if (linhaSelecionada == 0)
            {
                IniciarEdicao();
            }
        }
    }

    void IniciarEdicao()
    {
        editandoTecla = true;
        aguardandoNovaTecla = true;

        colunaEmEdicao = colunaSelecionada;
        quadroInicioEdicao = Time.frameCount;

        if (colunaEmEdicao == 0)
        {
            teclaAnterior = teclaAtaque;

            if (textoTeclaAtaque != null)
                textoTeclaAtaque.text = "";
        }
        else
        {
            teclaAnterior = teclaAtaqueArea;

            if (textoTeclaAtaqueArea != null)
                textoTeclaAtaqueArea.text = "";
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
                    if (colunaEmEdicao == 0)
                        teclaAtaque = tecla;
                    else
                        teclaAtaqueArea = tecla;

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

        AtualizarIndicadores();
    }

    void CancelarEdicao()
    {
        if (colunaEmEdicao == 0)
            teclaAtaque = teclaAnterior;
        else
            teclaAtaqueArea = teclaAnterior;

        editandoTecla = false;
        aguardandoNovaTecla = false;

        AtualizarTeclas();
        AtualizarIndicadores();
    }

    void AtualizarTeclas()
    {
        string nomeAtaque = ObterNomeTecla(teclaAtaque);
        string nomeAtaqueArea = ObterNomeTecla(teclaAtaqueArea);

        if (textoTeclaAtaque != null)
            textoTeclaAtaque.text = nomeAtaque;

        if (textoTeclaAtaqueArea != null)
            textoTeclaAtaqueArea.text = nomeAtaqueArea;

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
        {
            return textosEsquerda != null
                ? textosEsquerda.Length : 0;
        }

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

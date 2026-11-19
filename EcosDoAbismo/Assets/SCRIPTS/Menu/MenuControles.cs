
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

    [Header("Remapeamento - Ultimate")]
    [FormerlySerializedAs("textoTeclaCorrer")]
    public TextMeshProUGUI textoTeclaUltimate;

    [FormerlySerializedAs("imagemTeclaCorrer")]
    public Image imagemTeclaUltimate;

    [Header("Remapeamento - Pular")]
    public TextMeshProUGUI textoTeclaPular;
    public Image imagemTeclaPular;

    [Header("Remapeamento - Dash")]
    public TextMeshProUGUI textoTeclaDash;
    public Image imagemTeclaDash;

    [Header("Remapeamento - Andar Esquerda")]
    public TextMeshProUGUI textoTeclaAndarEsquerda;
    public Image imagemTeclaAndarEsquerda;

    [Header("Remapeamento - Andar Direita")]
    public TextMeshProUGUI textoTeclaAndarDireita;
    public Image imagemTeclaAndarDireita;

    [Header("Sprites das Teclas")]
    public Sprite spriteQuadrado;
    public Sprite spriteMedio;
    public Sprite spriteLongo;

    [Header("Tamanhos das Teclas")]
    public Vector2 tamanhoQuadrado =
        new Vector2(120f, 120f);

    public Vector2 tamanhoMedio =
        new Vector2(220f, 300f);

    public Vector2 tamanhoLongo =
        new Vector2(290f, 190f);

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

    [Header("Posicoes X - Ultimate")]
    [FormerlySerializedAs("deslocamentoXCorrerQuadrado")]
    [FormerlySerializedAs("posicaoXCorrerQuadrado")]
    public float posicaoXUltimateQuadrado;

    [FormerlySerializedAs("deslocamentoXCorrerMedio")]
    [FormerlySerializedAs("posicaoXCorrerMedio")]
    public float posicaoXUltimateMedio;

    [FormerlySerializedAs("deslocamentoXCorrerLongo")]
    [FormerlySerializedAs("posicaoXCorrerLongo")]
    public float posicaoXUltimateLongo;

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

    [Header("Posicoes X - Andar Esquerda")]
    public float posicaoXAndarEsquerdaQuadrado;
    public float posicaoXAndarEsquerdaMedio;
    public float posicaoXAndarEsquerdaLongo;

    [Header("Posicoes X - Andar Direita")]
    public float posicaoXAndarDireitaQuadrado;
    public float posicaoXAndarDireitaMedio;
    public float posicaoXAndarDireitaLongo;

    [Header("Cores")]
    public Color corNormal = Color.white;
    public Color corSelecionada = Color.yellow;

    private int colunaSelecionada;
    private int linhaSelecionada;
    private int comandoEmEdicao = -1;
    private int etapaEdicaoAndar;
    private int quadroInicioEdicao;
    private int quadroAtivacao;

    private bool editandoTecla;
    private bool aguardandoNovaTecla;

    private KeyCode teclaAnterior;
    private KeyCode teclaAnteriorAndarEsquerda;
    private KeyCode teclaAnteriorAndarDireita;

    private KeyCode teclaEmEdicao;
    private KeyCode teclaAndarEsquerda;
    private KeyCode teclaAndarDireita;

    // Identificadores da interface:
    // 0 Ataque
    // 1 Ataque em Area
    // 2 Defesa
    // 3 Cura
    // 4 Interagir
    // 5 Ultimate
    // 6 Pulo
    // 7 Dash
    // 8 Andar

    private static readonly ComandoElian[] comandos =
    {
        ComandoElian.Ataque,
        ComandoElian.AtaqueArea,
        ComandoElian.Defender,
        ComandoElian.Curar,
        ComandoElian.Interagir,
        ComandoElian.Ultimate,
        ComandoElian.Pular,
        ComandoElian.Dash
    };

    void OnEnable()
    {
        colunaSelecionada = 0;
        linhaSelecionada = 0;
        comandoEmEdicao = -1;
        etapaEdicaoAndar = 0;

        editandoTecla = false;
        aguardandoNovaTecla = false;

        quadroAtivacao = Time.frameCount;

        CarregarAndar();
        AtualizarSelecao();
        AtualizarIndicadores();
        AtualizarTeclas();
    }

    void Update()
    {
        if (Time.frameCount == quadroAtivacao)
            return;

        if (editandoTecla)
        {
            AtualizarEdicao();
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoverVertical(1);

        else if (Input.GetKeyDown(KeyCode.UpArrow))
            MoverVertical(-1);

        else if (Input.GetKeyDown(KeyCode.RightArrow))
            MoverHorizontal(1);

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoverHorizontal(-1);

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (colunaSelecionada == 0)
            {
                if (linhaSelecionada == 0)
                    IniciarEdicao(0);

                else if (linhaSelecionada == 1)
                    IniciarEdicao(8);

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

    void CarregarAndar()
    {
        teclaAndarEsquerda =
            ControlesElian.Obter(
                ComandoElian.AndarEsquerda
            );

        teclaAndarDireita =
            ControlesElian.Obter(
                ComandoElian.AndarDireita
            );
    }

    void IniciarEdicao(int comando)
    {
        comandoEmEdicao = comando;
        editandoTecla = true;
        aguardandoNovaTecla = true;

        quadroInicioEdicao = Time.frameCount;
        etapaEdicaoAndar = 0;

        if (comando == 8)
        {
            teclaAnteriorAndarEsquerda =
                teclaAndarEsquerda;

            teclaAnteriorAndarDireita =
                teclaAndarDireita;
        }
        else
        {
            teclaAnterior =
                ControlesElian.Obter(comandos[comando]);

            teclaEmEdicao = teclaAnterior;
        }

        AtualizarTeclas();
        ApagarTeclaEmEdicao();
        AtualizarIndicadores();
        TocarConfirmacao();
    }

    void AtualizarEdicao()
    {
        if (Time.frameCount == quadroInicioEdicao)
            return;

        // O MenuManager trata o cancelamento com Esc.
        if (Input.GetKeyDown(KeyCode.Escape))
            return;

        if (aguardandoNovaTecla)
        {
            foreach (KeyCode tecla in
                System.Enum.GetValues(typeof(KeyCode)))
            {
                if (TeclaReservada(tecla))
                    continue;

                if (!Input.GetKeyDown(tecla))
                    continue;

                if (comandoEmEdicao == 8)
                {
                    // Andar aceita apenas teclas curtas.
                    if (ObterNomeTecla(tecla).Length > 2)
                        return;

                    if (etapaEdicaoAndar == 0)
                    {
                        teclaAndarEsquerda = tecla;
                        etapaEdicaoAndar = 1;

                        AtualizarTeclas();

                        if (textoTeclaAndarDireita != null)
                            textoTeclaAndarDireita.text = "";
                    }
                    else
                    {
                        teclaAndarDireita = tecla;
                        etapaEdicaoAndar = 2;
                        aguardandoNovaTecla = false;

                        AtualizarTeclas();
                    }
                }
                else
                {
                    teclaEmEdicao = tecla;
                    aguardandoNovaTecla = false;

                    AtualizarTeclas();
                }

                TocarConfirmacao();
                return;
            }
        }
        else if (
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
        )
        {
            AplicarEdicao();
        }
    }

    static bool TeclaReservada(KeyCode tecla)
    {
        return tecla == KeyCode.None ||
               tecla == KeyCode.Escape ||
               tecla == KeyCode.Return ||
               tecla == KeyCode.KeypadEnter ||
               tecla == KeyCode.UpArrow ||
               tecla == KeyCode.DownArrow ||
               tecla == KeyCode.LeftArrow ||
               tecla == KeyCode.RightArrow;
    }

    void ApagarTeclaEmEdicao()
    {
        TextMeshProUGUI texto =
            ObterTexto(comandoEmEdicao);

        if (texto != null)
            texto.text = "";
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
        bool sucesso;

        if (comandoEmEdicao == 8)
        {
            sucesso = ControlesElian.TentarDefinirAndar(
                teclaAndarEsquerda,
                teclaAndarDireita
            );
        }
        else
        {
            sucesso = ControlesElian.TentarDefinir(
                comandos[comandoEmEdicao],
                teclaEmEdicao
            );
        }

        if (!sucesso)
        {
            Debug.LogWarning(
                "Tecla reservada ou utilizada por " +
                "outro comando. Escolha outra."
            );

            TocarVoltar();

            aguardandoNovaTecla = true;
            etapaEdicaoAndar = 0;

            CarregarAndar();
            AtualizarTeclas();
            ApagarTeclaEmEdicao();

            return;
        }

        editandoTecla = false;
        aguardandoNovaTecla = false;

        comandoEmEdicao = -1;
        etapaEdicaoAndar = 0;

        CarregarAndar();
        AtualizarTeclas();
        AtualizarIndicadores();

        TocarConfirmacao();
    }

    void CancelarEdicao()
    {
        editandoTecla = false;
        aguardandoNovaTecla = false;

        comandoEmEdicao = -1;
        etapaEdicaoAndar = 0;

        CarregarAndar();
        AtualizarTeclas();
        AtualizarIndicadores();

        TocarVoltar();
    }

    TextMeshProUGUI ObterTexto(int comando)
    {
        switch (comando)
        {
            case 0:
                return textoTeclaAtaque;

            case 1:
                return textoTeclaAtaqueArea;

            case 2:
                return textoTeclaDefender;

            case 3:
                return textoTeclaCura;

            case 4:
                return textoTeclaInteragir;

            case 5:
                return textoTeclaUltimate;

            case 6:
                return textoTeclaPular;

            case 7:
                return textoTeclaDash;

            case 8:
                return textoTeclaAndarEsquerda;

            default:
                return null;
        }
    }

    void AtualizarTeclas()
    {
        AtualizarTecla(
            textoTeclaAtaque,
            imagemTeclaAtaque,
            ObterTecla(0),
            posicaoXQuadrado,
            posicaoXMedio,
            posicaoXLongo
        );

        AtualizarTecla(
            textoTeclaAtaqueArea,
            imagemTeclaAtaqueArea,
            ObterTecla(1),
            posicaoXAreaQuadrado,
            posicaoXAreaMedio,
            posicaoXAreaLongo
        );

        AtualizarTecla(
            textoTeclaDefender,
            imagemTeclaDefender,
            ObterTecla(2),
            posicaoXDefenderQuadrado,
            posicaoXDefenderMedio,
            posicaoXDefenderLongo
        );

        AtualizarTecla(
            textoTeclaCura,
            imagemTeclaCura,
            ObterTecla(3),
            posicaoXCuraQuadrado,
            posicaoXCuraMedio,
            posicaoXCuraLongo
        );

        AtualizarTecla(
            textoTeclaInteragir,
            imagemTeclaInteragir,
            ObterTecla(4),
            posicaoXInteragirQuadrado,
            posicaoXInteragirMedio,
            posicaoXInteragirLongo
        );

        AtualizarTecla(
            textoTeclaUltimate,
            imagemTeclaUltimate,
            ObterTecla(5),
            posicaoXUltimateQuadrado,
            posicaoXUltimateMedio,
            posicaoXUltimateLongo
        );

        AtualizarTecla(
            textoTeclaPular,
            imagemTeclaPular,
            ObterTecla(6),
            posicaoXPularQuadrado,
            posicaoXPularMedio,
            posicaoXPularLongo
        );

        AtualizarTecla(
            textoTeclaDash,
            imagemTeclaDash,
            ObterTecla(7),
            posicaoXDashQuadrado,
            posicaoXDashMedio,
            posicaoXDashLongo
        );

        AtualizarTecla(
            textoTeclaAndarEsquerda,
            imagemTeclaAndarEsquerda,
            teclaAndarEsquerda,
            posicaoXAndarEsquerdaQuadrado,
            posicaoXAndarEsquerdaMedio,
            posicaoXAndarEsquerdaLongo
        );

        AtualizarTecla(
            textoTeclaAndarDireita,
            imagemTeclaAndarDireita,
            teclaAndarDireita,
            posicaoXAndarDireitaQuadrado,
            posicaoXAndarDireitaMedio,
            posicaoXAndarDireitaLongo
        );
    }

    KeyCode ObterTecla(int indice)
    {
        if (editandoTecla && comandoEmEdicao == indice)
            return teclaEmEdicao;

        return ControlesElian.Obter(comandos[indice]);
    }

    void AtualizarTecla(
        TextMeshProUGUI texto,
        Image imagem,
        KeyCode tecla,
        float xQuadrado,
        float xMedio,
        float xLongo)
    {
        string nome = ObterNomeTecla(tecla);

        if (texto != null)
            texto.text = nome;

        AtualizarImagemTecla(
            imagem,
            nome,
            xQuadrado,
            xMedio,
            xLongo
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

        linhaSelecionada =
            (linhaSelecionada + direcao + quantidade)
            % quantidade;

        AtualizarSelecao();
        TocarNavegacao();
    }

    void MoverHorizontal(int direcao)
    {
        if (
            direcao > 0 &&
            colunaSelecionada == 0 &&
            textosDireita != null &&
            textosDireita.Length > 0
        )
        {
            colunaSelecionada = 1;
        }
        else if (
            direcao < 0 &&
            colunaSelecionada == 1 &&
            textosEsquerda != null &&
            textosEsquerda.Length > 0
        )
        {
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
        TocarNavegacao();
    }

    int QuantidadeItensColuna()
    {
        return colunaSelecionada == 0
            ? (textosEsquerda != null
                ? textosEsquerda.Length : 0)
            : (textosDireita != null
                ? textosDireita.Length : 0);
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
                    textos[i].color =
                        selecionado
                            ? corSelecionada
                            : corNormal;
                }
            }
        }

        if (destaques != null)
        {
            for (int i = 0; i < destaques.Length; i++)
            {
                if (destaques[i] != null)
                {
                    destaques[i].SetActive(
                        colunaSelecionada == coluna &&
                        linhaSelecionada == i
                    );
                }
            }
        }
    }

    void TocarNavegacao()
    {
        if (SonsInterface.Instancia != null)
            SonsInterface.Instancia.TocarNavegacao();
    }

    void TocarConfirmacao()
    {
        if (SonsInterface.Instancia != null)
            SonsInterface.Instancia.TocarConfirmacao();
    }

    void TocarVoltar()
    {
        if (SonsInterface.Instancia != null)
            SonsInterface.Instancia.TocarVoltar();
    }
}

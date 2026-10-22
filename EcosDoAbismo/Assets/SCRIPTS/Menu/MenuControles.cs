
using TMPro;
using UnityEngine;

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

    [Header("Teste de Remapeamento - Ataque")]
    public TextMeshProUGUI textoTeclaAtaque;

    [Header("Cores")]
    public Color corNormal = Color.white;
    public Color corSelecionada = Color.yellow;

    private int colunaSelecionada = 0;
    private int linhaSelecionada = 0;

    private bool editandoTecla = false;
    private bool aguardandoNovaTecla = false;

    private KeyCode teclaAtaque = KeyCode.J;
    private KeyCode teclaAnterior;
    private int quadroInicioEdicao;

    void OnEnable()
    {
        colunaSelecionada = 0;
        linhaSelecionada = 0;

        editandoTecla = false;
        aguardandoNovaTecla = false;

        AtualizarSelecao();
        AtualizarIndicadores();
        AtualizarTextoAtaque();
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
            if (colunaSelecionada == 0 &&
                linhaSelecionada == 0)
            {
                IniciarEdicaoAtaque();
            }
        }
    }

    void IniciarEdicaoAtaque()
    {
        editandoTecla = true;
        aguardandoNovaTecla = true;

        teclaAnterior = teclaAtaque;
        quadroInicioEdicao = Time.frameCount;

        if (textoTeclaAtaque != null)
            textoTeclaAtaque.text = "";

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
                    teclaAtaque = tecla;
                    aguardandoNovaTecla = false;

                    AtualizarTextoAtaque();
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
        teclaAtaque = teclaAnterior;

        editandoTecla = false;
        aguardandoNovaTecla = false;

        AtualizarTextoAtaque();
        AtualizarIndicadores();
    }

    void AtualizarTextoAtaque()
    {
        if (textoTeclaAtaque != null)
            textoTeclaAtaque.text = teclaAtaque.ToString();
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

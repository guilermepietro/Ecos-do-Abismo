
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPausa : MonoBehaviour
{
    [Header("Telas")]
    public GameObject telaPausa;
    public GameObject telaCategoriasOpcoes;
    public GameObject telaOpcoes;
    public GameObject telaControles;

    [Header("Opcoes da Pausa")]
    public TextMeshProUGUI[] opcoesPausa;

    [Header("Cursor")]
    public RectTransform cursor;
    public RectTransform[] pontosCursor;
    public float tempoDeslizamento = 0.15f;

    [Header("Cores")]
    public Color corNormal = Color.white;
    public Color corSelecionada = Color.yellow;

    [Header("Elian")]
    public GameObject elian;

    private ElianColeta coleta;

    private MonoBehaviour[] scriptsJogador;
    private bool[] estadosAnteriores;

    private bool pausado = false;
    private int indiceSelecionado = 0;

    private float escalaTempoAnterior = 1f;

    private Vector3 destinoCursor;
    private Vector3 velocidadeCursor;

    public bool EstaPausado => pausado;

    void Start()
    {
        if (elian != null)
        {
            coleta = elian.GetComponent<ElianColeta>();

            scriptsJogador = new MonoBehaviour[]
            {
                elian.GetComponent<ElianMovimento>(),
                elian.GetComponent<ElianAtaque>(),
                elian.GetComponent<ElianDefesa>(),
                elian.GetComponent<ElianCura>(),
                elian.GetComponent<ElianUltimate>(),
                elian.GetComponent<ElianColeta>()
            };

            estadosAnteriores =
                new bool[scriptsJogador.Length];
        }
        else
        {
            Debug.LogWarning(
                "MenuPausa: Elian nao foi atribuido!"
            );
        }

        OcultarTodasTelas();
    }

    void Update()
    {
        if (!pausado)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                AbrirPausa();
            }

            return;
        }

        // O MenuOpcoes gerencia seu proprio Esc.
        if (telaOpcoes != null &&
            telaOpcoes.activeSelf)
        {
            return;
        }

        // O MenuCategoriasOpcoes gerencia seu Esc.
        if (telaCategoriasOpcoes != null &&
            telaCategoriasOpcoes.activeSelf)
        {
            return;
        }

        // A tela de controles necessita tratar
        // a edicao antes de permitir o retorno.
        if (telaControles != null &&
            telaControles.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MenuControles controles =
                    telaControles.GetComponent<MenuControles>();

                bool cancelou =
                    controles != null &&
                    controles.CancelarEdicaoSeAtiva();

                if (!cancelou)
                {
                    VoltarAoMenu();
                }
            }

            return;
        }

        if (telaPausa == null || !telaPausa.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ContinuarJogo();
            return;
        }

        if (opcoesPausa == null ||
            opcoesPausa.Length == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            indiceSelecionado++;

            if (indiceSelecionado >= opcoesPausa.Length)
                indiceSelecionado = 0;

            AtualizarVisual();
            TocarNavegacao();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            indiceSelecionado--;

            if (indiceSelecionado < 0)
                indiceSelecionado = opcoesPausa.Length - 1;

            AtualizarVisual();
            TocarNavegacao();
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SelecionarOpcao();
            return;
        }

        if (cursor != null)
        {
            cursor.position = Vector3.SmoothDamp(
                cursor.position,
                destinoCursor,
                ref velocidadeCursor,
                tempoDeslizamento,
                Mathf.Infinity,
                Time.unscaledDeltaTime
            );
        }
    }

    void AbrirPausa()
    {
        if (pausado)
            return;

        // Evita abrir a pausa durante
        // a interface de Viagem entre Ecos.
        if (coleta != null &&
            coleta.MenuViagemAberto)
        {
            return;
        }

        // Nao interfere com outros sistemas
        // que ja tenham pausado o jogo.
        if (Time.timeScale <= 0f)
            return;

        pausado = true;

        escalaTempoAnterior = Time.timeScale;

        BloquearJogador();

        Time.timeScale = 0f;

        OcultarTodasTelas();

        if (telaPausa != null)
            telaPausa.SetActive(true);

        indiceSelecionado = 0;
        velocidadeCursor = Vector3.zero;

        AtualizarVisual();

        if (cursor != null &&
            pontosCursor != null &&
            pontosCursor.Length > 0 &&
            pontosCursor[0] != null)
        {
            destinoCursor = pontosCursor[0].position;
            cursor.position = destinoCursor;
            cursor.gameObject.SetActive(true);
        }

        TocarConfirmacao();
    }

    void BloquearJogador()
    {
        if (scriptsJogador == null)
            return;

        for (int i = 0; i < scriptsJogador.Length; i++)
        {
            if (scriptsJogador[i] == null)
                continue;

            estadosAnteriores[i] =
                scriptsJogador[i].enabled;

            scriptsJogador[i].enabled = false;
        }
    }

    void LiberarJogador()
    {
        if (scriptsJogador == null)
            return;

        for (int i = 0; i < scriptsJogador.Length; i++)
        {
            if (scriptsJogador[i] == null)
                continue;

            scriptsJogador[i].enabled =
                estadosAnteriores[i];
        }
    }

    void SelecionarOpcao()
    {
        switch (indiceSelecionado)
        {
            case 0:
                ContinuarJogo();
                break;

            case 1:
                AbrirOpcoes();
                break;

            case 2:
                SairDoJogo();
                break;
        }
    }

    public void ContinuarJogo()
    {
        if (!pausado)
            return;

        TocarConfirmacao();

        OcultarTodasTelas();

        Time.timeScale = escalaTempoAnterior;

        LiberarJogador();

        pausado = false;
    }

    public void AbrirOpcoes()
    {
        if (!pausado ||
            telaCategoriasOpcoes == null)
        {
            return;
        }

        TocarConfirmacao();

        if (telaPausa != null)
            telaPausa.SetActive(false);

        telaCategoriasOpcoes.SetActive(true);

        if (cursor != null)
            cursor.gameObject.SetActive(false);
    }

    public void AbrirSom()
    {
        AbrirSubtela(telaOpcoes);
    }

    public void AbrirControles()
    {
        AbrirSubtela(telaControles);
    }

    void AbrirSubtela(GameObject subtela)
    {
        if (!pausado || subtela == null)
            return;

        TocarConfirmacao();

        if (telaCategoriasOpcoes != null)
            telaCategoriasOpcoes.SetActive(false);

        subtela.SetActive(true);
    }

    public void VoltarAoMenu()
    {
        if (!pausado)
            return;

        TocarVoltar();

        bool estavaEmSubtela =
            (telaOpcoes != null &&
             telaOpcoes.activeSelf) ||
            (telaControles != null &&
             telaControles.activeSelf);

        if (estavaEmSubtela)
        {
            if (telaOpcoes != null)
                telaOpcoes.SetActive(false);

            if (telaControles != null)
                telaControles.SetActive(false);

            if (telaCategoriasOpcoes != null)
                telaCategoriasOpcoes.SetActive(true);

            return;
        }

        if (telaCategoriasOpcoes != null)
            telaCategoriasOpcoes.SetActive(false);

        if (telaPausa != null)
            telaPausa.SetActive(true);

        if (cursor != null)
            cursor.gameObject.SetActive(true);

        AtualizarVisual();
    }

    void SairDoJogo()
    {
        TocarConfirmacao();

        OcultarTodasTelas();

        Time.timeScale = escalaTempoAnterior;

        LiberarJogador();

        pausado = false;

        SceneManager.LoadScene("Menu");
    }

    void OcultarTodasTelas()
    {
        if (telaPausa != null)
            telaPausa.SetActive(false);

        if (telaCategoriasOpcoes != null)
            telaCategoriasOpcoes.SetActive(false);

        if (telaOpcoes != null)
            telaOpcoes.SetActive(false);

        if (telaControles != null)
            telaControles.SetActive(false);
    }

    void AtualizarVisual()
    {
        if (opcoesPausa != null)
        {
            for (int i = 0; i < opcoesPausa.Length; i++)
            {
                if (opcoesPausa[i] == null)
                    continue;

                opcoesPausa[i].color =
                    i == indiceSelecionado
                        ? corSelecionada
                        : corNormal;
            }
        }

        if (pontosCursor != null &&
            indiceSelecionado < pontosCursor.Length &&
            pontosCursor[indiceSelecionado] != null)
        {
            destinoCursor =
                pontosCursor[indiceSelecionado].position;
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

    void OnDisable()
    {
        if (!pausado)
            return;

        Time.timeScale = escalaTempoAnterior;

        LiberarJogador();

        pausado = false;
    }
}

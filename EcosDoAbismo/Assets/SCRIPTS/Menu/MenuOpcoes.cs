
using UnityEngine;

public class MenuOpcoes : MonoBehaviour
{
    [Header("Cursor das Opções")]
    public RectTransform cursorOpcoes;

    [Header("Pontos de Seleção")]
    public RectTransform[] pontosSelecao;

    [Header("Movimento do Cursor")]
    public float tempoDeslizamento = 0.15f;

    [Header("Menu Principal")]
    public MenuManager menuManager;

    [Header("Indicadores de Controle")]
    public GameObject teclaContinuar;
    public GameObject teclaAplicar;

    private int indiceSelecionado = 0;

    private Vector3 destinoCursor;
    private Vector3 velocidadeCursor;

    private bool editandoOpcao = false;

    void OnEnable()
    {
        indiceSelecionado = 0;
        velocidadeCursor = Vector3.zero;
        editandoOpcao = false;

        AtualizarIndicadores();

        if (cursorOpcoes != null &&
            pontosSelecao != null &&
            pontosSelecao.Length > 0 &&
            pontosSelecao[0] != null)
        {
            destinoCursor = pontosSelecao[0].position;
            cursorOpcoes.position = destinoCursor;
        }
    }

    void Update()
    {
        if (pontosSelecao == null || pontosSelecao.Length == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (editandoOpcao)
            {
                CancelarEdicao();
            }
            else
            {
                if (menuManager != null)
                    menuManager.VoltarAoMenu();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (editandoOpcao)
                AplicarEdicao();
            else
                IniciarEdicao();

            return;
        }

        if (!editandoOpcao)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                indiceSelecionado++;

                if (indiceSelecionado >= pontosSelecao.Length)
                    indiceSelecionado = 0;

                AtualizarDestino();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                indiceSelecionado--;

                if (indiceSelecionado < 0)
                    indiceSelecionado = pontosSelecao.Length - 1;

                AtualizarDestino();
            }
        }

        if (cursorOpcoes != null)
        {
            cursorOpcoes.position = Vector3.SmoothDamp(
                cursorOpcoes.position,
                destinoCursor,
                ref velocidadeCursor,
                tempoDeslizamento
            );
        }
    }

    void IniciarEdicao()
    {
        editandoOpcao = true;
        AtualizarIndicadores();
    }

    void AplicarEdicao()
    {
        editandoOpcao = false;
        AtualizarIndicadores();
    }

    void CancelarEdicao()
    {
        editandoOpcao = false;
        AtualizarIndicadores();
    }

    void AtualizarIndicadores()
    {
        if (teclaContinuar != null)
            teclaContinuar.SetActive(!editandoOpcao);

        if (teclaAplicar != null)
            teclaAplicar.SetActive(editandoOpcao);
    }

    void AtualizarDestino()
    {
        if (pontosSelecao[indiceSelecionado] != null)
            destinoCursor = pontosSelecao[indiceSelecionado].position;
    }
}

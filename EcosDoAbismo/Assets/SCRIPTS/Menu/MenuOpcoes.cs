
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

    private int indiceSelecionado = 0;

    private Vector3 destinoCursor;
    private Vector3 velocidadeCursor;

    void OnEnable()
    {
        indiceSelecionado = 0;
        velocidadeCursor = Vector3.zero;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuManager != null)
                menuManager.VoltarAoMenu();

            return;
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

    void AtualizarDestino()
    {
        if (pontosSelecao[indiceSelecionado] != null)
            destinoCursor = pontosSelecao[indiceSelecionado].position;
    }
}

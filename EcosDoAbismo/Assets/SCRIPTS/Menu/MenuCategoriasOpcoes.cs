
using TMPro;
using UnityEngine;

public class MenuCategoriasOpcoes : MonoBehaviour
{
    [Header("Textos das Categorias")]
    public TextMeshProUGUI textoSom;
    public TextMeshProUGUI textoControles;

    [Header("Destaques Góticos")]
    public GameObject destaqueSom;
    public GameObject destaqueControles;

    [Header("Menu Principal")]
    public MenuManager menuManager;

    [Header("Cores")]
    public Color corNormal = Color.white;
    public Color corSelecionada = Color.yellow;

    private int indiceSelecionado = 0;
    private int quadroAtivacao;

    void OnEnable()
    {
        indiceSelecionado = 0;
        quadroAtivacao = Time.frameCount;
        AtualizarVisual();
    }

    void Update()
    {
        // Impede que a tecla usada para abrir esta tela
        // também acione uma opção no mesmo frame.
        if (Time.frameCount == quadroAtivacao)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            indiceSelecionado = (indiceSelecionado + 1) % 2;
            AtualizarVisual();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            indiceSelecionado--;

            if (indiceSelecionado < 0)
                indiceSelecionado = 1;

            AtualizarVisual();
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (menuManager != null)
            {
                if (indiceSelecionado == 0)
                    menuManager.AbrirSom();
                else
                    menuManager.AbrirControles();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuManager != null)
                menuManager.VoltarAoMenu();
        }
    }

    void AtualizarVisual()
    {
        if (textoSom != null)
            textoSom.color = indiceSelecionado == 0
                ? corSelecionada : corNormal;

        if (textoControles != null)
            textoControles.color = indiceSelecionado == 1
                ? corSelecionada : corNormal;

        if (destaqueSom != null)
            destaqueSom.SetActive(indiceSelecionado == 0);

        if (destaqueControles != null)
            destaqueControles.SetActive(indiceSelecionado == 1);
    }
}

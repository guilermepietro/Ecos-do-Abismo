
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Opções do Menu")]
    public TextMeshProUGUI[] menuOptions;

    [Header("Cursor")]
    public RectTransform cursor;
    public RectTransform[] cursorAnchors;

    [Header("Movimento do Cursor")]
    public float tempoDeslizamento = 0.15f;

    [Header("Cores")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("Transição")]
    public TransicaoTela transicaoTela;

    [Header("Tela de Opções - Som")]
    public GameObject telaOpcoes;

    [Header("Tela de Categorias")]
    public GameObject telaCategoriasOpcoes;

    [Header("Tela de Controles")]
    public GameObject telaControles;

    [Header("Tela de Seleção de Salvamentos")]
    public GameObject telaSelecaoSave;

    private int selectedIndex = 0;

    private Vector3 velocidadeCursor;
    private Vector3 destinoCursor;

    private bool emTransicao = false;

    void Start()
    {
        if (telaOpcoes != null)
        {
            MenuOpcoes menuOpcoes =
                telaOpcoes.GetComponent<MenuOpcoes>();

            if (menuOpcoes != null)
                menuOpcoes.InicializarVolumes();

            telaOpcoes.SetActive(false);
        }

        if (telaCategoriasOpcoes != null)
            telaCategoriasOpcoes.SetActive(false);

        if (telaControles != null)
            telaControles.SetActive(false);

        if (telaSelecaoSave != null)
            telaSelecaoSave.SetActive(false);

        UpdateMenuVisual();

        if (cursor != null &&
            cursorAnchors != null &&
            cursorAnchors.Length > selectedIndex &&
            cursorAnchors[selectedIndex] != null)
        {
            destinoCursor = cursorAnchors[selectedIndex].position;
            cursor.position = destinoCursor;
        }
    }

    void Update()
    {
        if (emTransicao)
            return;

        if (telaOpcoes != null && telaOpcoes.activeSelf)
            return;

        if (telaCategoriasOpcoes != null &&
            telaCategoriasOpcoes.activeSelf)
            return;

        if (telaControles != null && telaControles.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MenuControles menuControles =
                    telaControles.GetComponent<MenuControles>();

                bool cancelouEdicao =
                    menuControles != null &&
                    menuControles.CancelarEdicaoSeAtiva();

                if (!cancelouEdicao)
                    VoltarAoMenu();
            }

            return;
        }

        if (telaSelecaoSave != null && telaSelecaoSave.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;

            if (selectedIndex >= menuOptions.Length)
                selectedIndex = 0;

            UpdateMenuVisual();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;

            if (selectedIndex < 0)
                selectedIndex = menuOptions.Length - 1;

            UpdateMenuVisual();
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SelecionarOpcao();
        }

        if (cursor != null)
        {
            cursor.position = Vector3.SmoothDamp(
                cursor.position,
                destinoCursor,
                ref velocidadeCursor,
                tempoDeslizamento
            );
        }
    }

    void SelecionarOpcao()
    {
        if (selectedIndex == 0)
        {
            AbrirSelecaoSave();
        }
        else if (selectedIndex == 1)
        {
            AbrirOpcoes();
        }
        else if (selectedIndex == 2)
        {
            StartCoroutine(SairDoJogo());
        }
    }

    void AbrirSelecaoSave()
    {
        if (telaSelecaoSave != null)
        {
            telaSelecaoSave.SetActive(true);

            if (cursor != null)
                cursor.gameObject.SetActive(false);
        }
    }

    void AbrirOpcoes()
    {
        if (telaCategoriasOpcoes != null)
        {
            telaCategoriasOpcoes.SetActive(true);

            if (cursor != null)
                cursor.gameObject.SetActive(false);
        }
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
        if (subtela == null)
            return;

        subtela.SetActive(true);

        if (telaCategoriasOpcoes != null)
            telaCategoriasOpcoes.SetActive(false);
    }

    public void VoltarAoMenu()
    {
        bool estavaEmSubtela =
            (telaOpcoes != null && telaOpcoes.activeSelf) ||
            (telaControles != null && telaControles.activeSelf);

        if (estavaEmSubtela && telaCategoriasOpcoes != null)
        {
            telaCategoriasOpcoes.SetActive(true);

            if (telaOpcoes != null)
                telaOpcoes.SetActive(false);

            if (telaControles != null)
                telaControles.SetActive(false);

            if (cursor != null)
                cursor.gameObject.SetActive(false);

            return;
        }

        if (telaOpcoes != null)
            telaOpcoes.SetActive(false);

        if (telaControles != null)
            telaControles.SetActive(false);

        if (telaCategoriasOpcoes != null)
            telaCategoriasOpcoes.SetActive(false);

        if (telaSelecaoSave != null)
            telaSelecaoSave.SetActive(false);

        if (cursor != null)
            cursor.gameObject.SetActive(true);
    }

    IEnumerator IniciarJogo()
    {
        emTransicao = true;

        if (transicaoTela != null)
        {
            yield return StartCoroutine(
                transicaoTela.Escurecer()
            );
        }

        GerenciadorCarregamento.proximaCena = "Gameplay";

        SceneManager.LoadScene("Carregamento");
    }

    IEnumerator SairDoJogo()
    {
        emTransicao = true;

        if (transicaoTela != null)
        {
            yield return StartCoroutine(
                transicaoTela.Escurecer()
            );
        }

        Debug.Log("Saindo de Ecos do Abismo...");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void UpdateMenuVisual()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            if (i == selectedIndex)
                menuOptions[i].color = selectedColor;
            else
                menuOptions[i].color = normalColor;
        }

        if (cursor != null &&
            cursorAnchors != null &&
            cursorAnchors.Length > selectedIndex &&
            cursorAnchors[selectedIndex] != null)
        {
            destinoCursor = cursorAnchors[selectedIndex].position;
        }
    }
}

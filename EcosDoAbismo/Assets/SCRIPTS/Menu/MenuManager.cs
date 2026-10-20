
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

    [Header("Tela de Opções")]
    public GameObject telaOpcoes;

    private int selectedIndex = 0;

    private Vector3 velocidadeCursor;
    private Vector3 destinoCursor;

    private bool emTransicao = false;

    void Start()
    {
        if (telaOpcoes != null)
            telaOpcoes.SetActive(false);

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
            StartCoroutine(IniciarJogo());
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

    void AbrirOpcoes()
    {
        if (telaOpcoes != null)
        {
            telaOpcoes.SetActive(true);

            if (cursor != null)
                cursor.gameObject.SetActive(false);
        }
    }

    public void VoltarAoMenu()
    {
        if (telaOpcoes != null)
            telaOpcoes.SetActive(false);

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

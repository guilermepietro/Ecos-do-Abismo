using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Opções do Menu")]
    public TextMeshProUGUI[] menuOptions;

    [Header("Cursor")]
    public RectTransform cursor;
    public RectTransform[] cursorAnchors;

    [Header("Cores")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;

    void Start()
    {
        UpdateMenuVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;

            if (selectedIndex >= menuOptions.Length)
            {
                selectedIndex = 0;
            }

            UpdateMenuVisual();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;

            if (selectedIndex < 0)
            {
                selectedIndex = menuOptions.Length - 1;
            }

            UpdateMenuVisual();
        }
    }

    void UpdateMenuVisual()
    {
        // Atualiza as cores dos textos
        for (int i = 0; i < menuOptions.Length; i++)
        {
            if (i == selectedIndex)
            {
                menuOptions[i].color = selectedColor;
            }
            else
            {
                menuOptions[i].color = normalColor;
            }
        }

        // Move o cursor para o Anchor correspondente
        if (cursor != null &&
            cursorAnchors != null &&
            cursorAnchors.Length > selectedIndex)
        {
            cursor.position = cursorAnchors[selectedIndex].position;
        }
    }
}
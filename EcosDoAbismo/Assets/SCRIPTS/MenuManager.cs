using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI[] menuOptions;

    private int selectedIndex = 0;

    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

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
    }

    void UpdateMenuVisual()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].color =
                (i == selectedIndex)
                ? selectedColor
                : normalColor;
        }
    }
}
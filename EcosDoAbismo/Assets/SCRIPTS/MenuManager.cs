using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Opções do Menu")]
    public TextMeshProUGUI[] menuOptions;

    [Header("Cursor")]
    public RectTransform cursor;

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

        if (cursor != null)
        {
            Vector3 cursorPos = cursor.position;

            cursor.position = new Vector3(
                cursorPos.x,
                menuOptions[selectedIndex].transform.position.y,
                cursorPos.z
            );
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class MenuSelecaoSave : MonoBehaviour
{
    [Header("Slots de Salvamento")]
    public Image[] slots;

    [Header("Imagens dos Slots")]
    public Sprite slotNormal;
    public Sprite slotSelecionado;

    [Header("Menu Principal")]
    public MenuManager menuManager;

    private int indiceSelecionado = 0;

    void OnEnable()
    {
        indiceSelecionado = 0;
        AtualizarSelecao();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuManager != null)
                menuManager.VoltarAoMenu();

            return;
        }

        if (slots == null || slots.Length == 0)
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            indiceSelecionado++;

            if (indiceSelecionado >= slots.Length)
                indiceSelecionado = 0;

            AtualizarSelecao();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            indiceSelecionado--;

            if (indiceSelecionado < 0)
                indiceSelecionado = slots.Length - 1;

            AtualizarSelecao();
        }
    }

    void AtualizarSelecao()
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (i == indiceSelecionado)
                slots[i].sprite = slotSelecionado;
            else
                slots[i].sprite = slotNormal;
        }
    }
}

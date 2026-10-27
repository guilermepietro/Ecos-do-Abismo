
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuViagem : MonoBehaviour
{
    [Header("Slots dos Destinos")]
    public Image[] slots;

    [Header("Sprites dos Slots")]
    public Sprite slotNormal;
    public Sprite slotSelecionado;

    [Header("Informações do Destino")]
    public TMP_Text textoDestino;

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
            FecharMenu();
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

    void FecharMenu()
    {
        gameObject.SetActive(false);
    }

    void AtualizarSelecao()
    {
        if (slots == null || slots.Length == 0)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            slots[i].sprite =
                i == indiceSelecionado
                ? slotSelecionado
                : slotNormal;
        }

        if (textoDestino != null)
        {
            switch (indiceSelecionado)
            {
                case 0:
                    textoDestino.text = "FLORESTA";
                    break;

                case 1:
                    textoDestino.text = "RUÍNAS DO SELO";
                    break;

                case 2:
                    textoDestino.text = "CATEDRAL DAS SOMBRAS";
                    break;

                case 3:
                    textoDestino.text = "FORTALEZA ARDENTE";
                    break;
            }
        }
    }
}

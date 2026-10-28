
using UnityEngine;
using TMPro;

public class FraseViagem : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text textoPrincipal;
    public TMP_Text textoBrilho;

    [Header("Pulsação")]
    public float velocidadePulso = 2.5f;
    public float transparenciaMinima = 0.55f;
    public float transparenciaBrilho = 0.35f;

    private bool exibindo = false;
    private int ultimaFrase = -1;

    private readonly string[] frases =
    {
        "Nem toda passagem exige uma despedida.",
        "As raízes recordam caminhos que os homens esqueceram.",
        "Há lugares dos quais a alma jamais partiu.",
        "O Abismo não conhece distâncias, apenas ecos.",
        "Onde uma chama desperta, outra responde."
    };

    void Awake()
    {
        OcultarFrase();
    }

    void Update()
    {
        if (!exibindo)
            return;

        float pulso = (
            Mathf.Sin(Time.unscaledTime * velocidadePulso) + 1f
        ) / 2f;

        if (textoPrincipal != null)
        {
            Color cor = textoPrincipal.color;

            cor.a = Mathf.Lerp(
                transparenciaMinima,
                1f,
                pulso
            );

            textoPrincipal.color = cor;
        }

        if (textoBrilho != null)
        {
            Color cor = textoBrilho.color;

            cor.a = Mathf.Lerp(
                0.05f,
                transparenciaBrilho,
                pulso
            );

            textoBrilho.color = cor;
        }
    }

    public void MostrarFrase()
    {
        if (textoPrincipal == null || textoBrilho == null)
        {
            Debug.LogWarning(
                "Textos da FraseViagem não configurados!"
            );
            return;
        }

        int indice = Random.Range(0, frases.Length);

        if (frases.Length > 1)
        {
            while (indice == ultimaFrase)
            {
                indice = Random.Range(0, frases.Length);
            }
        }

        ultimaFrase = indice;

        string frase = frases[indice];

        textoPrincipal.text = frase;
        textoBrilho.text = frase;

        textoPrincipal.gameObject.SetActive(true);
        textoBrilho.gameObject.SetActive(true);

        exibindo = true;

        Debug.Log("Frase da viagem: " + frase);
    }

    public void OcultarFrase()
    {
        exibindo = false;

        if (textoPrincipal != null)
        {
            textoPrincipal.gameObject.SetActive(false);
        }

        if (textoBrilho != null)
        {
            textoBrilho.gameObject.SetActive(false);
        }
    }
}

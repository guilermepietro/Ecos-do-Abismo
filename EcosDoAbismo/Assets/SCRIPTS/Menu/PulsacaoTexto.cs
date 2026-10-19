
using UnityEngine;
using TMPro;

public class PulsacaoTexto : MonoBehaviour
{
    [Header("Configurações da Pulsação")]
    public float velocidade = 2f;

    [Range(0f, 1f)]
    public float transparenciaMinima = 0.45f;

    [Range(0f, 1f)]
    public float transparenciaMaxima = 1f;

    private TMP_Text texto;

    void Awake()
    {
        texto = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (texto == null)
            return;

        float oscilacao = (Mathf.Sin(Time.unscaledTime * velocidade) + 1f) / 2f;

        float transparencia = Mathf.Lerp(
            transparenciaMinima,
            transparenciaMaxima,
            oscilacao
        );

        texto.alpha = transparencia;
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransicaoTela : MonoBehaviour
{
    [Header("Imagem do Fade")]
    public Image imagemFade;

    [Header("Duração")]
    public float duracaoFade = 1f;

    public IEnumerator Escurecer()
    {
        if (imagemFade == null)
            yield break;

        float tempo = 0f;
        Color cor = imagemFade.color;

        while (tempo < duracaoFade)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / Mathf.Max(0.01f, duracaoFade));

            cor.a = progresso;
            imagemFade.color = cor;

            yield return null;
        }

        cor.a = 1f;
        imagemFade.color = cor;
    }

    public IEnumerator Clarear()
    {
        if (imagemFade == null)
            yield break;

        float tempo = 0f;
        Color cor = imagemFade.color;
        cor.a = 1f;
        imagemFade.color = cor;

        while (tempo < duracaoFade)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / Mathf.Max(0.01f, duracaoFade));

            cor.a = 1f - progresso;
            imagemFade.color = cor;

            yield return null;
        }

        cor.a = 0f;
        imagemFade.color = cor;
    }
}

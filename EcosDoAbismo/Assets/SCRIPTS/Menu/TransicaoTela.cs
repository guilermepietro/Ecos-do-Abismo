
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransicaoTela : MonoBehaviour
{
    [Header("Imagem do Fade")]
    public Image imagemFade;

    [Header("Duração")]
    public float duracaoEscurecer = 0.5f;
    public float duracaoClarear = 2f;

    public IEnumerator Escurecer()
    {
        if (imagemFade == null)
            yield break;

        float tempo = 0f;
        Color cor = imagemFade.color;

        while (tempo < duracaoEscurecer)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(
                tempo / Mathf.Max(0.01f, duracaoEscurecer)
            );

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

        while (tempo < duracaoClarear)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(
                tempo / Mathf.Max(0.01f, duracaoClarear)
            );

            cor.a = 1f - progresso;
            imagemFade.color = cor;

            yield return null;
        }

        cor.a = 0f;
        imagemFade.color = cor;
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeEntradaMenu : MonoBehaviour
{
    [Header("Imagem Preta")]
    public Image imagemFade;

    [Header("Duracao")]
    public float duracaoFade = 1f;

    private void Awake()
    {
        if (imagemFade != null)
        {
            imagemFade.gameObject.SetActive(true);
            imagemFade.color = Color.black;
            imagemFade.raycastTarget = false;
        }
    }

    private IEnumerator Start()
    {
        if (imagemFade == null)
            yield break;

        yield return null;

        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(
                tempo / Mathf.Max(0.01f, duracaoFade)
            );

            imagemFade.color = new Color(
                0f, 0f, 0f,
                1f - progresso
            );

            yield return null;
        }

        imagemFade.color = Color.clear;
        imagemFade.gameObject.SetActive(false);
    }
}

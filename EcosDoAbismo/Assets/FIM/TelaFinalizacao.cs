
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TelaFinalizacao : MonoBehaviour
{
    [Header("Cena do Menu Principal")]
    public string nomeCenaMenuPrincipal = "Menu";

    [Header("Imagem do Fade")]
    public Image fadeEntrada;

    [Header("Duracao dos Fades")]
    public float duracaoFadeEntrada = 1.5f;
    public float duracaoFadeSaida = 0.8f;

    private bool podeVoltar = false;
    private bool voltandoAoMenu = false;

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (fadeEntrada != null)
        {
            fadeEntrada.gameObject.SetActive(true);
            fadeEntrada.color = Color.black;
            fadeEntrada.raycastTarget = false;
        }
    }

    private IEnumerator Start()
    {
        podeVoltar = false;

        if (fadeEntrada == null)
        {
            Debug.LogWarning(
                "FadeEntrada nao configurado!"
            );

            podeVoltar = true;
            yield break;
        }

        // Comeca totalmente preto.
        yield return null;

        // Revela a tela de finalizacao.
        yield return StartCoroutine(
            FazerFade(1f, 0f, duracaoFadeEntrada)
        );

        fadeEntrada.gameObject.SetActive(false);

        // Permite apertar Enter.
        podeVoltar = true;

        Debug.Log(
            "Tela de finalizacao revelada!"
        );
    }

    private void Update()
    {
        if (!podeVoltar || voltandoAoMenu)
            return;

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            VoltarAoMenu();
        }
    }

    public void VoltarAoMenu()
    {
        if (!podeVoltar || voltandoAoMenu)
            return;

        voltandoAoMenu = true;
        podeVoltar = false;

        StartCoroutine(TransicaoParaMenu());
    }

    private IEnumerator TransicaoParaMenu()
    {
        Debug.Log(
            "Iniciando transicao para o menu principal."
        );

        // Reativa a imagem preta.
        if (fadeEntrada != null)
        {
            fadeEntrada.gameObject.SetActive(true);
            fadeEntrada.color = Color.clear;

            // Escurece a tela gradualmente.
            yield return StartCoroutine(
                FazerFade(0f, 1f, duracaoFadeSaida)
            );
        }

        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Carrega o menu somente apos
        // a tela ficar completamente preta.
        SceneManager.LoadScene(nomeCenaMenuPrincipal);
    }

    private IEnumerator FazerFade(
        float inicio,
        float fim,
        float duracao)
    {
        if (fadeEntrada == null)
            yield break;

        if (duracao <= 0f)
        {
            fadeEntrada.color =
                new Color(0f, 0f, 0f, fim);

            yield break;
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(
                tempo / duracao
            );

            float alpha = Mathf.Lerp(
                inicio,
                fim,
                progresso
            );

            fadeEntrada.color = new Color(
                0f, 0f, 0f, alpha
            );

            yield return null;
        }

        fadeEntrada.color =
            new Color(0f, 0f, 0f, fim);
    }
}

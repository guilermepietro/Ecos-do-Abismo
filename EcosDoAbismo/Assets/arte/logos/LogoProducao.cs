using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoProducao : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image logoIF;
    [SerializeField] private Image logoPG;

    [Header("Tempos")]
    [SerializeField] private float duracaoEntrada = 0.7f;
    [SerializeField] private float tempoExibicao = 1.6f;
    [SerializeField] private float duracaoSaida = 0.7f;
    [SerializeField] private float intervaloEntreLogos = 0.2f;

    [Header("Proxima Cena")]
    [SerializeField] private string nomeCena = "Boot";

    private IEnumerator Start()
    {
        DefinirAlpha(logoIF, 0f);
        DefinirAlpha(logoPG, 0f);

        yield return ExibirLogo(logoIF);

        yield return new WaitForSeconds(intervaloEntreLogos);

        yield return ExibirLogo(logoPG);

        SceneManager.LoadScene(nomeCena);
    }

    private IEnumerator ExibirLogo(Image logo)
    {
        yield return AnimarLogo(logo, 0f, 1f, duracaoEntrada);

        yield return new WaitForSeconds(tempoExibicao);

        yield return AnimarLogo(logo, 1f, 0f, duracaoSaida);
    }

    private IEnumerator AnimarLogo(Image logo, float inicio, float fim, float duracao)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float progresso = Mathf.Clamp01(tempo / duracao);

            Color cor = logo.color;
            cor.a = Mathf.Lerp(inicio, fim, progresso);
            logo.color = cor;

            yield return null;
        }

        DefinirAlpha(logo, fim);
    }

    private void DefinirAlpha(Image logo, float alpha)
    {
        Color cor = logo.color;
        cor.a = alpha;
        logo.color = cor;
    }
}
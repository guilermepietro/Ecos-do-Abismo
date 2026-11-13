
using System;
using System.Collections;
using UnityEngine;

public class IshkharothPiscarTela : MonoBehaviour
{
    [Header("Referencia")]
    [SerializeField] private CanvasGroup telaFlash;

    [Header("Piscadas durante a transformacao")]
    [SerializeField, Range(0f, 1f)]
    private float intensidadePiscada = 0.55f;

    [SerializeField] private float duracaoAcender = 0.16f;
    [SerializeField] private float duracaoApagar = 0.25f;
    [SerializeField] private float intervaloPiscadas = 0.15f;

    [Header("Clarao final")]
    [SerializeField] private float duracaoClarao = 0.20f;
    [SerializeField] private float tempoTelaClara = 0.70f;
    [SerializeField] private float duracaoRetorno = 0.55f;

    private Coroutine rotinaAtual;
    private bool claraoFinalIniciado;

    private void Awake()
    {
        if (telaFlash != null)
        {
            telaFlash.alpha = 0f;
            telaFlash.interactable = false;
            telaFlash.blocksRaycasts = false;
        }
    }

    public void IniciarPiscadas()
    {
        if (telaFlash == null)
        {
            Debug.LogWarning(
                "Ishkharoth: CanvasGroup do flash nao configurado."
            );
            return;
        }

        if (rotinaAtual != null)
        {
            StopCoroutine(rotinaAtual);
        }

        claraoFinalIniciado = false;
        rotinaAtual = StartCoroutine(RotinaPiscadas());
    }

    private IEnumerator RotinaPiscadas()
    {
        while (true)
        {
            yield return AlterarAlpha(
                telaFlash.alpha,
                intensidadePiscada,
                duracaoAcender
            );

            yield return AlterarAlpha(
                intensidadePiscada,
                0f,
                duracaoApagar
            );

            yield return new WaitForSecondsRealtime(
                Mathf.Max(0f, intervaloPiscadas)
            );
        }
    }

    public void ExecutarClaraoFinal(Action aoCobrirTela)
    {
        if (claraoFinalIniciado)
            return;

        claraoFinalIniciado = true;

        if (rotinaAtual != null)
        {
            StopCoroutine(rotinaAtual);
            rotinaAtual = null;
        }

        if (telaFlash == null)
        {
            aoCobrirTela?.Invoke();
            return;
        }

        rotinaAtual = StartCoroutine(
            RotinaClaraoFinal(aoCobrirTela)
        );
    }

    private IEnumerator RotinaClaraoFinal(Action aoCobrirTela)
    {
        yield return AlterarAlpha(
            telaFlash.alpha,
            1f,
            duracaoClarao
        );

        // A tela esta totalmente coberta.
        // A troca visual acontece neste instante.
        aoCobrirTela?.Invoke();

        yield return new WaitForSecondsRealtime(
            Mathf.Max(0f, tempoTelaClara)
        );

        yield return AlterarAlpha(
            1f,
            0f,
            duracaoRetorno
        );

        rotinaAtual = null;
    }

    private IEnumerator AlterarAlpha(
        float inicio,
        float destino,
        float duracao)
    {
        if (duracao <= 0f)
        {
            telaFlash.alpha = destino;
            yield break;
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(
                tempo / duracao
            );

            telaFlash.alpha = Mathf.Lerp(
                inicio,
                destino,
                progresso
            );

            yield return null;
        }

        telaFlash.alpha = destino;
    }

    private void OnDisable()
    {
        if (rotinaAtual != null)
        {
            StopCoroutine(rotinaAtual);
            rotinaAtual = null;
        }

        if (telaFlash != null)
            telaFlash.alpha = 0f;
    }
}

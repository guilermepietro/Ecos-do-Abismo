
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicaMenuPersistente : MonoBehaviour
{
    public static MusicaMenuPersistente Instancia { get; private set; }

    private AudioSource fonteAudio;
    private bool encerrando = false;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);

        fonteAudio = GetComponent<AudioSource>();

        if (!fonteAudio.isPlaying)
            fonteAudio.Play();
    }

    public void EncerrarComFade(float duracao)
    {
        if (encerrando)
            return;

        encerrando = true;
        StartCoroutine(FadeEEncerrar(duracao));
    }

    private IEnumerator FadeEEncerrar(float duracao)
    {
        float volumeInicial = fonteAudio.volume;
        float tempo = 0f;

        duracao = Mathf.Max(0.01f, duracao);

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / duracao);

            fonteAudio.volume = Mathf.Lerp(
                volumeInicial,
                0f,
                progresso
            );

            yield return null;
        }

        fonteAudio.Stop();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (Instancia == this)
            Instancia = null;
    }
}

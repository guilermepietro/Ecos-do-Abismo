
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GerenciadorMusicas : MonoBehaviour
{
    public static GerenciadorMusicas Instancia { get; private set; }

    [Header("Músicas das Regiões")]
    public AudioClip musicaFloresta;
    public AudioClip musicaRuinas;
    public AudioClip musicaFortaleza;
    public AudioClip musicaCatedral;
    public AudioClip musicaSalao;
    public AudioClip musicaDimensaoLuminar;

    [Header("Configurações de Volume")]
    [Range(0f, 1f)]
    public float volumeMaximo = 0.6f;

    [Header("Transições Musicais")]
    public float duracaoFadeEntrada = 1.5f;
    public float duracaoFadeSaida = 1.5f;

    private AudioSource fonteAudio;
    private Coroutine transicaoAtual;
    private AudioClip musicaSolicitada;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        fonteAudio = GetComponent<AudioSource>();

        fonteAudio.playOnAwake = false;
        fonteAudio.loop = true;
        fonteAudio.spatialBlend = 0f;
        fonteAudio.volume = 0f;
    }

    IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();

        if (musicaSolicitada == null)
        {
            TocarMusicaDaArea(GerenciadorArea.areaAtual);
        }
    }

    public void TocarMusicaDaArea(string nomeArea)
    {
        AudioClip novaMusica = ObterMusica(nomeArea);

        if (novaMusica == null)
        {
            Debug.LogWarning(
                "Música não encontrada para a área: " + nomeArea
            );
            return;
        }

        if (musicaSolicitada == novaMusica)
            return;

        musicaSolicitada = novaMusica;

        if (transicaoAtual != null)
            StopCoroutine(transicaoAtual);

        transicaoAtual = StartCoroutine(
            TrocarMusica(novaMusica)
        );
    }

    private AudioClip ObterMusica(string nomeArea)
    {
        if (string.IsNullOrWhiteSpace(nomeArea))
            return null;

        switch (nomeArea.Trim())
        {
            case "Floresta":
                return musicaFloresta;

            case "Ruinas Do Selo":
                return musicaRuinas;

            case "Fortaleza Ardente":
                return musicaFortaleza;

            case "Catedral Das Sombras":
                return musicaCatedral;

            case "Salão Dos Ecos":
                return musicaSalao;

            case "Dimensão Luminar":
                return musicaDimensaoLuminar;

            default:
                return null;
        }
    }

    private IEnumerator TrocarMusica(AudioClip novaMusica)
    {
        if (fonteAudio.isPlaying)
        {
            yield return StartCoroutine(
                AlterarVolume(0f, duracaoFadeSaida)
            );

            fonteAudio.Stop();
        }

        fonteAudio.clip = novaMusica;
        fonteAudio.volume = 0f;
        fonteAudio.Play();

        yield return StartCoroutine(
            AlterarVolume(volumeMaximo, duracaoFadeEntrada)
        );

        transicaoAtual = null;
    }

    private IEnumerator AlterarVolume(
        float volumeDestino,
        float duracao
    )
    {
        float volumeInicial = fonteAudio.volume;
        float tempo = 0f;

        duracao = Mathf.Max(0.01f, duracao);

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(
                tempo / duracao
            );

            fonteAudio.volume = Mathf.Lerp(
                volumeInicial,
                volumeDestino,
                progresso
            );

            yield return null;
        }

        fonteAudio.volume = volumeDestino;
    }

    void OnDestroy()
    {
        if (Instancia == this)
            Instancia = null;
    }
}

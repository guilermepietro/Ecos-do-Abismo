
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GerenciadorCutscenes : MonoBehaviour
{
    public static GerenciadorCutscenes Instancia;

    [Serializable]
    public class Cutscene
    {
        public string id;
        public VideoClip video;

        [Header("Audio")]
        public bool usarAudioDoVideo;
        public bool pausarAudioDoJogo;

        [Header("Ao Terminar")]
        public UnityEvent aoTerminar;
    }

    [Header("Componentes")]
    public VideoPlayer reprodutor;
    public AudioSource audioVideo;
    public RawImage imagemVideo;
    public Image fundoPreto;

    [Header("Lista de Cutscenes")]
    public Cutscene[] cutscenes;

    [Header("Duracao dos Fades")]
    public float tempoEscurecer = 0.2f;
    public float tempoRevelarVideo = 0.25f;
    public float tempoClarearJogo = 0.5f;

    [Header("Fades Exclusivos do Final")]
    public float tempoEscurecerAntesDoFinal = 0.4f;
    public float tempoRevelarVideoFinal = 0.6f;
    public float tempoFadeSaidaFinal = 0.7f;

    [Header("Controles do Elian")]
    public MonoBehaviour[] scriptsDoJogador;

    [Header("Teste Temporario")]
    public bool testarIntroducaoAoIniciar = false;

    public bool EmReproducao { get; private set; }

    private bool videoTerminou;
    private bool videoFalhou;

    private float escalaAnterior;
    private bool audioPausadoAnterior;
    private bool[] estadosAnteriores;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        reprodutor.playOnAwake = false;
        reprodutor.isLooping = false;
        reprodutor.timeUpdateMode =
            VideoTimeUpdateMode.UnscaledGameTime;

        audioVideo.playOnAwake = false;
        audioVideo.ignoreListenerPause = true;

        imagemVideo.enabled = false;

        fundoPreto.color = Color.clear;
        fundoPreto.raycastTarget = false;

        reprodutor.loopPointReached += FinalizarVideo;
        reprodutor.errorReceived += ErroVideo;
    }

    private void Start()
    {
        if (MenuSelecaoSave.introducaoPendente)
        {
            Time.timeScale = 1f;

            if (Reproduzir("Intro"))
            {
                MenuSelecaoSave.introducaoPendente = false;

                Debug.Log(
                    "Introducao iniciada para o slot novo."
                );
            }
            else
            {
                Debug.LogError(
                    "Nao foi possivel iniciar a introducao!"
                );
            }
        }
        else if (testarIntroducaoAoIniciar)
        {
            Reproduzir("Intro");
        }
    }

    // Reproducao normal das cutscenes.
    public bool Reproduzir(
        string id, Action acaoFinal = null)
    {
        if (EmReproducao)
            return false;

        Cutscene escolhida = Array.Find(
            cutscenes, c => c.id == id);

        if (escolhida == null || escolhida.video == null)
        {
            Debug.LogWarning(
                "Cutscene nao encontrada: " + id
            );

            return false;
        }

        StartCoroutine(
            Executar(escolhida, acaoFinal, null)
        );

        return true;
    }

    // NOVO: reproduz o encerramento e troca de cena.
    public bool ReproduzirFinal(
        string id, string cenaDestino)
    {
        if (EmReproducao)
            return false;

        if (string.IsNullOrEmpty(cenaDestino))
        {
            Debug.LogError(
                "Cena de finalizacao nao configurada!"
            );

            return false;
        }

        Cutscene escolhida = Array.Find(
            cutscenes, c => c.id == id);

        if (escolhida == null || escolhida.video == null)
        {
            Debug.LogWarning(
                "Cutscene final nao encontrada: " + id
            );

            return false;
        }

        StartCoroutine(
            Executar(escolhida, null, cenaDestino)
        );

        return true;
    }

    private IEnumerator Executar(
        Cutscene cutscene,
        Action acaoFinal,
        string cenaDestino)
    {
        EmReproducao = true;

        bool ehFinal =
            !string.IsNullOrEmpty(cenaDestino);

        escalaAnterior = Time.timeScale;
        audioPausadoAnterior = AudioListener.pause;

        BloquearControles();

        Time.timeScale = 0f;
        fundoPreto.raycastTarget = true;

        // Escurece o gameplay.
        float tempoEntradaPreto = ehFinal
            ? tempoEscurecerAntesDoFinal
            : tempoEscurecer;

        yield return Fade(
            0f, 1f, tempoEntradaPreto);

        videoTerminou = false;
        videoFalhou = false;

        reprodutor.Stop();
        reprodutor.clip = cutscene.video;

        if (cutscene.usarAudioDoVideo)
        {
            reprodutor.audioOutputMode =
                VideoAudioOutputMode.AudioSource;

            reprodutor.controlledAudioTrackCount = 1;
            reprodutor.EnableAudioTrack(0, true);
            reprodutor.SetTargetAudioSource(
                0, audioVideo);
        }
        else
        {
            reprodutor.audioOutputMode =
                VideoAudioOutputMode.None;
        }

        reprodutor.Prepare();

        float limite =
            Time.realtimeSinceStartup + 15f;

        while (!reprodutor.isPrepared &&
               !videoFalhou &&
               Time.realtimeSinceStartup < limite)
        {
            yield return null;
        }

        if (!reprodutor.isPrepared || videoFalhou)
        {
            Debug.LogError(
                "Falha ao preparar: " + cutscene.id
            );

            reprodutor.Stop();

            if (ehFinal)
            {
                CarregarCenaFinal(cenaDestino);
                yield break;
            }

            acaoFinal?.Invoke();

            yield return Fade(
                1f, 0f, tempoClarearJogo);

            RestaurarGameplay();
            yield break;
        }

        if (cutscene.pausarAudioDoJogo)
            AudioListener.pause = true;

        imagemVideo.enabled = true;

        reprodutor.Play();

        // Fade de entrada do video.
        float tempoEntradaVideo = ehFinal
            ? tempoRevelarVideoFinal
            : tempoRevelarVideo;

        yield return Fade(
            1f, 0f, tempoEntradaVideo);

        // NOVO: tratamento exclusivo do video final.
        if (ehFinal)
        {
            bool fadeSaidaIniciado = false;

            while (!videoTerminou && !videoFalhou)
            {
                double duracao = reprodutor.length;
                double tempoAtual = reprodutor.time;

                // Comeca o fade nos ultimos instantes
                // para nao cortar o fim do video.
                if (!fadeSaidaIniciado &&
                    duracao > 0 &&
                    tempoAtual >=
                    duracao - tempoFadeSaidaFinal)
                {
                    fadeSaidaIniciado = true;

                    yield return Fade(
                        0f, 1f, tempoFadeSaidaFinal);
                }

                yield return null;
            }

            // Garantia caso o tempo do video
            // nao tenha permitido antecipar o fade.
            if (!fadeSaidaIniciado)
            {
                yield return Fade(
                    0f, 1f, tempoFadeSaidaFinal);
            }

            fundoPreto.color = Color.black;

            imagemVideo.enabled = false;
            reprodutor.Stop();

            if (!videoFalhou)
                cutscene.aoTerminar?.Invoke();

            // Nao volta ao gameplay.
            // Vai diretamente para a tela final.
            CarregarCenaFinal(cenaDestino);
            yield break;
        }

        // Funcionamento original das outras cutscenes.
        while (!videoTerminou && !videoFalhou)
            yield return null;

        fundoPreto.color = Color.black;

        imagemVideo.enabled = false;
        reprodutor.Stop();

        AudioListener.pause = audioPausadoAnterior;

        if (!videoFalhou)
        {
            cutscene.aoTerminar?.Invoke();
        }

        acaoFinal?.Invoke();

        yield return Fade(
            1f, 0f, tempoClarearJogo);

        RestaurarGameplay();
    }

    // NOVO: troca de cena mantendo a tela preta.
    private void CarregarCenaFinal(string cenaDestino)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        EmReproducao = false;

        SceneManager.LoadScene(cenaDestino);
    }

    private IEnumerator Fade(
        float inicio, float fim, float duracao)
    {
        float tempo = 0f;

        if (duracao <= 0f)
        {
            fundoPreto.color =
                new Color(0f, 0f, 0f, fim);

            yield break;
        }

        while (tempo < duracao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso =
                Mathf.Clamp01(tempo / duracao);

            float alpha =
                Mathf.Lerp(inicio, fim, progresso);

            fundoPreto.color =
                new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        fundoPreto.color =
            new Color(0f, 0f, 0f, fim);
    }

    private void BloquearControles()
    {
        estadosAnteriores =
            new bool[scriptsDoJogador.Length];

        for (int i = 0; i < scriptsDoJogador.Length; i++)
        {
            if (scriptsDoJogador[i] == null)
                continue;

            estadosAnteriores[i] =
                scriptsDoJogador[i].enabled;

            scriptsDoJogador[i].enabled = false;
        }
    }

    private void RestaurarGameplay()
    {
        Time.timeScale = escalaAnterior;
        AudioListener.pause = audioPausadoAnterior;

        for (int i = 0; i < scriptsDoJogador.Length; i++)
        {
            if (scriptsDoJogador[i] != null)
            {
                scriptsDoJogador[i].enabled =
                    estadosAnteriores[i];

                Debug.Log(
                    "[Cutscene] Script: " +
                    scriptsDoJogador[i].GetType().Name +
                    " | Ativado: " +
                    scriptsDoJogador[i].enabled
                );
            }
        }

        fundoPreto.color = Color.clear;
        fundoPreto.raycastTarget = false;

        EmReproducao = false;

        Debug.Log(
            "[Cutscene] Gameplay restaurado! TimeScale: " +
            Time.timeScale
        );
    }

    private void FinalizarVideo(VideoPlayer player)
    {
        videoTerminou = true;
    }

    private void ErroVideo(
        VideoPlayer player, string mensagem)
    {
        Debug.LogError(
            "Erro no video: " + mensagem
        );

        videoFalhou = true;
    }

    private void OnDestroy()
    {
        if (Instancia != this)
            return;

        reprodutor.loopPointReached -= FinalizarVideo;
        reprodutor.errorReceived -= ErroVideo;

        if (EmReproducao)
        {
            Time.timeScale = escalaAnterior;
            AudioListener.pause = audioPausadoAnterior;

            if (estadosAnteriores != null)
            {
                for (int i = 0; i < scriptsDoJogador.Length; i++)
                {
                    if (scriptsDoJogador[i] != null)
                    {
                        scriptsDoJogador[i].enabled =
                            estadosAnteriores[i];
                    }
                }
            }
        }

        Instancia = null;
    }
}

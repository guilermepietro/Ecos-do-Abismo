
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioRakthas : MonoBehaviour
{
    [Header("Movimentação")]
    public AudioClip somPasso;

    [Header("Fonte Exclusiva dos Passos")]
    public AudioSource audioPassos;

    [Header("Suavização dos Passos")]
    public float duracaoFadePassos = 0.15f;

    [Header("Ataques")]
    public AudioClip somRaio;
    public AudioClip somOnda;
    public AudioClip somSoco;

    private AudioSource audioSource;
    private RakthasMovimento movimento;
    private RakthasVida vida;
    private RakthasAtaques ataques;
    private Animator animator;

    private float volumeNormalPassos;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        movimento = GetComponent<RakthasMovimento>();
        vida = GetComponent<RakthasVida>();
        ataques = GetComponent<RakthasAtaques>();
        animator = GetComponent<Animator>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (audioPassos != null)
        {
            if (audioPassos == audioSource)
            {
                Debug.LogWarning(
                    "AudioRakthas: os passos precisam " +
                    "de um AudioSource separado."
                );

                audioPassos = null;
            }
            else
            {
                audioPassos.playOnAwake = false;
                audioPassos.loop = false;

                volumeNormalPassos = audioPassos.volume;
            }
        }
    }

    private void Update()
    {
        if (audioPassos == null)
            return;

        if (PodeTocarPasso())
        {
            if (audioPassos.volume < volumeNormalPassos)
            {
                audioPassos.volume = volumeNormalPassos;
            }

            return;
        }

        if (!audioPassos.isPlaying)
        {
            audioPassos.volume = volumeNormalPassos;
            return;
        }

        float duracao = Mathf.Max(
            0.01f,
            duracaoFadePassos
        );

        float velocidadeFade =
            volumeNormalPassos / duracao;

        audioPassos.volume = Mathf.MoveTowards(
            audioPassos.volume,
            0f,
            velocidadeFade * Time.deltaTime
        );

        if (audioPassos.volume <= 0.001f)
        {
            audioPassos.Stop();

            audioPassos.volume = volumeNormalPassos;
        }
    }

    private bool PodeTocarPasso()
    {
        if (movimento == null ||
            !movimento.EstaAtivado)
        {
            return false;
        }

        if (vida != null &&
            (vida.EstaMorto || vida.EstaTomandoDano))
        {
            return false;
        }

        if (ataques != null &&
            (ataques.EstaAtacando ||
             ataques.EstaEmPausaDecisao))
        {
            return false;
        }

        if (animator != null &&
            !animator.GetBool("estaCorrendo"))
        {
            return false;
        }

        return true;
    }

    public void TocarPasso()
    {
        if (!PodeTocarPasso())
            return;

        if (audioPassos == null || somPasso == null)
            return;

        audioPassos.Stop();

        audioPassos.volume = volumeNormalPassos;

        audioPassos.PlayOneShot(somPasso);
    }

    public void TocarRaio()
    {
        ReproduzirSom(somRaio);
    }

    public void TocarOnda()
    {
        ReproduzirSom(somOnda);
    }

    public void TocarSoco()
    {
        ReproduzirSom(somSoco);
    }

    private void ReproduzirSom(AudioClip som)
    {
        if (audioSource != null && som != null)
        {
            audioSource.PlayOneShot(som);
        }
    }
}


using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVoragul : MonoBehaviour
{
    [Header("Movimentação")]
    public AudioClip somPasso;

    [Header("Fonte Exclusiva dos Passos")]
    public AudioSource audioPassos;

    [Header("Suavização dos Passos")]
    public float duracaoFadePassos = 0.15f;

    [Header("Ataques")]
    public AudioClip somAtaque1;
    public AudioClip somAtaque2;
    public AudioClip somAtaque3;

    [Header("Vida")]
    public AudioClip somDano;
    public AudioClip somMorte;

    private AudioSource audioSource;
    private VoragulMovimento movimento;
    private VoragulVida vida;
    private Animator animator;

    // NOVO: guarda o volume original dos passos
    private float volumeNormalPassos;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        movimento = GetComponent<VoragulMovimento>();
        vida = GetComponent<VoragulVida>();
        animator = GetComponent<Animator>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (audioPassos != null)
        {
            if (audioPassos == audioSource)
            {
                Debug.LogWarning(
                    "AudioVoragul: os passos precisam " +
                    "de um AudioSource separado."
                );

                audioPassos = null;
            }
            else
            {
                audioPassos.playOnAwake = false;
                audioPassos.loop = false;

                // NOVO
                volumeNormalPassos = audioPassos.volume;
            }
        }
    }

    private void Update()
    {
        if (audioPassos == null)
            return;

        // Se voltou a caminhar, restaura o volume normal
        if (PodeTocarPasso())
        {
            if (audioPassos.volume < volumeNormalPassos)
            {
                audioPassos.volume = volumeNormalPassos;
            }

            return;
        }

        // Se não há passo tocando, não precisa suavizar
        if (!audioPassos.isPlaying)
        {
            audioPassos.volume = volumeNormalPassos;
            return;
        }

        // NOVO: reduz o volume gradualmente
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

        // Interrompe somente depois do fade
        if (audioPassos.volume <= 0.001f)
        {
            audioPassos.Stop();

            // Prepara o volume para o próximo passo
            audioPassos.volume = volumeNormalPassos;
        }
    }

    private bool PodeTocarPasso()
    {
        if (movimento == null ||
            !movimento.lutaIniciada ||
            !movimento.podeMover)
        {
            return false;
        }

        if (vida != null && vida.EstaMorto())
            return false;

        if (animator != null &&
            !animator.GetBool("andando"))
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

        // Mantém a prevenção de passos sobrepostos
        audioPassos.Stop();

        // NOVO: garante o volume original
        audioPassos.volume = volumeNormalPassos;

        audioPassos.PlayOneShot(somPasso);
    }

    public void TocarAtaque1()
    {
        ReproduzirSom(somAtaque1);
    }

    public void TocarAtaque2()
    {
        ReproduzirSom(somAtaque2);
    }

    public void TocarAtaque3()
    {
        ReproduzirSom(somAtaque3);
    }

    public void TocarDano()
    {
        ReproduzirSom(somDano);
    }

    public void TocarMorte()
    {
        ReproduzirSom(somMorte);
    }

    private void ReproduzirSom(AudioClip som)
    {
        if (audioSource != null && som != null)
        {
            audioSource.PlayOneShot(som);
        }
    }
}

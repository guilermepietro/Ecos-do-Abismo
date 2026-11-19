
using UnityEngine;

public class AudioIshkharoth : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private IshkharothVida vida;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D corpo;

    [Header("Fontes de Audio")]
    [SerializeField] private AudioSource audioEfeitos;
    [SerializeField] private AudioSource audioPassos;

    [Header("Ataques - Primeira Fase")]
    [SerializeField] private AudioClip somEsferaConjuracao;
    [SerializeField] private AudioClip somOndaEspinhos;
    [SerializeField] private AudioClip somChuvaConjuracao;

    [Header("Ataques - Segunda Fase")]
    [SerializeField] private AudioClip somProjetilVermelho;
    [SerializeField] private AudioClip somMeiaLua;
    [SerializeField] private AudioClip somPilaresConjuracao;

    [Header("Transformacao")]
    [SerializeField] private AudioClip somTransformacao;

    [Header("Passos")]
    [SerializeField] private AudioClip passosFase1;
    [SerializeField] private AudioClip passosFase2;

    [Header("Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float volumeAtaques = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float volumeTransformacao = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float volumePassos = 0.7f;

    private void Awake()
    {
        if (vida == null)
            vida = GetComponent<IshkharothVida>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (corpo == null)
            corpo = GetComponent<Rigidbody2D>();

        if (audioEfeitos != null)
        {
            audioEfeitos.playOnAwake = false;
            audioEfeitos.loop = false;
        }

        if (audioPassos != null)
        {
            audioPassos.playOnAwake = false;
            audioPassos.loop = true;
        }
    }

    private void Update()
    {
        AtualizarPassos();
    }

    private void ReproduzirEfeito(
        AudioClip clipe,
        float volume)
    {
        if (audioEfeitos == null || clipe == null)
            return;

        audioEfeitos.PlayOneShot(clipe, volume);
    }

    // PRIMEIRA FASE

    public void SomEsferaConjuracao()
    {
        ReproduzirEfeito(
            somEsferaConjuracao,
            volumeAtaques
        );
    }

    public void SomOndaEspinhos()
    {
        ReproduzirEfeito(
            somOndaEspinhos,
            volumeAtaques
        );
    }

    public void SomChuvaConjuracao()
    {
        ReproduzirEfeito(
            somChuvaConjuracao,
            volumeAtaques
        );
    }

    // SEGUNDA FASE

    public void SomProjetilVermelho()
    {
        ReproduzirEfeito(
            somProjetilVermelho,
            volumeAtaques
        );
    }

    public void SomMeiaLua()
    {
        ReproduzirEfeito(
            somMeiaLua,
            volumeAtaques
        );
    }

    public void SomPilaresConjuracao()
    {
        ReproduzirEfeito(
            somPilaresConjuracao,
            volumeAtaques
        );
    }

    // TRANSFORMACAO

    public void SomTransformacao()
    {
        ReproduzirEfeito(
            somTransformacao,
            volumeTransformacao
        );
    }

    // PASSOS

    private void AtualizarPassos()
    {
        if (audioPassos == null ||
            animator == null ||
            vida == null)
        {
            return;
        }

        if (vida.Morto ||
            vida.EmTransicao ||
            !animator.isActiveAndEnabled)
        {
            PararPassos();
            return;
        }

        AudioClip clipePassos = null;

        if (vida.EstaNaFase1)
        {
            clipePassos = passosFase1;
        }
        else if (vida.EstaNaFase2)
        {
            clipePassos = passosFase2;
        }
        else
        {
            PararPassos();
            return;
        }

        bool andando = animator.GetBool("Andando");

        bool estaSeMovendo = corpo != null &&
            Mathf.Abs(corpo.linearVelocity.x) > 0.05f;

        if (!andando ||
            !estaSeMovendo ||
            clipePassos == null)
        {
            PararPassos();
            return;
        }

        if (audioPassos.clip != clipePassos)
        {
            audioPassos.Stop();
            audioPassos.clip = clipePassos;
        }

        audioPassos.volume = volumePassos;

        if (!audioPassos.isPlaying)
        {
            audioPassos.Play();
        }
    }

    private void PararPassos()
    {
        if (audioPassos != null &&
            audioPassos.isPlaying)
        {
            audioPassos.Stop();
        }
    }

    private void OnDisable()
    {
        PararPassos();
    }
}

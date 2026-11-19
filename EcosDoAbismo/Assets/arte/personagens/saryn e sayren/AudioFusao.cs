
using UnityEngine;

public class AudioFusao : MonoBehaviour
{
    [Header("Fontes de Audio")]
    [SerializeField] private AudioSource audioEfeitos;
    [SerializeField] private AudioSource audioPassos;
    [SerializeField] private AudioSource audioGritoLoop;

    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private FusaoVida vida;
    [SerializeField] private FusaoAtaque3 ataque3;
    [SerializeField] private Rigidbody2D corpo;
    [SerializeField] private Animator animator;

    [Header("Passos")]
    [SerializeField] private AudioClip somPassos;

    [Header("Ataque 1")]
    [SerializeField] private AudioClip somSoco;
    [SerializeField] private AudioClip somChama;

    [Header("Ataque 2")]
    [SerializeField] private AudioClip somConjuracao;
    [SerializeField] private AudioClip somAviso;
    [SerializeField] private AudioClip somPilar;

    [Header("Ataque 3")]
    [SerializeField] private AudioClip somGritoInicio;
    [SerializeField] private AudioClip somGritoLoop;
    [SerializeField] private AudioClip somGritoFim;
    [SerializeField] private AudioClip somPedra;

    [Header("Morte")]
    [SerializeField] private AudioClip somMorte;

    [Header("Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float volumeEfeitos = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float volumePassos = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float volumeGrito = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float volumePedras = 0.45f;

    [Header("Controle das Pedras")]
    [SerializeField] private float intervaloSomPedras = 0.12f;

    private float ultimoSomPedra = -999f;
    private bool morteTocada;

    private void Awake()
    {
        if (movimento == null)
            movimento = GetComponent<FusaoMovimento>();

        if (vida == null)
            vida = GetComponent<FusaoVida>();

        if (ataque3 == null)
            ataque3 = GetComponent<FusaoAtaque3>();

        if (corpo == null)
            corpo = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioEfeitos != null)
        {
            audioEfeitos.playOnAwake = false;
            audioEfeitos.loop = false;
            audioEfeitos.spatialBlend = 0f;
        }

        if (audioPassos != null)
        {
            audioPassos.playOnAwake = false;
            audioPassos.loop = true;
            audioPassos.spatialBlend = 0f;
            audioPassos.clip = somPassos;
            audioPassos.volume = volumePassos;
        }

        if (audioGritoLoop != null)
        {
            audioGritoLoop.playOnAwake = false;
            audioGritoLoop.loop = true;
            audioGritoLoop.spatialBlend = 0f;
            audioGritoLoop.clip = somGritoLoop;
            audioGritoLoop.volume = volumeGrito;
        }
    }

    private void Update()
    {
        if (vida != null && vida.Morto)
        {
            PararPassos();
            PararGrito();

            if (!morteTocada)
            {
                morteTocada = true;
                Reproduzir(somMorte);
            }

            return;
        }

        AtualizarPassos();

        // NOVO: interrompe o loop quando
        // a chuva de pedras termina.
        AtualizarGrito();
    }

    private void AtualizarPassos()
    {
        if (audioPassos == null || somPassos == null)
            return;

        bool andando = false;

        if (movimento != null &&
            movimento.CombateAtivo &&
            corpo != null &&
            animator != null &&
            animator.GetBool("Andando"))
        {
            andando =
                Mathf.Abs(corpo.linearVelocity.x) > 0.05f;
        }

        if (andando)
        {
            if (!audioPassos.isPlaying)
                audioPassos.Play();
        }
        else
        {
            PararPassos();
        }
    }

    private void PararPassos()
    {
        if (audioPassos != null && audioPassos.isPlaying)
            audioPassos.Stop();
    }

    // =========================
    // CONTROLE AUTOMATICO DO GRITO
    // =========================

    private void AtualizarGrito()
    {
        if (audioGritoLoop == null ||
            !audioGritoLoop.isPlaying)
            return;

        bool ataqueTerminou =
            ataque3 == null || !ataque3.EmAtaque;

        bool chuvaTerminou =
            animator != null &&
            animator.GetBool("ChuvaTerminou");

        if (ataqueTerminou || chuvaTerminou)
            PararGrito();
    }

    private void Reproduzir(
        AudioClip som,
        float multiplicador = 1f)
    {
        if (som == null || audioEfeitos == null)
            return;

        audioEfeitos.PlayOneShot(
            som,
            volumeEfeitos * multiplicador
        );
    }

    // =========================
    // ATAQUE 1
    // =========================

    public void SomSoco()
    {
        Reproduzir(somSoco);
    }

    public void SomChama()
    {
        Reproduzir(somChama);
    }

    // =========================
    // ATAQUE 2
    // =========================

    public void SomConjuracao()
    {
        Reproduzir(somConjuracao);
    }

    public void SomAviso()
    {
        Reproduzir(somAviso);
    }

    public void SomPilar()
    {
        Reproduzir(somPilar);
    }

    // =========================
    // ATAQUE 3
    // =========================

    public void SomGritoInicio()
    {
        Reproduzir(somGritoInicio);
    }

    public void IniciarSomGritoLoop()
    {
        if (audioGritoLoop == null ||
            somGritoLoop == null)
            return;

        if (ataque3 == null || !ataque3.EmAtaque)
            return;

        if (animator != null &&
            animator.GetBool("ChuvaTerminou"))
            return;

        if (!audioGritoLoop.isPlaying)
            audioGritoLoop.Play();
    }

    public void SomGritoFim()
    {
        PararGrito();
        Reproduzir(somGritoFim);
    }

    private void PararGrito()
    {
        if (audioGritoLoop != null &&
            audioGritoLoop.isPlaying)
        {
            audioGritoLoop.Stop();
        }
    }

    public void SomPedra()
    {
        if (Time.time - ultimoSomPedra <
            intervaloSomPedras)
            return;

        ultimoSomPedra = Time.time;

        Reproduzir(
            somPedra,
            volumePedras
        );
    }

    private void OnDisable()
    {
        PararPassos();
        PararGrito();
    }
}

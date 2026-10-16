using UnityEngine;

public class VoragulAtaque1 : MonoBehaviour
{
    [Header("Ataque 1")]
    public float distanciaAtaque = 3f;
    public float tempoRecuperacao = 1.5f;
    public float tempoEntreAtaques = 2f;

    public VoragulHitboxAtaque1 hitboxAtaque1;

    private VoragulMovimento movimento;
    private VoragulVida vida;
    private Animator animator;
    private CameraFollow cameraFollow;

    private bool atacando = false;
    private float proximoAtaque = 0f;

    private void Awake()
    {
        movimento = GetComponent<VoragulMovimento>();
        vida = GetComponent<VoragulVida>();
        animator = GetComponent<Animator>();

        cameraFollow = FindFirstObjectByType<CameraFollow>();
    }

    private void IniciarAtaque()
    {
        atacando = true;

        movimento.podeMover = false;

        animator.SetBool("andando", false);
        animator.SetTrigger("Ataque1");
    }

    public void ForcarAtaque()
    {
        if (!PodeAtacar())
            return;

        IniciarAtaque();
    }

    public bool PodeAtacar()
    {
        if (atacando)
            return false;

        if (Time.time < proximoAtaque)
            return false;

        if (vida != null && vida.EstaMorto())
            return false;

        if (movimento == null || !movimento.lutaIniciada)
            return false;

        return true;
    }

    public void ImpactoAtaque1()
    {
        if (vida != null && vida.EstaMorto())
            return;

        if (hitboxAtaque1 != null)
        {
            hitboxAtaque1.VerificarAcerto();
        }

        if (cameraFollow != null)
        {
            cameraFollow.TremerCamera();
        }
    }

    public void FinalizarAtaque1()
    {
        Invoke(nameof(LiberarMovimento), tempoRecuperacao);
    }

    private void LiberarMovimento()
    {
        if (vida != null && vida.EstaMorto())
            return;

        atacando = false;

        proximoAtaque = Time.time + tempoEntreAtaques;

        movimento.podeMover = true;
    }

    public bool EstaAtacando()
    {
        return atacando;
    }
}
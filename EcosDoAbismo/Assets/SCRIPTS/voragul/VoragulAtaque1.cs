
using UnityEngine;

public class VoragulAtaque1 : MonoBehaviour
{
    [Header("Ataque 1")]
    public float distanciaAtaque = 3f;
    public float tempoRecuperacao = 1f;
    public float tempoEntreAtaques = 2f;
    public VoragulHitboxAtaque1 hitboxAtaque1;
    private CameraFollow cameraFollow;

    private VoragulMovimento movimento;
    private VoragulVida vida;
    private Animator animator;

    private bool atacando = false;
    private float proximoAtaque = 0f;

    private void Awake()
    {
        movimento = GetComponent<VoragulMovimento>();
        vida = GetComponent<VoragulVida>();
        animator = GetComponent<Animator>();
        cameraFollow = FindFirstObjectByType<CameraFollow>();
    }

    private void Update()
    {
        if (!movimento.lutaIniciada || !movimento.podeMover)
            return;

        if (atacando || Time.time < proximoAtaque)
            return;

        if (vida != null && vida.EstaMorto())
            return;

        if (movimento.elian == null)
            return;

        float distancia = Mathf.Abs(
            movimento.elian.position.x - transform.position.x
        );

        if (distancia <= distanciaAtaque)
        {
            IniciarAtaque();
        }
    }

    private void IniciarAtaque()
    {
        atacando = true;
        movimento.podeMover = false;

        animator.SetBool("andando", false);
        animator.SetTrigger("Ataque1");
    }

    // Animation Event no final do ataque
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
}
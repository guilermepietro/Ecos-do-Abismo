using UnityEngine;

public class VoragulAtaque2 : MonoBehaviour
{
    [Header("Ataque 2")]
    public float distanciaMinima = 3.1f;
    public float distanciaMaxima = 12f;
    public float tempoRecuperacao = 1.5f;
    public float tempoEntreAtaques = 2f;

    public VoragulHitboxAtaque2 hitboxAtaque2;

    private VoragulMovimento movimento;
    private VoragulVida vida;
    private VoragulAtaque1 ataque1;
    private Animator animator;

    private bool atacando = false;
    private float proximoAtaque = 0f;

    private void Awake()
    {
        movimento = GetComponent<VoragulMovimento>();
        vida = GetComponent<VoragulVida>();
        ataque1 = GetComponent<VoragulAtaque1>();
        animator = GetComponent<Animator>();
    }

    private void IniciarAtaque2()
    {
        atacando = true;

        movimento.podeMover = false;

        animator.SetBool("andando", false);
        animator.SetTrigger("Ataque2");
    }

    public void ForcarAtaque()
    {
        if (!PodeAtacar())
            return;

        IniciarAtaque2();
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

    public void AtivarDanoAtaque2()
    {
        if (vida != null && vida.EstaMorto())
            return;

        if (hitboxAtaque2 != null)
        {
            hitboxAtaque2.AtivarDano();
        }
    }

    public void DesativarDanoAtaque2()
    {
        if (hitboxAtaque2 != null)
        {
            hitboxAtaque2.DesativarDano();
        }
    }

    public void FinalizarAtaque2()
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
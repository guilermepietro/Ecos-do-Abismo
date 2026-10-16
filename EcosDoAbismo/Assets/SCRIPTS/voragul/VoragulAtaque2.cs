using UnityEngine;

public class VoragulAtaque2 : MonoBehaviour
{
    [Header("Ataque 2")]
    public float distanciaMinima = 3.1f;
    public float distanciaMaxima = 7f;
    public float tempoRecuperacao = 1.5f;
    public float tempoEntreAtaques = 2f;

    private VoragulMovimento movimento;
    private VoragulVida vida;
    private VoragulAtaque1 ataque1;
    private Animator animator;
    public VoragulHitboxAtaque2 hitboxAtaque2;

    private bool atacando = false;
    private float proximoAtaque = 0f;

    private void Awake()
    {
        movimento = GetComponent<VoragulMovimento>();
        vida = GetComponent<VoragulVida>();
        ataque1 = GetComponent<VoragulAtaque1>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (movimento == null)
            return;

        if (!movimento.lutaIniciada || !movimento.podeMover)
            return;

        if (atacando || Time.time < proximoAtaque)
            return;

        if (vida != null && vida.EstaMorto())
            return;

        if (ataque1 != null && ataque1.EstaAtacando())
            return;

        if (movimento.elian == null)
            return;

        float distancia = Mathf.Abs(
            movimento.elian.position.x - transform.position.x
        );

        if (distancia >= distanciaMinima && distancia <= distanciaMaxima)
        {
            IniciarAtaque2();
        }
    }

    private void IniciarAtaque2()
    {
        atacando = true;

        movimento.podeMover = false;

        animator.SetBool("andando", false);
        animator.SetTrigger("Ataque2");
    }

    public void ImpactoAtaque2()
{
    if (vida != null && vida.EstaMorto())
        return;

    if (hitboxAtaque2 != null)
    {
        hitboxAtaque2.VerificarAcerto();
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

//dsdsdsdsds
using UnityEngine;

public class EsqueleticoAtaque : MonoBehaviour
{
    
    public float tempoEntreAtaques = 1.2f;

    private Animator animator;
    private Transform elian;
    private ElianVida vidaElian;
    private EsqueleticoHitboxAtaque hitboxAtaque;
    private EsqueleticoVida vida;

    private float proximoAtaque;

    void Start()
    {
        hitboxAtaque = GetComponentInChildren<EsqueleticoHitboxAtaque>();
        animator = GetComponent<Animator>();
        vida = GetComponent<EsqueleticoVida>();

        GameObject jogador = GameObject.Find("elian");

        if (jogador != null)
        {
            elian = jogador.transform;
            vidaElian = jogador.GetComponent<ElianVida>();
        }
    }

    void Update()
    {
        if (elian == null)
            return;
            if (vida != null && (vida.EstaMorto || vida.EstaTomandoDano))
    return;

        if (vidaElian != null && vidaElian.EstaMorto)
            return;

        AnimatorStateInfo estado = animator.GetCurrentAnimatorStateInfo(0);

        if (estado.IsName("Attack") ||
            estado.IsName("Damage") ||
            estado.IsName("Death"))
            return;
        

        if (hitboxAtaque != null &&
    hitboxAtaque.ElianDentro &&
    Time.time >= proximoAtaque)
{
    animator.SetTrigger("Atacar");
    proximoAtaque = Time.time + tempoEntreAtaques;
}
    }

    public void AcertarAtaque()
{
    EsqueleticoHitboxAtaque ataque =
        GetComponentInChildren<EsqueleticoHitboxAtaque>();

    if (ataque != null)
        ataque.VerificarAcerto();
}
}
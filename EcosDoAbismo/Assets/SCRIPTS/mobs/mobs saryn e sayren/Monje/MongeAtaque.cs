
using UnityEngine;

public class MongeAtaque : MonoBehaviour
{
    public float distanciaAtaque = 3f;
    public float tempoEntreAtaques = 1.5f;

    private Transform elian;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MongeMovimento movimento;
    public Transform hitboxAtaque;
    public MongeHitboxAtaque hitbox;

    private float proximoAtaque;

    public bool EstaAtacando { get; private set; }

    // NOVO: áudio universal do mob
    private AudioMob audioMob;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movimento = GetComponent<MongeMovimento>();

        // NOVO
        audioMob = GetComponentInParent<AudioMob>();

        GameObject jogador = GameObject.Find("elian");

        if (jogador != null)
        {
            elian = jogador.transform;
        }
    }

    public void AtivarDanoAtaque()
    {
        if (hitbox != null)
            hitbox.AtivarDano();
    }

    public void DesativarDanoAtaque()
    {
        if (hitbox != null)
            hitbox.DesativarDano();
    }

    void Update()
    {
        if (elian == null)
            return;

        if (EstaAtacando)
            return;

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        if (distancia <= distanciaAtaque &&
            Time.time >= proximoAtaque)
        {
            float direcao = Mathf.Sign(
                elian.position.x - transform.position.x
            );

            spriteRenderer.flipX = direcao < 0;

            Vector3 posicaoHitbox =
                hitboxAtaque.localPosition;

            if (direcao < 0)
            {
                posicaoHitbox.x = -0.629f;
            }
            else
            {
                posicaoHitbox.x = 0.496f;
            }

            hitboxAtaque.localPosition = posicaoHitbox;

            EstaAtacando = true;

            if (movimento != null)
                movimento.PodeMover = false;

            animator.SetBool("Correndo", false);
            animator.SetTrigger("Atacar");

            // NOVO: som ao iniciar o ataque
            if (audioMob != null)
            {
                audioMob.TocarAtaque();
            }

            proximoAtaque =
                Time.time + tempoEntreAtaques;
        }
    }

    public void FinalizarAtaque()
    {
        EstaAtacando = false;

        if (movimento != null)
            movimento.PodeMover = true;
    }

    public void CancelarAtaque()
    {
        EstaAtacando = false;

        animator.ResetTrigger("Atacar");

        if (movimento != null)
            movimento.PodeMover = false;

        proximoAtaque =
            Time.time + tempoEntreAtaques;
    }
}

using UnityEngine;

public class VoragulAtaque3 : MonoBehaviour
{
    [Header("Ataque 3")]
    public float distanciaMinima = 12.1f;
    public float tempoRecuperacao = 1.5f;
    public float tempoEntreAtaques = 2f;

    [Header("Pedra")]
    public GameObject prefabPedra;
    public Transform pontoArremesso;

    [Header("Posição do Arremesso")]
    public float posicaoArremessoDireita = 0.5f;
    public float posicaoArremessoEsquerda = -0.5f;

    private VoragulMovimento movimento;
    private VoragulVida vida;
    private VoragulAtaque1 ataque1;
    private VoragulAtaque2 ataque2;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool atacando = false;
    private float proximoAtaque = 0f;

    private void Awake()
    {
        movimento = GetComponent<VoragulMovimento>();
        vida = GetComponent<VoragulVida>();
        ataque1 = GetComponent<VoragulAtaque1>();
        ataque2 = GetComponent<VoragulAtaque2>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (pontoArremesso == null || spriteRenderer == null)
            return;

        Vector3 posicao = pontoArremesso.localPosition;

        posicao.x = spriteRenderer.flipX
            ? posicaoArremessoEsquerda
            : posicaoArremessoDireita;

        pontoArremesso.localPosition = posicao;
    }

    private void IniciarAtaque3()
    {
        atacando = true;

        movimento.podeMover = false;

        animator.SetBool("andando", false);
        animator.SetTrigger("Ataque3");
    }

    public void ForcarAtaque()
    {
        if (!PodeAtacar())
            return;

        IniciarAtaque3();
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

    public void LancarPedra()
    {
        if (vida != null && vida.EstaMorto())
            return;

        if (prefabPedra == null || pontoArremesso == null)
            return;

        GameObject novaPedra = Instantiate(
            prefabPedra,
            pontoArremesso.position,
            Quaternion.identity
        );

        PedraVoragul pedra = novaPedra.GetComponent<PedraVoragul>();

        if (pedra != null)
        {
            Vector2 direcao;

            if (spriteRenderer != null && spriteRenderer.flipX)
            {
                direcao = Vector2.left;
            }
            else
            {
                direcao = Vector2.right;
            }

            pedra.Lancar(direcao);
        }
    }

    public void FinalizarAtaque3()
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
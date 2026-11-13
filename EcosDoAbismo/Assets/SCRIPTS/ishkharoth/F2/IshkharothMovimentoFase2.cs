
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class IshkharothMovimentoFase2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Animator animator;
    [SerializeField] private IshkharothVida vida;
    [SerializeField] private IshkharothMovimento movimentoFase1;

    [Header("Movimentacao da Fase 2")]
    [SerializeField] private float velocidade = 3.2f;
    [SerializeField] private float distanciaParar = 2.5f;

    [Header("Inicio da Fase 2")]
    [SerializeField] private float esperaAposTransformacao = 1.3f;

    [Header("Direcao")]
    [SerializeField] private bool spriteOriginalOlhaParaEsquerda = true;

    private Rigidbody2D corpo;

    private bool fase2Ativa;
    private bool movimentoBloqueado;

    private float instanteLiberacao;

    public bool Fase2Ativa => fase2Ativa;
    public bool MovimentoBloqueado => movimentoBloqueado;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();

        if (visual == null)
            visual = GetComponent<SpriteRenderer>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (vida == null)
            vida = GetComponent<IshkharothVida>();

        if (movimentoFase1 == null)
            movimentoFase1 =
                GetComponent<IshkharothMovimento>();
    }

    private void Update()
    {
        if (!fase2Ativa)
        {
            if (vida == null || !vida.Fase2Iniciada)
                return;

            AtivarMovimentoFase2();
        }

        if (elian == null || movimentoBloqueado)
            return;

        if (Time.time < instanteLiberacao)
            return;

        AtualizarDirecao();
    }

    private void FixedUpdate()
    {
        if (!fase2Ativa || corpo == null)
            return;

        float movimentoX = 0f;

        bool podeMover =
            !movimentoBloqueado &&
            elian != null &&
            Time.time >= instanteLiberacao;

        if (podeMover)
        {
            float diferencaX =
                elian.position.x - transform.position.x;

            float distancia = Mathf.Abs(diferencaX);

            if (distancia > distanciaParar)
            {
                float direcao = Mathf.Sign(diferencaX);

                movimentoX = direcao * velocidade;
            }
        }

        corpo.linearVelocity = new Vector2(
            movimentoX,
            corpo.linearVelocity.y
        );

        if (animator != null && animator.enabled)
        {
            animator.SetBool(
                "Andando",
                Mathf.Abs(movimentoX) > 0.01f
            );
        }
    }

    private void AtivarMovimentoFase2()
    {
        fase2Ativa = true;
        movimentoBloqueado = false;

        // A fase 1 nao deve mais controlar o Rigidbody2D.
        if (movimentoFase1 != null)
            movimentoFase1.enabled = false;

        instanteLiberacao =
            Time.time + Mathf.Max(
                0f,
                esperaAposTransformacao
            );

        if (corpo != null)
        {
            corpo.linearVelocity = new Vector2(
                0f,
                corpo.linearVelocity.y
            );
        }

        if (animator != null)
        {
            animator.enabled = true;
            animator.SetBool("Andando", false);
        }

        Debug.Log(
            "Ishkharoth: movimentacao da fase 2 ativada!"
        );
    }

    private void AtualizarDirecao()
    {
        if (visual == null || elian == null)
            return;

        float diferencaX =
            elian.position.x - transform.position.x;

        if (Mathf.Abs(diferencaX) < 0.05f)
            return;

        bool olhandoDireita = diferencaX > 0f;

        visual.flipX = spriteOriginalOlhaParaEsquerda
            ? olhandoDireita
            : !olhandoDireita;
    }

    public void BloquearMovimento()
    {
        movimentoBloqueado = true;

        if (corpo != null)
        {
            corpo.linearVelocity = new Vector2(
                0f,
                corpo.linearVelocity.y
            );
        }

        if (animator != null && animator.enabled)
            animator.SetBool("Andando", false);
    }

    public void LiberarMovimento()
    {
        movimentoBloqueado = false;
    }
}

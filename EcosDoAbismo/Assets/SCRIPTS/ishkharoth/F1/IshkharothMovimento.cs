
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class IshkharothMovimento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Animator animator;

    [Header("Movimentacao")]
    [SerializeField] private float velocidade = 2f;
    [SerializeField] private float distanciaParar = 3f;

    [Header("Direcao")]
    [SerializeField] private bool spriteOriginalOlhaParaEsquerda = true;

    private Rigidbody2D corpo;
    private bool combateAtivo;
    private bool movimentoBloqueado;

    public bool CombateAtivo => combateAtivo;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();

        if (visual == null)
            visual = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!combateAtivo || elian == null || movimentoBloqueado)
            return;

        AtualizarDirecao();
    }

    private void FixedUpdate()
    {
        if (corpo == null)
            return;

        float movimentoX = 0f;

        if (combateAtivo && !movimentoBloqueado && elian != null)
        {
            float diferencaX = elian.position.x - transform.position.x;
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

        if (animator != null)
            animator.SetBool("Andando", Mathf.Abs(movimentoX) > 0.01f);
    }

    // Chamado pela area de ativacao.
    public void AtivarCombate()
    {
        if (combateAtivo)
            return;

        combateAtivo = true;
    }

    private void AtualizarDirecao()
    {
        if (visual == null || elian == null)
            return;

        float diferencaX = elian.position.x - transform.position.x;

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
    }

    public void LiberarMovimento()
    {
        movimentoBloqueado = false;
    }
}

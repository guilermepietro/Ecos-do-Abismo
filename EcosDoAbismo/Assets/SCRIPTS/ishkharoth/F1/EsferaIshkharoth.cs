
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class EsferaIshkharoth : MonoBehaviour
{
    [Header("Movimentacao")]
    [SerializeField] private float velocidade = 7f;
    [SerializeField] private float tempoVida = 6f;

    [Header("Crescimento")]
    [SerializeField] private float escalaInicial = 0.25f;
    [SerializeField] private float distanciaAntesCrescer = 0.6f;
    [SerializeField] private float distanciaCrescimento = 2f;

    [Header("Seguranca do Ishkharoth")]
    [SerializeField] private float distanciaMinimaLiberacao = 1.5f;
    [SerializeField] private float margemSeguranca = 0.3f;

    [Header("Colisao")]
    [SerializeField] private LayerMask camadasObstaculos;

    private Rigidbody2D corpo;
    private CircleCollider2D colisor;

    private IshkharothIA ia;
    private Collider2D colisorIshkharoth;

    private Vector2 direcao;
    private Vector3 escalaOriginal;

    private float escalaFinal = 1f;
    private float distanciaPercorrida;
    private int dano;

    private bool inicializada;
    private bool afastamentoNotificado;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();
        colisor = GetComponent<CircleCollider2D>();

        corpo.bodyType = RigidbodyType2D.Kinematic;
        corpo.gravityScale = 0f;
        corpo.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        colisor.isTrigger = true;

        escalaOriginal = transform.localScale;
    }

    public void Inicializar(
        Vector2 novaDirecao,
        int novoDano,
        float novaEscalaFinal,
        IshkharothIA novaIA,
        Collider2D colisorBoss)
    {
        direcao = novaDirecao.sqrMagnitude > 0.001f
            ? novaDirecao.normalized
            : Vector2.left;

        dano = novoDano;
        escalaFinal = Mathf.Max(escalaInicial, novaEscalaFinal);

        ia = novaIA;
        colisorIshkharoth = colisorBoss;

        distanciaPercorrida = 0f;
        afastamentoNotificado = false;

        transform.localScale = escalaOriginal * escalaInicial;

        inicializada = true;

        Destroy(gameObject, tempoVida);
    }

    private void FixedUpdate()
    {
        if (!inicializada)
            return;

        float deslocamento = velocidade * Time.fixedDeltaTime;

        corpo.MovePosition(
            corpo.position + direcao * deslocamento
        );

        distanciaPercorrida += deslocamento;

        AtualizarCrescimento();
        VerificarAfastamento();
    }

    private void AtualizarCrescimento()
    {
        float progresso = Mathf.InverseLerp(
            distanciaAntesCrescer,
            distanciaAntesCrescer +
                Mathf.Max(0.01f, distanciaCrescimento),
            distanciaPercorrida
        );

        float escalaAtual = Mathf.SmoothStep(
            escalaInicial,
            escalaFinal,
            progresso
        );

        transform.localScale = escalaOriginal * escalaAtual;
    }

    private void VerificarAfastamento()
    {
        if (afastamentoNotificado)
            return;

        if (distanciaPercorrida < distanciaMinimaLiberacao)
            return;

        if (colisorIshkharoth != null)
        {
            Vector2 posicao = corpo.position;

            Vector2 pontoProximo =
                colisorIshkharoth.ClosestPoint(posicao);

            float distanciaCorpo = Vector2.Distance(
                pontoProximo,
                posicao
            );

            float raioEsfera = colisor.radius *
                Mathf.Max(
                    Mathf.Abs(transform.lossyScale.x),
                    Mathf.Abs(transform.lossyScale.y)
                );

            if (distanciaCorpo < raioEsfera + margemSeguranca)
                return;
        }

        NotificarAfastamento();
    }

    private void NotificarAfastamento()
    {
        if (afastamentoNotificado)
            return;

        afastamentoNotificado = true;

        if (ia != null)
            ia.EsferaAfastou();
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (!inicializada)
            return;

        if (outro.GetComponentInParent<IshkharothMovimento>() != null)
            return;

        ElianVida vidaElian =
            outro.GetComponentInParent<ElianVida>();

        if (vidaElian != null)
        {
            vidaElian.ReceberDano(dano);
            NotificarAfastamento();
            Destroy(gameObject);
            return;
        }

        if (((1 << outro.gameObject.layer) &
            camadasObstaculos.value) != 0)
        {
            NotificarAfastamento();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (inicializada)
            NotificarAfastamento();
    }
}

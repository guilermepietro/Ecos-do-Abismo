
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ProjetilIshkharothFase2 : MonoBehaviour
{
    [Header("Movimentacao")]
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private float distanciaMaxima = 12f;
    [SerializeField] private float tempoMaximo = 5f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private bool spriteOriginalOlhaParaEsquerda = true;

    [Header("Colisao")]
    [SerializeField] private LayerMask camadasObstaculos;

    private Rigidbody2D corpo;
    private Vector2 direcao;
    private float distanciaPercorrida;
    private int dano;

    private bool inicializado;
    private bool destruindo;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();

        if (visual == null)
            visual = GetComponent<SpriteRenderer>();

        corpo.bodyType = RigidbodyType2D.Kinematic;
        corpo.gravityScale = 0f;
        corpo.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Inicializar(int novaDirecao, int novoDano)
    {
        int lado = novaDirecao >= 0 ? 1 : -1;

        direcao = new Vector2(lado, 0f);
        dano = novoDano;

        if (visual != null)
        {
            visual.flipX = spriteOriginalOlhaParaEsquerda
                ? lado == 1
                : lado == -1;
        }

        distanciaPercorrida = 0f;
        inicializado = true;

        Destroy(gameObject, tempoMaximo);
    }

    private void FixedUpdate()
    {
        if (!inicializado || destruindo)
            return;

        float deslocamento =
            velocidade * Time.fixedDeltaTime;

        corpo.MovePosition(
            corpo.position + direcao * deslocamento
        );

        distanciaPercorrida += deslocamento;

        if (distanciaPercorrida >= distanciaMaxima)
            DestruirProjetil();
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (!inicializado || destruindo)
            return;

        // Ignora o proprio Ishkharoth.
        if (outro.GetComponentInParent<IshkharothVida>() != null)
            return;

        ElianVida vidaElian =
            outro.GetComponentInParent<ElianVida>();

        if (vidaElian != null)
        {
            destruindo = true;
            vidaElian.ReceberDano(dano);
            Destroy(gameObject);
            return;
        }

        if (((1 << outro.gameObject.layer) &
            camadasObstaculos.value) != 0)
        {
            DestruirProjetil();
        }
    }

    // Pode ser chamado por Animation Event
    // quando a animacao do projetil termina.
    public void DestruirProjetil()
    {
        if (destruindo)
            return;

        destruindo = true;
        Destroy(gameObject);
    }
}

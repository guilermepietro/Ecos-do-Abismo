
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ChamaFusao : MonoBehaviour
{
    [Header("Projetil")]
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private int dano = 20;
    [SerializeField] private float tempoVida = 5f;

    [Header("Visual")]
    [SerializeField] private bool spriteOlhaDireita = true;

    [Header("Colisao")]
    [SerializeField] private LayerMask camadasCenario;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float direcao = 1f;
    private bool atingiu;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    public void DefinirDirecao(float novaDirecao)
    {
        direcao = novaDirecao >= 0f ? 1f : -1f;

        if (spriteRenderer != null)
        {
            bool direita = direcao > 0f;
            spriteRenderer.flipX = direita != spriteOlhaDireita;
        }
    }

    private void FixedUpdate()
    {
        if (atingiu)
            return;

        rb.MovePosition(
            rb.position +
            Vector2.right * direcao * velocidade * Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (atingiu)
            return;

        ElianVida vidaElian =
            outro.GetComponentInParent<ElianVida>();

        if (vidaElian != null)
        {
            atingiu = true;
            vidaElian.ReceberDano(dano);
            Destroy(gameObject);
            return;
        }

        if ((camadasCenario.value & (1 << outro.gameObject.layer)) != 0)
        {
            atingiu = true;
            Destroy(gameObject);
        }
    }
}

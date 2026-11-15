
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PedraFusao : MonoBehaviour
{
    [Header("Queda")]
    [SerializeField] private float velocidadeQueda = 8f;
    [SerializeField] private float tempoVida = 5f;

    [Header("Dano")]
    [SerializeField] private int dano = 20;

    [Header("Colisao")]
    [SerializeField] private LayerMask camadaChao;

    private Rigidbody2D rb;
    private bool atingiu;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    private void FixedUpdate()
    {
        if (atingiu)
            return;

        rb.MovePosition(
            rb.position +
            Vector2.down *
            velocidadeQueda *
            Time.fixedDeltaTime
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

        if ((camadaChao.value &
            (1 << outro.gameObject.layer)) != 0)
        {
            atingiu = true;
            Destroy(gameObject);
        }
    }
}

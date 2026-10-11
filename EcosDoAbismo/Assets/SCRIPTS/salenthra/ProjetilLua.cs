using UnityEngine;

public class ProjetilLua : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 10f;

    [Header("Dano")]
    public int dano = 25;

    [Header("Vida")]
    public float tempoDestruir = 4f;

    [HideInInspector]
    public float direcao;

    [HideInInspector]
    public SalenthraVida vidaSalenthra;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool acertou = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (direcao > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direcao < 0)
        {
            spriteRenderer.flipX = true;
        }

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            0f
        );

        Destroy(
            gameObject,
            tempoDestruir
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (acertou)
            return;

        ElianVida vidaElian =
            collision.GetComponent<ElianVida>();

        if (vidaElian != null)
        {
            acertou = true;

            int danoFinal = dano;

            if (vidaSalenthra != null)
            {
                danoFinal =
                    vidaSalenthra.CalcularDano(
                        dano
                    );
            }

            vidaElian.ReceberDano(
                danoFinal
            );

            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class EspadaChuva : MonoBehaviour
{
    [Header("Queda")]
    public float velocidadeQueda = 8f;

    [Header("Dano")]
    public int dano = 20;

    [Header("Vida")]
    public float tempoDestruir = 4f;

    [HideInInspector]
    public ChuvaEspadas chuvaEspadas;

    [HideInInspector]
    public SalenthraVida vidaSalenthra;

    private Rigidbody2D rb;

    private bool destruindo = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.linearVelocity = Vector2.down * velocidadeQueda;

        Invoke(nameof(DestruirEspada), tempoDestruir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Chao"))
        {
            DestruirEspada();
            return;
        }

        ElianVida vidaElian = collision.gameObject.GetComponent<ElianVida>();

        if (vidaElian != null)
        {
            int danoFinal = dano;

if (vidaSalenthra != null)
{
    danoFinal = vidaSalenthra.CalcularDano(dano);
}

vidaElian.ReceberDano(danoFinal);

            DestruirEspada();
        }
    }

    private void DestruirEspada()
    {
        if (destruindo)
            return;

        destruindo = true;

        if (chuvaEspadas != null)
        {
            chuvaEspadas.EspadaDesapareceu();
        }

        Destroy(gameObject);
    }
}
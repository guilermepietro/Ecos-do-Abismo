using UnityEngine;

public class EspadaChuva : MonoBehaviour
{
    [Header("Queda")]
    public float velocidadeQueda = 8f;

    [Header("Vida")]
    public float tempoDestruir = 4f;

    [HideInInspector]
    public ChuvaEspadas chuvaEspadas;

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
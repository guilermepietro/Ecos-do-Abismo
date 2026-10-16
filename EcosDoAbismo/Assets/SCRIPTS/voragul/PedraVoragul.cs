using UnityEngine;

public class PedraVoragul : MonoBehaviour
{
    public float velocidade = 8f;
    public int dano = 25;
    public float tempoVida = 5f;

    private Vector2 direcao;
    private Rigidbody2D rb;
    private CameraFollow cameraFollow;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        cameraFollow = FindFirstObjectByType<CameraFollow>();
    }

    private void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    public void Lancar(Vector2 novaDirecao)
    {
        direcao = novaDirecao.normalized;

        rb.linearVelocity = direcao * velocidade;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ElianVida vidaElian = other.GetComponent<ElianVida>();

            if (vidaElian != null)
            {
                vidaElian.ReceberDano(dano);
            }

            Destroy(gameObject);
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Chao"))
        {
            if (cameraFollow != null)
            {
                cameraFollow.TremerCamera(0.12f, 0.08f);
            }

            Destroy(gameObject);
        }
    }
}
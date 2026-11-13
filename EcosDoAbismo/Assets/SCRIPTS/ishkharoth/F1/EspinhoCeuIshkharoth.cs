
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EspinhoCeuIshkharoth : MonoBehaviour
{
    [Header("Seguranca")]
    [SerializeField] private float tempoMaximo = 10f;

    [Header("Colisao")]
    [SerializeField] private LayerMask camadasChao;

    private Rigidbody2D corpo;

    private float velocidadeQueda;
    private int dano;

    private bool inicializado;
    private bool destruindo;

    private Action aoDestruir;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();

        corpo.bodyType = RigidbodyType2D.Kinematic;
        corpo.gravityScale = 0f;
        corpo.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Inicializar(
        int novoDano,
        float novaVelocidade,
        Action callbackDestruicao)
    {
        dano = novoDano;
        velocidadeQueda = novaVelocidade;
        aoDestruir = callbackDestruicao;

        inicializado = true;

        Destroy(gameObject, tempoMaximo);
    }

    private void FixedUpdate()
    {
        if (!inicializado || destruindo)
            return;

        Vector2 novaPosicao = corpo.position +
            Vector2.down * velocidadeQueda * Time.fixedDeltaTime;

        corpo.MovePosition(novaPosicao);
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        VerificarColisao(outro);
    }

    private void OnTriggerStay2D(Collider2D outro)
    {
        VerificarColisao(outro);
    }

    private void VerificarColisao(Collider2D outro)
    {
        if (!inicializado || destruindo || outro == null)
            return;

        if (outro.GetComponentInParent<IshkharothMovimento>() != null)
            return;

        if (outro.GetComponentInParent<EspinhoCeuIshkharoth>() != null)
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
            camadasChao.value) != 0)
        {
            destruindo = true;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (inicializado && aoDestruir != null)
        {
            aoDestruir.Invoke();
        }
    }
}

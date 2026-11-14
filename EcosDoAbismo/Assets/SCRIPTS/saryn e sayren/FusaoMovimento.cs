
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FusaoMovimento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private Animator animator;

    [Header("Movimento")]
    [SerializeField] private float velocidade = 2.5f;
    [SerializeField] private float distanciaParada = 2f;
    [SerializeField] private bool spriteOlhaDireita = true;

    private Rigidbody2D rb;
    private bool combateAtivo;
    private bool movimentoBloqueado;
    private float escalaOriginalX;

    public bool CombateAtivo => combateAtivo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        escalaOriginalX = Mathf.Abs(transform.localScale.x);

        if (elian == null)
        {
            GameObject objeto = GameObject.Find("elian");

            if (objeto != null)
                elian = objeto.transform;
        }
    }

    private void FixedUpdate()
    {
        if (!combateAtivo || movimentoBloqueado || elian == null)
        {
            Parar();
            return;
        }

        float distanciaX = elian.position.x - transform.position.x;

        OlharParaElian();

        if (Mathf.Abs(distanciaX) <= distanciaParada)
        {
            Parar();
            return;
        }

        float direcao = Mathf.Sign(distanciaX);

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );

        if (animator != null)
            animator.SetBool("Andando", true);
    }

    private void Virar(float direcao)
    {
        bool direita = direcao > 0f;

        float sinal = direita == spriteOlhaDireita ? 1f : -1f;

        Vector3 escala = transform.localScale;
        escala.x = escalaOriginalX * sinal;
        transform.localScale = escala;
    }

    public void OlharParaElian()
    {
        if (elian == null)
            return;

        float distanciaX = elian.position.x - transform.position.x;

        if (Mathf.Abs(distanciaX) > 0.01f)
            Virar(Mathf.Sign(distanciaX));
    }

    private void Parar()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (animator != null)
            animator.SetBool("Andando", false);
    }

    public void AtivarCombate()
    {
        combateAtivo = true;
    }

    public void BloquearMovimento(bool bloquear)
    {
        movimentoBloqueado = bloquear;

        if (bloquear)
            Parar();
    }

    public void DesativarCombate()
    {
        combateAtivo = false;
        Parar();
    }
}

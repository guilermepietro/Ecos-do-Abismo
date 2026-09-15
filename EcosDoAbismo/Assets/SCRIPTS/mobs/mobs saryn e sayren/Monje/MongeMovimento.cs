using UnityEngine;

public class MongeMovimento : MonoBehaviour
{
    public float distanciaDeteccao = 7f;
    public float distanciaAtaque = 3f;
    public float velocidade = 2.5f;

    private Transform elian;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public bool PodeMover { get; set; } = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject jogador = GameObject.Find("elian");

        if (jogador != null)
        {
            elian = jogador.transform;
        }
    }

    void Update()
    {
        if (elian == null)
            return;

        if (!PodeMover)
        {
            animator.SetBool("Correndo", false);
            return;
        }

        float distancia = Mathf.Abs(elian.position.x - transform.position.x);

        if (distancia > distanciaDeteccao)
        {
            animator.SetBool("Correndo", false);
            return;
        }

        if (distancia <= distanciaAtaque)
        {
            animator.SetBool("Correndo", false);
            return;
        }

        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        spriteRenderer.flipX = direcao < 0;
        animator.SetBool("Correndo", true);

        transform.position += new Vector3(
            direcao * velocidade * Time.deltaTime,
            0f,
            0f
        );
    }
}
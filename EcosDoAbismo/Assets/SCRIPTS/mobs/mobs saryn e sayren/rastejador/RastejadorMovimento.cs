using UnityEngine;

public class RastejadorMovimento : MonoBehaviour
{
    public float distanciaDeteccao = 5f;
    public float velocidade = 3f;

    private Transform elian;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool PodeMover { get; set; } = true;

    private bool ativado = false;

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
                return;

        if (!ativado)
        {
            float distancia = Mathf.Abs(elian.position.x - transform.position.x);

            if (distancia <= distanciaDeteccao)
            {
                ativado = true;
            }
        }

        if (!ativado)
        {
            animator.SetBool("Correndo", false);
            return;
        }

        float direcao = Mathf.Sign(elian.position.x - transform.position.x);

        animator.SetBool("Correndo", true);

        spriteRenderer.flipX = direcao < 0;

        transform.position += new Vector3(
            direcao * velocidade * Time.deltaTime,
            0f,
            0f
        );
    }
}
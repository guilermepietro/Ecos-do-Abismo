using UnityEngine;

public class ElianUltimate : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private SpriteRenderer spriteRenderer;

    private bool usandoUltimate = false;

    [Header("Ultimate")]
    public GameObject prefabFogoUltimate;
    public Transform pontoUltimate;

    private float posicaoXOriginalPonto;

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (pontoUltimate != null)
        {
            posicaoXOriginalPonto =
                Mathf.Abs(pontoUltimate.localPosition.x);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !usandoUltimate)
        {
            IniciarUltimate();
        }
    }

    void IniciarUltimate()
    {
        usandoUltimate = true;

        movimento.podeMover = false;

        animator.SetFloat("velocidade", 0);
        animator.SetTrigger("Ultimate");
    }

    public void DispararUltimate()
    {
        if (prefabFogoUltimate == null || pontoUltimate == null)
        {
            return;
        }

        // MUDA O PONTO DE DISPARO DE ACORDO
        // COM A DIREÇÃO QUE O ELIAN ESTÁ OLHANDO
        Vector3 posicaoLocal = pontoUltimate.localPosition;

        if (spriteRenderer.flipX)
        {
            posicaoLocal.x = -posicaoXOriginalPonto;
        }
        else
        {
            posicaoLocal.x = posicaoXOriginalPonto;
        }

        pontoUltimate.localPosition = posicaoLocal;

        // CRIA O FOGO
        GameObject fogo = Instantiate(
            prefabFogoUltimate,
            pontoUltimate.position,
            Quaternion.identity
        );

        float direcao =
            spriteRenderer.flipX ? -1f : 1f;

        FogoUltimate fogoUltimate =
            fogo.GetComponent<FogoUltimate>();

        if (fogoUltimate != null)
        {
            fogoUltimate.DefinirDirecao(direcao);
        }
    }

    public void FinalizarUltimate()
    {
        usandoUltimate = false;

        movimento.podeMover = true;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ElianUltimate : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private SpriteRenderer spriteRenderer;

    private bool usandoUltimate = false;

    [Header("Ultimate")]
    public GameObject prefabFogoUltimate;
    public Transform pontoUltimate;

    [Header("Carga da Ultimate")]
    public int cargaMaxima = 100;

    private int cargaAtual = 0;

    [Header("HUD")]
    public Image barraUltimate;
    public float velocidadeBarraUltimate = 1.5f;

    private float valorVisualUltimate = 0f;

    private float posicaoXOriginalPonto;

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        cargaAtual = 0;
        valorVisualUltimate = 0f;

        if (barraUltimate != null)
        {
            barraUltimate.fillAmount = 0f;
        }

        if (pontoUltimate != null)
        {
            posicaoXOriginalPonto =
                Mathf.Abs(pontoUltimate.localPosition.x);
        }
    }

    void Update()
    {
        AtualizarBarraUltimate();

        if (Input.GetKeyDown(KeyCode.R) &&
            !usandoUltimate &&
            cargaAtual >= cargaMaxima)
        {
            IniciarUltimate();
        }
    }

    void IniciarUltimate()
    {
        usandoUltimate = true;

        cargaAtual = 0;

        movimento.podeMover = false;

        animator.SetFloat("velocidade", 0);
        animator.SetTrigger("Ultimate");

        Debug.Log("Ultimate utilizada! Carga voltou para 0.");
    }

    public void AdicionarCarga(int quantidade)
    {
        if (usandoUltimate)
        {
            return;
        }

        cargaAtual += quantidade;

        if (cargaAtual > cargaMaxima)
        {
            cargaAtual = cargaMaxima;
        }

        Debug.Log(
            "Carga da Ultimate: " +
            cargaAtual +
            "/" +
            cargaMaxima
        );

        if (cargaAtual >= cargaMaxima)
        {
            Debug.Log("ULTIMATE PRONTA!");
        }
    }

    void AtualizarBarraUltimate()
    {
        if (barraUltimate == null)
        {
            return;
        }

        float alvo =
            (float)cargaAtual / cargaMaxima;

        valorVisualUltimate = Mathf.MoveTowards(
            valorVisualUltimate,
            alvo,
            velocidadeBarraUltimate * Time.deltaTime
        );

        barraUltimate.fillAmount =
            valorVisualUltimate;
    }

    public void DispararUltimate()
    {
        if (prefabFogoUltimate == null ||
            pontoUltimate == null)
        {
            return;
        }

        Vector3 posicaoLocal =
            pontoUltimate.localPosition;

        if (spriteRenderer.flipX)
        {
            posicaoLocal.x =
                -posicaoXOriginalPonto;
        }
        else
        {
            posicaoLocal.x =
                posicaoXOriginalPonto;
        }

        pontoUltimate.localPosition =
            posicaoLocal;

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
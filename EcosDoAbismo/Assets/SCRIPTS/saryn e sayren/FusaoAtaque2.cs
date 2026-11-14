
using System.Collections;
using UnityEngine;

public class FusaoAtaque2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform elian;

    [Header("Efeitos")]
    [SerializeField] private GameObject prefabAviso;
    [SerializeField] private GameObject prefabPilar;

    [Header("Escalas dos Efeitos")]
    [SerializeField] private Vector3 escalaAviso =
        new Vector3(2.6225f, 2.6225f, 2.6225f);

    [SerializeField] private Vector3 escalaPilar =
        new Vector3(1f, 6.377214f, 1f);

    [Header("Tempos")]
    [SerializeField] private float tempoAviso = 0.8f;
    [SerializeField] private float duracaoPilar = 1.2f;

    [Header("Posicao do Pilar")]
    [SerializeField] private LayerMask camadaChao;
    [SerializeField] private float distanciaBuscaChao = 15f;
    [SerializeField] private float ajusteY = 0f;

    [Header("Ajuste do Aviso")]
    [SerializeField] private float ajusteAvisoY = 0f;

    private bool emAtaque;
    private bool avisoCriado;
    private bool animacaoTerminou;
    private bool efeitoTerminou;

    public bool EmAtaque => emAtaque;

    private void Awake()
    {
        if (movimento == null)
            movimento = GetComponent<FusaoMovimento>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (elian == null)
        {
            GameObject objeto = GameObject.Find("elian");

            if (objeto != null)
                elian = objeto.transform;
        }
    }

    public void IniciarAtaque()
    {
        if (emAtaque || movimento == null ||
            !movimento.CombateAtivo || elian == null)
            return;

        if (prefabAviso == null || prefabPilar == null)
        {
            Debug.LogWarning("Configure os prefabs do Ataque 2.");
            return;
        }

        emAtaque = true;
        avisoCriado = false;
        animacaoTerminou = false;
        efeitoTerminou = false;

        movimento.OlharParaElian();
        movimento.BloquearMovimento(true);

        if (animator != null)
            animator.SetTrigger("Ataque2");
    }

    // Animation Event quando o boss se abaixa.
    public void CriarAviso()
    {
        if (!emAtaque || avisoCriado)
            return;

        avisoCriado = true;

        Vector3 posicao = CalcularPosicaoPilar();

        StartCoroutine(ExecutarPilar(posicao));
    }

    private Vector3 CalcularPosicaoPilar()
    {
        Vector3 posicao = elian.position;

        Vector2 origem = new Vector2(
            elian.position.x,
            elian.position.y + 0.3f
        );

        RaycastHit2D contato = Physics2D.Raycast(
            origem,
            Vector2.down,
            distanciaBuscaChao,
            camadaChao
        );

        if (contato.collider != null)
        {
            posicao.y = contato.point.y;
        }
        else
        {
            Collider2D colliderElian =
                elian.GetComponent<Collider2D>();

            if (colliderElian != null)
                posicao.y = colliderElian.bounds.min.y;
            else
                Debug.LogWarning(
                    "FusaoAtaque2: Chao nao encontrado."
                );
        }

        posicao.y += ajusteY;
        posicao.z = transform.position.z;

        return posicao;
    }

    private IEnumerator ExecutarPilar(Vector3 posicao)
    {
        GameObject aviso = Instantiate(
            prefabAviso,
            posicao,
            prefabAviso.transform.rotation
        );

        aviso.transform.localScale = escalaAviso;

        SpriteRenderer spriteAviso =
            aviso.GetComponentInChildren<SpriteRenderer>();

        if (spriteAviso != null)
        {
            float diferenca =
                posicao.y - spriteAviso.bounds.min.y;

            aviso.transform.position +=
                Vector3.up * diferenca;
        }

        aviso.transform.position +=
            Vector3.up * ajusteAvisoY;

        yield return new WaitForSeconds(tempoAviso);

        if (aviso != null)
            Destroy(aviso);

        GameObject pilar = Instantiate(
            prefabPilar,
            posicao,
            Quaternion.identity
        );

        pilar.transform.localScale = escalaPilar;

        yield return new WaitForSeconds(duracaoPilar);

        if (pilar != null)
            Destroy(pilar);

        efeitoTerminou = true;
        TentarFinalizarAtaque();
    }

    // Animation Event no ultimo frame.
    public void FinalizarAtaque2()
    {
        if (!emAtaque)
            return;

        animacaoTerminou = true;
        TentarFinalizarAtaque();
    }

    private void TentarFinalizarAtaque()
    {
        if (!emAtaque || !animacaoTerminou || !efeitoTerminou)
            return;

        emAtaque = false;

        if (movimento != null)
            movimento.BloquearMovimento(false);
    }
}

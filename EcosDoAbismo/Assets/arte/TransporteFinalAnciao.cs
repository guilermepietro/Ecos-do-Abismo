
using System.Collections;
using UnityEngine;

public class TransporteFinalAnciao : MonoBehaviour
{
    [Header("Destino")]
    [SerializeField]
    private Transform pontoChegadaLuminar;

    [Header("Transicao")]
    [SerializeField]
    private TransicaoTela transicaoTela;

    [SerializeField, Min(0f)]
    private float tempoTelaEscura = 3f;

    private bool transportando = false;

    void Awake()
    {
        if (transicaoTela == null)
        {
            transicaoTela =
                FindFirstObjectByType<TransicaoTela>();
        }
    }

    public bool IniciarTransporte(ElianMovimento movimento)
    {
        if (transportando)
            return false;

        if (movimento == null ||
            pontoChegadaLuminar == null ||
            transicaoTela == null ||
            transicaoTela.imagemFade == null)
        {
            Debug.LogError(
                "TransporteFinalAnciao: " +
                "configure Elian, destino e fade."
            );

            return false;
        }

        StartCoroutine(ExecutarTransporte(movimento));
        return true;
    }

    private IEnumerator ExecutarTransporte(
        ElianMovimento movimento)
    {
        transportando = true;

        Rigidbody2D rb =
            movimento.GetComponent<Rigidbody2D>();

        movimento.podeMover = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Escurece a tela antes de mover Elian.
        yield return StartCoroutine(
            transicaoTela.Escurecer()
        );

        Vector3 destino = pontoChegadaLuminar.position;
        destino.z = movimento.transform.position.z;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = new Vector2(
                destino.x,
                destino.y
            );
        }

        movimento.transform.position = destino;

        Physics2D.SyncTransforms();

        // Reposiciona a camera enquanto a tela esta preta.
        Camera cameraPrincipal = Camera.main;

        if (cameraPrincipal != null)
        {
            Vector3 posicaoCamera =
                cameraPrincipal.transform.position;

            posicaoCamera.x = destino.x;
            posicaoCamera.y = destino.y;

            cameraPrincipal.transform.position =
                posicaoCamera;
        }

        // Permite que fisica e camera atualizem.
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Mantem a tela escura.
        yield return new WaitForSecondsRealtime(
            tempoTelaEscura
        );

        // Revela a arena.
        yield return StartCoroutine(
            transicaoTela.Clarear()
        );

        ElianVida vida =
            movimento.GetComponent<ElianVida>();

        if (vida == null || !vida.EstaMorto)
        {
            movimento.podeMover = true;
        }

        transportando = false;

        Debug.Log(
            "Elian transportado para a Dimensao Luminar!"
        );
    }
}

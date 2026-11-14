
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Teletransporte : MonoBehaviour
{
    [Header("Identificacao")]
    [SerializeField] private int meuID = 1;
    [SerializeField] private int idDestino = 2;

    [Header("Referencias")]
    [SerializeField] private Transform pontoChegada;
    [SerializeField] private TransicaoTela transicaoTela;

    [Header("Configuracao do Fade")]
    [SerializeField] [Min(0f)]
    private float tempoTelaEscura = 1f;

    private static bool teletransportando;

    private void Awake()
    {
        BoxCollider2D area = GetComponent<BoxCollider2D>();
        area.isTrigger = true;

        if (pontoChegada == null)
        {
            pontoChegada = transform.Find("PontoChegada");
        }

        if (transicaoTela == null)
        {
            transicaoTela =
                FindFirstObjectByType<TransicaoTela>();
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (teletransportando || outro.isTrigger)
            return;

        Rigidbody2D rbElian = outro.attachedRigidbody;

        if (rbElian == null ||
            rbElian.gameObject.name != "elian")
            return;

        Teletransporte destino = EncontrarDestino();

        if (destino == null)
        {
            Debug.LogError(
                $"Teletransporte {meuID}: destino {idDestino} nao encontrado!"
            );
            return;
        }

        if (destino.pontoChegada == null)
        {
            Debug.LogError(
                $"Teletransporte {idDestino}: PontoChegada nao encontrado!"
            );
            return;
        }

        if (transicaoTela == null ||
            transicaoTela.imagemFade == null)
        {
            Debug.LogError(
                "TransicaoTela nao encontrada ou sem imagem de fade!"
            );
            return;
        }

        StartCoroutine(ExecutarTeletransporte(
            rbElian,
            destino.pontoChegada
        ));
    }

    private Teletransporte EncontrarDestino()
    {
        Teletransporte[] portais =
            FindObjectsByType<Teletransporte>(
                FindObjectsSortMode.None
            );

        foreach (Teletransporte portal in portais)
        {
            if (portal != this &&
                portal.meuID == idDestino)
            {
                return portal;
            }
        }

        return null;
    }

    private IEnumerator ExecutarTeletransporte(
        Rigidbody2D rbElian,
        Transform destino
    )
    {
        teletransportando = true;

        // Escurece a tela.
        yield return StartCoroutine(
            transicaoTela.Escurecer()
        );

        // Para o movimento do Elian.
        rbElian.linearVelocity = Vector2.zero;

        // Move para o ponto de chegada.
        Vector3 novaPosicao = destino.position;
        novaPosicao.z = rbElian.transform.position.z;

        rbElian.position = novaPosicao;
        rbElian.transform.position = novaPosicao;

        Physics2D.SyncTransforms();

        // Permite que a camera atualize.
        yield return new WaitForEndOfFrame();

        // Mantem a tela completamente preta.
        yield return new WaitForSeconds(tempoTelaEscura);

        // Clareia a tela.
        yield return StartCoroutine(
            transicaoTela.Clarear()
        );

        teletransportando = false;
    }
}

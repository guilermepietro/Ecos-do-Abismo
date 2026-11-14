using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform alvo;

    [Header("Posição da Câmera")]
    public float alturaCamera = 1.5f;

    [Header("Movimento Vertical")]
    public bool travarY = true;

    [Header("Tremor da Câmera")]
    public float duracaoTremor = 0.2f;
    public float intensidadeTremor = 0.15f;

    private Vector3 deslocamentoTremor;

    private Coroutine tremorAtual;

    private float alturaTravada;

    void Start()
    {
        if (alvo != null)
        {
            alturaTravada =
                alvo.position.y + alturaCamera;
        }
    }

    void LateUpdate()
    {
        if (alvo == null)
            return;

        float posicaoY;

        if (travarY)
        {
            posicaoY = alturaTravada;
        }
        else
        {
            posicaoY =
                alvo.position.y + alturaCamera;
        }

        Vector3 novaPosicao = new Vector3(
            alvo.position.x,
            posicaoY,
            transform.position.z
        );

        transform.position =
            novaPosicao + deslocamentoTremor;
    }

    public void TremerCamera()
    {
        TremerCamera(
            duracaoTremor,
            intensidadeTremor
        );
    }

    public void TremerCamera(
        float duracao,
        float intensidade
    )
    {
        if (tremorAtual != null)
            StopCoroutine(tremorAtual);

        tremorAtual =
            StartCoroutine(
                ExecutarTremor(
                    duracao,
                    intensidade
                )
            );
    }

    private IEnumerator ExecutarTremor(
        float duracao,
        float intensidade
    )
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            deslocamentoTremor = (Vector3)(
                Random.insideUnitCircle *
                intensidade
            );

            tempo += Time.deltaTime;

            yield return null;
        }

        deslocamentoTremor = Vector3.zero;

        tremorAtual = null;
    }
}
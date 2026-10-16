using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform alvo;

    public float velocidade = 5f;

    [Header("Posição da Câmera")]
    public float alturaCamera = 1.5f;

    [Header("Tremor da Câmera")]
    public float duracaoTremor = 0.2f;
    public float intensidadeTremor = 0.15f;

    private Vector3 deslocamentoTremor;
    private Vector3 deslocamentoAnterior;

    private Coroutine tremorAtual;

    void LateUpdate()
    {
        if (alvo == null)
            return;

        Vector3 novaPosicao = new Vector3(
            alvo.position.x,
            alvo.position.y + alturaCamera,
            transform.position.z
        );

        Vector3 posicaoBase =
            transform.position - deslocamentoAnterior;

        Vector3 posicaoSuave = Vector3.Lerp(
            posicaoBase,
            novaPosicao,
            velocidade * Time.deltaTime
        );

        transform.position =
            posicaoSuave + deslocamentoTremor;

        deslocamentoAnterior = deslocamentoTremor;
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
        deslocamentoAnterior = Vector3.zero;
        tremorAtual = null;
    }
}
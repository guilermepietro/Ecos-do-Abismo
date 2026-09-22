
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform alvo;

    public float velocidade = 5f;

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
            alvo.position.y,
            transform.position.z
        );

        // Remove o deslocamento do tremor anterior.
        Vector3 posicaoBase = transform.position - deslocamentoAnterior;

        // Mantém o acompanhamento normal do Elian.
        Vector3 posicaoSuave = Vector3.Lerp(
            posicaoBase,
            novaPosicao,
            velocidade * Time.deltaTime
        );

        // Adiciona o tremor sem interferir no acompanhamento.
        transform.position = posicaoSuave + deslocamentoTremor;

        deslocamentoAnterior = deslocamentoTremor;
    }

    public void TremerCamera()
    {
        if (tremorAtual != null)
            StopCoroutine(tremorAtual);

        tremorAtual = StartCoroutine(ExecutarTremor());
    }

    private IEnumerator ExecutarTremor()
    {
        float tempo = 0f;

        while (tempo < duracaoTremor)
        {
            deslocamentoTremor = (Vector3)(
                Random.insideUnitCircle * intensidadeTremor
            );

            tempo += Time.deltaTime;

            yield return null;
        }

        deslocamentoTremor = Vector3.zero;
        tremorAtual = null;
    }
}
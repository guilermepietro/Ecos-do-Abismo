
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform alvo;

    [Header("Posição da Câmera")]
    public float alturaCamera = 1.5f;

    [Header("Movimento Horizontal")]
    public float suavizacaoHorizontal = 0.15f;
    public float antecipacaoHorizontal = 1.2f;
    public float suavizacaoAntecipacao = 0.25f;

    [Header("Movimento Vertical")]
    public bool travarY = false;
    public float zonaMortaVertical = 1.5f;
    public float suavizacaoVertical = 0.3f;

    [Header("Tremor da Câmera")]
    public float duracaoTremor = 0.2f;
    public float intensidadeTremor = 0.15f;

    private Vector3 deslocamentoTremor;
    private Coroutine tremorAtual;

    private float alturaTravada;
    private float posicaoHorizontal;
    private float posicaoVertical;
    private float velocidadeHorizontal;
    private float velocidadeVertical;
    private float antecipacaoAtual;
    private float velocidadeAntecipacao;
    private float ultimaPosicaoAlvoX;
    private float direcaoHorizontal;

    void Start()
    {
        if (alvo == null)
            return;

        alturaTravada = alvo.position.y + alturaCamera;

        posicaoHorizontal = alvo.position.x;
        posicaoVertical = alturaTravada;
        ultimaPosicaoAlvoX = alvo.position.x;

        transform.position = new Vector3(
            posicaoHorizontal,
            posicaoVertical,
            transform.position.z
        );
    }

    void LateUpdate()
    {
        if (alvo == null)
            return;

        // Antecipação horizontal
        float movimentoX = alvo.position.x - ultimaPosicaoAlvoX;

        if (Mathf.Abs(movimentoX) > 0.001f)
            direcaoHorizontal = Mathf.Sign(movimentoX);

        float antecipacaoDesejada =
            direcaoHorizontal * antecipacaoHorizontal;

        antecipacaoAtual = Mathf.SmoothDamp(
            antecipacaoAtual,
            antecipacaoDesejada,
            ref velocidadeAntecipacao,
            Mathf.Max(0.01f, suavizacaoAntecipacao)
        );

        float destinoHorizontal =
            alvo.position.x + antecipacaoAtual;

        posicaoHorizontal = Mathf.SmoothDamp(
            posicaoHorizontal,
            destinoHorizontal,
            ref velocidadeHorizontal,
            Mathf.Max(0.01f, suavizacaoHorizontal)
        );

        ultimaPosicaoAlvoX = alvo.position.x;

        // Zona morta vertical
        if (travarY)
        {
            posicaoVertical = alturaTravada;
            velocidadeVertical = 0f;
        }
        else
        {
            float centroVertical = posicaoVertical - alturaCamera;
            float metadeZona = Mathf.Max(0f, zonaMortaVertical) / 2f;
            float diferencaVertical = alvo.position.y - centroVertical;

            float destinoVertical = posicaoVertical;

            if (diferencaVertical > metadeZona)
            {
                destinoVertical =
                    alvo.position.y + alturaCamera - metadeZona;
            }
            else if (diferencaVertical < -metadeZona)
            {
                destinoVertical =
                    alvo.position.y + alturaCamera + metadeZona;
            }

            posicaoVertical = Mathf.SmoothDamp(
                posicaoVertical,
                destinoVertical,
                ref velocidadeVertical,
                Mathf.Max(0.01f, suavizacaoVertical)
            );
        }

        Vector3 novaPosicao = new Vector3(
            posicaoHorizontal,
            posicaoVertical,
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

    public void TremerCamera(float duracao, float intensidade)
    {
        if (tremorAtual != null)
            StopCoroutine(tremorAtual);

        tremorAtual = StartCoroutine(
            ExecutarTremor(duracao, intensidade)
        );
    }

    private IEnumerator ExecutarTremor(float duracao, float intensidade)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            deslocamentoTremor = (Vector3)(
                Random.insideUnitCircle * intensidade
            );

            tempo += Time.deltaTime;

            yield return null;
        }

        deslocamentoTremor = Vector3.zero;
        tremorAtual = null;
    }
}

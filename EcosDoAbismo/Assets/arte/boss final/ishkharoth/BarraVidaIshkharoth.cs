
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BarraVidaIshkharoth : MonoBehaviour
{
    [Header("Referencias")]
    public IshkharothVida ishkharothVida;
    public Image barraVida;

    [Header("Aparencia das fases")]
    public Sprite spriteFase1;
    public Sprite spriteFase2;

    [Header("Animacao do HUD")]
    public float velocidadeBarraVida = 1.5f;
    public float velocidadeAparicao = 3f;
    public float tempoRecargaFase2 = 1.5f;
    public float tempoAntesDeSumir = 0.6f;

    private CanvasGroup grupo;
    private float valorVisualVida = 1f;
    private float tempoDepoisDeZerar;

    private enum EstadoBarra
    {
        Oculta,
        Fase1,
        EsvaziandoFase1,
        RecarregandoFase2,
        Fase2,
        Desaparecendo
    }

    private EstadoBarra estado = EstadoBarra.Oculta;

    private void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        grupo.alpha = 0f;
        grupo.interactable = false;
        grupo.blocksRaycasts = false;

        if (barraVida != null)
        {
            barraVida.type = Image.Type.Filled;
            barraVida.fillMethod = Image.FillMethod.Horizontal;
            barraVida.fillOrigin =
                (int)Image.OriginHorizontal.Left;
            barraVida.fillAmount = 1f;

            if (spriteFase1 != null)
                barraVida.sprite = spriteFase1;
        }
    }

    private void Update()
    {
        if (estado == EstadoBarra.Oculta ||
            ishkharothVida == null || barraVida == null)
            return;

        switch (estado)
        {
            case EstadoBarra.Fase1:
                if (ishkharothVida.EmTransicao ||
                    ishkharothVida.PorcentagemVida <= 0f)
                {
                    estado = EstadoBarra.EsvaziandoFase1;
                }
                else
                {
                    valorVisualVida = Mathf.MoveTowards(
                        valorVisualVida,
                        Mathf.Clamp01(
                            ishkharothVida.PorcentagemVida
                        ),
                        velocidadeBarraVida * Time.deltaTime
                    );
                }
                break;

            case EstadoBarra.EsvaziandoFase1:
                valorVisualVida = Mathf.MoveTowards(
                    valorVisualVida,
                    0f,
                    velocidadeBarraVida * Time.deltaTime
                );

                if (valorVisualVida <= 0.001f)
                {
                    valorVisualVida = 0f;

                    if (spriteFase2 != null)
                        barraVida.sprite = spriteFase2;

                    estado = EstadoBarra.RecarregandoFase2;
                }
                break;

            case EstadoBarra.RecarregandoFase2:
                if (ishkharothVida.Morto)
                {
                    estado = EstadoBarra.Fase2;
                    break;
                }

                float alvoRecarga =
                    ishkharothVida.Fase2Iniciada
                    ? Mathf.Clamp01(
                        ishkharothVida.PorcentagemVidaFase2
                    )
                    : 0.95f;

                valorVisualVida = Mathf.MoveTowards(
                    valorVisualVida,
                    alvoRecarga,
                    Time.deltaTime /
                    Mathf.Max(0.05f, tempoRecargaFase2)
                );

                if (ishkharothVida.Fase2Iniciada &&
                    Mathf.Abs(valorVisualVida - alvoRecarga)
                    <= 0.001f)
                {
                    estado = EstadoBarra.Fase2;
                }
                break;

            case EstadoBarra.Fase2:
                valorVisualVida = Mathf.MoveTowards(
                    valorVisualVida,
                    Mathf.Clamp01(
                        ishkharothVida.PorcentagemVidaFase2
                    ),
                    velocidadeBarraVida * Time.deltaTime
                );

                if (ishkharothVida.Morto &&
                    valorVisualVida <= 0.001f)
                {
                    tempoDepoisDeZerar += Time.deltaTime;

                    if (tempoDepoisDeZerar >=
                        tempoAntesDeSumir)
                    {
                        estado = EstadoBarra.Desaparecendo;
                    }
                }
                break;
        }

        barraVida.fillAmount =
            Mathf.Clamp01(valorVisualVida);

        grupo.alpha = Mathf.MoveTowards(
            grupo.alpha,
            estado == EstadoBarra.Desaparecendo
                ? 0f : 1f,
            velocidadeAparicao * Time.deltaTime
        );

        if (estado == EstadoBarra.Desaparecendo &&
            grupo.alpha <= 0.001f)
        {
            estado = EstadoBarra.Oculta;
        }
    }

    public void Exibir()
    {
        if (estado != EstadoBarra.Oculta ||
            ishkharothVida == null ||
            !ishkharothVida.gameObject.activeInHierarchy ||
            !ishkharothVida.EstaNaFase1 ||
            barraVida == null)
        {
            return;
        }

        if (spriteFase1 != null)
            barraVida.sprite = spriteFase1;

        valorVisualVida = Mathf.Clamp01(
            ishkharothVida.PorcentagemVida
        );

        barraVida.fillAmount = valorVisualVida;
        tempoDepoisDeZerar = 0f;
        grupo.alpha = 0f;
        estado = EstadoBarra.Fase1;
    }
}


using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BarraVidaVoragul : MonoBehaviour
{
    [Header("Referencias")]
    public VoragulVida voragulVida;
    public Image barraVida;

    [Header("Animacao do HUD")]
    public float velocidadeBarraVida = 1.5f;
    public float velocidadeAparicao = 3f;
    public float tempoAntesDeSumir = 0.6f;

    private CanvasGroup grupo;
    private float valorVisualVida = 1f;
    private float tempoDepoisDeZerar = 0f;
    private bool exibindo = false;
    private bool desaparecendo = false;

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
        }
    }

    private void Update()
    {
        if (!exibindo || voragulVida == null ||
            barraVida == null)
            return;

        float alvo = Mathf.Clamp01(
            voragulVida.ObterPorcentagemVida()
        );

        valorVisualVida = Mathf.MoveTowards(
            valorVisualVida,
            alvo,
            velocidadeBarraVida * Time.deltaTime
        );

        barraVida.fillAmount = valorVisualVida;

        if (voragulVida.EstaMorto() &&
            valorVisualVida <= 0.001f)
        {
            tempoDepoisDeZerar += Time.deltaTime;

            if (tempoDepoisDeZerar >= tempoAntesDeSumir)
                desaparecendo = true;
        }

        grupo.alpha = Mathf.MoveTowards(
            grupo.alpha,
            desaparecendo ? 0f : 1f,
            velocidadeAparicao * Time.deltaTime
        );

        if (desaparecendo && grupo.alpha <= 0.001f)
            exibindo = false;
    }

    public void Exibir()
    {
        if (voragulVida == null ||
            !voragulVida.gameObject.activeInHierarchy ||
            voragulVida.EstaMorto() ||
            barraVida == null)
        {
            return;
        }

        valorVisualVida = Mathf.Clamp01(
            voragulVida.ObterPorcentagemVida()
        );

        barraVida.fillAmount = valorVisualVida;

        tempoDepoisDeZerar = 0f;
        desaparecendo = false;
        exibindo = true;
        grupo.alpha = 0f;
    }
}

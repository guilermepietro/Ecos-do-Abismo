
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElianMovimento))]
[RequireComponent(typeof(ElianBencaos))]
public class DanoDashFlamejante : MonoBehaviour
{
    private ElianMovimento movimento;
    private ElianBencaos bencaos;
    private Collider2D colisorElian;
    private Rigidbody2D corpo;

    private bool acompanhandoDash;
    private Vector2 posicaoAnterior;

    private readonly HashSet<Component> atingidos =
        new HashSet<Component>();

    void Awake()
    {
        movimento = GetComponent<ElianMovimento>();
        bencaos = GetComponent<ElianBencaos>();
        colisorElian = GetComponent<Collider2D>();
        corpo = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (movimento == null ||
            bencaos == null ||
            colisorElian == null)
        {
            return;
        }

        Vector2 posicaoAtual = corpo != null
            ? corpo.position
            : (Vector2)transform.position;

        if (!movimento.EstaDashando ||
            !bencaos.PossuiInvestidaFlamejante)
        {
            acompanhandoDash = false;
            atingidos.Clear();
            return;
        }

        if (!acompanhandoDash)
        {
            acompanhandoDash = true;
            atingidos.Clear();
            posicaoAnterior = posicaoAtual;
        }

        // Detecta os inimigos no caminho do dash
        Vector2 deslocamento =
            posicaoAtual - posicaoAnterior;

        Bounds limites = colisorElian.bounds;

        Vector2 tamanho = limites.size;
        Vector2 centro = limites.center;

        Vector2 centroAnterior =
            centro - deslocamento;

        Vector2 centroVarredura =
            (centro + centroAnterior) * 0.5f;

        Vector2 tamanhoVarredura = new Vector2(
            tamanho.x + Mathf.Abs(deslocamento.x),
            tamanho.y + Mathf.Abs(deslocamento.y)
        );

        Collider2D[] encontrados =
            Physics2D.OverlapBoxAll(
                centroVarredura,
                tamanhoVarredura,
                0f
            );

        foreach (Collider2D encontrado in encontrados)
        {
            if (encontrado == null ||
                encontrado == colisorElian ||
                encontrado.transform.IsChildOf(transform))
            {
                continue;
            }

            bool inimigo =
                encontrado.CompareTag("Inimigo") ||
                encontrado.transform.root.CompareTag("Inimigo");

            if (!inimigo)
                continue;

            AplicarDano(encontrado);
        }

        posicaoAnterior = posicaoAtual;
    }

    private void AplicarDano(Collider2D colisor)
    {
        int dano = bencaos.DanoInvestidaFlamejante;

        Component alvo = null;

        IshkharothVida ishkharoth =
            colisor.GetComponentInParent<IshkharothVida>();

        FusaoVida fusao =
            colisor.GetComponentInParent<FusaoVida>();

        if (ishkharoth != null)
        {
            if (!ishkharoth.PodeReceberDano)
                return;

            alvo = ishkharoth;
        }
        else if (fusao != null)
        {
            if (!fusao.PodeReceberDano)
                return;

            alvo = fusao;
        }
        else
        {
            alvo = colisor.GetComponentInParent<VidaInimigo>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<VoragulVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<RakthasVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<SalenthraVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<CabecaDeChamaVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<EsqueleticoVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<GosmaHumanoideVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<CerberoVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<MorcegoVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<GolemVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<EstatuaVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<RestosVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<CarcacaVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<RastejadorVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<MongeVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<SombraVida>();

            if (alvo == null)
                alvo = colisor.GetComponentInParent<CriaturaVida>();
        }

        // Cada inimigo recebe dano uma vez por dash
        if (alvo == null || !atingidos.Add(alvo))
            return;

        if (alvo is IshkharothVida)
            ((IshkharothVida)alvo).ReceberDano(dano);

        else if (alvo is FusaoVida)
            ((FusaoVida)alvo).ReceberDano(dano);

        else if (alvo is VidaInimigo)
            ((VidaInimigo)alvo).ReceberDano(dano);

        else if (alvo is VoragulVida)
            ((VoragulVida)alvo).ReceberDano(dano);

        else if (alvo is RakthasVida)
            ((RakthasVida)alvo).ReceberDano(dano);

        else if (alvo is SalenthraVida)
            ((SalenthraVida)alvo).ReceberDano(dano);

        else if (alvo is CabecaDeChamaVida)
            ((CabecaDeChamaVida)alvo).ReceberDano(dano);

        else if (alvo is EsqueleticoVida)
            ((EsqueleticoVida)alvo).ReceberDano(dano);

        else if (alvo is GosmaHumanoideVida)
            ((GosmaHumanoideVida)alvo).ReceberDano(dano);

        else if (alvo is CerberoVida)
            ((CerberoVida)alvo).ReceberDano(dano);

        else if (alvo is MorcegoVida)
            ((MorcegoVida)alvo).ReceberDano(dano);

        else if (alvo is GolemVida)
            ((GolemVida)alvo).ReceberDano(dano);

        else if (alvo is EstatuaVida)
            ((EstatuaVida)alvo).ReceberDano(dano);

        else if (alvo is RestosVida)
            ((RestosVida)alvo).ReceberDano(dano);

        else if (alvo is CarcacaVida)
            ((CarcacaVida)alvo).ReceberDano(dano);

        else if (alvo is RastejadorVida)
            ((RastejadorVida)alvo).ReceberDano(dano);

        else if (alvo is MongeVida)
            ((MongeVida)alvo).ReceberDano(dano);

        else if (alvo is SombraVida)
            ((SombraVida)alvo).ReceberDano(dano);

        else if (alvo is CriaturaVida)
            ((CriaturaVida)alvo).ReceberDano(dano);
    }
}

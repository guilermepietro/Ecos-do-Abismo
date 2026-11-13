
using UnityEngine;
using System.Collections.Generic;

public class HitboxAtaque : MonoBehaviour
{
    public int danoAtual = 20;

    private BoxCollider2D boxCollider;
    private List<GameObject> inimigosAtingidos = new List<GameObject>();

    public float forcaKnockback = 3f;

    private ElianUltimate ultimate;

    private ElianAtaque elianAtaque;
    private bool somImpactoReproduzido = false;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        ultimate = GetComponentInParent<ElianUltimate>();

        elianAtaque = GetComponentInParent<ElianAtaque>();
    }

    public void LimparInimigosAtingidos()
    {
        inimigosAtingidos.Clear();

        somImpactoReproduzido = false;
    }

    public void DefinirDano(int dano)
    {
        danoAtual = dano;
    }

    public void VerificarAcerto()
    {
        Collider2D[] inimigos = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f
        );

        foreach (Collider2D inimigo in inimigos)
        {
            if (inimigo.CompareTag("Inimigo"))
            {
                if (!inimigosAtingidos.Contains(inimigo.gameObject))
                {
                    inimigosAtingidos.Add(inimigo.gameObject);

                    bool acertouInimigo = false;

                    VidaInimigo vida = inimigo.GetComponent<VidaInimigo>();

                    if (vida != null)
                    {
                        vida.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    VoragulVida vidaVoragul = inimigo.GetComponent<VoragulVida>();

                    if (vidaVoragul != null)
                    {
                        vidaVoragul.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    RakthasVida vidaRakthas = inimigo.GetComponent<RakthasVida>();

                    if (vidaRakthas != null)
                    {
                        vidaRakthas.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    SalenthraVida vidaSalenthra = inimigo.GetComponent<SalenthraVida>();

                    if (vidaSalenthra != null)
                    {
                        vidaSalenthra.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    // Ishkharoth - Fases 1 e 2
                    IshkharothVida vidaIshkharoth =
                        inimigo.GetComponent<IshkharothVida>();

                    if (vidaIshkharoth != null &&
                        vidaIshkharoth.PodeReceberDano)
                    {
                        vidaIshkharoth.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    CabecaDeChamaVida vidaCabecaDeChama =
                        inimigo.GetComponent<CabecaDeChamaVida>();

                    if (vidaCabecaDeChama != null)
                    {
                        vidaCabecaDeChama.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    EsqueleticoVida vidaEsqueletico =
                        inimigo.GetComponent<EsqueleticoVida>();

                    if (vidaEsqueletico != null)
                    {
                        vidaEsqueletico.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    GosmaHumanoideVida vidaGosma =
                        inimigo.GetComponent<GosmaHumanoideVida>();

                    if (vidaGosma != null)
                    {
                        vidaGosma.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    CerberoVida vidaCerbero =
                        inimigo.GetComponent<CerberoVida>();

                    if (vidaCerbero != null)
                    {
                        vidaCerbero.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    MorcegoVida vidaMorcego =
                        inimigo.GetComponent<MorcegoVida>();

                    if (vidaMorcego != null)
                    {
                        vidaMorcego.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    GolemVida vidaGolem =
                        inimigo.GetComponent<GolemVida>();

                    if (vidaGolem != null)
                    {
                        vidaGolem.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    EstatuaVida vidaEstatua =
                        inimigo.GetComponent<EstatuaVida>();

                    if (vidaEstatua != null)
                    {
                        vidaEstatua.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    RestosVida vidaRestos =
                        inimigo.GetComponent<RestosVida>();

                    if (vidaRestos != null)
                    {
                        vidaRestos.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    CarcacaVida vidaCarcaca =
                        inimigo.GetComponent<CarcacaVida>();

                    if (vidaCarcaca != null)
                    {
                        vidaCarcaca.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    RastejadorVida vidaRastejador =
                        inimigo.GetComponent<RastejadorVida>();

                    if (vidaRastejador != null)
                    {
                        vidaRastejador.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    MongeVida vidaMonge =
                        inimigo.GetComponent<MongeVida>();

                    if (vidaMonge != null)
                    {
                        vidaMonge.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    SombraVida vidaSombra =
                        inimigo.GetComponent<SombraVida>();

                    if (vidaSombra != null)
                    {
                        vidaSombra.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    CriaturaVida vidaCriatura =
                        inimigo.GetComponent<CriaturaVida>();

                    if (vidaCriatura != null)
                    {
                        vidaCriatura.ReceberDano(danoAtual);
                        acertouInimigo = true;
                    }

                    // Som de impacto quando o golpe acerta
                    if (acertouInimigo &&
                        !somImpactoReproduzido &&
                        elianAtaque != null)
                    {
                        elianAtaque.TocarSomImpactoEspada();
                        somImpactoReproduzido = true;
                    }

                    // CARGA DA ULTIMATE
                    if (acertouInimigo && ultimate != null)
                    {
                        if (danoAtual == 20)
                        {
                            ultimate.AdicionarCarga(4);
                        }
                        else if (danoAtual == 30)
                        {
                            ultimate.AdicionarCarga(6);
                        }
                        else if (danoAtual == 50)
                        {
                            ultimate.AdicionarCarga(10);
                        }
                    }

                    KnockbackInimigo knockback =
                        inimigo.GetComponent<KnockbackInimigo>();

                    if (knockback != null)
                    {
                        float direcao =
                            transform.position.x < inimigo.transform.position.x
                            ? 1f
                            : -1f;

                        knockback.AplicarEmpurrao(
                            forcaKnockback,
                            direcao
                        );
                    }
                }
            }
        }
    }

    public void DefinirKnockback(float forca)
    {
        forcaKnockback = forca;
    }
}

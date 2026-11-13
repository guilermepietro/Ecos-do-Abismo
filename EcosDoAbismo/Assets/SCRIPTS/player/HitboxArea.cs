
using UnityEngine;
using System.Collections.Generic;

public class HitboxArea : MonoBehaviour
{
    public int dano = 30;

    private BoxCollider2D boxCollider;

    private List<GameObject> inimigosAtingidos =
        new List<GameObject>();

    private ElianUltimate ultimate;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        ultimate = GetComponentInParent<ElianUltimate>();
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
                if (inimigosAtingidos.Contains(inimigo.gameObject))
                {
                    continue;
                }

                inimigosAtingidos.Add(inimigo.gameObject);

                bool acertouInimigo = false;

                VidaInimigo vida = inimigo.GetComponent<VidaInimigo>();

                if (vida != null)
                {
                    vida.ReceberDano(dano);
                    acertouInimigo = true;
                }

                VoragulVida vidaVoragul =
                    inimigo.GetComponent<VoragulVida>();

                if (vidaVoragul != null)
                {
                    vidaVoragul.ReceberDano(dano);
                    acertouInimigo = true;
                }

                RakthasVida vidaRakthas =
                    inimigo.GetComponent<RakthasVida>();

                if (vidaRakthas != null)
                {
                    vidaRakthas.ReceberDano(dano);
                    acertouInimigo = true;
                }

                SalenthraVida vidaSalenthra =
                    inimigo.GetComponent<SalenthraVida>();

                if (vidaSalenthra != null)
                {
                    vidaSalenthra.ReceberDano(dano);
                    acertouInimigo = true;
                }

                // Ishkharoth - Fases 1 e 2
                IshkharothVida vidaIshkharoth =
                    inimigo.GetComponent<IshkharothVida>();

                if (vidaIshkharoth != null &&
                    vidaIshkharoth.PodeReceberDano)
                {
                    vidaIshkharoth.ReceberDano(dano);
                    acertouInimigo = true;
                }

                CabecaDeChamaVida vidaCabecaDeChama =
                    inimigo.GetComponent<CabecaDeChamaVida>();

                if (vidaCabecaDeChama != null)
                {
                    vidaCabecaDeChama.ReceberDano(dano);
                    acertouInimigo = true;
                }

                EsqueleticoVida vidaEsqueletico =
                    inimigo.GetComponent<EsqueleticoVida>();

                if (vidaEsqueletico != null)
                {
                    vidaEsqueletico.ReceberDano(dano);
                    acertouInimigo = true;
                }

                GosmaHumanoideVida vidaGosma =
                    inimigo.GetComponent<GosmaHumanoideVida>();

                if (vidaGosma != null)
                {
                    vidaGosma.ReceberDano(dano);
                    acertouInimigo = true;
                }

                CerberoVida vidaCerbero =
                    inimigo.GetComponent<CerberoVida>();

                if (vidaCerbero != null)
                {
                    vidaCerbero.ReceberDano(dano);
                    acertouInimigo = true;
                }

                MorcegoVida vidaMorcego =
                    inimigo.GetComponent<MorcegoVida>();

                if (vidaMorcego != null)
                {
                    vidaMorcego.ReceberDano(dano);
                    acertouInimigo = true;
                }

                GolemVida vidaGolem =
                    inimigo.GetComponent<GolemVida>();

                if (vidaGolem != null)
                {
                    vidaGolem.ReceberDano(dano);
                    acertouInimigo = true;
                }

                EstatuaVida estatua =
                    inimigo.GetComponent<EstatuaVida>();

                if (estatua != null)
                {
                    estatua.ReceberDano(dano);
                    acertouInimigo = true;
                }

                RestosVida vidaRestos =
                    inimigo.GetComponent<RestosVida>();

                if (vidaRestos != null)
                {
                    vidaRestos.ReceberDano(dano);
                    acertouInimigo = true;
                }

                CarcacaVida vidaCarcaca =
                    inimigo.GetComponent<CarcacaVida>();

                if (vidaCarcaca != null)
                {
                    vidaCarcaca.ReceberDano(dano);
                    acertouInimigo = true;
                }

                RastejadorVida vidaRastejador =
                    inimigo.GetComponent<RastejadorVida>();

                if (vidaRastejador != null)
                {
                    vidaRastejador.ReceberDano(dano);
                    acertouInimigo = true;
                }

                MongeVida vidaMonge =
                    inimigo.GetComponent<MongeVida>();

                if (vidaMonge != null)
                {
                    vidaMonge.ReceberDano(dano);
                    acertouInimigo = true;
                }

                SombraVida vidaSombra =
                    inimigo.GetComponent<SombraVida>();

                if (vidaSombra != null)
                {
                    vidaSombra.ReceberDano(dano);
                    acertouInimigo = true;
                }

                CriaturaVida vidaCriatura =
                    inimigo.GetComponent<CriaturaVida>();

                if (vidaCriatura != null)
                {
                    vidaCriatura.ReceberDano(dano);
                    acertouInimigo = true;
                }

                // CARGA DA ULTIMATE
                if (acertouInimigo && ultimate != null)
                {
                    ultimate.AdicionarCarga(8);
                }
            }
        }
    }

    public void AtivarHitbox()
    {
        inimigosAtingidos.Clear();

        gameObject.SetActive(true);
    }

    public void DesativarHitbox()
    {
        gameObject.SetActive(false);
    }
}

using UnityEngine;

public class HitboxArea : MonoBehaviour
{
    public int dano = 30;

    private BoxCollider2D boxCollider;
    

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
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
                VidaInimigo vida = inimigo.GetComponent<VidaInimigo>();

if (vida != null)
{
    vida.ReceberDano(dano);
}

VoragulVida vidaVoragul = inimigo.GetComponent<VoragulVida>();

if (vidaVoragul != null)
{
    vidaVoragul.ReceberDano(dano);
}

RakthasVida vidaRakthas = inimigo.GetComponent<RakthasVida>();

if (vidaRakthas != null)
{
    vidaRakthas.ReceberDano(dano);
}

SalenthraVida vidaSalenthra = inimigo.GetComponent<SalenthraVida>();

if (vidaSalenthra != null)
{
    vidaSalenthra.ReceberDano(dano);
}
CabecaDeChamaVida vidaCabecaDeChama = inimigo.GetComponent<CabecaDeChamaVida>();

if (vidaCabecaDeChama != null)
{
    vidaCabecaDeChama.ReceberDano(dano);
}
EsqueleticoVida vidaEsqueletico = inimigo.GetComponent<EsqueleticoVida>();

if (vidaEsqueletico != null)
{
    vidaEsqueletico.ReceberDano(dano);
}
GosmaHumanoideVida vidaGosma = inimigo.GetComponent<GosmaHumanoideVida>();

if (vidaGosma != null)
{
    vidaGosma.ReceberDano(dano);
}

CerberoVida vidaCerbero = inimigo.GetComponent<CerberoVida>();

if (vidaCerbero != null)
{
    vidaCerbero.ReceberDano(dano);
}

MorcegoVida vidaMorcego = inimigo.GetComponent<MorcegoVida>();

if (vidaMorcego != null)
{
    vidaMorcego.ReceberDano(dano);
}

GolemVida vidaGolem = inimigo.GetComponent<GolemVida>();

if (vidaGolem != null)
{
    vidaGolem.ReceberDano(dano);
}


EstatuaVida estatua = inimigo.GetComponent<EstatuaVida>();

if (estatua != null)
{
    estatua.ReceberDano(dano);
}

RestosVida vidaRestos = inimigo.GetComponent<RestosVida>();

if (vidaRestos != null)
{
    vidaRestos.ReceberDano(dano);
}

CarcacaVida vidaCarcaca = inimigo.GetComponent<CarcacaVida>();

if (vidaCarcaca != null)
{
    vidaCarcaca.ReceberDano(dano);
}

RastejadorVida vidaRastejador = inimigo.GetComponent<RastejadorVida>();

if (vidaRastejador != null)
{
    vidaRastejador.ReceberDano(dano);
}

MongeVida vidaMonge = inimigo.GetComponent<MongeVida>();

if (vidaMonge != null)
{
    vidaMonge.ReceberDano(dano);
}

            }
        }
    }

    

    public void AtivarHitbox()
{
    gameObject.SetActive(true);
}

public void DesativarHitbox()
{
    gameObject.SetActive(false);
}

    
}
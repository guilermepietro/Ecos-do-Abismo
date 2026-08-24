using UnityEngine;
using System.Collections.Generic;

public class HitboxAtaque : MonoBehaviour
{
    public int danoAtual = 20;

    private BoxCollider2D boxCollider;
    private List<GameObject> inimigosAtingidos = new List<GameObject>();
    public float forcaKnockback = 3f;
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void LimparInimigosAtingidos()
    {
        inimigosAtingidos.Clear();
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

                    VidaInimigo vida = inimigo.GetComponent<VidaInimigo>();

if (vida != null)
{
    vida.ReceberDano(danoAtual);
}

VoragulVida vidaVoragul = inimigo.GetComponent<VoragulVida>();

if (vidaVoragul != null)
{
    vidaVoragul.ReceberDano(danoAtual);
}


RakthasVida vidaRakthas = inimigo.GetComponent<RakthasVida>();

if (vidaRakthas != null)
{
    vidaRakthas.ReceberDano(danoAtual);
}

SalenthraVida vidaSalenthra = inimigo.GetComponent<SalenthraVida>();

if (vidaSalenthra != null)
{
    vidaSalenthra.ReceberDano(danoAtual);
}

KnockbackInimigo knockback = inimigo.GetComponent<KnockbackInimigo>();

if (knockback != null)
{
    float direcao = transform.position.x < inimigo.transform.position.x ? 1f : -1f;

    knockback.AplicarEmpurrao(forcaKnockback, direcao);
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
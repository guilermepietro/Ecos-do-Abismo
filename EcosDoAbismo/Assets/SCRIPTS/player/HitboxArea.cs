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
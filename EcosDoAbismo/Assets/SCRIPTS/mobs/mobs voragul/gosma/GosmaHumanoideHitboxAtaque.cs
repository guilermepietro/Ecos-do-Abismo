using UnityEngine;

public class GosmaHumanoideHitboxAtaque : MonoBehaviour
{
    public int dano = 20;

    private GosmaHumanoideAtaque ataque;

    void Start()
    {
        ataque = GetComponentInParent<GosmaHumanoideAtaque>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ataque == null)
            return;

        if (!ataque.EstaAtacando)
            return;

        ElianVida vida = other.GetComponentInParent<ElianVida>();

        if (vida != null && !vida.EstaMorto)
        {
            vida.ReceberDano(dano);
        }
        if (vida != null && !vida.EstaMorto)
{
    vida.ReceberDano(dano);

    ataque.PararAoAcertar();
}
    }

    
}
using UnityEngine;

public class CarcacaHitboxAtaque : MonoBehaviour
{
    public int dano = 20;

    private CarcacaAtaque ataque;

    void Start()
    {
        ataque = GetComponentInParent<CarcacaAtaque>();
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

            ataque.PararAoAcertar();
        }
    }
}
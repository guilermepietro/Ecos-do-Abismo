using UnityEngine;

public class CerberoHitboxAtaque : MonoBehaviour
{
    [Header("Dano")]
    public int dano = 20;

    private BoxCollider2D hitbox;

    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hitbox.enabled)
            return;

        if (collision.CompareTag("Player"))
        {
            ElianVida vidaElian = collision.GetComponent<ElianVida>();

            if (vidaElian != null)
                vidaElian.ReceberDano(dano);
        }
    }

    public void AtivarHitbox()
    {
        hitbox.enabled = true;
    }

    public void DesativarHitbox()
    {
        hitbox.enabled = false;
    }
}
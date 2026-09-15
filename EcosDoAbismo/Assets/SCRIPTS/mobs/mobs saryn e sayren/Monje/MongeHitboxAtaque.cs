using UnityEngine;

public class MongeHitboxAtaque : MonoBehaviour
{
    public int dano = 20;

    private MongeAtaque ataque;
    private Collider2D hitbox;

    private bool danoAtivo = false;
    private bool jaCausouDano = false;

    void Start()
    {
        ataque = GetComponentInParent<MongeAtaque>();
        hitbox = GetComponent<Collider2D>();

        hitbox.enabled = false;
    }

    public void AtivarDano()
    {
        danoAtivo = true;
        jaCausouDano = false;

        hitbox.enabled = true;
    }

    public void DesativarDano()
    {
        danoAtivo = false;

        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TentarDarDano(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TentarDarDano(other);
    }

    void TentarDarDano(Collider2D other)
    {
        if (!danoAtivo)
            return;

        if (jaCausouDano)
            return;

        if (ataque == null || !ataque.EstaAtacando)
            return;

        ElianVida vida = other.GetComponentInParent<ElianVida>();

        if (vida != null && !vida.EstaMorto)
        {
            vida.ReceberDano(dano);

            jaCausouDano = true;
        }
    }
}
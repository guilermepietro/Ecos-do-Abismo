using UnityEngine;

public class HitboxControle : MonoBehaviour
{
    public GameObject hitboxAtaque;

    public void AtivarHitbox()
    {
        HitboxAtaque hitbox = hitboxAtaque.GetComponent<HitboxAtaque>();

        hitbox.LimparInimigosAtingidos();

        hitboxAtaque.SetActive(true);
    }

    public void DanoAtaque1()
    {
        HitboxAtaque hitbox = hitboxAtaque.GetComponent<HitboxAtaque>();
        hitbox.DefinirDano(20);
    }

    public void DanoAtaque2()
    {
        HitboxAtaque hitbox = hitboxAtaque.GetComponent<HitboxAtaque>();
        hitbox.DefinirDano(30);
    }

    public void DanoAtaque3()
    {
        HitboxAtaque hitbox = hitboxAtaque.GetComponent<HitboxAtaque>();
        hitbox.DefinirDano(50);
    }

    public void VerificarAcerto()
    {
        HitboxAtaque hitbox = hitboxAtaque.GetComponent<HitboxAtaque>();

        hitbox.VerificarAcerto();
    }

    public void DesativarHitbox()
    {
        hitboxAtaque.SetActive(false);
    }
}
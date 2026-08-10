using UnityEngine;

public class HitboxControle : MonoBehaviour
{
    public GameObject hitboxAtaque;

    private Vector3 posicaoOriginal;

    void Start()
{
    posicaoOriginal = hitboxAtaque.transform.localPosition;
}


public void AtualizarDirecao(bool viradoEsquerda)
{
    Vector3 novaPosicao = hitboxAtaque.transform.localPosition;

    if (viradoEsquerda)
    {
        novaPosicao.x = -0.5f;
    }
    else
    {
        novaPosicao.x = 0.228f;
    }

    hitboxAtaque.transform.localPosition = novaPosicao;
}

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

    public void KnockbackAtaque1()
{
    HitboxAtaque hitbox = hitboxAtaque.GetComponent<HitboxAtaque>();
    hitbox.DefinirKnockback(3f);
}

public void KnockbackAtaque2()
{
    HitboxAtaque hitbox = hitboxAtaque.GetComponent<HitboxAtaque>();
    hitbox.DefinirKnockback(4f);
}

public void KnockbackAtaque3()
{
    HitboxAtaque hitbox = hitboxAtaque.GetComponent<HitboxAtaque>();
    hitbox.DefinirKnockback(10f);
}
}
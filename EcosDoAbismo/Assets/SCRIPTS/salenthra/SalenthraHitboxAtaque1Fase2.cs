using UnityEngine;

public class SalenthraHitboxAtaque1Fase2 : MonoBehaviour
{
    [Header("Dano")]
    public int dano = 25;

    private BoxCollider2D boxCollider;
    private SalenthraVida vidaSalenthra;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        vidaSalenthra = GetComponentInParent<SalenthraVida>();
    }

    public void VerificarAcerto()
    {
        Collider2D[] atingidos = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f
        );

        foreach (Collider2D atingido in atingidos)
        {
            ElianVida vidaElian = atingido.GetComponent<ElianVida>();

            if (vidaElian != null)
            {
                int danoFinal = dano;

                if (vidaSalenthra != null)
                {
                    danoFinal = vidaSalenthra.CalcularDano(dano);
                }

                vidaElian.ReceberDano(danoFinal);
            }
        }
    }
}
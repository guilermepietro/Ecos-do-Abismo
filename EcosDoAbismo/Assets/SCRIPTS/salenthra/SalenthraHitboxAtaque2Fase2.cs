using UnityEngine;
using System.Collections.Generic;

public class SalenthraHitboxAtaque2Fase2 : MonoBehaviour
{
    [Header("Dano")]
    public int dano = 30;

    private BoxCollider2D boxCollider;
    private SalenthraVida vidaSalenthra;

    private HashSet<GameObject> alvosAtingidos =
        new HashSet<GameObject>();

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        vidaSalenthra =
            GetComponentInParent<SalenthraVida>();
    }

    public void PrepararNovoDash()
    {
        alvosAtingidos.Clear();
    }

    public void VerificarAcerto()
    {
        Collider2D[] atingidos =
            Physics2D.OverlapBoxAll(
                boxCollider.bounds.center,
                boxCollider.bounds.size,
                0f
            );

        foreach (Collider2D atingido in atingidos)
        {
            ElianVida vidaElian =
                atingido.GetComponent<ElianVida>();

            if (vidaElian == null)
                continue;

            if (alvosAtingidos.Contains(
                atingido.gameObject
            ))
                continue;

            alvosAtingidos.Add(
                atingido.gameObject
            );

            int danoFinal = dano;

            if (vidaSalenthra != null)
            {
                danoFinal =
                    vidaSalenthra.CalcularDano(
                        dano
                    );
            }

            vidaElian.ReceberDano(
                danoFinal
            );
        }
    }
}
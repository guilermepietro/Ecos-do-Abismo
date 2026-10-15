using UnityEngine;
using System.Collections.Generic;

public class SalenthraRaioFase2 : MonoBehaviour
{
    [Header("Dano")]
    public int dano = 35;

    [Header("Referências")]
    public BoxCollider2D boxCollider;

    private SalenthraVida vidaSalenthra;

    private bool causandoDano = false;

    private HashSet<GameObject> alvosAtingidos =
        new HashSet<GameObject>();

    private void Awake()
    {
        vidaSalenthra =
            GetComponentInParent<SalenthraVida>();
    }

    private void FixedUpdate()
    {
        if (!causandoDano)
            return;

        VerificarAcerto();
    }

    public void ComecarDano()
    {
        alvosAtingidos.Clear();

        causandoDano = true;
    }

    public void PararDano()
    {
        causandoDano = false;
    }

    private void VerificarAcerto()
    {
        if (boxCollider == null)
            return;

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

            if (
                alvosAtingidos.Contains(
                    atingido.gameObject
                )
            )
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
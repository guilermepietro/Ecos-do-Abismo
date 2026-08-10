using UnityEngine;
using System.Collections;

public class KnockbackInimigo : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void AplicarEmpurrao(float forca, float direcao)
    {
        StopAllCoroutines();

        rb.linearVelocity = new Vector2(
            direcao * forca,
            0f
        );

        StartCoroutine(PararEmpurrao());
    }

    IEnumerator PararEmpurrao()
    {
        yield return new WaitForSeconds(0.1f);

        rb.linearVelocity = Vector2.zero;
    }
}
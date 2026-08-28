using UnityEngine;

public class RakthasRaioHitbox : MonoBehaviour
{
    public int dano = 20;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void VerificarDano()
    {
        Collider2D[] atingidos = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f
        );

        foreach (Collider2D atingido in atingidos)
        {
            if (!atingido.CompareTag("Player"))
                continue;

            ElianVida vidaElian = atingido.GetComponent<ElianVida>();

            if (vidaElian != null)
            {
                vidaElian.ReceberDano(dano);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        ElianVida vidaElian = other.GetComponent<ElianVida>();

        if (vidaElian != null)
        {
            vidaElian.ReceberDano(dano);
        }
    }
}
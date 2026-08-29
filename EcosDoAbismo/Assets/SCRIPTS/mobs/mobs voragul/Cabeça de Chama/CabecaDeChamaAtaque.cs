using UnityEngine;

public class CabecaDeChamaAtaque : MonoBehaviour
{
    public int dano = 20;

    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
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
            ElianVida vida = atingido.GetComponentInParent<ElianVida>();

            if (vida != null)
            {
                vida.ReceberDano(dano);
                return;
            }
        }
    }
    
}
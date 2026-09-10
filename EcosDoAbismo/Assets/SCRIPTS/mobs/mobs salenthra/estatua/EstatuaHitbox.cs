using UnityEngine;

public class EstatuaHitbox : MonoBehaviour
{
    public int dano = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ElianVida vida = other.GetComponent<ElianVida>();

            if (vida != null)
            {
                vida.ReceberDano(dano);
            }
        }
    }
}
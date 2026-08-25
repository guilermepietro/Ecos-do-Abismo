using UnityEngine;

public class SalenthraDanoContato : MonoBehaviour
{
    [Header("Dano")]
    public int dano = 20;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ElianVida vidaElian = collision.gameObject.GetComponent<ElianVida>();

        if (vidaElian != null)
        {
            vidaElian.ReceberDano(dano);
        }
    }
}
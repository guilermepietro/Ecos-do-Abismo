using UnityEngine;

public class SombraAtaque : MonoBehaviour
{
    [Header("Ataque")]
    public BoxCollider2D hitboxAtaque;
    public int dano = 20;

    public void DarDano()
    {
        Collider2D[] atingidos = Physics2D.OverlapBoxAll(
            hitboxAtaque.bounds.center,
            hitboxAtaque.bounds.size,
            0f
        );

        foreach (Collider2D alvo in atingidos)
        {
            if (alvo.CompareTag("Player"))
            {
                alvo.gameObject.SendMessage(
                    "ReceberDano",
                    dano,
                    SendMessageOptions.DontRequireReceiver
                );

                break;
            }
        }
    }
}
using UnityEngine;

public class RakthasContato : MonoBehaviour
{
    [Header("Dano de Contato")]
    public int danoContato = 10;
    public float cooldownContato = 1f;

    private float proximoDano = 0f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < proximoDano)
            return;

        ElianVida vidaElian = collision.gameObject.GetComponent<ElianVida>();

        if (vidaElian != null)
        {
            vidaElian.ReceberDano(danoContato);
            proximoDano = Time.time + cooldownContato;
        }
    }
}
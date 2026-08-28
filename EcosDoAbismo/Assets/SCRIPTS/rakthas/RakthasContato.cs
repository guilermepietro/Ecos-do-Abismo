using UnityEngine;

public class RakthasContato : MonoBehaviour
{
    [Header("Dano de Contato")]
    public int danoContato = 10;
    public float cooldownContato = 1f;

    private float proximoDano = 0f;
    private RakthasVida vida;

    private void Awake()
    {
        vida = GetComponent<RakthasVida>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (vida != null && vida.EstaMorto)
            return;

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
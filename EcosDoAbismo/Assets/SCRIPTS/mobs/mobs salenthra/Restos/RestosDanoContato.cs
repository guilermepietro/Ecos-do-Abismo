using UnityEngine;

public class RestosDanoContato : MonoBehaviour
{
    public int dano = 20;
    public float tempoEntreDanos = 1.5f;

    private float proximoDano;
    private bool morto;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (morto)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < proximoDano)
            return;

        ElianVida vidaElian = collision.gameObject.GetComponent<ElianVida>();

        if (vidaElian != null)
        {
            vidaElian.ReceberDano(dano);

            proximoDano = Time.time + tempoEntreDanos;
        }
    }

    public void Morrer()
    {
        morto = true;
    }
}
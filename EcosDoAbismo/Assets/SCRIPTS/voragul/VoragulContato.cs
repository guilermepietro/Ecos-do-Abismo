using UnityEngine;

public class VoragulContato : MonoBehaviour
{
    public int danoContato = 10;
    public float tempoEntreDanos = 0.8f;

    private bool podeDarDano = true;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!podeDarDano)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            ElianVida vidaElian = collision.gameObject.GetComponent<ElianVida>();

            if (vidaElian != null)
            {
                vidaElian.ReceberDano(danoContato);

                podeDarDano = false;
                Invoke(nameof(LiberarDano), tempoEntreDanos);
            }
        }
    }

    private void LiberarDano()
    {
        podeDarDano = true;
    }
}
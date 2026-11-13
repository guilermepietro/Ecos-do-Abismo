
using UnityEngine;

public class AtivadorRakthas : MonoBehaviour
{
    public RakthasMovimento rakthasMovimento;
    public BarraVidaRakthas barraVidaRakthas;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (rakthasMovimento == null ||
                !rakthasMovimento.gameObject.activeInHierarchy)
                return;

            rakthasMovimento.AtivarRakthas();

            if (barraVidaRakthas != null)
            {
                barraVidaRakthas.Exibir();
            }
        }
    }
}

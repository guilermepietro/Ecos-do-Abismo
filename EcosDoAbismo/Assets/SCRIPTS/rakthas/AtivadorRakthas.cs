using UnityEngine;

public class AtivadorRakthas : MonoBehaviour
{
    public RakthasMovimento rakthasMovimento;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            rakthasMovimento.AtivarRakthas();
        }
    }
}
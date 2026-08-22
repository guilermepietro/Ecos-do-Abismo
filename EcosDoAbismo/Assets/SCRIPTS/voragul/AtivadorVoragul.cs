using UnityEngine;

public class AtivadorVoragul : MonoBehaviour
{
    public VoragulMovimento voragul;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            voragul.lutaIniciada = true;
            gameObject.SetActive(false);
        }
    }
}
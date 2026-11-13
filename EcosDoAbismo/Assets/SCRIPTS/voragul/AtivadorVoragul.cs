
using UnityEngine;

public class AtivadorVoragul : MonoBehaviour
{
    public VoragulMovimento voragul;
    public BarraVidaVoragul barraVidaVoragul;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (voragul == null ||
                !voragul.gameObject.activeInHierarchy)
                return;

            voragul.lutaIniciada = true;

            if (barraVidaVoragul != null)
                barraVidaVoragul.Exibir();

            gameObject.SetActive(false);
        }
    }
}

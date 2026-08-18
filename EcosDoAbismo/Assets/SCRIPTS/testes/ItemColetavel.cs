using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    private bool jogadorPerto = false;
    private bool coletando = false;

    private void OnTriggerEnter2D(Collider2D outro)
{
    if (outro.CompareTag("Player"))
    {
        jogadorPerto = true;

        ElianColeta coleta = outro.GetComponent<ElianColeta>();

        if (coleta != null)
        {
            coleta.DefinirItemPerto(this);
        }

        Debug.Log("Elian está perto do item!");
    }
}

    private void OnTriggerExit2D(Collider2D outro)
{
    if (outro.CompareTag("Player"))
    {
        jogadorPerto = false;

        ElianColeta coleta = outro.GetComponent<ElianColeta>();

        if (coleta != null)
        {
            coleta.DefinirItemPerto(null);
        }

        Debug.Log("Elian saiu de perto do item!");
    }
}

    public void Coletar()
{
    if (!jogadorPerto || coletando)
    {
        return;
    }

    coletando = true;

    Debug.Log("Item coletado!");

    gameObject.SetActive(false);
}
}
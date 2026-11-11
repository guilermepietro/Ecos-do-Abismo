
using UnityEngine;

public class AreaMapa : MonoBehaviour
{
    [Header("Identificação da Área")]
    public string nomeArea = "Floresta";

    private void OnTriggerEnter2D(Collider2D outro)
    {
        ElianColeta elian =
            outro.GetComponent<ElianColeta>();

        if (elian == null)
            return;

        GerenciadorArea.areaAtual = nomeArea;

        Debug.Log("Elian entrou na área: " + nomeArea);

        // NOVO: atualiza a música da região.
        if (GerenciadorMusicas.Instancia != null)
        {
            GerenciadorMusicas.Instancia.TocarMusicaDaArea(
                nomeArea
            );
        }
    }
}

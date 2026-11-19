
using UnityEngine;

public class AtivadorVoragul : MonoBehaviour
{
    [Header("Referências do Boss")]
    public VoragulMovimento voragul;
    public BarraVidaVoragul barraVidaVoragul;

    [Header("Cutscene de Apresentação")]
    public string idCutscene = "Voragul_Apresentacao";

    private bool ativado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || ativado)
            return;

        if (voragul == null ||
            !voragul.gameObject.activeInHierarchy)
            return;

        ativado = true;

        // Tenta reproduzir a cutscene antes da luta.
        if (GerenciadorCutscenes.Instancia != null)
        {
            bool iniciou =
                GerenciadorCutscenes.Instancia.Reproduzir(
                    idCutscene,
                    IniciarLuta
                );

            if (iniciou)
            {
                // Impede que a área seja ativada novamente.
                gameObject.SetActive(false);
                return;
            }
        }

        // Segurança: se a cutscene não estiver
        // disponível, inicia a luta normalmente.
        Debug.LogWarning(
            "Cutscene do Voragul indisponível. " +
            "Iniciando combate normalmente."
        );

        IniciarLuta();

        gameObject.SetActive(false);
    }

    private void IniciarLuta()
    {
        if (voragul == null ||
            !voragul.gameObject.activeInHierarchy)
            return;

        // Libera o movimento e os ataques do boss.
        voragul.lutaIniciada = true;

        // Exibe a barra de vida.
        if (barraVidaVoragul != null)
            barraVidaVoragul.Exibir();

        Debug.Log(
            "Cutscene finalizada! Combate contra Voragul iniciado."
        );
    }
}

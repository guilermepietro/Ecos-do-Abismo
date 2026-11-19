
using UnityEngine;

public class AtivadorRakthas : MonoBehaviour
{
    [Header("Referências do Boss")]
    public RakthasMovimento rakthasMovimento;
    public BarraVidaRakthas barraVidaRakthas;

    [Header("Cutscene de Apresentação")]
    public string idCutscene = "Rakthas_Apresentacao";

    private bool ativado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || ativado)
            return;

        if (rakthasMovimento == null ||
            !rakthasMovimento.gameObject.activeInHierarchy)
            return;

        ativado = true;

        // Reproduz a apresentação antes da luta.
        if (GerenciadorCutscenes.Instancia != null)
        {
            bool iniciou =
                GerenciadorCutscenes.Instancia.Reproduzir(
                    idCutscene,
                    IniciarLuta
                );

            if (iniciou)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        // Caso a cutscene esteja indisponível.
        Debug.LogWarning(
            "Cutscene de apresentação do Rakthas " +
            "indisponível. Iniciando combate."
        );

        IniciarLuta();
        gameObject.SetActive(false);
    }

    private void IniciarLuta()
    {
        if (rakthasMovimento == null ||
            !rakthasMovimento.gameObject.activeInHierarchy)
            return;

        // Inicia o combate como antes.
        rakthasMovimento.AtivarRakthas();

        // NOVO: bloqueia teletransportes.
        Teletransporte.BloquearDuranteCombate(
            rakthasMovimento.gameObject
        );

        if (barraVidaRakthas != null)
        {
            barraVidaRakthas.Exibir();
        }

        Debug.Log(
            "Apresentação concluída! " +
            "Combate contra Rakthas iniciado."
        );
    }
}

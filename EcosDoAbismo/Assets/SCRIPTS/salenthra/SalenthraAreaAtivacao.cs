
using UnityEngine;

public class SalenthraAreaAtivacao : MonoBehaviour
{
    [Header("Referencias")]
    public SalenthraMovimento movimento;
    public BarraVidaSalenthra barraVidaSalenthra;

    [Header("Cutscene de Apresentacao")]
    public string idCutscene = "Salenthra_Apresentacao";

    private bool ativada = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ativada || movimento == null ||
            movimento.elian == null ||
            !movimento.gameObject.activeInHierarchy)
            return;

        if (other.transform != movimento.elian &&
            !other.transform.IsChildOf(movimento.elian))
            return;

        ativada = true;

        if (GerenciadorCutscenes.Instancia != null)
        {
            bool iniciou =
                GerenciadorCutscenes.Instancia.Reproduzir(
                    idCutscene,
                    IniciarCombate
                );

            if (iniciou)
                return;
        }

        Debug.LogWarning(
            "Salenthra: cutscene de apresentacao indisponivel."
        );

        IniciarCombate();
    }

    private void IniciarCombate()
    {
        if (movimento == null ||
            !movimento.gameObject.activeInHierarchy)
            return;

        movimento.Ativar();

        // NOVO: bloqueia teletransportes.
        Teletransporte.BloquearDuranteCombate(
            movimento.gameObject
        );

        if (barraVidaSalenthra != null)
            barraVidaSalenthra.Exibir();

        Debug.Log(
            "Salenthra: apresentacao concluida! " +
            "Combate da fase 1 iniciado."
        );
    }
}

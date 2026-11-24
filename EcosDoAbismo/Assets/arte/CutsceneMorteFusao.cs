using UnityEngine;

public class CutsceneMorteFusao : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoVida vidaFusao;

    [Header("Cutscene")]
    [SerializeField] private string idCutscene =
        "Fusao_Morte";

    private bool cutsceneIniciada = false;

    private void Awake()
    {
        if (vidaFusao == null)
        {
            vidaFusao =
                GetComponentInParent<FusaoVida>();
        }
    }

    // Chamado pelo Animation Event
    // no final da animacao Morte.
    public void IniciarCutsceneMorte()
    {
        if (cutsceneIniciada)
            return;

        cutsceneIniciada = true;

        if (
            GerenciadorCutscenes.Instancia != null
        )
        {
            bool iniciou =
                GerenciadorCutscenes.Instancia
                    .Reproduzir(
                        idCutscene,
                        ConcluirMorte
                    );

            if (iniciou)
                return;
        }

        Debug.LogWarning(
            "Cutscene da Fusão indisponível."
        );

        ConcluirMorte();
    }

    private void ConcluirMorte()
    {
        if (vidaFusao != null)
        {
            vidaFusao.FinalizarMorte();
        }
    }
}
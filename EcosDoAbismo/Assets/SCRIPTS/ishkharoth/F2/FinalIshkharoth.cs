
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalIshkharoth : MonoBehaviour
{
    [Header("Cutscene Final")]
    public string idCutsceneFinal = "Ishkharoth_Final";

    [Header("Cena de Finalizacao")]
    public string nomeCenaFinalizacao = "Finalizacao";

    private bool finalIniciado = false;

    public void IniciarFinalDoJogo()
    {
        if (finalIniciado)
            return;

        finalIniciado = true;

        // Mantem a finalizacao original da morte.
        IshkharothVida vida =
            GetComponent<IshkharothVida>();

        if (vida != null)
            vida.FinalizarMorteF2();

        // Reproduz a cutscene com fades exclusivos
        // e carrega a cena final ao terminar.
        if (GerenciadorCutscenes.Instancia != null)
        {
            bool iniciou =
                GerenciadorCutscenes.Instancia.ReproduzirFinal(
                    idCutsceneFinal,
                    nomeCenaFinalizacao
                );

            if (iniciou)
            {
                Debug.Log(
                    "Encerramento do Ishkharoth iniciado!"
                );

                return;
            }
        }

        Debug.LogWarning(
            "Cutscene final indisponivel. " +
            "Carregando tela de finalizacao."
        );

        CarregarTelaFinal();
    }

    private void CarregarTelaFinal()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(nomeCenaFinalizacao);
    }
}

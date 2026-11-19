
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

        IshkharothVida vida =
            GetComponent<IshkharothVida>();

        if (vida != null)
            vida.FinalizarMorteF2();

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
            "Iniciando carregamento da finalizacao."
        );

        CarregarTelaFinal();
    }

    private void CarregarTelaFinal()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Usa a tela de carregamento existente.
        GerenciadorCarregamento.proximaCena =
            nomeCenaFinalizacao;

        SceneManager.LoadScene("Carregamento");
    }
}

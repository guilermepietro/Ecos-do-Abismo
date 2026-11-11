
using UnityEngine;

public class ReliquiaColetavel : MonoBehaviour
{
    [Header("Relíquia")]
    public string idReliquia = "LaminaCeleste";

    private bool jogadorPerto = false;
    private bool coletando = false;

    // NOVO: referência ao Elian
    private ElianColeta elianPerto;

    private void Start()
    {
        VerificarSalvamento();
    }

    private void VerificarSalvamento()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
            return;

        if (dados.reliquiasObtidas == null)
            return;

        if (dados.reliquiasObtidas.Contains(idReliquia))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (!outro.CompareTag("Player"))
            return;

        jogadorPerto = true;

        ElianColeta coleta =
            outro.GetComponent<ElianColeta>();

        if (coleta != null)
        {
            coleta.DefinirReliquiaPerto(this);

            // NOVO
            elianPerto = coleta;
        }

        Debug.Log(
            "Elian está perto da relíquia: " +
            idReliquia
        );
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (!outro.CompareTag("Player"))
            return;

        jogadorPerto = false;

        ElianColeta coleta =
            outro.GetComponent<ElianColeta>();

        if (coleta != null)
        {
            coleta.DefinirReliquiaPerto(null);

            // NOVO
            if (elianPerto == coleta)
                elianPerto = null;
        }

        Debug.Log(
            "Elian saiu de perto da relíquia: " +
            idReliquia
        );
    }

    public void Coletar()
    {
        if (!jogadorPerto || coletando)
            return;

        coletando = true;

        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogWarning(
                "Relíquia coletada sem slot válido."
            );

            coletando = false;
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
        {
            Debug.LogError(
                "Não foi possível carregar o salvamento."
            );

            coletando = false;
            return;
        }

        if (dados.reliquiasObtidas == null)
        {
            Debug.LogError(
                "Lista de relíquias obtidas indisponível."
            );

            coletando = false;
            return;
        }

        if (!dados.reliquiasObtidas.Contains(idReliquia))
        {
            dados.reliquiasObtidas.Add(idReliquia);
        }

        if (ContadorTempoJogo.instancia != null)
        {
            ContadorTempoJogo.instancia
                .AtualizarTempoNosDados(dados);
        }

        if (GerenciadorSalvamento.Salvar(slot, dados))
        {
            Debug.Log(
                "Relíquia obtida: " +
                idReliquia
            );

            // NOVO: som antes de desativar a relíquia
            if (elianPerto != null)
            {
                elianPerto.TocarSomColetaReliquia();
            }

            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "Falha ao salvar a relíquia: " +
                idReliquia
            );

            coletando = false;
        }
    }
}

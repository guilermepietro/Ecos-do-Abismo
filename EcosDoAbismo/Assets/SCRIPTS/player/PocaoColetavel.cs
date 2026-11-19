
using UnityEngine;

public class PocaoColetavel : MonoBehaviour
{
    [Header("Identificação da Poção")]
    [SerializeField] private string idPocao;

    private ElianColeta coleta;
    private ElianPocoes pocoes;

    private bool coletada = false;

    private void Start()
    {
        if (string.IsNullOrWhiteSpace(idPocao))
        {
            Debug.LogError(
                "Poção sem ID configurado: " +
                gameObject.name
            );
            return;
        }

        idPocao = idPocao.Trim();

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

        if (dados.pocoesColetadas == null)
            return;

        if (dados.pocoesColetadas.Contains(idPocao))
        {
            coletada = true;

            Debug.Log(
                "Poção já coletada, removendo: " +
                idPocao
            );

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RegistrarElian(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        RegistrarElian(other);
    }

    private void RegistrarElian(Collider2D other)
    {
        if (coletada)
            return;

        ElianColeta elian =
            other.GetComponentInParent<ElianColeta>();

        if (elian == null)
            return;

        coleta = elian;
        pocoes = elian.GetComponent<ElianPocoes>();

        if (PodeColetar())
        {
            coleta.DefinirPocaoPerto(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ElianColeta elian =
            other.GetComponentInParent<ElianColeta>();

        if (elian == null || elian != coleta)
            return;

        coleta.RemoverPocaoPerto(this);

        coleta = null;
        pocoes = null;
    }

    public bool PodeColetar()
    {
        return !coletada &&
               !string.IsNullOrWhiteSpace(idPocao) &&
               pocoes != null &&
               pocoes.PodeColetar;
    }

    public void Coletar()
    {
        if (!PodeColetar())
            return;

        // Registra ID e quantidade no mesmo save
        if (pocoes.ColetarPocao(idPocao))
        {
            coletada = true;

            if (coleta != null)
            {
                coleta.RemoverPocaoPerto(this);
            }

            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (coleta != null)
        {
            coleta.RemoverPocaoPerto(this);
        }
    }
}

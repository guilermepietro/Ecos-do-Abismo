using UnityEngine;
using System.Collections.Generic;

public class AreaTutorial : MonoBehaviour
{
    [Header("Gerenciador")]
    [SerializeField] private GerenciadorTutoriais gerenciador;

    [Header("Identificacao")]
    [SerializeField] private string idTutorial;

    [Header("Conteudo do Tutorial")]
    [SerializeField] private string titulo;

    [SerializeField, TextArea(3, 8)]
    private string descricao;

    [SerializeField] private Sprite imagem1;
    [SerializeField] private Sprite imagem2;

    private bool jaExibido;

    public string Titulo => titulo;
    public string Descricao => descricao;
    public Sprite Imagem1 => imagem1;
    public Sprite Imagem2 => imagem2;

    private void Start()
    {
        VerificarSeJaFoiExibido();
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        TentarAtivarTutorial(outro);
    }

    private void OnTriggerStay2D(Collider2D outro)
    {
        TentarAtivarTutorial(outro);
    }

    private void TentarAtivarTutorial(Collider2D outro)
    {
        if (jaExibido || gerenciador == null)
            return;

        bool ehElian =
            outro.gameObject.name == "elian";

        if (!ehElian &&
            outro.attachedRigidbody != null)
        {
            ehElian =
                outro.attachedRigidbody
                    .gameObject.name == "elian";
        }

        if (!ehElian)
            return;

        ExibirTutorial();
    }

    public bool AbrirManualmente()
    {
        if (jaExibido || gerenciador == null)
            return false;

        return ExibirTutorial();
    }

    private bool ExibirTutorial()
    {
        if (jaExibido)
            return false;

        if (gerenciador.AbrirTutorial(this))
        {
            jaExibido = true;

            RegistrarTutorialExibido();

            return true;
        }

        return false;
    }

    private void VerificarSeJaFoiExibido()
    {
        if (string.IsNullOrEmpty(idTutorial))
        {
            Debug.LogWarning(
                "Tutorial '" +
                gameObject.name +
                "' esta sem ID!"
            );

            return;
        }

        int slot =
            MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
            return;

        if (dados.tutoriaisExibidos == null)
        {
            dados.tutoriaisExibidos =
                new List<string>();
        }

        jaExibido =
            dados.tutoriaisExibidos.Contains(
                idTutorial
            );
    }

    private void RegistrarTutorialExibido()
    {
        if (string.IsNullOrEmpty(idTutorial))
            return;

        int slot =
            MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogWarning(
                "Nao foi possivel salvar o tutorial '" +
                idTutorial +
                "' porque nenhum slot esta ativo."
            );

            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
        {
            Debug.LogWarning(
                "Nao foi possivel carregar o save para " +
                "registrar o tutorial '" +
                idTutorial +
                "'."
            );

            return;
        }

        if (dados.tutoriaisExibidos == null)
        {
            dados.tutoriaisExibidos =
                new List<string>();
        }

        if (!dados.tutoriaisExibidos.Contains(idTutorial))
        {
            dados.tutoriaisExibidos.Add(
                idTutorial
            );

            GerenciadorSalvamento.Salvar(
                slot,
                dados
            );

            Debug.Log(
                "Tutorial salvo: " +
                idTutorial
            );
        }
    }

    public bool JaExibido()
    {
        return jaExibido;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointArvore : MonoBehaviour
{
    [Header("Identificação do Checkpoint")]
    public string idCheckpoint = "Floresta_01";

    [Header("Animator da Árvore")]
    public Animator animator;

    private bool ativada = false;

    void Start()
    {
        CarregarEstado();
    }

    void CarregarEstado()
    {
        if (animator == null)
        {
            Debug.LogError(
                "Animator não configurado na árvore!"
            );
            return;
        }

        ativada = false;

        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            animator.Play("Checkpoint_Apagada", 0, 0f);
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados != null &&
            dados.checkpointsAtivados != null &&
            dados.checkpointsAtivados.Contains(idCheckpoint))
        {
            ativada = true;
            animator.Play("Checkpoint_Acesa", 0, 0f);
        }
        else
        {
            animator.Play("Checkpoint_Apagada", 0, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        ElianColeta elian =
            outro.GetComponent<ElianColeta>();

        if (elian != null)
        {
            elian.DefinirArvorePerto(this);

            Debug.Log("Elian próximo da árvore.");
        }
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        ElianColeta elian =
            outro.GetComponent<ElianColeta>();

        if (elian != null)
        {
            elian.DefinirArvorePerto(null);
        }
    }

    public void Interagir()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogWarning(
                "Nenhum slot selecionado. " +
                "Entre no Gameplay pelo menu de saves."
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(idCheckpoint))
        {
            Debug.LogError(
                "A árvore está sem ID de checkpoint!"
            );
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
        {
            Debug.LogError(
                "Não foi possível carregar o slot " + slot
            );
            return;
        }

        if (dados.checkpointsAtivados == null)
        {
            dados.checkpointsAtivados =
                new List<string>();
        }

        // Registra a árvore ativada
        if (!dados.checkpointsAtivados.Contains(idCheckpoint))
        {
            dados.checkpointsAtivados.Add(idCheckpoint);
        }

        // Define o checkpoint atual
        dados.idCheckpoint = idCheckpoint;

        // Registra a cena correspondente
        dados.cenaCheckpoint =
            SceneManager.GetActiveScene().name;

        // Registra a região atual de Elian
        dados.nomeArea = GerenciadorArea.areaAtual;

        // Grava o progresso antes de ativar visualmente
        if (!GerenciadorSalvamento.Salvar(slot, dados))
        {
            Debug.LogError(
                "Falha ao salvar no checkpoint " +
                idCheckpoint
            );
            return;
        }

        Debug.Log(
            "JOGO SALVO!" +
            "\nSlot: " + slot +
            "\nÁrea: " + dados.nomeArea +
            "\nCena: " + dados.cenaCheckpoint +
            "\nCheckpoint: " + dados.idCheckpoint
        );

        if (!ativada)
        {
            ativada = true;

            if (animator != null)
            {
                animator.Play("acendendo_Clip", 0, 0f);
            }

            Debug.Log("Árvore ativada!");
        }
        else
        {
            Debug.Log("Progresso salvo na árvore já ativada.");
        }
    }
}

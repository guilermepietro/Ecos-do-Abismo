
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

    private bool EhArvoreDosEcos()
    {
        return GetComponent<ArvoreDosEcos>() != null;
    }

    void Start()
    {
        CarregarEstado();
    }

    void InicializarControleComuns(DadosSalvamento dados)
    {
        if (dados.checkpointsComunsAtivos == null)
        {
            dados.checkpointsComunsAtivos =
                new List<string>();
        }

        if (dados.controleComunsInicializado)
            return;

        // Compatibilidade com salvamentos anteriores.
        // Recupera até três árvores comuns já descobertas.
        if (dados.checkpointsAtivados != null)
        {
            foreach (string id in dados.checkpointsAtivados)
            {
                if (string.IsNullOrEmpty(id))
                    continue;

                // IDs Ecos_ são reservados às árvores gigantes.
                if (id.StartsWith("Ecos_"))
                    continue;

                if (!dados.checkpointsComunsAtivos.Contains(id))
                {
                    dados.checkpointsComunsAtivos.Add(id);
                }
            }
        }

        while (dados.checkpointsComunsAtivos.Count > 3)
        {
            dados.checkpointsComunsAtivos.RemoveAt(0);
        }

        dados.controleComunsInicializado = true;
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

        if (dados == null)
        {
            animator.Play("Checkpoint_Apagada", 0, 0f);
            return;
        }

        if (EhArvoreDosEcos())
        {
            // As árvores gigantes nunca são apagadas.
            ativada =
                dados.checkpointsAtivados != null &&
                dados.checkpointsAtivados.Contains(idCheckpoint);
        }
        else
        {
            InicializarControleComuns(dados);

            ativada =
                dados.checkpointsComunsAtivos.Contains(
                    idCheckpoint
                );
        }

        if (ativada)
        {
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

        bool jaEstavaAtivada = ativada;
        bool arvoreGigante = EhArvoreDosEcos();

        if (dados.checkpointsAtivados == null)
        {
            dados.checkpointsAtivados =
                new List<string>();
        }

        if (!dados.checkpointsAtivados.Contains(idCheckpoint))
        {
            dados.checkpointsAtivados.Add(idCheckpoint);
        }

        string idArvoreApagada = "";

        if (!arvoreGigante)
        {
            InicializarControleComuns(dados);

            // Reutilizar uma árvore ativa também a torna
            // a mais recente.
            dados.checkpointsComunsAtivos.Remove(idCheckpoint);

            dados.checkpointsComunsAtivos.Add(idCheckpoint);

            // Mantém no máximo três árvores comuns ativas.
            if (dados.checkpointsComunsAtivos.Count > 3)
            {
                idArvoreApagada =
                    dados.checkpointsComunsAtivos[0];

                dados.checkpointsComunsAtivos.RemoveAt(0);
            }
        }

        dados.idCheckpoint = idCheckpoint;

        dados.cenaCheckpoint =
            SceneManager.GetActiveScene().name;

        dados.nomeArea = GerenciadorArea.areaAtual;

        if (ContadorTempoJogo.instancia != null)
        {
            ContadorTempoJogo.instancia
                .AtualizarTempoNosDados(dados);
        }

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
            Debug.Log(
                "Progresso salvo na árvore já ativada."
            );
        }

        // Apaga visualmente a árvore comum mais antiga.
        if (!string.IsNullOrEmpty(idArvoreApagada))
        {
            CheckpointArvore[] arvores =
                FindObjectsByType<CheckpointArvore>(
                    FindObjectsSortMode.None
                );

            foreach (CheckpointArvore arvore in arvores)
            {
                if (arvore.idCheckpoint == idArvoreApagada &&
                    !arvore.EhArvoreDosEcos())
                {
                    arvore.ApagarArvore();
                }
            }

            Debug.Log(
                "Limite de três checkpoints comuns atingido." +
                "\nÁrvore apagada: " + idArvoreApagada
            );
        }

        // Somente árvores gigantes abrem a viagem.
        if (arvoreGigante && jaEstavaAtivada)
        {
            ArvoreDosEcos arvoreDosEcos =
                GetComponent<ArvoreDosEcos>();

            if (arvoreDosEcos != null)
            {
                arvoreDosEcos.AbrirMenu();
            }
        }
    }

    void ApagarArvore()
    {
        ativada = false;

        if (animator != null)
        {
            animator.Play("Checkpoint_Apagada", 0, 0f);
        }
    }
}

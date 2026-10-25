
using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorRenascimento : MonoBehaviour
{
    [Header("Personagem")]
    public Transform elian;

    void Start()
    {
        CarregarPosicaoSalva();
    }

    void CarregarPosicaoSalva()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.Log(
                "Nenhum slot selecionado. " +
                "Elian permanecerá na posição inicial."
            );
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
        {
            Debug.LogWarning(
                "Não foi possível carregar o salvamento."
            );
            return;
        }

        if (string.IsNullOrEmpty(dados.idCheckpoint))
        {
            Debug.Log(
                "Partida sem checkpoint salvo. " +
                "Elian começará na posição inicial."
            );
            return;
        }

        if (dados.cenaCheckpoint !=
            SceneManager.GetActiveScene().name)
        {
            Debug.LogWarning(
                "O checkpoint salvo pertence à cena " +
                dados.cenaCheckpoint
            );
            return;
        }

        if (elian == null)
        {
            Debug.LogError(
                "Elian não configurado no GerenciadorRenascimento!"
            );
            return;
        }

        CheckpointArvore[] arvores =
            FindObjectsByType<CheckpointArvore>(
                FindObjectsSortMode.None
            );

        foreach (CheckpointArvore arvore in arvores)
        {
            if (arvore.idCheckpoint != dados.idCheckpoint)
                continue;

            Transform raizArvore = arvore.transform.parent;

            if (raizArvore == null)
            {
                Debug.LogError(
                    "O checkpoint não possui uma árvore pai."
                );
                return;
            }

            Transform ponto =
                raizArvore.Find("PontoRenascimento");

            if (ponto == null)
            {
                Debug.LogError(
                    "PontoRenascimento não encontrado em " +
                    raizArvore.name
                );
                return;
            }

            Rigidbody2D rb =
                elian.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = ponto.position;
            }

            elian.position = ponto.position;

            Physics2D.SyncTransforms();

            Debug.Log(
                "Elian carregado no checkpoint!" +
                "\nSlot: " + slot +
                "\nÁrea: " + dados.nomeArea +
                "\nCheckpoint: " + dados.idCheckpoint
            );

            return;
        }

        Debug.LogWarning(
            "Checkpoint não encontrado: " +
            dados.idCheckpoint
        );
    }
}

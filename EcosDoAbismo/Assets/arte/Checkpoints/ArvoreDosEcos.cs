
using UnityEngine;

public class ArvoreDosEcos : MonoBehaviour
{
    [Header("Interface de Viagem")]
    public GameObject telaViagem;

    private CheckpointArvore checkpointArvore;

    void Awake()
    {
        checkpointArvore = GetComponent<CheckpointArvore>();
    }

    public void AbrirMenu()
    {
        if (telaViagem == null)
        {
            Debug.LogError(
                "TelaViagem não configurada na Árvore dos Ecos!"
            );
            return;
        }

        if (checkpointArvore == null)
        {
            Debug.LogError(
                "CheckpointArvore não encontrado na Árvore dos Ecos!"
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(checkpointArvore.idCheckpoint))
        {
            Debug.LogError(
                "A Árvore dos Ecos está sem ID de checkpoint!"
            );
            return;
        }

        Debug.Log(
            "Viagem iniciada a partir da árvore: " +
            checkpointArvore.idCheckpoint
        );

        telaViagem.SetActive(true);
    }
}

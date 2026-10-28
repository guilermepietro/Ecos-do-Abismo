
using System;
using System.Collections.Generic;

[Serializable]
public class DadosSalvamento
{
    public int numeroSlot;

    public string nomeArea = "Floresta";
    public float tempoTotalJogado = 0f;

    public string cenaCheckpoint = "Gameplay";
    public string idCheckpoint = "";

    public List<string> checkpointsAtivados =
        new List<string>();

    public List<string> chefesDerrotados =
        new List<string>();

    public List<string> reliquiasObtidas =
        new List<string>();

    // Novos dados: controle das árvores comuns.
    public List<string> checkpointsComunsAtivos =
        new List<string>();

    public bool controleComunsInicializado = false;

    public DadosSalvamento(int slot)
    {
        numeroSlot = slot;
    }
}

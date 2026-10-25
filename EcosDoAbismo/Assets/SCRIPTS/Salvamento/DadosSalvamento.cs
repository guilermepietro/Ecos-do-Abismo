
using System;
using System.Collections.Generic;

[Serializable]
public class DadosSalvamento
{
    // Identificação da partida
    public int numeroSlot;
    public string nomeArea = "Floresta";
    public float tempoTotalJogado = 0f;

    // Último checkpoint salvo
    public string cenaCheckpoint = "Gameplay";
    public string idCheckpoint = "";

    // Progresso permanente
    public List<string> checkpointsAtivados = new List<string>();
    public List<string> chefesDerrotados = new List<string>();
    public List<string> reliquiasObtidas = new List<string>();

    // Construtor para uma nova partida
    public DadosSalvamento(int slot)
    {
        numeroSlot = slot;
    }
}


using System;
using System.IO;
using UnityEngine;

public static class GerenciadorSalvamento
{
    private const int totalSlots = 3;

    // Caminho do arquivo principal
    private static string CaminhoSlot(int slot)
    {
        return Path.Combine(
            Application.persistentDataPath,
            "save_" + slot + ".json"
        );
    }

    // Caminho do backup
    private static string CaminhoBackup(int slot)
    {
        return CaminhoSlot(slot) + ".bak";
    }

    // Caminho do arquivo temporário
    private static string CaminhoTemporario(int slot)
    {
        return CaminhoSlot(slot) + ".tmp";
    }

    private static bool SlotValido(int slot)
    {
        if (slot < 1 || slot > totalSlots)
        {
            Debug.LogWarning("Slot inválido: " + slot);
            return false;
        }

        return true;
    }

    // Verifica se existe um salvamento
    public static bool ExisteSalvamento(int slot)
    {
        if (!SlotValido(slot))
            return false;

        return File.Exists(CaminhoSlot(slot)) ||
               File.Exists(CaminhoBackup(slot));
    }

    // Salva os dados com proteção
    public static bool Salvar(int slot, DadosSalvamento dados)
    {
        if (!SlotValido(slot) || dados == null)
            return false;

        // Evita sobrescrever uma partida recuperável
        // quando o arquivo principal desapareceu.
        if (!File.Exists(CaminhoSlot(slot)) &&
            File.Exists(CaminhoBackup(slot)))
        {
            Debug.LogError(
                "Existe um backup no slot " + slot +
                ". Recupere a partida antes de salvar."
            );

            return false;
        }

        string caminho = CaminhoSlot(slot);
        string backup = CaminhoBackup(slot);
        string temporario = CaminhoTemporario(slot);

        try
        {
            dados.numeroSlot = slot;

            Directory.CreateDirectory(
                Application.persistentDataPath
            );

            string json = JsonUtility.ToJson(dados, true);

            // Primeiro grava em arquivo temporário
            File.WriteAllText(temporario, json);

            // Confere se os dados temporários são válidos
            DadosSalvamento verificacao =
                LerArquivo(temporario, slot);

            if (verificacao == null)
            {
                Debug.LogError(
                    "Falha na verificação do salvamento."
                );

                return false;
            }

            if (File.Exists(caminho))
            {
                // Substitui o principal e guarda o anterior
                File.Replace(temporario, caminho, backup);
            }
            else
            {
                // Primeira gravação do slot
                File.Move(temporario, caminho);
            }

            Debug.Log(
                "Jogo salvo com segurança no slot " + slot
            );

            return true;
        }
        catch (Exception erro)
        {
            Debug.LogError(
                "Erro ao salvar no slot " + slot +
                ": " + erro.Message
            );

            return false;
        }
    }

    // Lê e valida um arquivo JSON
    private static DadosSalvamento LerArquivo(
        string caminho, int slot)
    {
        try
        {
            if (!File.Exists(caminho))
                return null;

            string json = File.ReadAllText(caminho);

            DadosSalvamento dados =
                JsonUtility.FromJson<DadosSalvamento>(json);

            if (dados == null || dados.numeroSlot != slot)
                return null;

            if (dados.checkpointsAtivados == null ||
                dados.chefesDerrotados == null ||
                dados.reliquiasObtidas == null)
            {
                return null;
            }

            return dados;
        }
        catch (Exception)
        {
            return null;
        }
    }

    // Carrega o salvamento principal
    public static DadosSalvamento Carregar(int slot)
    {
        if (!SlotValido(slot))
            return null;

        string caminho = CaminhoSlot(slot);
        string backup = CaminhoBackup(slot);

        DadosSalvamento dados = LerArquivo(caminho, slot);

        if (dados != null)
        {
            Debug.Log("Salvamento carregado: slot " + slot);
            return dados;
        }

        // Tenta recuperar o backup
        DadosSalvamento dadosBackup =
            LerArquivo(backup, slot);

        if (dadosBackup != null)
        {
            Debug.LogWarning(
                "Arquivo principal indisponível ou inválido. " +
                "Recuperando backup do slot " + slot
            );

            try
            {
                File.Copy(backup, caminho, true);

                Debug.Log(
                    "Backup restaurado no slot " + slot
                );
            }
            catch (Exception erro)
            {
                Debug.LogWarning(
                    "Backup carregado, mas não foi possível " +
                    "restaurar o arquivo principal: " +
                    erro.Message
                );
            }

            return dadosBackup;
        }

        if (File.Exists(caminho) || File.Exists(backup))
        {
            Debug.LogError(
                "Não foi possível recuperar o slot " + slot
            );
        }

        return null;
    }

    // Cria uma nova partida
    public static DadosSalvamento CriarNovoJogo(int slot)
    {
        if (!SlotValido(slot))
            return null;

        if (ExisteSalvamento(slot))
        {
            Debug.LogWarning(
                "O slot " + slot + " já está ocupado."
            );

            return null;
        }

        DadosSalvamento dados =
            new DadosSalvamento(slot);

        if (!Salvar(slot, dados))
            return null;

        return dados;
    }

    // Exclui o salvamento e seus arquivos auxiliares
    public static bool Excluir(int slot)
    {
        if (!SlotValido(slot))
            return false;

        string caminho = CaminhoSlot(slot);
        string backup = CaminhoBackup(slot);
        string temporario = CaminhoTemporario(slot);

        if (!File.Exists(caminho) &&
            !File.Exists(backup) &&
            !File.Exists(temporario))
        {
            return false;
        }

        try
        {
            if (File.Exists(caminho))
                File.Delete(caminho);

            if (File.Exists(backup))
                File.Delete(backup);

            if (File.Exists(temporario))
                File.Delete(temporario);

            Debug.Log(
                "Salvamento excluído do slot " + slot
            );

            return true;
        }
        catch (Exception erro)
        {
            Debug.LogError(
                "Erro ao excluir salvamento: " +
                erro.Message
            );

            return false;
        }
    }
}

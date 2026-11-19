
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ElianPocoes : MonoBehaviour
{
    [Header("Poções")]
    [SerializeField] private int quantidadeAtual = 0;
    [SerializeField] private int quantidadeMaxima = 3;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI textoQuantidade;

    public int QuantidadeAtual => quantidadeAtual;
    public bool TemPocao => quantidadeAtual > 0;
    public bool PodeColetar => quantidadeAtual < quantidadeMaxima;

    private void Awake()
    {
        quantidadeMaxima = Mathf.Max(1, quantidadeMaxima);

        quantidadeAtual = Mathf.Clamp(
            quantidadeAtual, 0, quantidadeMaxima
        );

        AtualizarHUD();
    }

    private void Start()
    {
        CarregarQuantidade();
    }

    // Coleta normal, mantida por compatibilidade
    public bool ColetarPocao()
    {
        if (!PodeColetar)
            return false;

        quantidadeAtual++;

        AtualizarHUD();
        SalvarQuantidade();

        return true;
    }

    // Coleta com registro permanente do ID
    public bool ColetarPocao(string idPocao)
    {
        if (!PodeColetar)
            return false;

        if (string.IsNullOrWhiteSpace(idPocao))
        {
            Debug.LogWarning(
                "Poção sem ID! Coleta cancelada."
            );
            return false;
        }

        idPocao = idPocao.Trim();

        int slot = MenuSelecaoSave.slotPartidaAtual;

        // Permite testar diretamente na Unity
        // sem slot, mas sem persistencia.
        if (slot < 1 || slot > 3)
        {
            return ColetarPocao();
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
        {
            Debug.LogWarning(
                "Não foi possível carregar o salvamento."
            );
            return false;
        }

        if (dados.pocoesColetadas == null)
        {
            dados.pocoesColetadas =
                new List<string>();
        }

        // Impede coletar novamente o mesmo frasco
        if (dados.pocoesColetadas.Contains(idPocao))
        {
            Debug.Log(
                "Poção já coletada: " + idPocao
            );
            return false;
        }

        int novaQuantidade = quantidadeAtual + 1;

        // Registra quantidade e ID juntos
        dados.quantidadePocoes = novaQuantidade;
        dados.pocoesColetadas.Add(idPocao);

        // Só confirma a coleta após salvar
        if (!GerenciadorSalvamento.Salvar(slot, dados))
        {
            Debug.LogWarning(
                "Falha ao salvar a coleta da poção: " +
                idPocao
            );
            return false;
        }

        quantidadeAtual = novaQuantidade;

        AtualizarHUD();

        Debug.Log(
            "Poção coletada e salva: " + idPocao
        );

        return true;
    }

    public bool ConsumirPocao()
    {
        if (!TemPocao)
            return false;

        quantidadeAtual--;

        AtualizarHUD();
        SalvarQuantidade();

        return true;
    }

    public void DefinirQuantidade(int quantidade)
    {
        quantidadeAtual = Mathf.Clamp(
            quantidade, 0, quantidadeMaxima
        );

        AtualizarHUD();
        SalvarQuantidade();
    }

    private void CarregarQuantidade()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
            return;

        quantidadeAtual = Mathf.Clamp(
            dados.quantidadePocoes,
            0,
            quantidadeMaxima
        );

        AtualizarHUD();

        Debug.Log(
            "Poções carregadas: " + quantidadeAtual
        );
    }

    private void SalvarQuantidade()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
        {
            Debug.LogWarning(
                "Não foi possível carregar o slot " +
                "para salvar as poções."
            );
            return;
        }

        dados.quantidadePocoes = quantidadeAtual;

        if (GerenciadorSalvamento.Salvar(slot, dados))
        {
            Debug.Log(
                "Quantidade de poções salva: " +
                quantidadeAtual
            );
        }
        else
        {
            Debug.LogWarning(
                "Não foi possível salvar as poções."
            );
        }
    }

    private void AtualizarHUD()
    {
        if (textoQuantidade != null)
        {
            textoQuantidade.text =
                "× " + quantidadeAtual;
        }
    }
}

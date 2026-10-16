using UnityEngine;

public class VoragulIA : MonoBehaviour
{
    [Header("Referências")]
    public VoragulMovimento movimento;
    public VoragulVida vida;
    public VoragulAtaque1 ataque1;
    public VoragulAtaque2 ataque2;
    public VoragulAtaque3 ataque3;

    [Header("Distâncias")]
    public float distanciaCurta = 3f;
    public float distanciaMedia = 12f;

    [Header("Controle da IA")]
    public float intervaloDecisao = 0.3f;

    [Header("Movimento entre ataques")]
    public float tempoMovimentoInicio = 1.5f;
    public float tempoMovimentoMeio = 1.15f;
    public float tempoMovimentoFinal = 0.8f;

    [Header("Memória de Distância")]
    public float tempoParaAdaptar = 3f;
    public float bonusAdaptacao = 20f;

    [Header("Anti-Abuso de Distância")]
    public float tempoParaConfirmarNovaZona = 0.45f;

    [Header("Combinações Contextuais")]
    public float bonusContextual = 25f;
    public float janelaContextual = 4f;

    private float proximaDecisao = 0f;

    private int ultimoAtaque = 0;
    private int penultimoAtaque = 0;

    private bool estavaAtacando = false;

    private float tempoCurta = 0f;
    private float tempoMedia = 0f;
    private float tempoLonga = 0f;

    private int zonaAtual = 0;
    private int zonaCandidata = 0;
    private float tempoNaZonaCandidata = 0f;

    private float tempoUltimoAtaque = -100f;

    private void Awake()
    {
        if (movimento == null)
            movimento = GetComponent<VoragulMovimento>();

        if (vida == null)
            vida = GetComponent<VoragulVida>();

        if (ataque1 == null)
            ataque1 = GetComponent<VoragulAtaque1>();

        if (ataque2 == null)
            ataque2 = GetComponent<VoragulAtaque2>();

        if (ataque3 == null)
            ataque3 = GetComponent<VoragulAtaque3>();
    }

    private void Update()
    {
        if (movimento == null || movimento.elian == null)
            return;

        if (!movimento.lutaIniciada)
            return;

        if (vida != null && vida.EstaMorto())
            return;

        AtualizarZonaEstabilizada();
        AtualizarMemoriaDistancia();

        bool ataqueEmAndamento = AtaqueEmAndamento();

        if (ataqueEmAndamento)
        {
            estavaAtacando = true;
            return;
        }

        if (estavaAtacando)
        {
            estavaAtacando = false;

            proximaDecisao =
                Time.time + ObterTempoMovimentoAtual();

            return;
        }

        if (!movimento.podeMover)
            return;

        if (Time.time < proximaDecisao)
            return;

        proximaDecisao =
            Time.time + intervaloDecisao;

        EscolherAtaque();
    }

    private float ObterTempoMovimentoAtual()
    {
        if (vida == null)
            return tempoMovimentoInicio;

        float porcentagemVida =
            vida.ObterPorcentagemVida();

        if (porcentagemVida > 0.70f)
        {
            return tempoMovimentoInicio;
        }

        if (porcentagemVida > 0.35f)
        {
            return tempoMovimentoMeio;
        }

        return tempoMovimentoFinal;
    }

    private void AtualizarZonaEstabilizada()
    {
        float distancia = Mathf.Abs(
            movimento.elian.position.x - transform.position.x
        );

        int novaZona = ObterZonaPorDistancia(distancia);

        if (zonaAtual == 0)
        {
            zonaAtual = novaZona;
            zonaCandidata = novaZona;
            tempoNaZonaCandidata = 0f;
            return;
        }

        if (novaZona == zonaAtual)
        {
            zonaCandidata = zonaAtual;
            tempoNaZonaCandidata = 0f;
            return;
        }

        if (novaZona != zonaCandidata)
        {
            zonaCandidata = novaZona;
            tempoNaZonaCandidata = 0f;
            return;
        }

        tempoNaZonaCandidata += Time.deltaTime;

        if (tempoNaZonaCandidata >= tempoParaConfirmarNovaZona)
        {
            zonaAtual = zonaCandidata;
            tempoNaZonaCandidata = 0f;
        }
    }

    private int ObterZonaPorDistancia(float distancia)
    {
        if (distancia <= distanciaCurta)
            return 1;

        if (distancia <= distanciaMedia)
            return 2;

        return 3;
    }

    private void AtualizarMemoriaDistancia()
    {
        if (zonaAtual == 1)
        {
            tempoCurta += Time.deltaTime;

            tempoMedia = Mathf.Max(
                0f,
                tempoMedia - Time.deltaTime
            );

            tempoLonga = Mathf.Max(
                0f,
                tempoLonga - Time.deltaTime
            );
        }
        else if (zonaAtual == 2)
        {
            tempoMedia += Time.deltaTime;

            tempoCurta = Mathf.Max(
                0f,
                tempoCurta - Time.deltaTime
            );

            tempoLonga = Mathf.Max(
                0f,
                tempoLonga - Time.deltaTime
            );
        }
        else if (zonaAtual == 3)
        {
            tempoLonga += Time.deltaTime;

            tempoCurta = Mathf.Max(
                0f,
                tempoCurta - Time.deltaTime
            );

            tempoMedia = Mathf.Max(
                0f,
                tempoMedia - Time.deltaTime
            );
        }
    }

    private bool AtaqueEmAndamento()
    {
        if (ataque1 != null && ataque1.EstaAtacando())
            return true;

        if (ataque2 != null && ataque2.EstaAtacando())
            return true;

        if (ataque3 != null && ataque3.EstaAtacando())
            return true;

        return false;
    }

    private void EscolherAtaque()
    {
        float porcentagemVida = 1f;

        if (vida != null)
        {
            porcentagemVida =
                vida.ObterPorcentagemVida();
        }

        float pesoAtaque1 = 0f;
        float pesoAtaque2 = 0f;
        float pesoAtaque3 = 0f;

        if (porcentagemVida > 0.70f)
        {
            PesosInicio(
                ref pesoAtaque1,
                ref pesoAtaque2,
                ref pesoAtaque3
            );
        }
        else if (porcentagemVida > 0.35f)
        {
            PesosMeio(
                ref pesoAtaque1,
                ref pesoAtaque2,
                ref pesoAtaque3
            );
        }
        else
        {
            PesosFinal(
                ref pesoAtaque1,
                ref pesoAtaque2,
                ref pesoAtaque3
            );
        }

        AplicarAdaptacaoDistancia(
            ref pesoAtaque1,
            ref pesoAtaque2,
            ref pesoAtaque3
        );

        AplicarCombinacaoContextual(
            ref pesoAtaque1,
            ref pesoAtaque2,
            ref pesoAtaque3
        );

        AplicarLimitesContextuais(
            ref pesoAtaque1,
            ref pesoAtaque2,
            ref pesoAtaque3
        );

        if (ataque1 == null || !ataque1.PodeAtacar())
            pesoAtaque1 = 0f;

        if (ataque2 == null || !ataque2.PodeAtacar())
            pesoAtaque2 = 0f;

        if (ataque3 == null || !ataque3.PodeAtacar())
            pesoAtaque3 = 0f;

        AplicarMemoria(
            ref pesoAtaque1,
            ref pesoAtaque2,
            ref pesoAtaque3,
            porcentagemVida
        );

        float pesoTotal =
            pesoAtaque1 +
            pesoAtaque2 +
            pesoAtaque3;

        if (pesoTotal <= 0f)
            return;

        int ataqueEscolhido = SortearAtaque(
            pesoAtaque1,
            pesoAtaque2,
            pesoAtaque3
        );

        ExecutarAtaque(ataqueEscolhido);
    }

    private void AplicarLimitesContextuais(
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (zonaAtual == 1)
        {
            peso3 = 0f;
        }
        else if (zonaAtual == 2)
        {
            return;
        }
        else if (zonaAtual == 3)
        {
            peso1 = 0f;
        }
    }

    private void AplicarCombinacaoContextual(
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (ultimoAtaque == 0)
            return;

        if (Time.time - tempoUltimoAtaque > janelaContextual)
            return;

        if (ultimoAtaque == 1)
        {
            if (zonaAtual == 2)
            {
                peso2 += bonusContextual;
            }
            else if (zonaAtual == 3)
            {
                peso3 += bonusContextual;
            }
        }
        else if (ultimoAtaque == 2)
        {
            if (zonaAtual == 1)
            {
                peso1 += bonusContextual;
            }
            else if (zonaAtual == 3)
            {
                peso3 += bonusContextual;
            }
        }
        else if (ultimoAtaque == 3)
        {
            if (zonaAtual == 1)
            {
                peso1 += bonusContextual;
            }
            else if (zonaAtual == 2)
            {
                peso2 += bonusContextual;
            }
        }
    }

    private void PesosInicio(
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (zonaAtual == 1)
        {
            peso1 = 80f;
            peso2 = 18f;
            peso3 = 2f;
        }
        else if (zonaAtual == 2)
        {
            peso1 = 15f;
            peso2 = 75f;
            peso3 = 10f;
        }
        else
        {
            peso1 = 2f;
            peso2 = 18f;
            peso3 = 80f;
        }
    }

    private void PesosMeio(
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (zonaAtual == 1)
        {
            peso1 = 65f;
            peso2 = 28f;
            peso3 = 7f;
        }
        else if (zonaAtual == 2)
        {
            peso1 = 22f;
            peso2 = 58f;
            peso3 = 20f;
        }
        else
        {
            peso1 = 7f;
            peso2 = 28f;
            peso3 = 65f;
        }
    }

    private void PesosFinal(
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (zonaAtual == 1)
        {
            peso1 = 55f;
            peso2 = 35f;
            peso3 = 10f;
        }
        else if (zonaAtual == 2)
        {
            peso1 = 27f;
            peso2 = 46f;
            peso3 = 27f;
        }
        else
        {
            peso1 = 10f;
            peso2 = 35f;
            peso3 = 55f;
        }
    }

    private void AplicarAdaptacaoDistancia(
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (tempoCurta >= tempoParaAdaptar)
        {
            peso1 += bonusAdaptacao;
        }

        if (tempoMedia >= tempoParaAdaptar)
        {
            peso2 += bonusAdaptacao;
        }

        if (tempoLonga >= tempoParaAdaptar)
        {
            peso3 += bonusAdaptacao;
        }
    }

    private void AplicarMemoria(
        ref float peso1,
        ref float peso2,
        ref float peso3,
        float porcentagemVida
    )
    {
        float penalidadeABA;

        if (porcentagemVida > 0.70f)
        {
            penalidadeABA = 0.35f;
        }
        else if (porcentagemVida > 0.35f)
        {
            penalidadeABA = 0.45f;
        }
        else
        {
            penalidadeABA = 0.55f;
        }

        // Proíbe repetição imediata.
        if (ultimoAtaque == 1)
            peso1 = 0f;

        if (ultimoAtaque == 2)
            peso2 = 0f;

        if (ultimoAtaque == 3)
            peso3 = 0f;

        // Reduz a chance de padrão A-B-A.
        if (penultimoAtaque == 1)
            peso1 *= penalidadeABA;

        if (penultimoAtaque == 2)
            peso2 *= penalidadeABA;

        if (penultimoAtaque == 3)
            peso3 *= penalidadeABA;
    }

    private int SortearAtaque(
        float peso1,
        float peso2,
        float peso3
    )
    {
        float total =
            peso1 +
            peso2 +
            peso3;

        float sorteio =
            Random.Range(0f, total);

        if (sorteio < peso1)
            return 1;

        sorteio -= peso1;

        if (sorteio < peso2)
            return 2;

        return 3;
    }

    private void ExecutarAtaque(int ataqueEscolhido)
    {
        bool executou = false;

        if (ataqueEscolhido == 1 && ataque1 != null)
        {
            ataque1.ForcarAtaque();
            executou = true;
        }
        else if (ataqueEscolhido == 2 && ataque2 != null)
        {
            ataque2.ForcarAtaque();
            executou = true;
        }
        else if (ataqueEscolhido == 3 && ataque3 != null)
        {
            ataque3.ForcarAtaque();
            executou = true;
        }

        if (executou)
        {
            penultimoAtaque = ultimoAtaque;
            ultimoAtaque = ataqueEscolhido;

            tempoUltimoAtaque = Time.time;
        }
    }
}
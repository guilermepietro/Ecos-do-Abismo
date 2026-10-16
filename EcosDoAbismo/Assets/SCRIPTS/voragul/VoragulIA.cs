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
    public float tempoMovimentoAposAtaque = 1.2f;

    private float proximaDecisao = 0f;

    private int ultimoAtaque = 0;
    private int penultimoAtaque = 0;

    private bool estavaAtacando = false;

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

        bool ataqueEmAndamento = AtaqueEmAndamento();

        if (ataqueEmAndamento)
        {
            estavaAtacando = true;
            return;
        }

        // Acabou de terminar um ataque.
        if (estavaAtacando)
        {
            estavaAtacando = false;

            proximaDecisao =
                Time.time + tempoMovimentoAposAtaque;

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
        float distancia = Mathf.Abs(
            movimento.elian.position.x - transform.position.x
        );

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
                distancia,
                ref pesoAtaque1,
                ref pesoAtaque2,
                ref pesoAtaque3
            );
        }
        else if (porcentagemVida > 0.35f)
        {
            PesosMeio(
                distancia,
                ref pesoAtaque1,
                ref pesoAtaque2,
                ref pesoAtaque3
            );
        }
        else
        {
            PesosFinal(
                distancia,
                ref pesoAtaque1,
                ref pesoAtaque2,
                ref pesoAtaque3
            );
        }

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

    private void PesosInicio(
        float distancia,
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (distancia <= distanciaCurta)
        {
            peso1 = 80f;
            peso2 = 18f;
            peso3 = 2f;
        }
        else if (distancia <= distanciaMedia)
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
        float distancia,
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (distancia <= distanciaCurta)
        {
            peso1 = 65f;
            peso2 = 28f;
            peso3 = 7f;
        }
        else if (distancia <= distanciaMedia)
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
        float distancia,
        ref float peso1,
        ref float peso2,
        ref float peso3
    )
    {
        if (distancia <= distanciaCurta)
        {
            peso1 = 55f;
            peso2 = 35f;
            peso3 = 10f;
        }
        else if (distancia <= distanciaMedia)
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

    private void AplicarMemoria(
        ref float peso1,
        ref float peso2,
        ref float peso3,
        float porcentagemVida
    )
    {
        float penalidadeRepeticao;
        float penalidadeABA;

        if (porcentagemVida > 0.70f)
        {
            penalidadeRepeticao = 0.10f;
            penalidadeABA = 0.35f;
        }
        else if (porcentagemVida > 0.35f)
        {
            penalidadeRepeticao = 0.15f;
            penalidadeABA = 0.45f;
        }
        else
        {
            penalidadeRepeticao = 0.20f;
            penalidadeABA = 0.55f;
        }

        if (ultimoAtaque == 1)
            peso1 *= penalidadeRepeticao;

        if (ultimoAtaque == 2)
            peso2 *= penalidadeRepeticao;

        if (ultimoAtaque == 3)
            peso3 *= penalidadeRepeticao;

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
        else if (
            ataqueEscolhido == 2 &&
            ataque2 != null
        )
        {
            ataque2.ForcarAtaque();
            executou = true;
        }
        else if (
            ataqueEscolhido == 3 &&
            ataque3 != null
        )
        {
            ataque3.ForcarAtaque();
            executou = true;
        }

        if (executou)
        {
            penultimoAtaque = ultimoAtaque;
            ultimoAtaque = ataqueEscolhido;
        }
    }
}
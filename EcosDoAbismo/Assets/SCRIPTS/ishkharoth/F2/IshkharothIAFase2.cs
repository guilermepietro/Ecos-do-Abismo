
using UnityEngine;

public class IshkharothIAFase2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private Animator animator;
    [SerializeField] private IshkharothVida vida;
    [SerializeField] private IshkharothMovimentoFase2 movimento;

    [Header("Distancias")]
    [SerializeField] private float distanciaMaximaAtaque = 10f;
    [SerializeField] private float distanciaCurta = 4f;
    [SerializeField] private float distanciaMedia = 7f;

    [Header("Ritmo do Combate")]
    [SerializeField] private float intervaloEntreAtaques = 1.1f;
    [SerializeField] private float esperaInicial = 1.5f;
    [SerializeField] private float intervaloMinimo = 0.75f;

    [Header("Recargas Individuais")]
    [SerializeField] private float recargaAtaque1 = 2.5f;
    [SerializeField] private float recargaAtaque2 = 4f;
    [SerializeField] private float recargaAtaque3 = 7f;

    [Header("Recuperacao Adicional")]
    [SerializeField] private float recuperacaoProjetil = 0.1f;
    [SerializeField] private float recuperacaoMeiaLua = 0.3f;
    [SerializeField] private float recuperacaoPilares = 1f;

    [Header("Inteligencia")]
    [SerializeField] private float tempoEstabilidadeZona = 0.45f;
    [SerializeField] private float tempoParaAdaptacao = 3f;

    [Range(0f, 1f)]
    [SerializeField] private float penalidadeABA = 0.35f;

    private bool combateIniciado;
    private bool atacando;
    private bool zonaInicializada;

    private float proximaDecisao;
    private float inicioZona;
    private float inicioZonaCandidata;

    private readonly float[] recargas = new float[3];

    private int ultimoAtaque = -1;
    private int penultimoAtaque = -1;
    private int ataqueAtual = -1;

    // 0 = curta, 1 = media, 2 = longa
    private int zonaAtual;
    private int zonaCandidata;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (vida == null)
            vida = GetComponent<IshkharothVida>();

        if (movimento == null)
            movimento =
                GetComponent<IshkharothMovimentoFase2>();
    }

    private void Update()
    {
        if (vida == null ||
            !vida.Fase2Iniciada ||
            vida.Morto ||
            vida.EmTransicao)
        {
            return;
        }

        if (movimento == null ||
            !movimento.Fase2Ativa ||
            elian == null ||
            animator == null)
        {
            return;
        }

        if (!combateIniciado)
        {
            combateIniciado = true;
            proximaDecisao = Time.time + esperaInicial;
        }

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        AtualizarZona(distancia);

        if (atacando || Time.time < proximaDecisao)
            return;

        if (distancia > distanciaMaximaAtaque)
            return;

        int ataque = EscolherAtaque();

        if (ataque >= 0)
            ExecutarAtaque(ataque);
    }

    private void AtualizarZona(float distancia)
    {
        int novaZona;

        if (distancia <= distanciaCurta)
            novaZona = 0;
        else if (distancia <= distanciaMedia)
            novaZona = 1;
        else
            novaZona = 2;

        if (!zonaInicializada)
        {
            zonaInicializada = true;
            zonaAtual = novaZona;
            zonaCandidata = novaZona;

            inicioZona = Time.time;
            inicioZonaCandidata = Time.time;
            return;
        }

        if (novaZona != zonaCandidata)
        {
            zonaCandidata = novaZona;
            inicioZonaCandidata = Time.time;
        }

        if (zonaCandidata != zonaAtual &&
            Time.time - inicioZonaCandidata >=
            tempoEstabilidadeZona)
        {
            zonaAtual = zonaCandidata;
            inicioZona = Time.time;
        }
    }

    private int EscolherAtaque()
    {
        float[] pesos;

        switch (zonaAtual)
        {
            case 0:
                // Perto: projetil e pilares.
                pesos = new float[] { 5f, 2f, 4f };
                break;

            case 1:
                // Medio: todos os ataques viaveis.
                pesos = new float[] { 3f, 4f, 5f };
                break;

            default:
                // Longe: meia-lua e pilares.
                pesos = new float[] { 2f, 5f, 4f };
                break;
        }

        AjustarPesosPorVida(pesos);
        AjustarPesosPorComportamento(pesos);

        float soma = 0f;

        for (int i = 0; i < pesos.Length; i++)
        {
            // Nunca repete imediatamente.
            if (i == ultimoAtaque)
            {
                pesos[i] = 0f;
            }

            // Reduz A-B-A.
            if (i == penultimoAtaque)
            {
                pesos[i] *= penalidadeABA;
            }

            // Respeita a recarga individual.
            if (Time.time < recargas[i])
            {
                pesos[i] = 0f;
            }

            soma += pesos[i];
        }

        // Se todos estiverem indisponiveis,
        // Ishkharoth aguarda.
        if (soma <= 0f)
            return -1;

        float sorteio = Random.Range(0f, soma);

        for (int i = 0; i < pesos.Length; i++)
        {
            if (pesos[i] <= 0f)
                continue;

            sorteio -= pesos[i];

            if (sorteio <= 0f)
                return i;
        }

        // Protecao contra arredondamento.
        for (int i = pesos.Length - 1; i >= 0; i--)
        {
            if (pesos[i] > 0f)
                return i;
        }

        return -1;
    }

    private void AjustarPesosPorVida(float[] pesos)
    {
        float porcentagem = vida.PorcentagemVidaFase2;

        if (porcentagem <= 0.35f)
        {
            // Fase critica: pressao maior,
            // sem aumentar o dano.
            pesos[0] *= 1.25f;
            pesos[1] *= 1.2f;
            pesos[2] *= 1.1f;
        }
        else if (porcentagem <= 0.70f)
        {
            pesos[0] *= 1.1f;
            pesos[1] *= 1.1f;
            pesos[2] *= 1.05f;
        }
    }

    private void AjustarPesosPorComportamento(
        float[] pesos)
    {
        float tempoNaZona = Time.time - inicioZona;

        if (tempoNaZona < tempoParaAdaptacao)
            return;

        // Elian permanece perto por muito tempo:
        // aumenta a chance de pilares.
        if (zonaAtual == 0)
        {
            pesos[2] *= 1.5f;
        }

        // Elian permanece a media distancia:
        // aumenta a variacao dos ataques.
        else if (zonaAtual == 1)
        {
            pesos[0] *= 1.2f;
            pesos[2] *= 1.2f;
        }

        // Elian permanece longe:
        // aumenta a chance de meia-lua.
        else
        {
            pesos[1] *= 1.5f;
        }
    }

    private void ExecutarAtaque(int indice)
    {
        atacando = true;
        ataqueAtual = indice;

        penultimoAtaque = ultimoAtaque;
        ultimoAtaque = indice;

        movimento.BloquearMovimento();
        animator.SetBool("Andando", false);

        switch (indice)
        {
            case 0:
                recargas[0] =
                    Time.time + recargaAtaque1;

                animator.SetTrigger("Ataque1F2");
                break;

            case 1:
                recargas[1] =
                    Time.time + recargaAtaque2;

                animator.SetTrigger("Ataque2F2");
                break;

            case 2:
                recargas[2] =
                    Time.time + recargaAtaque3;

                animator.SetTrigger("Ataque3F2");
                break;
        }
    }

    private float ObterIntervaloAtual()
    {
        float intervalo = intervaloEntreAtaques;

        float porcentagem = vida.PorcentagemVidaFase2;

        if (porcentagem <= 0.35f)
        {
            intervalo *= 0.75f;
        }
        else if (porcentagem <= 0.70f)
        {
            intervalo *= 0.90f;
        }

        return Mathf.Max(intervaloMinimo, intervalo);
    }

    // Metodo ja utilizado pelos Animation Events
    // e pelo controlador dos pilares.
    public void FinalizarAtaqueFase2()
    {
        if (!atacando)
            return;

        atacando = false;

        float recuperacaoExtra = 0f;

        switch (ataqueAtual)
        {
            case 0:
                recuperacaoExtra = recuperacaoProjetil;
                break;

            case 1:
                recuperacaoExtra = recuperacaoMeiaLua;
                break;

            case 2:
                recuperacaoExtra = recuperacaoPilares;
                break;
        }

        ataqueAtual = -1;

        proximaDecisao =
            Time.time +
            ObterIntervaloAtual() +
            recuperacaoExtra;

        if (movimento != null &&
            vida != null &&
            !vida.Morto)
        {
            movimento.LiberarMovimento();
        }
    }

    private void OnDisable()
    {
        if (atacando &&
            movimento != null &&
            vida != null &&
            !vida.Morto)
        {
            movimento.LiberarMovimento();
        }

        atacando = false;
        ataqueAtual = -1;
    }
}

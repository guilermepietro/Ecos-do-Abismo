
using UnityEngine;

[RequireComponent(typeof(IshkharothMovimento))]
[RequireComponent(typeof(Animator))]
public class IshkharothIA : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private Animator animator;
    [SerializeField] private IshkharothMovimento movimento;
    [SerializeField] private IshkharothVida vida;

    [Header("Combate")]
    [SerializeField] private float distanciaMaximaAtaque = 8f;
    [SerializeField] private float alcanceEspinhosChao = 6f;

    [Header("Ritmo do Combate")]
    [SerializeField] private float intervaloEntreAtaques = 1.3f;
    [SerializeField] private float intervaloMinimo = 1f;
    [SerializeField] private float esperaInicial = 0.8f;

    [Header("Recarga dos Ataques")]
    [SerializeField] private float recargaEsfera = 2.5f;
    [SerializeField] private float recargaEspinhosChao = 4f;
    [SerializeField] private float recargaEspinhosCeu = 5f;

    [Header("Recuperacao Apos Ataques")]
    [SerializeField] private float recuperacaoEsfera = 0.25f;
    [SerializeField] private float recuperacaoEspinhosChao = 0.4f;
    [SerializeField] private float recuperacaoEspinhosCeu = 1.1f;

    [Header("Distancias de Decisao")]
    [SerializeField] private float distanciaCurta = 3f;
    [SerializeField] private float distanciaMedia = 6f;

    [Header("Adaptacao ao Elian")]
    [SerializeField] private float tempoEstabilidadeZona = 0.45f;
    [SerializeField] private float tempoParaAdaptacao = 3f;

    [Header("Anti-Repeticao")]
    [Range(0f, 1f)]
    [SerializeField] private float penalidadeABA = 0.35f;

    [Header("Variedade de Ataques")]
    [SerializeField, Min(1)]
    private int limiteAtaquesSemEsfera = 2;

    private bool combateIniciado;
    private bool atacando;

    private float proximaDecisao;

    private readonly float[] proximasRecargas = new float[3];

    private int ultimoAtaque = -1;
    private int penultimoAtaque = -1;
    private int ataqueAtual = -1;

    // Evita longas sequencias sem a esfera.
    private int ataquesSemEsfera;

    // Controle especial da esfera.
    private bool aguardandoEsfera;
    private bool animacaoAtaqueTerminou;

    // Controle da distancia do Elian.
    // 0 = curta, 1 = media, 2 = longa.
    private bool zonaInicializada;
    private int zonaAtual;
    private int zonaCandidata;

    private float inicioZona;
    private float inicioZonaCandidata;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movimento == null)
            movimento = GetComponent<IshkharothMovimento>();

        if (vida == null)
            vida = GetComponent<IshkharothVida>();
    }

    private void Update()
    {
        if (elian == null ||
            movimento == null ||
            animator == null ||
            vida == null)
        {
            return;
        }

        if (!vida.EstaNaFase1)
            return;

        if (!movimento.CombateAtivo)
            return;

        if (!combateIniciado)
        {
            combateIniciado = true;
            proximaDecisao = Time.time + esperaInicial;
        }

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        AtualizarZona(distancia);

        if (atacando)
            return;

        if (Time.time < proximaDecisao)
            return;

        if (distancia > distanciaMaximaAtaque)
            return;

        int ataqueEscolhido = EscolherAtaque(distancia);

        if (ataqueEscolhido >= 0)
            ExecutarAtaque(ataqueEscolhido);
    }

    // =====================================
    // ANALISE DA DISTANCIA
    // =====================================

    private void AtualizarZona(float distancia)
    {
        int novaZona;

        if (distancia <= distanciaCurta)
        {
            novaZona = 0;
        }
        else if (distancia <= distanciaMedia)
        {
            novaZona = 1;
        }
        else
        {
            novaZona = 2;
        }

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

    // =====================================
    // ESCOLHA DOS ATAQUES
    // =====================================

    private int EscolherAtaque(float distancia)
    {
        float[] pesos;

        switch (zonaAtual)
        {
            case 0:
                // Curta distancia:
                // esfera e onda equilibradas.
                pesos = new float[] { 4f, 4f, 2f };
                break;

            case 1:
                // Media distancia:
                // esfera levemente favorecida.
                pesos = new float[] { 5f, 3f, 2f };
                break;

            default:
                // Longa distancia:
                // esfera favorecida, chuva secundaria.
                pesos = new float[] { 6f, 0f, 3f };
                break;
        }

        AjustarPesosPorVida(pesos);
        AjustarPesosPorComportamento(pesos);

        float somaPesos = 0f;

        for (int i = 0; i < pesos.Length; i++)
        {
            // Respeita o cooldown individual.
            if (Time.time < proximasRecargas[i])
            {
                pesos[i] = 0f;
            }

            // Impede repetir imediatamente.
            if (i == ultimoAtaque)
            {
                pesos[i] = 0f;
            }

            // Desestimula sequencias A-B-A.
            if (i == penultimoAtaque)
            {
                pesos[i] *= penalidadeABA;
            }

            // Onda apenas dentro do alcance.
            if (i == 1 &&
                distancia > alcanceEspinhosChao)
            {
                pesos[i] = 0f;
            }

            somaPesos += pesos[i];
        }

        if (somaPesos <= 0f)
            return -1;

        // =====================================
        // CONTROLE DE VARIEDADE
        // =====================================

        // Se Ishkharoth usou dois ataques sem
        // utilizar a esfera, ela passa a ter
        // prioridade, desde que esteja disponivel.
        //
        // Isso nao ignora o cooldown nem permite
        // repetir o mesmo ataque imediatamente.
        if (ataquesSemEsfera >= limiteAtaquesSemEsfera &&
            Time.time >= proximasRecargas[0] &&
            ultimoAtaque != 0 &&
            pesos[0] > 0f)
        {
            return 0;
        }

        // =====================================
        // SORTEIO NORMAL
        // =====================================

        float sorteio = Random.Range(0f, somaPesos);

        for (int i = 0; i < pesos.Length; i++)
        {
            if (pesos[i] <= 0f)
                continue;

            sorteio -= pesos[i];

            if (sorteio <= 0f)
                return i;
        }

        for (int i = pesos.Length - 1; i >= 0; i--)
        {
            if (pesos[i] > 0f)
                return i;
        }

        return -1;
    }

    // =====================================
    // PROGRESSAO POR VIDA
    // =====================================

    private void AjustarPesosPorVida(float[] pesos)
    {
        float porcentagem = vida.PorcentagemVida;

        if (porcentagem <= 0.35f)
        {
            pesos[0] *= 1.2f;
            pesos[1] *= 1.1f;
        }
        else if (porcentagem <= 0.70f)
        {
            pesos[0] *= 1.05f;
            pesos[1] *= 1.05f;
        }
    }

    // =====================================
    // ADAPTACAO AO JOGADOR
    // =====================================

    private void AjustarPesosPorComportamento(
        float[] pesos)
    {
        float tempoNaZona = Time.time - inicioZona;

        if (tempoNaZona < tempoParaAdaptacao)
            return;

        switch (zonaAtual)
        {
            case 0:
                // Se Elian fica perto, Ishkharoth
                // continua perigoso sem usar
                // apenas a onda de espinhos.
                pesos[0] *= 1.25f;
                pesos[1] *= 1.1f;
                break;

            case 1:
                // Favorece variedade a media distancia.
                pesos[0] *= 1.15f;
                pesos[2] *= 1.15f;
                break;

            case 2:
                // Pressiona quem fica longe
                // utilizando mais a esfera.
                pesos[0] *= 1.3f;
                break;
        }
    }

    // =====================================
    // EXECUCAO DOS ATAQUES
    // =====================================

    private void ExecutarAtaque(int indice)
    {
        atacando = true;
        ataqueAtual = indice;

        aguardandoEsfera = false;
        animacaoAtaqueTerminou = false;

        movimento.BloquearMovimento();
        animator.SetBool("Andando", false);

        penultimoAtaque = ultimoAtaque;
        ultimoAtaque = indice;

        // Registra quantos ataques ocorreram
        // desde a ultima esfera.
        if (indice == 0)
        {
            ataquesSemEsfera = 0;
        }
        else
        {
            ataquesSemEsfera++;
        }

        switch (indice)
        {
            case 0:
                proximasRecargas[0] =
                    Time.time + recargaEsfera;

                animator.SetTrigger("Ataque1");
                break;

            case 1:
                proximasRecargas[1] =
                    Time.time + recargaEspinhosChao;

                animator.SetTrigger("Ataque2");
                break;

            case 2:
                proximasRecargas[2] =
                    Time.time + recargaEspinhosCeu;

                animator.SetTrigger("Ataque3");
                break;
        }
    }

    // =====================================
    // CONTROLE ESPECIAL DA ESFERA
    // =====================================

    public void AguardarAfastamentoEsfera()
    {
        if (atacando)
            aguardandoEsfera = true;
    }

    public void EsferaAfastou()
    {
        if (!atacando || !aguardandoEsfera)
            return;

        aguardandoEsfera = false;

        if (animacaoAtaqueTerminou)
            ConcluirAtaque();
    }

    // =====================================
    // FINALIZACAO DOS ATAQUES
    // =====================================

    public void FinalizarAtaque()
    {
        if (!atacando)
            return;

        animacaoAtaqueTerminou = true;

        if (aguardandoEsfera)
            return;

        ConcluirAtaque();
    }

    private void ConcluirAtaque()
    {
        if (!atacando)
            return;

        atacando = false;

        aguardandoEsfera = false;
        animacaoAtaqueTerminou = false;

        float recuperacaoExtra = 0f;

        switch (ataqueAtual)
        {
            case 0:
                recuperacaoExtra = recuperacaoEsfera;
                break;

            case 1:
                recuperacaoExtra = recuperacaoEspinhosChao;
                break;

            case 2:
                recuperacaoExtra = recuperacaoEspinhosCeu;
                break;
        }

        ataqueAtual = -1;

        proximaDecisao =
            Time.time +
            ObterIntervaloAtual() +
            recuperacaoExtra;

        if (movimento != null &&
            vida != null &&
            vida.EstaNaFase1)
        {
            movimento.LiberarMovimento();
        }
    }

    // =====================================
    // AGRESSIVIDADE PROGRESSIVA
    // =====================================

    private float ObterIntervaloAtual()
    {
        float intervalo = intervaloEntreAtaques;

        float porcentagem = vida.PorcentagemVida;

        if (porcentagem <= 0.35f)
        {
            intervalo *= 0.84f;
        }
        else if (porcentagem <= 0.70f)
        {
            intervalo *= 0.92f;
        }

        return Mathf.Max(
            intervaloMinimo,
            intervalo
        );
    }

    // =====================================
    // SEGURANCA NA TRANSFORMACAO
    // =====================================

    private void OnDisable()
    {
        if (atacando && movimento != null)
        {
            movimento.LiberarMovimento();
        }

        atacando = false;

        aguardandoEsfera = false;
        animacaoAtaqueTerminou = false;

        ataqueAtual = -1;
    }
}

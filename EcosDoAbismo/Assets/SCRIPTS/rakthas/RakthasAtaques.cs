using UnityEngine;
using System.Collections.Generic;

public class RakthasAtaques : MonoBehaviour
{
    [Header("Hitbox do Raio")]
    public GameObject hitboxRaio;

    private bool atacando = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Posição da Hitbox do Raio")]
    public float posicaoXDireita = 0.352f;
    public float posicaoXEsquerda = -1.054f;

    [Header("Hitboxes da Onda")]
    public BoxCollider2D hitboxOndaDireita;
    public BoxCollider2D hitboxOndaEsquerda;

    [Header("Hitboxes do Soco")]
    public BoxCollider2D hitboxSocoDireita;
    public BoxCollider2D hitboxSocoEsquerda;

    [Header("Velocidade do Soco")]
    public AnimationCurve curvaVelocidadeSoco =
        new AnimationCurve(
            new Keyframe(0f, 1.25f),
            new Keyframe(0.25f, 1.25f),
            new Keyframe(0.40f, 1.70f),
            new Keyframe(0.58f, 1f),
            new Keyframe(0.80f, 0.70f),
            new Keyframe(1f, 0.70f)
        );

    [Header("Inteligência Artificial")]
    public float distanciaSoco = 2.2f;
    public float distanciaCorrente = 5f;
    public float distanciaMaximaRaio = 8f;

    [Header("Pesos - Curta Distância")]
    public float pesoSocoPerto = 70f;
    public float pesoCorrentePerto = 30f;

    [Header("Pesos - Média Distância")]
    public float pesoSocoMedio = 15f;
    public float pesoCorrenteMedio = 65f;
    public float pesoRaioMedio = 20f;

    [Header("Pesos - Longa Distância")]
    public float pesoCorrenteLonge = 25f;
    public float pesoRaioLonge = 75f;

    [Header("Leitura do Elian")]
    public float intervaloLeituraElian = 0.2f;
    public float diferencaMinimaMovimento = 0.15f;

    [Header("Influência - Elian se Aproximando")]
    public float bonusSocoAproximando = 25f;
    public float bonusCorrenteAproximando = 10f;
    public float penalidadeRaioAproximando = 15f;

    [Header("Influência - Elian se Afastando")]
    public float bonusRaioAfastando = 25f;
    public float bonusCorrenteAfastando = 10f;
    public float penalidadeSocoAfastando = 15f;

    [Header("Comportamento Prolongado")]
    public float tempoParaConsiderarFuga = 1.2f;
    public float tempoParaConsiderarAproximacao = 1f;
    public float tempoParaEsquecerComportamento = 0.6f;

    [Header("Influência - Fuga Prolongada")]
    public float bonusRaioFugaProlongada = 20f;
    public float bonusCorrenteFugaProlongada = 15f;
    public float penalidadeSocoFugaProlongada = 10f;

    [Header("Influência - Aproximação Prolongada")]
    public float bonusSocoAproximacaoProlongada = 20f;
    public float bonusCorrenteAproximacaoProlongada = 10f;
    public float penalidadeRaioAproximacaoProlongada = 10f;

    private enum MovimentoElian
    {
        Parado,
        Aproximando,
        Afastando
    }

    private MovimentoElian movimentoAtualElian =
        MovimentoElian.Parado;

    private float distanciaAnteriorElian;
    private float proximaLeituraElian;
    private bool primeiraLeituraElian = true;

    private float tempoAfastando = 0f;
    private float tempoAproximando = 0f;
    private float tempoNeutro = 0f;

    private bool elianFugindoMuito = false;
    private bool elianAproximandoMuito = false;

    public bool ElianFugindoMuito => elianFugindoMuito;

    [Header("Preparação da Corrente")]
    public float preparacaoCorrente = 0.35f;

    private bool preparandoCorrente = false;
    private float momentoAtaqueCorrente;

    [Header("Preparação do Raio")]
    public float preparacaoRaio = 0.2f;

    private bool preparandoRaio = false;
    private float momentoAtaqueRaio;

    [Header("Recuperação dos Ataques")]
    public float recuperacaoRaio = 0.8f;
    public float recuperacaoCorrente = 1.2f;
    public float recuperacaoSoco = 1.5f;

    [Header("Variação após o Raio")]
    [Range(0f, 1f)]
    public float chanceCorrenteAposRaio = 0.4f;

    private bool perseguirAteCorrente = false;

    [Header("Pausa de Decisão")]
    [Range(0f, 1f)]
    public float chancePausaDecisao = 0.4f;

    public float pausaDecisaoMinima = 0.3f;
    public float pausaDecisaoMaxima = 0.6f;

    private bool emPausaDecisao = false;
    private float fimPausaDecisao;

    public bool EstaEmPausaDecisao => emPausaDecisao;

    public float esperaInicial = 1f;

    private float proximoAtaque;
    private bool iniciouCombate = false;

    private RakthasMovimento movimento;
    private RakthasVida vida;

    private bool perseguicaoAgressivaObrigatoria = false;

    // =========================
    // MEMÓRIA DE ATAQUES
    // =========================

    private List<string> historicoAtaques =
        new List<string>();

    public bool EstaAtacando => atacando;

    public bool PerseguicaoAgressivaObrigatoria
        => perseguicaoAgressivaObrigatoria;

    public float DistanciaSoco => distanciaSoco;

    public float DistanciaMaximaRaio
        => distanciaMaximaRaio;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movimento = GetComponent<RakthasMovimento>();
        vida = GetComponent<RakthasVida>();
    }

    private void Update()
    {
        AtualizarLeituraElian();

        // =========================
        // PREPARAÇÃO DA CORRENTE
        // =========================

        if (preparandoCorrente &&
            Time.time >= momentoAtaqueCorrente)
        {
            preparandoCorrente = false;

            if (vida != null &&
                (vida.EstaMorto ||
                 vida.EstaTomandoDano))
            {
                atacando = false;
                return;
            }

            animator.SetTrigger("Ataque1");
        }

        // =========================
        // PREPARAÇÃO DO RAIO
        // =========================

        if (preparandoRaio &&
            Time.time >= momentoAtaqueRaio)
        {
            preparandoRaio = false;

            if (vida != null &&
                (vida.EstaMorto ||
                 vida.EstaTomandoDano))
            {
                atacando = false;
                return;
            }

            animator.SetTrigger("Raio");
        }

        AtualizarPosicaoHitboxRaio();

        // =========================
        // VELOCIDADE VARIÁVEL DO SOCO
        // =========================

        if (atacando)
        {
            AnimatorStateInfo estado =
                animator.GetCurrentAnimatorStateInfo(0);

            if (estado.IsName("Attack3"))
            {
                float progresso = Mathf.Clamp01(
                    estado.normalizedTime
                );

                animator.speed =
                    curvaVelocidadeSoco.Evaluate(
                        progresso
                    );
            }
        }

        // =========================
        // TECLAS TEMPORÁRIAS
        // =========================

        if (PodeAtacar())
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                IniciarAtaque("Raio");
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                IniciarAtaque("Ataque1");
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                IniciarAtaque("Ataque3");
            }
        }

        // =========================
        // PAUSA DE DECISÃO
        // =========================

        if (emPausaDecisao)
        {
            if (Time.time >= fimPausaDecisao)
            {
                emPausaDecisao = false;
            }
            else
            {
                return;
            }
        }

        EscolherAtaque();
    }

    // =========================
    // LEITURA DO ELIAN
    // =========================

    private void AtualizarLeituraElian()
    {
        if (movimento == null ||
            movimento.elian == null)
            return;

        if (Time.time < proximaLeituraElian)
            return;

        proximaLeituraElian =
            Time.time + intervaloLeituraElian;

        float distanciaAtual =
            Mathf.Abs(
                movimento.elian.position.x -
                transform.position.x
            );

        if (primeiraLeituraElian)
        {
            distanciaAnteriorElian =
                distanciaAtual;

            primeiraLeituraElian = false;

            movimentoAtualElian =
                MovimentoElian.Parado;

            return;
        }

        float diferenca =
            distanciaAtual -
            distanciaAnteriorElian;

        if (diferenca <
            -diferencaMinimaMovimento)
        {
            movimentoAtualElian =
                MovimentoElian.Aproximando;
        }
        else if (diferenca >
                 diferencaMinimaMovimento)
        {
            movimentoAtualElian =
                MovimentoElian.Afastando;
        }
        else
        {
            movimentoAtualElian =
                MovimentoElian.Parado;
        }

        AtualizarComportamentoProlongado();

        distanciaAnteriorElian =
            distanciaAtual;
    }

    private void AtualizarComportamentoProlongado()
    {
        if (movimentoAtualElian ==
            MovimentoElian.Afastando)
        {
            tempoAfastando +=
                intervaloLeituraElian;

            tempoAproximando = 0f;
            tempoNeutro = 0f;

            if (tempoAfastando >=
                tempoParaConsiderarFuga)
            {
                elianFugindoMuito = true;
                elianAproximandoMuito = false;
            }

            return;
        }

        if (movimentoAtualElian ==
            MovimentoElian.Aproximando)
        {
            tempoAproximando +=
                intervaloLeituraElian;

            tempoAfastando = 0f;
            tempoNeutro = 0f;

            if (tempoAproximando >=
                tempoParaConsiderarAproximacao)
            {
                elianAproximandoMuito = true;
                elianFugindoMuito = false;
            }

            return;
        }

        tempoAfastando = 0f;
        tempoAproximando = 0f;

        tempoNeutro +=
            intervaloLeituraElian;

        if (tempoNeutro >=
            tempoParaEsquecerComportamento)
        {
            elianFugindoMuito = false;
            elianAproximandoMuito = false;
        }
    }

    // =========================
    // PAUSA DE DECISÃO
    // =========================

    private void TentarPausaDecisao()
    {
        if (Random.value >
            chancePausaDecisao)
            return;

        emPausaDecisao = true;

        fimPausaDecisao =
            Time.time +
            Random.Range(
                pausaDecisaoMinima,
                pausaDecisaoMaxima
            );
    }

    // =========================
    // MEMÓRIA
    // =========================

    private void RegistrarAtaque(
        string nomeAtaque)
    {
        historicoAtaques.Add(
            nomeAtaque
        );

        if (historicoAtaques.Count > 3)
        {
            historicoAtaques.RemoveAt(0);
        }
    }

    private bool RepetiriaAtaque(
        string nomeAtaque)
    {
        if (historicoAtaques.Count == 0)
            return false;

        string ultimo =
            historicoAtaques[
                historicoAtaques.Count - 1
            ];

        return ultimo == nomeAtaque;
    }

    private bool CriariaPadraoABA(
        string nomeAtaque)
    {
        if (historicoAtaques.Count < 2)
            return false;

        string penultimo =
            historicoAtaques[
                historicoAtaques.Count - 2
            ];

        string ultimo =
            historicoAtaques[
                historicoAtaques.Count - 1
            ];

        if (penultimo == nomeAtaque &&
            ultimo != nomeAtaque)
        {
            return true;
        }

        return false;
    }

    private bool AtaqueDeveSerEvitado(
        string nomeAtaque)
    {
        if (RepetiriaAtaque(nomeAtaque))
            return true;

        if (CriariaPadraoABA(nomeAtaque))
            return true;

        return false;
    }

    // =========================
    // PESOS
    // =========================

    private string EscolherPorPeso(
        float pesoSoco,
        float pesoCorrente,
        float pesoRaio)
    {
        // =========================
        // MOVIMENTO INSTANTÂNEO
        // =========================

        if (movimentoAtualElian ==
            MovimentoElian.Aproximando)
        {
            pesoSoco +=
                bonusSocoAproximando;

            pesoCorrente +=
                bonusCorrenteAproximando;

            pesoRaio -=
                penalidadeRaioAproximando;
        }
        else if (movimentoAtualElian ==
                 MovimentoElian.Afastando)
        {
            pesoRaio +=
                bonusRaioAfastando;

            pesoCorrente +=
                bonusCorrenteAfastando;

            pesoSoco -=
                penalidadeSocoAfastando;
        }

        // =========================
        // COMPORTAMENTO PROLONGADO
        // =========================

        if (elianFugindoMuito)
        {
            pesoRaio +=
                bonusRaioFugaProlongada;

            pesoCorrente +=
                bonusCorrenteFugaProlongada;

            pesoSoco -=
                penalidadeSocoFugaProlongada;
        }

        if (elianAproximandoMuito)
        {
            pesoSoco +=
                bonusSocoAproximacaoProlongada;

            pesoCorrente +=
                bonusCorrenteAproximacaoProlongada;

            pesoRaio -=
                penalidadeRaioAproximacaoProlongada;
        }

        pesoSoco =
            Mathf.Max(0f, pesoSoco);

        pesoCorrente =
            Mathf.Max(0f, pesoCorrente);

        pesoRaio =
            Mathf.Max(0f, pesoRaio);

        // =========================
        // MEMÓRIA
        // =========================

        if (AtaqueDeveSerEvitado(
                "Ataque3"))
        {
            pesoSoco = 0f;
        }

        if (AtaqueDeveSerEvitado(
                "Ataque1"))
        {
            pesoCorrente = 0f;
        }

        if (AtaqueDeveSerEvitado(
                "Raio"))
        {
            pesoRaio = 0f;
        }

        float pesoTotal =
            pesoSoco +
            pesoCorrente +
            pesoRaio;

        if (pesoTotal <= 0f)
            return "";

        float escolha =
            Random.Range(
                0f,
                pesoTotal
            );

        if (escolha < pesoSoco)
        {
            return "Ataque3";
        }

        escolha -= pesoSoco;

        if (escolha < pesoCorrente)
        {
            return "Ataque1";
        }

        return "Raio";
    }

    // =========================
    // PODE ATACAR
    // =========================

    private bool PodeAtacar()
    {
        if (atacando)
            return false;

        if (vida != null &&
            (vida.EstaMorto ||
             vida.EstaTomandoDano))
        {
            return false;
        }

        return true;
    }

    // =========================
    // ESCOLHA DE ATAQUE
    // =========================

    private void EscolherAtaque()
    {
        if (movimento == null ||
            !movimento.EstaAtivado)
        {
            iniciouCombate = false;

            primeiraLeituraElian = true;

            movimentoAtualElian =
                MovimentoElian.Parado;

            tempoAfastando = 0f;
            tempoAproximando = 0f;
            tempoNeutro = 0f;

            elianFugindoMuito = false;
            elianAproximandoMuito = false;

            return;
        }

        if (!iniciouCombate)
        {
            iniciouCombate = true;

            proximoAtaque =
                Time.time +
                esperaInicial;

            return;
        }

        if (!PodeAtacar())
            return;

        if (movimento.elian == null)
            return;

        float distancia =
            Mathf.Abs(
                movimento.elian.position.x -
                transform.position.x
            );

        // =========================
        // PERSEGUIÇÃO AGRESSIVA
        // =========================

        if (perseguicaoAgressivaObrigatoria)
        {
            if (Time.time < proximoAtaque)
                return;

            if (distancia <= distanciaSoco)
            {
                perseguirAteCorrente = false;

                string ataque =
                    EscolherPorPeso(
                        pesoSocoPerto,
                        pesoCorrentePerto,
                        0f
                    );

                if (ataque != "")
                {
                    IniciarAtaque(ataque);
                }
                else
                {
                    IniciarAtaque(
                        "Ataque3"
                    );
                }

                return;
            }

            if (perseguirAteCorrente &&
                distancia <=
                distanciaCorrente)
            {
                if (!AtaqueDeveSerEvitado(
                        "Ataque1"))
                {
                    perseguirAteCorrente =
                        false;

                    IniciarAtaque(
                        "Ataque1"
                    );

                    return;
                }

                perseguirAteCorrente =
                    false;
            }

            return;
        }

        // =========================
        // FORA DO ALCANCE
        // =========================

        if (distancia >
            distanciaMaximaRaio)
        {
            return;
        }

        if (Time.time < proximoAtaque)
            return;

        string ataqueEscolhido = "";

        // =========================
        // CURTA DISTÂNCIA
        // =========================

        if (distancia <= distanciaSoco)
        {
            ataqueEscolhido =
                EscolherPorPeso(
                    pesoSocoPerto,
                    pesoCorrentePerto,
                    0f
                );
        }

        // =========================
        // MÉDIA DISTÂNCIA
        // =========================

        else if (distancia <=
                 distanciaCorrente)
        {
            ataqueEscolhido =
                EscolherPorPeso(
                    pesoSocoMedio,
                    pesoCorrenteMedio,
                    pesoRaioMedio
                );
        }

        // =========================
        // LONGA DISTÂNCIA
        // =========================

        else
        {
            ataqueEscolhido =
                EscolherPorPeso(
                    0f,
                    pesoCorrenteLonge,
                    pesoRaioLonge
                );
        }

        if (ataqueEscolhido != "")
        {
            IniciarAtaque(
                ataqueEscolhido
            );
        }
    }

    // =========================
    // INICIAR ATAQUE
    // =========================

    private void IniciarAtaque(
        string nomeAtaque)
    {
        if (!PodeAtacar())
            return;

        if (movimento != null &&
            movimento.elian != null)
        {
            float diferencaX =
                movimento.elian.position.x -
                transform.position.x;

            if (diferencaX > 0.1f)
            {
                spriteRenderer.flipX =
                    false;
            }
            else if (diferencaX < -0.1f)
            {
                spriteRenderer.flipX =
                    true;
            }
        }

        atacando = true;

        RegistrarAtaque(
            nomeAtaque
        );

        animator.speed =
            nomeAtaque == "Ataque3"
            ? curvaVelocidadeSoco.Evaluate(0f)
            : 1f;

        animator.SetBool(
            "estaCorrendo",
            false
        );

        if (nomeAtaque == "Ataque1")
        {
            preparandoCorrente = true;

            momentoAtaqueCorrente =
                Time.time +
                preparacaoCorrente;
        }
        else if (nomeAtaque == "Raio")
        {
            preparandoRaio = true;

            momentoAtaqueRaio =
                Time.time +
                preparacaoRaio;
        }
        else
        {
            animator.SetTrigger(
                nomeAtaque
            );
        }
    }

    // =========================
    // RAIO
    // =========================

    private void AtualizarPosicaoHitboxRaio()
    {
        if (hitboxRaio == null ||
            spriteRenderer == null)
            return;

        Vector3 posicao =
            hitboxRaio.transform.localPosition;

        if (spriteRenderer.flipX)
        {
            posicao.x =
                posicaoXEsquerda;
        }
        else
        {
            posicao.x =
                posicaoXDireita;
        }

        hitboxRaio.transform.localPosition =
            posicao;
    }

    public void AtivarHitboxRaio()
    {
        if (hitboxRaio != null)
        {
            hitboxRaio.SetActive(true);

            RakthasRaioHitbox raio =
                hitboxRaio.GetComponent<
                    RakthasRaioHitbox
                >();

            if (raio != null)
            {
                raio.VerificarDano();
            }
        }
    }

    public void DesativarHitboxRaio()
    {
        if (hitboxRaio != null)
        {
            hitboxRaio.SetActive(false);
        }
    }

    public void FinalizarRaio()
    {
        atacando = false;

        proximoAtaque =
            Time.time +
            recuperacaoRaio;

        perseguicaoAgressivaObrigatoria =
            true;

        perseguirAteCorrente =
            Random.value <
            chanceCorrenteAposRaio;
    }

    // =========================
    // CORRENTE
    // =========================

    public void AtivarHitboxOnda()
    {
        if (hitboxOndaDireita == null ||
            hitboxOndaEsquerda == null)
            return;

        hitboxOndaDireita.enabled = false;
        hitboxOndaEsquerda.enabled = false;

        BoxCollider2D hitboxAtual =
            spriteRenderer.flipX
            ? hitboxOndaEsquerda
            : hitboxOndaDireita;

        RakthasOndaHitbox onda =
            hitboxAtual.GetComponent<
                RakthasOndaHitbox
            >();

        if (onda != null)
        {
            onda.ReiniciarAcertos();
        }

        hitboxAtual.enabled = true;

        if (onda != null)
        {
            onda.VerificarDano();
        }
    }

    public void DesativarHitboxOnda()
    {
        if (hitboxOndaDireita != null)
        {
            hitboxOndaDireita.enabled =
                false;
        }

        if (hitboxOndaEsquerda != null)
        {
            hitboxOndaEsquerda.enabled =
                false;
        }
    }

    public void FinalizarAtaque1()
    {
        atacando = false;

        proximoAtaque =
            Time.time +
            recuperacaoCorrente;

        perseguirAteCorrente = false;

        perseguicaoAgressivaObrigatoria =
            true;
    }

    // =========================
    // SOCO
    // =========================

    public void AtivarHitboxSoco()
    {
        if (hitboxSocoDireita == null ||
            hitboxSocoEsquerda == null)
            return;

        hitboxSocoDireita.enabled = false;
        hitboxSocoEsquerda.enabled = false;

        BoxCollider2D hitboxAtual =
            spriteRenderer.flipX
            ? hitboxSocoEsquerda
            : hitboxSocoDireita;

        RakthasSocoHitbox soco =
            hitboxAtual.GetComponent<
                RakthasSocoHitbox
            >();

        if (soco != null)
        {
            soco.ReiniciarAcertos();
        }

        hitboxAtual.enabled = true;

        if (soco != null)
        {
            soco.VerificarDano();
        }
    }

    public void DesativarHitboxSoco()
    {
        if (hitboxSocoDireita != null)
        {
            hitboxSocoDireita.enabled =
                false;
        }

        if (hitboxSocoEsquerda != null)
        {
            hitboxSocoEsquerda.enabled =
                false;
        }
    }

    public void FinalizarAtaque3()
    {
        atacando = false;

        animator.speed = 1f;

        proximoAtaque =
            Time.time +
            recuperacaoSoco;

        perseguicaoAgressivaObrigatoria =
            false;

        TentarPausaDecisao();
    }

    // =========================
    // CANCELAMENTO
    // =========================

    public void CancelarAtaque()
    {
        atacando = false;

        preparandoCorrente = false;
        preparandoRaio = false;

        if (hitboxRaio != null)
        {
            hitboxRaio.SetActive(false);
        }

        if (hitboxOndaDireita != null)
        {
            hitboxOndaDireita.enabled =
                false;
        }

        if (hitboxOndaEsquerda != null)
        {
            hitboxOndaEsquerda.enabled =
                false;
        }

        if (hitboxSocoDireita != null)
        {
            hitboxSocoDireita.enabled =
                false;
        }

        if (hitboxSocoEsquerda != null)
        {
            hitboxSocoEsquerda.enabled =
                false;
        }

        animator.speed = 1f;

        animator.SetBool(
            "estaCorrendo",
            false
        );
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ElianVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;

    private int vidaAtual;

    public bool PrecisaDeCura =>
        !EstaMorto && vidaAtual < vidaMaxima;

    [Header("HUD")]
    public Image barraVida;
    public float velocidadeBarraVida = 1.5f;

    private float valorVisualVida = 1f;

    private Animator animator;
    private ElianDefesa defesa;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private ElianMovimento movimento;
    private ElianBencaos bencaos;

    public bool EstaMorto { get; private set; }

    [Header("Dano")]
    public float forcaEmpurrao = 5f;
    public float forcaEmpurraoVertical = 4f;

    private bool invencivel = false;
    private bool invencivelDash = false;

    public float tempoInvencibilidade = 1f;

    [Header("Segunda Existencia")]
    [Range(1, 100)]
    public int porcentagemVidaSegundaExistencia = 50;

    [Min(0.1f)]
    public float drenagemVidaPorSegundo = 5f;

    private bool segundaExistenciaConsumida = false;
    private bool segundaExistenciaAtiva = false;

    private float acumuladorDrenagem = 0f;

    public bool SegundaExistenciaAtiva =>
        segundaExistenciaAtiva;

    public bool SegundaExistenciaDisponivel =>
        bencaos != null &&
        bencaos.PossuiSegundaExistencia &&
        !segundaExistenciaConsumida &&
        !EstaMorto;

    [Header("Renascimento")]
    public TransicaoTela transicaoTela;

    private bool renascimentoIniciado = false;

    [Header("Audio - Vida")]
    public AudioSource audioVida;
    public AudioClip somDano1;
    public AudioClip somDano2;
    public AudioClip somMorte;

    private bool proximoSomDano1 = true;

    void Start()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        movimento = GetComponent<ElianMovimento>();
        defesa = GetComponent<ElianDefesa>();
        bencaos = GetComponent<ElianBencaos>();

        segundaExistenciaConsumida = false;
        segundaExistenciaAtiva = false;
        acumuladorDrenagem = 0f;

        valorVisualVida = 1f;
        AtualizarBarraVidaImediatamente();
    }

    void Update()
    {
        AtualizarBarraVidaSuave();

        AtualizarDrenagemSegundaExistencia();

        // Atalho de teste que ja existia.
        if (Input.GetKeyDown(KeyCode.H))
        {
            ReceberDano(20);
        }
    }

    public void ReceberDano(int dano)
    {
        if (EstaMorto)
            return;

        if (invencivel || invencivelDash)
            return;

        // ESCUDO
        if (defesa != null && defesa.EstaDefendendo())
        {
            dano = defesa.ReceberDanoEscudo(dano);

            if (dano <= 0)
                return;
        }

        // CANCELA ATAQUES INTERROMPIDOS PELO DANO
        ElianAtaque ataque = GetComponent<ElianAtaque>();

        if (ataque != null)
        {
            ataque.CancelarAtaquePorDano();
        }

        vidaAtual -= dano;

        if (vidaAtual < 0)
            vidaAtual = 0;

        // NOVO: impede a primeira morte fatal.
        if (vidaAtual <= 0 &&
            TentarAtivarSegundaExistencia())
        {
            HitStop.instancia.Executar(0.05f);

            animator.SetTrigger("RecebeuDano");

            AplicarEmpurrao();

            StartCoroutine(TempoInvencibilidade());

            TocarSomDano();

            AtualizarBarraVidaImediatamente();

            return;
        }

        Debug.Log("Vida do Elian: " + vidaAtual);

        HitStop.instancia.Executar(0.05f);

        animator.SetTrigger("RecebeuDano");

        AplicarEmpurrao();

        StartCoroutine(TempoInvencibilidade());

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            TocarSomDano();
        }
    }

    private bool TentarAtivarSegundaExistencia()
    {
        if (!SegundaExistenciaDisponivel)
            return false;

        segundaExistenciaConsumida = true;
        segundaExistenciaAtiva = true;

        acumuladorDrenagem = 0f;

        vidaAtual = Mathf.Clamp(
            Mathf.CeilToInt(
                vidaMaxima *
                (porcentagemVidaSegundaExistencia / 100f)
            ),
            1,
            vidaMaxima
        );

        Debug.Log(
            "SEGUNDA EXISTENCIA ATIVADA! " +
            "Vida restaurada: " + vidaAtual
        );

        return true;
    }

    private void AtualizarDrenagemSegundaExistencia()
    {
        if (!segundaExistenciaAtiva || EstaMorto)
            return;

        // O Time.deltaTime respeita pausas e tutoriais.
        acumuladorDrenagem +=
            drenagemVidaPorSegundo * Time.deltaTime;

        int pontosPerdidos =
            Mathf.FloorToInt(acumuladorDrenagem);

        if (pontosPerdidos <= 0)
            return;

        acumuladorDrenagem -= pontosPerdidos;

        // A drenagem ignora escudo e invencibilidade.
        vidaAtual = Mathf.Max(
            0,
            vidaAtual - pontosPerdidos
        );

        if (vidaAtual <= 0)
        {
            segundaExistenciaAtiva = false;

            Debug.Log(
                "Segunda Existencia esgotada!"
            );

            Morrer();
        }
    }

    void Morrer()
    {
        if (EstaMorto)
            return;

        EstaMorto = true;
        segundaExistenciaAtiva = false;

        movimento.podeMover = false;

        rb.linearVelocity = Vector2.zero;

        animator.SetFloat("velocidade", 0f);
        animator.SetTrigger("Morreu");

        TocarSomVida(somMorte);

        Debug.Log("Elian morreu!");
    }

    // Animation Event no final da animacao de morte
    public void FinalizarMorte()
    {
        if (!EstaMorto || renascimentoIniciado)
            return;

        Debug.Log(
            "Animacao de morte do Elian finalizada!"
        );

        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogWarning(
                "Nenhum slot selecionado. " +
                "Renascimento nao iniciado."
            );
            return;
        }

        if (GerenciadorSalvamento.Carregar(slot) == null)
        {
            Debug.LogError(
                "Nao foi possivel carregar o salvamento. " +
                "Renascimento cancelado."
            );
            return;
        }

        if (transicaoTela == null)
        {
            Debug.LogError(
                "TransicaoTela nao configurada no ElianVida!"
            );
            return;
        }

        renascimentoIniciado = true;

        StartCoroutine(ExecutarRenascimento());
    }

    IEnumerator ExecutarRenascimento()
    {
        Debug.Log("Iniciando renascimento do Elian...");

        yield return StartCoroutine(
            transicaoTela.Escurecer()
        );

        Debug.Log("Recarregando Gameplay...");

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void AtivarInvencibilidadeDash()
    {
        invencivelDash = true;
    }

    public void DesativarInvencibilidadeDash()
    {
        invencivelDash = false;
    }

    IEnumerator TempoInvencibilidade()
    {
        invencivel = true;

        float tempoPiscada = 0.1f;
        float tempoPassado = 0f;

        while (tempoPassado < tempoInvencibilidade)
        {
            if (EstaMorto)
                break;

            spriteRenderer.enabled = false;

            yield return new WaitForSeconds(tempoPiscada);

            spriteRenderer.enabled = true;

            yield return new WaitForSeconds(tempoPiscada);

            tempoPassado += tempoPiscada * 2;
        }

        spriteRenderer.enabled = true;
        invencivel = false;
    }

    void AplicarEmpurrao()
    {
        if (EstaMorto)
            return;

        movimento.podeMover = false;

        float direcao =
            spriteRenderer.flipX ? 1f : -1f;

        rb.linearVelocity = new Vector2(
            direcao * forcaEmpurrao,
            forcaEmpurraoVertical
        );

        StartCoroutine(LiberarMovimento());
    }

    IEnumerator LiberarMovimento()
    {
        yield return new WaitForSeconds(0.2f);

        if (!EstaMorto)
            movimento.podeMover = true;
    }

    public void Curar(int quantidade)
    {
        if (EstaMorto || quantidade <= 0)
            return;

        vidaAtual += quantidade;

        if (vidaAtual > vidaMaxima)
            vidaAtual = vidaMaxima;

        Debug.Log(
            "Elian foi curado! Vida atual: " + vidaAtual
        );

        // A cura NAO interrompe a drenagem.
    }

    void AtualizarBarraVidaSuave()
    {
        if (barraVida == null || vidaMaxima <= 0)
            return;

        float alvo =
            (float)vidaAtual / vidaMaxima;

        valorVisualVida = Mathf.MoveTowards(
            valorVisualVida,
            alvo,
            velocidadeBarraVida * Time.deltaTime
        );

        barraVida.fillAmount = valorVisualVida;
    }

    void AtualizarBarraVidaImediatamente()
    {
        if (barraVida == null || vidaMaxima <= 0)
            return;

        valorVisualVida =
            (float)vidaAtual / vidaMaxima;

        barraVida.fillAmount = valorVisualVida;
    }

    void TocarSomDano()
    {
        AudioClip somEscolhido =
            proximoSomDano1 ? somDano1 : somDano2;

        if (somEscolhido == null)
        {
            somEscolhido =
                proximoSomDano1 ? somDano2 : somDano1;
        }

        proximoSomDano1 = !proximoSomDano1;

        TocarSomVida(somEscolhido);
    }

    void TocarSomVida(AudioClip som)
    {
        if (audioVida != null && som != null)
        {
            audioVida.PlayOneShot(som);
        }
    }
}

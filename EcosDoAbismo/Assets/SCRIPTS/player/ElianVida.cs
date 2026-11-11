
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ElianVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;

    private int vidaAtual;

    [Header("HUD")]
    public Image barraVida;
    public float velocidadeBarraVida = 1.5f;

    private float valorVisualVida = 1f;

    private Animator animator;
    private ElianDefesa defesa;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    public bool EstaMorto { get; private set; }

    [Header("Dano")]
    public float forcaEmpurrao = 5f;
    public float forcaEmpurraoVertical = 4f;

    private bool invencivel = false;
    private bool invencivelDash = false;

    public float tempoInvencibilidade = 1f;

    [Header("Renascimento")]
    public TransicaoTela transicaoTela;

    private bool renascimentoIniciado = false;

    private ElianMovimento movimento;

    [Header("Audio - Vida")]
    public AudioSource audioVida;
    public AudioClip somDano1;
    public AudioClip somDano2;
    public AudioClip somMorte;

    // NOVO: alterna os grunhidos para evitar repetição
    private bool proximoSomDano1 = true;

    void Start()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        movimento = GetComponent<ElianMovimento>();
        defesa = GetComponent<ElianDefesa>();

        valorVisualVida = 1f;
        AtualizarBarraVidaImediatamente();
    }

    void Update()
    {
        AtualizarBarraVidaSuave();

        if (Input.GetKeyDown(KeyCode.H))
        {
            ReceberDano(20);
        }
    }

    public void ReceberDano(int dano)
    {
        if (EstaMorto)
        {
            return;
        }

        if (invencivel || invencivelDash)
        {
            return;
        }

        // ESCUDO
        if (defesa != null && defesa.EstaDefendendo())
        {
            dano = defesa.ReceberDanoEscudo(dano);

            if (dano <= 0)
            {
                return;
            }
        }

        // CANCELA ATAQUES INTERROMPIDOS PELO DANO
        ElianAtaque ataque = GetComponent<ElianAtaque>();

        if (ataque != null)
        {
            ataque.CancelarAtaquePorDano();
        }

        vidaAtual -= dano;

        if (vidaAtual < 0)
        {
            vidaAtual = 0;
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
            // NOVO: alterna entre os dois sons de dano
            TocarSomDano();
        }
    }

    void Morrer()
    {
        if (EstaMorto)
            return;

        EstaMorto = true;

        movimento.podeMover = false;

        rb.linearVelocity = Vector2.zero;

        animator.SetFloat("velocidade", 0f);
        animator.SetTrigger("Morreu");

        TocarSomVida(somMorte);

        Debug.Log("Elian morreu!");
    }

    // Animation Event no final da animação de morte
    public void FinalizarMorte()
    {
        if (!EstaMorto || renascimentoIniciado)
            return;

        Debug.Log("Animação de morte do Elian finalizada!");

        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogWarning(
                "Nenhum slot selecionado. " +
                "Renascimento não iniciado."
            );
            return;
        }

        if (GerenciadorSalvamento.Carregar(slot) == null)
        {
            Debug.LogError(
                "Não foi possível carregar o salvamento. " +
                "Renascimento cancelado."
            );
            return;
        }

        if (transicaoTela == null)
        {
            Debug.LogError(
                "TransicaoTela não configurada no ElianVida!"
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

        float direcao = spriteRenderer.flipX ? 1f : -1f;

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
        vidaAtual += quantidade;

        if (vidaAtual > vidaMaxima)
        {
            vidaAtual = vidaMaxima;
        }

        Debug.Log("Elian foi curado! Vida atual: " + vidaAtual);
    }

    void AtualizarBarraVidaSuave()
    {
        if (barraVida == null)
        {
            return;
        }

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
        if (barraVida == null)
        {
            return;
        }

        valorVisualVida =
            (float)vidaAtual / vidaMaxima;

        barraVida.fillAmount =
            valorVisualVida;
    }

    // NOVO: seleção alternada dos grunhidos
    void TocarSomDano()
    {
        AudioClip somEscolhido =
            proximoSomDano1 ? somDano1 : somDano2;

        // Permite usar apenas um som enquanto o outro não é configurado.
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

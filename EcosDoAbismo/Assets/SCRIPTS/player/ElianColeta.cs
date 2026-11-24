
using UnityEngine;

public class ElianColeta : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private Rigidbody2D rb;

    private ItemColetavel itemPerto;
    private ReliquiaColetavel reliquiaPerto;
    private CheckpointArvore arvorePerto;
    private AnciaoInteracao anciaoPerto;

    // Poções de cura
    private PocaoColetavel pocaoPerto;

    private bool interagindo = false;

    private bool coletandoBencao = false;
    private AnciaoInteracao anciaoDaBencao;

    [Header("Interface de Viagem")]
    public GameObject telaViagem;

    private bool aguardandoFechamentoViagem = false;

    [Header("Audio - Interacoes")]
    public AudioSource audioInteracoes;
    public AudioClip somAtivarCheckpoint;
    public AudioClip somColetarReliquia;

    public bool MenuViagemAberto
    {
        get
        {
            return telaViagem != null &&
                   telaViagem.activeInHierarchy;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (aguardandoFechamentoViagem && !MenuViagemAberto)
        {
            aguardandoFechamentoViagem = false;

            ElianVida vida = GetComponent<ElianVida>();

            if (vida == null || !vida.EstaMorto)
            {
                movimento.podeMover = true;
            }
        }

        if (
            ControlesElian.Pressionou(ComandoElian.Interagir) &&
            animator.GetBool("estaNoChao") &&
            movimento.podeMover &&
            !interagindo &&
            !MenuViagemAberto
        )
        {
            if (anciaoPerto != null)
            {
                anciaoPerto.AbrirConversa();
                return;
            }

            if (
                itemPerto != null ||
                reliquiaPerto != null ||
                arvorePerto != null ||
                (pocaoPerto != null && pocaoPerto.PodeColetar())
            )
            {
                interagindo = true;

                movimento.podeMover = false;

                // Impede o Elian de deslizar na coleta.
                rb.linearVelocity = new Vector2(
                    0f,
                    rb.linearVelocity.y
                );

                animator.SetFloat("velocidade", 0);
                animator.SetTrigger("Coletar");
            }
        }
    }

    public void DefinirAnciaoPerto(AnciaoInteracao anciao)
    {
        anciaoPerto = anciao;
    }

    public void DefinirItemPerto(ItemColetavel item)
    {
        itemPerto = item;
    }

    public void DefinirReliquiaPerto(
        ReliquiaColetavel reliquia)
    {
        reliquiaPerto = reliquia;
    }

    public void DefinirArvorePerto(CheckpointArvore arvore)
    {
        arvorePerto = arvore;
    }

    // Registra uma poção próxima
    public void DefinirPocaoPerto(PocaoColetavel pocao)
    {
        if (pocaoPerto == null)
        {
            pocaoPerto = pocao;
        }
    }

    // Remove somente a poção correspondente
    public void RemoverPocaoPerto(PocaoColetavel pocao)
    {
        if (pocaoPerto == pocao)
        {
            pocaoPerto = null;
        }
    }

    public void IniciarBencao(AnciaoInteracao anciao)
    {
        if (interagindo || anciao == null)
            return;

        interagindo = true;
        coletandoBencao = true;
        anciaoDaBencao = anciao;

        movimento.podeMover = false;

        // Impede o Elian de deslizar ao receber bênçãos.
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );

        animator.SetFloat("velocidade", 0f);
        animator.SetTrigger("Coletar");
    }

    public void ExecutarColeta()
    {
        if (coletandoBencao)
        {
            if (anciaoDaBencao != null)
            {
                anciaoDaBencao.ConcederBencao();
            }

            return;
        }

        if (reliquiaPerto != null)
        {
            reliquiaPerto.Coletar();
            reliquiaPerto = null;
        }
        else if (itemPerto != null)
        {
            itemPerto.Coletar();
            itemPerto = null;
        }
        else if (
            pocaoPerto != null &&
            pocaoPerto.PodeColetar()
        )
        {
            pocaoPerto.Coletar();
            pocaoPerto = null;
        }
        else if (arvorePerto != null)
        {
            arvorePerto.Interagir();
        }
    }

    public void FinalizarColeta()
    {
        if (coletandoBencao)
        {
            AnciaoInteracao anciao =
                anciaoDaBencao;

            coletandoBencao = false;
            anciaoDaBencao = null;
            interagindo = false;

            ElianVida vida = GetComponent<ElianVida>();

            if (vida == null || !vida.EstaMorto)
            {
                movimento.podeMover = true;
            }

            // Exibe o tutorial somente depois
            // da animacao de coleta.
            if (anciao != null)
            {
                anciao.FinalizarRecebimentoBencao();
            }

            return;
        }

        interagindo = false;

        if (MenuViagemAberto)
        {
            aguardandoFechamentoViagem = true;
            movimento.podeMover = false;
        }
        else
        {
            movimento.podeMover = true;
        }
    }

    public void TocarSomAtivacaoCheckpoint()
    {
        TocarSomInteracao(somAtivarCheckpoint);
    }

    public void TocarSomColetaReliquia()
    {
        TocarSomInteracao(somColetarReliquia);
    }

    private void TocarSomInteracao(AudioClip som)
    {
        if (audioInteracoes != null && som != null)
        {
            audioInteracoes.PlayOneShot(som);
        }
    }
}


using UnityEngine;

public class ElianColeta : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private ItemColetavel itemPerto;
    private ReliquiaColetavel reliquiaPerto;
    private CheckpointArvore arvorePerto;
    private AnciaoInteracao anciaoPerto;

    private bool interagindo = false;

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
            Input.GetKeyDown(KeyCode.E) &&
            animator.GetBool("estaNoChao") &&
            movimento.podeMover &&
            !interagindo &&
            !MenuViagemAberto
        )
        {
            // Conversar com o Anciao sem animacao de coleta
            if (anciaoPerto != null)
            {
                anciaoPerto.AbrirConversa();
                return;
            }

            if (
                itemPerto != null ||
                reliquiaPerto != null ||
                arvorePerto != null
            )
            {
                interagindo = true;

                movimento.podeMover = false;
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

    public void DefinirReliquiaPerto(ReliquiaColetavel reliquia)
    {
        reliquiaPerto = reliquia;
    }

    public void DefinirArvorePerto(CheckpointArvore arvore)
    {
        arvorePerto = arvore;
    }

    public void ExecutarColeta()
    {
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
        else if (arvorePerto != null)
        {
            arvorePerto.Interagir();
        }
    }

    public void FinalizarColeta()
    {
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

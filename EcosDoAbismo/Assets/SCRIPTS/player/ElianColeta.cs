
using UnityEngine;

public class ElianColeta : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private ItemColetavel itemPerto;

    private CheckpointArvore arvorePerto;
    private bool interagindo = false;

    [Header("Interface de Viagem")]
    public GameObject telaViagem;

    private bool aguardandoFechamentoViagem = false;

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
        // Libera o movimento após fechar a viagem,
        // desde que a animação de coleta já tenha terminado.
        if (aguardandoFechamentoViagem && !MenuViagemAberto)
        {
            aguardandoFechamentoViagem = false;

            ElianVida vida = GetComponent<ElianVida>();

            if (vida == null || !vida.EstaMorto)
            {
                movimento.podeMover = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) &&
            animator.GetBool("estaNoChao") &&
            movimento.podeMover &&
            !interagindo &&
            !MenuViagemAberto &&
            (itemPerto != null || arvorePerto != null))
        {
            interagindo = true;

            movimento.podeMover = false;
            animator.SetFloat("velocidade", 0);
            animator.SetTrigger("Coletar");
        }
    }

    public void DefinirItemPerto(ItemColetavel item)
    {
        itemPerto = item;
    }

    public void DefinirArvorePerto(CheckpointArvore arvore)
    {
        arvorePerto = arvore;
    }

    public void ExecutarColeta()
    {
        if (itemPerto != null)
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
}

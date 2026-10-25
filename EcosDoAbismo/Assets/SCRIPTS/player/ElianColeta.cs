
using UnityEngine;

public class ElianColeta : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private ItemColetavel itemPerto;

    private CheckpointArvore arvorePerto;
    private bool interagindo = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) &&
            animator.GetBool("estaNoChao") &&
            movimento.podeMover &&
            !interagindo &&
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
        movimento.podeMover = true;
        interagindo = false;
    }
}

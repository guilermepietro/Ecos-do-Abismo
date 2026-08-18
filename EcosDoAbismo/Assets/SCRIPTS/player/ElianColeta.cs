using UnityEngine;

public class ElianColeta : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private ItemColetavel itemPerto;

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && animator.GetBool("estaNoChao") && itemPerto != null)
        {
            movimento.podeMover = false;
            animator.SetFloat("velocidade", 0);
            animator.SetTrigger("Coletar");
        }
    }

    public void DefinirItemPerto(ItemColetavel item)
{
    itemPerto = item;
}

    public void ExecutarColeta()
{
    if (itemPerto != null)
    {
        itemPerto.Coletar();
        itemPerto = null;
    }
}

    public void FinalizarColeta()
    {
        movimento.podeMover = true;
    }
}
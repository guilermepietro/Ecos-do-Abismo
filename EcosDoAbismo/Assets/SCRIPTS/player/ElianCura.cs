
using UnityEngine;

public class ElianCura : MonoBehaviour
{
    private Animator animator;
    private ElianMovimento movimento;
    private ElianVida vida;
    private ElianPocoes pocoes;
    private ElianColeta coleta;

    private bool curando = false;
    private bool curaExecutada = false;

    [Header("Configurações")]
    public int quantidadeCura = 30;

    void Start()
    {
        animator = GetComponent<Animator>();
        movimento = GetComponent<ElianMovimento>();
        vida = GetComponent<ElianVida>();
        pocoes = GetComponent<ElianPocoes>();
        coleta = GetComponent<ElianColeta>();
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.L))
            return;

        if (curando)
            return;

        if (!animator.GetBool("estaNoChao"))
            return;

        if (!movimento.podeMover)
            return;

        // Impede cura com a vida cheia ou após morte
        if (vida == null || !vida.PrecisaDeCura)
            return;

        if (pocoes == null || !pocoes.TemPocao)
            return;

        if (coleta != null && coleta.MenuViagemAberto)
            return;

        curando = true;
        curaExecutada = false;

        movimento.podeMover = false;
        animator.SetFloat("velocidade", 0);
        animator.SetTrigger("Cura");
    }

    // Animation Event da animacao Cura
    public void ExecutarCura()
    {
        if (!curando || curaExecutada)
            return;

        // Verifica novamente antes de consumir
        if (vida == null || !vida.PrecisaDeCura)
            return;

        if (pocoes == null || !pocoes.ConsumirPocao())
            return;

        curaExecutada = true;

        vida.Curar(quantidadeCura);
    }

    // Animation Event do final da animacao Cura
    public void FinalizarCura()
    {
        curando = false;
        curaExecutada = false;

        if (vida != null && vida.EstaMorto)
            return;

        if (coleta != null && coleta.MenuViagemAberto)
            return;

        movimento.podeMover = true;
    }
}

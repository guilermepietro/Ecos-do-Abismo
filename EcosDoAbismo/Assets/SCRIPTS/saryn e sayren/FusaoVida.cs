
using UnityEngine;

public class FusaoVida : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField, Min(1)] private int vidaMaxima = 500;
    [SerializeField] private int vidaAtual;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D corpo;
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private FusaoIA ia;

    [Header("Ataques")]
    [SerializeField] private FusaoAtaque1 ataque1;
    [SerializeField] private FusaoAtaque2 ataque2;
    [SerializeField] private FusaoAtaque3 ataque3;

    [Header("Morte")]
    [SerializeField] private bool usarAnimacaoMorte = false;

    private bool morto;
    private bool morteFinalizada;

    public int VidaAtual => vidaAtual;
    public int VidaMaxima => vidaMaxima;

    public float PorcentagemVida =>
        vidaMaxima > 0
            ? (float)vidaAtual / vidaMaxima
            : 0f;

    public bool Morto => morto;
    public bool EstaMorto => morto;
    public bool MorteFinalizada => morteFinalizada;

    public bool PodeReceberDano => !morto;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (corpo == null)
            corpo = GetComponent<Rigidbody2D>();

        if (movimento == null)
            movimento = GetComponent<FusaoMovimento>();

        if (ia == null)
            ia = GetComponent<FusaoIA>();

        if (ataque1 == null)
            ataque1 = GetComponent<FusaoAtaque1>();

        if (ataque2 == null)
            ataque2 = GetComponent<FusaoAtaque2>();

        if (ataque3 == null)
            ataque3 = GetComponent<FusaoAtaque3>();

        vidaAtual = vidaMaxima;

        morto = false;
        morteFinalizada = false;
    }

    public void ReceberDano(int dano)
    {
        if (dano <= 0 || !PodeReceberDano)
            return;

        vidaAtual = Mathf.Max(
            0,
            vidaAtual - dano
        );

        Debug.Log(
            "Fusao Saryn & Sayren - Vida: " +
            vidaAtual + "/" + vidaMaxima
        );

        if (vidaAtual <= 0)
            IniciarMorte();
    }

    private void IniciarMorte()
    {
        if (morto)
            return;

        morto = true;
        morteFinalizada = false;

        Debug.Log(
            "Fusao Saryn & Sayren: morte iniciada!"
        );

        // Interrompe as decisoes da IA.
        if (ia != null)
            ia.enabled = false;

        // Para e desativa a movimentacao.
        if (movimento != null)
        {
            movimento.DesativarCombate();
            movimento.BloquearMovimento(true);
            movimento.enabled = false;
        }

        // Interrompe os ataques.
        if (ataque1 != null)
            ataque1.enabled = false;

        if (ataque2 != null)
        {
            ataque2.StopAllCoroutines();
            ataque2.enabled = false;
        }

        if (ataque3 != null)
        {
            ataque3.StopAllCoroutines();
            ataque3.enabled = false;
        }

        // Interrompe o deslocamento horizontal.
        if (corpo != null)
        {
            corpo.linearVelocity = new Vector2(
                0f,
                corpo.linearVelocity.y
            );
        }

        if (animator != null)
        {
            animator.SetBool("Andando", false);

            animator.ResetTrigger("Ataque1");
            animator.ResetTrigger("Ataque2");
            animator.ResetTrigger("Ataque3");

            if (usarAnimacaoMorte)
            {
                animator.SetTrigger("Morte");
            }
            else
            {
                // Mantem o ultimo sprite visivel.
                animator.enabled = false;
                FinalizarMorte();
            }
        }
        else
        {
            FinalizarMorte();
        }
    }

    // Animation Event no ultimo frame da morte.
    public void FinalizarMorte()
    {
        if (!morto || morteFinalizada)
            return;

        morteFinalizada = true;

        // Congela no ultimo frame da animacao.
        if (animator != null)
            animator.enabled = false;

        Debug.Log(
            "Fusao Saryn & Sayren: morte definitiva!"
        );
    }

    [ContextMenu("TESTE - Perder 100 de vida")]
    private void TestarDano()
    {
        if (Application.isPlaying)
            ReceberDano(100);
    }
}

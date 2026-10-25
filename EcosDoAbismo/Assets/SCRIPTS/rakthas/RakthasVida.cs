
using UnityEngine;

public class RakthasVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 300;

    private int vidaAtual;
    private bool morto = false;
    private bool tomandoDano = false;

    private RakthasAtaques ataques;

    [Header("Referências")]
    public Animator animator;

    public bool EstaTomandoDano => tomandoDano;
    public bool EstaMorto => morto;

    private void Start()
    {
        vidaAtual = vidaMaxima;
        ataques = GetComponent<RakthasAtaques>();

        VerificarSalvamento();
    }

    private void VerificarSalvamento()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null || dados.chefesDerrotados == null)
            return;

        if (dados.chefesDerrotados.Contains("Rakthas"))
        {
            Debug.Log(
                "Rakthas já foi derrotado no slot " + slot
            );

            gameObject.SetActive(false);
        }
    }

    public void ReceberDano(int dano)
    {
        if (morto)
            return;

        vidaAtual -= dano;

        // MORTE
        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        // SE ESTÁ ATACANDO:
        // recebe dano, mas não interrompe o ataque
        if (ataques != null && ataques.EstaAtacando)
        {
            return;
        }

        // DANO NORMAL
        tomandoDano = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }

        animator.SetBool("estaCorrendo", false);
        animator.speed = 1f;
        animator.SetTrigger("Dano");
    }

    private void Morrer()
    {
        morto = true;
        tomandoDano = false;

        // NA MORTE, CANCELA QUALQUER ATAQUE
        if (ataques != null)
        {
            ataques.CancelarAtaque();
        }

        animator.speed = 1f;
        animator.SetBool("estaCorrendo", false);
        animator.SetTrigger("Morte");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }

        // Registra a derrota no salvamento
        RegistrarDerrota();
    }

    private void RegistrarDerrota()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogWarning(
                "Rakthas derrotado sem slot selecionado. " +
                "A derrota não será salva."
            );
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
        {
            Debug.LogError(
                "Não foi possível carregar o salvamento " +
                "para registrar a derrota do Rakthas."
            );
            return;
        }

        if (dados.chefesDerrotados == null)
        {
            Debug.LogError(
                "Lista de chefes derrotados indisponível."
            );
            return;
        }

        if (!dados.chefesDerrotados.Contains("Rakthas"))
        {
            dados.chefesDerrotados.Add("Rakthas");

            if (GerenciadorSalvamento.Salvar(slot, dados))
            {
                Debug.Log(
                    "Rakthas derrotado permanentemente!" +
                    "\nSlot: " + slot
                );
            }
            else
            {
                Debug.LogError(
                    "Falha ao salvar a derrota do Rakthas."
                );
            }
        }
    }

    public void FinalizarDano()
    {
        tomandoDano = false;
    }
}

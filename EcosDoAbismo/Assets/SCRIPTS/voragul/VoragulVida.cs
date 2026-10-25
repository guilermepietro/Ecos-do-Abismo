
using UnityEngine;

public class VoragulVida : MonoBehaviour
{
    public int vidaMaxima = 500;

    private int vidaAtual;
    private bool morto = false;
    private bool levandoDano = false;

    private Animator animator;

    private void Awake()
    {
        vidaAtual = vidaMaxima;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
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

        if (dados.chefesDerrotados.Contains("Voragul"))
        {
            Debug.Log(
                "Voragul já foi derrotado no slot " + slot
            );

            gameObject.SetActive(false);
        }
    }

    public void ReceberDano(int dano)
    {
        if (morto)
            return;

        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            Morrer();
            return;
        }

        VoragulAtaque1 ataque1 = GetComponent<VoragulAtaque1>();

        if (ataque1 != null && ataque1.EstaAtacando())
        {
            return;
        }

        VoragulAtaque2 ataque2 = GetComponent<VoragulAtaque2>();

        if (ataque2 != null && ataque2.EstaAtacando())
        {
            return;
        }

        VoragulAtaque3 ataque3 = GetComponent<VoragulAtaque3>();

        if (ataque3 != null && ataque3.EstaAtacando())
        {
            return;
        }

        if (!levandoDano)
        {
            levandoDano = true;

            VoragulMovimento movimento =
                GetComponent<VoragulMovimento>();

            if (movimento != null)
            {
                movimento.podeMover = false;
            }

            animator.SetTrigger("Dano");
        }
    }

    private void Morrer()
    {
        morto = true;

        CancelInvoke(nameof(LiberarMovimento));

        VoragulMovimento movimento =
            GetComponent<VoragulMovimento>();

        if (movimento != null)
        {
            movimento.podeMover = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }

        RegistrarDerrota();

        animator.SetTrigger("Morte");
    }

    private void RegistrarDerrota()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogWarning(
                "Voragul derrotado sem slot selecionado. " +
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
                "para registrar a derrota do Voragul."
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

        if (!dados.chefesDerrotados.Contains("Voragul"))
        {
            dados.chefesDerrotados.Add("Voragul");

            // Atualiza o tempo antes de gravar a derrota
            if (ContadorTempoJogo.instancia != null)
            {
                ContadorTempoJogo.instancia
                    .AtualizarTempoNosDados(dados);
            }

            if (GerenciadorSalvamento.Salvar(slot, dados))
            {
                Debug.Log(
                    "Voragul derrotado permanentemente!" +
                    "\nSlot: " + slot +
                    "\nTempo: " +
                    dados.tempoTotalJogado.ToString("F1") +
                    " segundos"
                );
            }
            else
            {
                Debug.LogError(
                    "Falha ao salvar a derrota do Voragul."
                );
            }
        }
    }

    public void FinalizarDano()
    {
        levandoDano = false;

        Invoke(nameof(LiberarMovimento), 1.1f);
    }

    private void LiberarMovimento()
    {
        if (morto)
            return;

        VoragulMovimento movimento =
            GetComponent<VoragulMovimento>();

        if (movimento != null)
        {
            movimento.podeMover = true;
        }
    }

    public bool EstaMorto()
    {
        return morto;
    }

    public float ObterPorcentagemVida()
    {
        if (vidaMaxima <= 0)
            return 0f;

        return (float)vidaAtual / vidaMaxima;
    }
}

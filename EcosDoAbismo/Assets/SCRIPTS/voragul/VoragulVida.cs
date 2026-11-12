
using UnityEngine;

public class VoragulVida : MonoBehaviour
{
    public int vidaMaxima = 500;

    [Header("Relíquia")]
    public GameObject prefabReliquia;
    public Transform pontoDropReliquia;

    private int vidaAtual;
    private bool morto = false;
    private bool levandoDano = false;
    private bool reliquiaDropada = false;

    private Animator animator;

    // NOVO
    private AudioVoragul audioVoragul;

    private void Awake()
    {
        vidaAtual = vidaMaxima;
        animator = GetComponent<Animator>();

        // NOVO
        audioVoragul = GetComponent<AudioVoragul>();
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

            bool coroaJaColetada =
                dados.reliquiasObtidas != null &&
                dados.reliquiasObtidas.Contains("CoroaDoAbismo");

            if (!coroaJaColetada)
            {
                DroparReliquia();
            }

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

        // NOVO: som sempre que perde vida e sobrevive
        if (audioVoragul != null)
        {
            audioVoragul.TocarDano();
        }

        VoragulAtaque1 ataque1 =
            GetComponent<VoragulAtaque1>();

        if (ataque1 != null && ataque1.EstaAtacando())
        {
            return;
        }

        VoragulAtaque2 ataque2 =
            GetComponent<VoragulAtaque2>();

        if (ataque2 != null && ataque2.EstaAtacando())
        {
            return;
        }

        VoragulAtaque3 ataque3 =
            GetComponent<VoragulAtaque3>();

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

        DesativarColliders();

        RegistrarDerrota();

        animator.SetTrigger("Morte");

        // NOVO: som exclusivo de morte
        if (audioVoragul != null)
        {
            audioVoragul.TocarMorte();
        }
    }

    private void DesativarColliders()
    {
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void DroparReliquia()
    {
        if (reliquiaDropada)
            return;

        if (prefabReliquia == null)
        {
            Debug.LogWarning(
                "Prefab da Coroa do Abismo não foi configurado."
            );

            return;
        }

        if (pontoDropReliquia == null)
        {
            Debug.LogWarning(
                "Ponto de drop da Coroa do Abismo não foi configurado."
            );

            return;
        }

        Instantiate(
            prefabReliquia,
            pontoDropReliquia.position,
            Quaternion.identity
        );

        reliquiaDropada = true;
    }

    public void FinalizarMorte()
    {
        gameObject.SetActive(false);
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

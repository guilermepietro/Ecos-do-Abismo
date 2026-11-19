
using UnityEngine;

public class VoragulVida : MonoBehaviour
{
    public int vidaMaxima = 500;

    [Header("Relíquia")]
    public GameObject prefabReliquia;
    public Transform pontoDropReliquia;

    [Header("Cutscene de Morte")]
    public string idCutsceneMorte = "Voragul_Morte";

    [Header("Interface")]
    public GameObject barraVidaVisual;

    private int vidaAtual;

    private bool morto = false;
    private bool reliquiaDropada = false;
    private bool morteFinalizada = false;
    private bool cutsceneMorteIniciada = false;

    private Animator animator;
    private AudioVoragul audioVoragul;

    private void Awake()
    {
        vidaAtual = vidaMaxima;

        animator = GetComponent<Animator>();
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

        if (dados == null ||
            dados.chefesDerrotados == null)
        {
            return;
        }

        if (dados.chefesDerrotados.Contains("Voragul"))
        {
            Debug.Log(
                "Voragul já foi derrotado no slot " +
                slot
            );

            bool coroaJaColetada =
                dados.reliquiasObtidas != null &&
                dados.reliquiasObtidas.Contains(
                    "CoroaDoAbismo"
                );

            if (!coroaJaColetada)
            {
                DroparReliquia();
            }

            if (barraVidaVisual != null)
            {
                barraVidaVisual.SetActive(false);
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

        // O Voragul recebe o dano normalmente,
        // mas não toca animação de dano
        // e não interrompe movimento ou ataques.
        if (audioVoragul != null)
        {
            audioVoragul.TocarDano();
        }
    }

    private void Morrer()
    {
        if (morto)
            return;

        morto = true;

        // NOVO: libera os teletransportes.
        Teletransporte.LiberarBloqueio();

        VoragulMovimento movimento =
            GetComponent<VoragulMovimento>();

        if (movimento != null)
        {
            movimento.podeMover = false;
            movimento.lutaIniciada = false;
        }

        // Congela completamente o Voragul
        // no ponto onde morreu.
        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.simulated = false;
        }

        // Evita que o corpo ou as hitboxes
        // atrapalhem o Elian durante a morte.
        DesativarColliders();

        // Para passos e outros sons
        // que poderiam continuar tocando.
        if (audioVoragul != null)
        {
            if (audioVoragul.audioPassos != null)
            {
                audioVoragul.audioPassos.Stop();
            }

            AudioSource efeitos =
                audioVoragul.GetComponent<AudioSource>();

            if (efeitos != null)
            {
                efeitos.Stop();
            }
        }

        // A derrota continua sendo salva
        // imediatamente.
        RegistrarDerrota();

        // Inicia a animação de morte.
        if (animator != null)
        {
            animator.SetTrigger("Morte");
        }

        // Som da morte.
        if (audioVoragul != null)
        {
            audioVoragul.TocarMorte();
        }

        Debug.Log(
            "Voragul iniciou sua animação de morte."
        );

        // A cutscene será chamada pelo
        // Animation Event no final do clip.
    }

    public void IniciarCutsceneMorte()
    {
        if (!morto ||
            cutsceneMorteIniciada)
        {
            return;
        }

        cutsceneMorteIniciada = true;

        if (
            GerenciadorCutscenes.Instancia != null
        )
        {
            bool iniciou =
                GerenciadorCutscenes
                .Instancia
                .Reproduzir(
                    idCutsceneMorte,
                    FinalizarMorte
                );

            if (iniciou)
            {
                Debug.Log(
                    "Animação do Voragul concluída! " +
                    "Iniciando cutscene de morte."
                );

                return;
            }
        }

        Debug.LogWarning(
            "Cutscene de morte do Voragul " +
            "indisponível. Finalizando normalmente."
        );

        FinalizarMorte();
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
                "Prefab da Coroa do Abismo " +
                "não configurado."
            );

            return;
        }

        if (pontoDropReliquia == null)
        {
            Debug.LogWarning(
                "Ponto de drop da Coroa " +
                "não configurado."
            );

            return;
        }

        Instantiate(
            prefabReliquia,
            pontoDropReliquia.position,
            Quaternion.identity
        );

        reliquiaDropada = true;

        Debug.Log(
            "Coroa do Abismo deixada na arena."
        );
    }

    public void FinalizarMorte()
    {
        if (!morto ||
            morteFinalizada)
        {
            return;
        }

        morteFinalizada = true;

        // A relíquia aparece somente
        // depois da cutscene.
        DroparReliquia();

        if (barraVidaVisual != null)
        {
            barraVidaVisual.SetActive(false);
        }

        gameObject.SetActive(false);

        Debug.Log(
            "Voragul desapareceu após a cutscene."
        );
    }

    private void RegistrarDerrota()
    {
        int slot =
            MenuSelecaoSave.slotPartidaAtual;

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

        if (
            !dados.chefesDerrotados.Contains(
                "Voragul"
            )
        )
        {
            dados.chefesDerrotados.Add(
                "Voragul"
            );

            if (
                ContadorTempoJogo.instancia != null
            )
            {
                ContadorTempoJogo
                    .instancia
                    .AtualizarTempoNosDados(dados);
            }

            if (
                GerenciadorSalvamento.Salvar(
                    slot,
                    dados
                )
            )
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

    public bool EstaMorto()
    {
        return morto;
    }

    public float ObterPorcentagemVida()
    {
        if (vidaMaxima <= 0)
            return 0f;

        return (float)vidaAtual /
               vidaMaxima;
    }
}

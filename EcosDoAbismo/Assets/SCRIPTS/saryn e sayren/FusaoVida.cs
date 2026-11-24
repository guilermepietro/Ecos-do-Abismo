using UnityEngine;
using System.Collections.Generic;

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

    [Header("Reliquia - Fragmentos Gemeos")]
    [SerializeField] private GameObject prefabReliquia;
    [SerializeField] private Transform pontoDropReliquia;

    private bool reliquiaDropada;
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
        reliquiaDropada = false;
    }

    private void Start()
    {
        VerificarSalvamento();
    }

    private void VerificarSalvamento()
    {
        int slot =
            MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (
            dados == null ||
            dados.chefesDerrotados == null
        )
        {
            return;
        }

        if (
            dados.chefesDerrotados.Contains(
                "Fusao"
            )
        )
        {
            bool reliquiaColetada =
                dados.reliquiasObtidas != null &&
                dados.reliquiasObtidas.Contains(
                    "FragmentosGemeos"
                );

            if (!reliquiaColetada)
            {
                DroparReliquia();
            }

            gameObject.SetActive(false);
        }
    }

    public void ReceberDano(int dano)
    {
        if (
            dano <= 0 ||
            !PodeReceberDano
        )
        {
            return;
        }

        vidaAtual =
            Mathf.Max(
                0,
                vidaAtual - dano
            );

        Debug.Log(
            "Fusao Saryn & Sayren - Vida: " +
            vidaAtual + "/" + vidaMaxima
        );

        if (vidaAtual <= 0)
        {
            IniciarMorte();
        }
    }

    private void IniciarMorte()
    {
        if (morto)
            return;

        morto = true;
        morteFinalizada = false;

        // Libera os teletransportes.
        Teletransporte.LiberarBloqueio();

        // Salva imediatamente a derrota.
        RegistrarDerrota();

        Debug.Log(
            "Fusao Saryn & Sayren: morte iniciada!"
        );

        // Desativa a IA.
        if (ia != null)
        {
            ia.enabled = false;
        }

        // Para completamente o movimento.
        if (movimento != null)
        {
            movimento.DesativarCombate();
            movimento.BloquearMovimento(true);
            movimento.enabled = false;
        }

        // Desativa os ataques.
        if (ataque1 != null)
        {
            ataque1.enabled = false;
        }

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

        // Congela completamente a física
        // para impedir que a Fusão caia
        // após desligarmos os colliders.
        if (corpo != null)
        {
            corpo.linearVelocity =
                Vector2.zero;

            corpo.angularVelocity =
                0f;

            corpo.simulated =
                false;
        }

        // Desativa corpo e hitboxes.
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (
            Collider2D col
            in colliders
        )
        {
            col.enabled = false;
        }

        // Prepara e inicia a animação de morte.
        if (animator != null)
        {
            animator.speed = 1f;

            animator.SetBool(
                "Andando",
                false
            );

            animator.ResetTrigger(
                "Ataque1"
            );

            animator.ResetTrigger(
                "Ataque2"
            );

            animator.ResetTrigger(
                "Ataque3"
            );

            animator.SetBool(
                "ChuvaTerminou",
                false
            );

            animator.SetTrigger(
                "Morte"
            );
        }
        else
        {
            // Segurança caso o Animator
            // esteja ausente.
            FinalizarMorte();
        }
    }

    // =========================
    // DROP DA RELIQUIA
    // =========================

    public void DroparReliquia()
    {
        if (reliquiaDropada)
            return;

        if (
            prefabReliquia == null ||
            pontoDropReliquia == null
        )
        {
            Debug.LogWarning(
                "Fusao: prefab ou ponto de drop nao configurado!"
            );

            return;
        }

        Instantiate(
            prefabReliquia,
            pontoDropReliquia.position,
            prefabReliquia.transform.rotation
        );

        reliquiaDropada = true;

        Debug.Log(
            "Fusao: Fragmentos Gemeos apareceram!"
        );
    }

    // =========================
    // SALVAMENTO DA DERROTA
    // =========================

    private void RegistrarDerrota()
    {
        int slot =
            MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
            return;

        if (dados.chefesDerrotados == null)
        {
            dados.chefesDerrotados =
                new List<string>();
        }

        if (
            !dados.chefesDerrotados.Contains(
                "Fusao"
            )
        )
        {
            dados.chefesDerrotados.Add(
                "Fusao"
            );

            if (
                ContadorTempoJogo.instancia != null
            )
            {
                ContadorTempoJogo.instancia
                    .AtualizarTempoNosDados(
                        dados
                    );
            }

            if (
                GerenciadorSalvamento.Salvar(
                    slot,
                    dados
                )
            )
            {
                Debug.Log(
                    "Fusao Saryn & Sayren derrotada permanentemente!" +
                    "\nSlot: " +
                    slot +
                    "\nTempo: " +
                    dados.tempoTotalJogado.ToString("F1") +
                    " segundos"
                );
            }
            else
            {
                Debug.LogError(
                    "Falha ao salvar a derrota da Fusao."
                );
            }
        }
    }

    // =========================
    // FINAL DA MORTE
    // =========================

    public void FinalizarMorte()
    {
        if (
            !morto ||
            morteFinalizada
        )
        {
            return;
        }

        // Garante que os Fragmentos
        // Gêmeos apareçam.
        DroparReliquia();

        morteFinalizada = true;

        Debug.Log(
            "Fusao Saryn & Sayren: morte finalizada!"
        );

        gameObject.SetActive(false);
    }

    [ContextMenu("TESTE - Perder 100 de vida")]
    private void TestarDano()
    {
        if (Application.isPlaying)
        {
            ReceberDano(100);
        }
    }
}
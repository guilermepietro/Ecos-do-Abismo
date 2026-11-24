using UnityEngine;

public class RakthasVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 450;

    private int vidaAtual;
    private bool morto = false;

    private RakthasAtaques ataques;

    [Header("Referências")]
    public Animator animator;

    [Header("Relíquia")]
    public GameObject prefabReliquia;
    public Transform pontoDropReliquia;

    [Header("Cutscene de Morte")]
    public string idCutsceneMorte = "Rakthas_Morte";

    [Header("Interface")]
    public GameObject barraVidaVisual;

    private bool reliquiaDropada = false;
    private bool cutsceneMorteIniciada = false;
    private bool morteFinalizada = false;

    public bool EstaMorto => morto;

    private void Start()
    {
        vidaAtual = vidaMaxima;

        ataques =
            GetComponent<RakthasAtaques>();

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
                "Rakthas"
            )
        )
        {
            Debug.Log(
                "Rakthas já foi derrotado no slot " +
                slot
            );

            bool possuiCaliceDeBrasas =
                false;

            if (
                dados.reliquiasObtidas != null
            )
            {
                possuiCaliceDeBrasas =
                    dados.reliquiasObtidas.Contains(
                        "CaliceDeBrasas"
                    );
            }

            if (!possuiCaliceDeBrasas)
            {
                DroparReliquia();

                Debug.Log(
                    "Cálice de Brasas ainda não foi coletado. " +
                    "Relíquia restaurada na arena."
                );
            }

            if (barraVidaVisual != null)
            {
                barraVidaVisual.SetActive(
                    false
                );
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

        // Rakthas recebe dano normalmente,
        // mas não possui animação de dano
        // e não interrompe movimento ou ataques.
    }

    private void Morrer()
    {
        if (morto)
            return;

        morto = true;

        // Libera os teletransportes.
        Teletransporte.LiberarBloqueio();

        // Cancela qualquer ataque atual.
        if (ataques != null)
        {
            ataques.CancelarAtaque();
        }

        // Impede o movimento de continuar
        // alterando o Animator durante a morte.
        RakthasMovimento movimento =
            GetComponent<RakthasMovimento>();

        if (movimento != null)
        {
            movimento.enabled = false;
        }

        // Congela completamente a física.
        // Assim Rakthas não cai da arena
        // depois que os colliders forem desligados.
        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity =
                0f;

            rb.simulated =
                false;
        }

        // Desativa corpo e hitboxes.
        Collider2D[] colliders =
            GetComponentsInChildren<
                Collider2D
            >();

        foreach (
            Collider2D collider
            in colliders
        )
        {
            collider.enabled = false;
        }

        // Prepara a animação de morte.
        if (animator != null)
        {
            animator.speed = 1f;

            animator.SetBool(
                "estaCorrendo",
                false
            );
        }

        // A derrota continua sendo
        // registrada imediatamente.
        RegistrarDerrota();

        // Inicia a animação de morte.
        if (animator != null)
        {
            animator.SetTrigger(
                "Morte"
            );
        }

        Debug.Log(
            "Rakthas iniciou sua animação de morte."
        );

        // O Animation Event no final do clip
        // chamará FinalizarMorte().
    }

    // =========================
    // CUTSCENE DE MORTE
    // =========================

    public void FinalizarMorte()
    {
        if (
            !morto ||
            cutsceneMorteIniciada
        )
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
                        ConcluirMorte
                    );

            if (iniciou)
            {
                Debug.Log(
                    "Animação de morte concluída! " +
                    "Cutscene de morte do Rakthas iniciada."
                );

                return;
            }
        }

        Debug.LogWarning(
            "Cutscene de morte do Rakthas " +
            "indisponível. Finalizando derrota."
        );

        ConcluirMorte();
    }

    private void ConcluirMorte()
    {
        if (
            !morto ||
            morteFinalizada
        )
        {
            return;
        }

        morteFinalizada = true;

        // Primeiro cria a relíquia.
        DroparReliquia();

        // Esconde a barra de vida.
        if (barraVidaVisual != null)
        {
            barraVidaVisual.SetActive(
                false
            );
        }

        // Depois remove o Rakthas.
        gameObject.SetActive(false);

        Debug.Log(
            "Rakthas desapareceu após a cutscene. " +
            "Cálice de Brasas deixado na arena."
        );
    }

    // =========================
    // DROP DA RELÍQUIA
    // =========================

    public void DroparReliquia()
    {
        if (reliquiaDropada)
            return;

        if (prefabReliquia == null)
        {
            Debug.LogWarning(
                "Prefab do Cálice de Brasas " +
                "não foi definido."
            );

            return;
        }

        if (pontoDropReliquia == null)
        {
            Debug.LogWarning(
                "Ponto de drop do Cálice de Brasas " +
                "não foi definido."
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
            "Rakthas dropou o Cálice de Brasas."
        );
    }

    // =========================
    // SALVAMENTO DA DERROTA
    // =========================

    private void RegistrarDerrota()
    {
        int slot =
            MenuSelecaoSave.slotPartidaAtual;

        if (
            slot < 1 ||
            slot > 3
        )
        {
            Debug.LogWarning(
                "Rakthas derrotado sem slot selecionado. " +
                "A derrota não será salva."
            );

            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(
                slot
            );

        if (dados == null)
        {
            Debug.LogError(
                "Não foi possível carregar o salvamento " +
                "para registrar a derrota do Rakthas."
            );

            return;
        }

        if (
            dados.chefesDerrotados == null
        )
        {
            Debug.LogError(
                "Lista de chefes derrotados indisponível."
            );

            return;
        }

        if (
            !dados.chefesDerrotados.Contains(
                "Rakthas"
            )
        )
        {
            dados.chefesDerrotados.Add(
                "Rakthas"
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
                    "Rakthas derrotado permanentemente!" +
                    "\nSlot: " +
                    slot +
                    "\nTempo: " +
                    dados.tempoTotalJogado
                        .ToString("F1") +
                    " segundos"
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

    // =========================
    // HUD DE VIDA
    // =========================

    public float ObterPorcentagemVida()
    {
        if (vidaMaxima <= 0)
            return 0f;

        return Mathf.Clamp01(
            (float)vidaAtual /
            vidaMaxima
        );
    }
}
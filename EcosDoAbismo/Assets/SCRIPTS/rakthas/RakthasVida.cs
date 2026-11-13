
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

    [Header("Relíquia")]
    public GameObject prefabReliquia;
    public Transform pontoDropReliquia;

    private bool reliquiaDropada = false;

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

        if (dados == null ||
            dados.chefesDerrotados == null)
            return;

        if (dados.chefesDerrotados.Contains("Rakthas"))
        {
            Debug.Log(
                "Rakthas já foi derrotado no slot " +
                slot
            );

            bool possuiCaliceDeBrasas = false;

            if (dados.reliquiasObtidas != null)
            {
                possuiCaliceDeBrasas =
                    dados.reliquiasObtidas.Contains(
                        "CaliceDeBrasas"
                    );
            }

            // Rakthas morreu, mas o jogador
            // ainda não coletou a relíquia.
            if (!possuiCaliceDeBrasas)
            {
                DroparReliquia();

                Debug.Log(
                    "Cálice de Brasas ainda não foi coletado. " +
                    "Relíquia restaurada na arena."
                );
            }

            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // TECLA TEMPORÁRIA PARA TESTE
        if (Input.GetKeyDown(KeyCode.Alpha2) && !morto)
        {
            ReceberDano(vidaAtual);
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

        // Durante um ataque, Rakthas recebe dano,
        // mas o ataque não é interrompido.
        if (ataques != null && ataques.EstaAtacando)
        {
            return;
        }

        tomandoDano = true;

        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                new Vector2(
                    0,
                    rb.linearVelocity.y
                );
        }

        animator.SetBool(
            "estaCorrendo",
            false
        );

        animator.speed = 1f;

        animator.SetTrigger("Dano");
    }

    private void Morrer()
    {
        morto = true;
        tomandoDano = false;

        if (ataques != null)
        {
            ataques.CancelarAtaque();
        }

        // DESATIVA TODOS OS COLLIDERS
        // PARA O CORPO MORTO NÃO ATRAPALHAR
        // O ELIAN NEM A RELÍQUIA.
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        animator.speed = 1f;

        animator.SetBool(
            "estaCorrendo",
            false
        );

        animator.SetTrigger("Morte");

        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;
        }

        // REGISTRA A DERROTA DEFINITIVA
        RegistrarDerrota();
    }

    public void FinalizarDano()
    {
        tomandoDano = false;
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

            if (ContadorTempoJogo.instancia != null)
            {
                ContadorTempoJogo.instancia
                    .AtualizarTempoNosDados(dados);
            }

            if (GerenciadorSalvamento.Salvar(slot, dados))
            {
                Debug.Log(
                    "Rakthas derrotado permanentemente!" +
                    "\nSlot: " + slot +
                    "\nTempo: " +
                    dados.tempoTotalJogado.ToString("F1") +
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
    // FINAL DA MORTE
    // =========================

    public void FinalizarMorte()
    {
        gameObject.SetActive(false);
    }

    // =========================
    // HUD DE VIDA
    // =========================

    public float ObterPorcentagemVida()
    {
        if (vidaMaxima <= 0)
            return 0f;

        return Mathf.Clamp01(
            (float)vidaAtual / vidaMaxima
        );
    }
}

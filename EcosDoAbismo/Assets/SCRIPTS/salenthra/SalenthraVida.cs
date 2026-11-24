
using UnityEngine;

public class SalenthraVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaximaFase1 = 100;
    public int vidaMaximaFase2 = 150;

    private int vidaAtualFase1;
    private int vidaAtualFase2;

    [Header("Referencias")]
    public Animator animator;

    [Header("Reliquia")]
    public GameObject laminaCelesteDropPrefab;
    public Transform pontoDropReliquia;

    [Header("Cutscenes")]
    public string idCutsceneTransformacao =
        "Salenthra_Transformacao";

    public string idCutsceneMorte =
        "Salenthra_Morte";

    private SalenthraMovimento movimento;

    private int faseAtual = 1;
    private bool transformando = false;
    private bool morta = false;
    private bool reliquiaSolta = false;

    // Evita cutscenes repetidas.
    private bool cutsceneTransformacaoIniciada = false;
    private bool cutsceneMorteIniciada = false;
    private bool morteFinalizada = false;

    public bool EstaMorta
    {
        get { return morta; }
    }

    private void Awake()
    {
        vidaAtualFase1 = vidaMaximaFase1;
        vidaAtualFase2 = vidaMaximaFase2;

        movimento = GetComponent<SalenthraMovimento>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        VerificarSalvamento();
    }

    private void Update()
{
    if (
        transformando &&
        faseAtual == 1 &&
        !cutsceneTransformacaoIniciada &&
        !morta &&
        animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Fase 2")
    )
    {
        IniciarCutsceneTransformacao();
    }
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

        if (dados.chefesDerrotados.Contains("Salenthra"))
        {
            Debug.Log(
                "Salenthra ja foi derrotada no slot " + slot
            );

            bool possuiLaminaCeleste = false;

            if (dados.reliquiasObtidas != null)
            {
                possuiLaminaCeleste =
                    dados.reliquiasObtidas.Contains(
                        "LaminaCeleste"
                    );
            }

            if (!possuiLaminaCeleste)
            {
                SoltarReliquia();

                Debug.Log(
                    "Lamina Celeste restaurada na arena."
                );
            }

            gameObject.SetActive(false);
        }
    }

    public void ReceberDano(int dano)
    {
        if (transformando || morta)
            return;

        // =========================
        // FASE 1
        // =========================

        if (faseAtual == 1)
        {
            vidaAtualFase1 -= dano;

            if (vidaAtualFase1 <= 0)
            {
                vidaAtualFase1 = 0;

                if (movimento != null)
                {
                    movimento.CancelarAtaquesPorDano();
                }

                IniciarTransformacao();
                return;
            }
        }

        // =========================
        // FASE 2
        // =========================

        else if (faseAtual == 2)
        {
            vidaAtualFase2 -= dano;

            if (vidaAtualFase2 <= 0)
            {
                vidaAtualFase2 = 0;

                if (movimento != null)
                {
                    movimento.CancelarAtaquesPorDano();
                }

                MorrerFase2();
                return;
            }
        }
    }

    // =========================
    // TRANSFORMACAO
    // =========================

    private void IniciarTransformacao()
    {
        if (transformando || morta)
            return;

        transformando = true;

        if (movimento != null)
            movimento.podeMover = false;

        animator.speed = 1f;
        animator.SetBool("Andando", false);
        animator.SetTrigger("Transformar");

        Debug.Log(
            "Salenthra: animacao de transformacao iniciada."
        );

        // O Animation Event iniciara a cutscene.
    }

    // NOVO: Animation Event no final
    // do clip Transformar.
    public void IniciarCutsceneTransformacao()
    {
        if (!transformando ||
            faseAtual != 1 ||
            cutsceneTransformacaoIniciada ||
            morta)
            return;

        cutsceneTransformacaoIniciada = true;

        if (GerenciadorCutscenes.Instancia != null)
        {
            bool iniciou =
                GerenciadorCutscenes.Instancia.Reproduzir(
                    idCutsceneTransformacao,
                    IniciarFase2
                );

            if (iniciou)
            {
                Debug.Log(
                    "Salenthra: cutscene de transformacao iniciada."
                );
                return;
            }
        }

        Debug.LogWarning(
            "Cutscene de transformacao indisponivel. " +
            "Iniciando fase 2 normalmente."
        );

        IniciarFase2();
    }

    // Agora so inicia depois da cutscene.
    public void IniciarFase2()
    {
        if (!transformando ||
            morta ||
            faseAtual != 1)
            return;

        faseAtual = 2;
        transformando = false;
        vidaAtualFase2 = vidaMaximaFase2;

        if (movimento != null)
        {
            movimento.EntrarFase2();
            movimento.podeMover = true;
        }

        Debug.Log(
            "Salenthra: fase 2 liberada apos a cutscene!"
        );
    }

    // =========================
    // DANO
    // =========================

    public void FinalizarDano()
    {
        if (transformando || morta)
            return;

        if (movimento != null)
            movimento.podeMover = true;
    }

    public void FinalizarDanoFase2()
    {
        if (morta || transformando)
            return;

        if (movimento != null)
            movimento.podeMover = true;
    }

    // =========================
    // MORTE DA FASE 2
    // =========================

    private void MorrerFase2()
    {
        if (morta)
            return;

        morta = true;

        // NOVO: libera os teletransportes.
        Teletransporte.LiberarBloqueio();

        if (movimento != null)
        {
            movimento.podeMover = false;

            // Impede novos comandos de movimento.
            movimento.enabled = false;
        }

        // Congela a fisica, impedindo a queda.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // Desativa os colliders do boss morto.
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
            collider.enabled = false;

        animator.speed = 1f;
        animator.SetBool("Andando", false);
        animator.SetTrigger("MorteFase2");

        // Mantem o salvamento imediato.
        RegistrarDerrota();

        Debug.Log(
            "Salenthra: animacao de morte da fase 2 iniciada."
        );

        // O Animation Event iniciara a cutscene.
    }

    // NOVO: Animation Event no final
    // do clip MorteFase2.
    public void IniciarCutsceneMorte()
    {
        if (!morta ||
            morteFinalizada ||
            cutsceneMorteIniciada)
            return;

        cutsceneMorteIniciada = true;

        if (GerenciadorCutscenes.Instancia != null)
        {
            bool iniciou =
                GerenciadorCutscenes.Instancia.Reproduzir(
                    idCutsceneMorte,
                    FinalizarMorte
                );

            if (iniciou)
            {
                Debug.Log(
                    "Salenthra: cutscene de morte iniciada."
                );
                return;
            }
        }

        Debug.LogWarning(
            "Cutscene de morte indisponivel. " +
            "Finalizando morte normalmente."
        );

        FinalizarMorte();
    }

    // Executado ao finalizar a cutscene.
    public void FinalizarMorte()
    {
        if (!morta || morteFinalizada)
            return;

        morteFinalizada = true;

        // Cria a reliquia antes de esconder o boss.
        SoltarReliquia();

        Debug.Log(
            "Salenthra: morte finalizada! " +
            "Lamina Celeste deixada na arena."
        );

        gameObject.SetActive(false);
    }

    // =========================
    // DROP DA RELIQUIA
    // =========================

    public void SoltarReliquia()
    {
        if (reliquiaSolta)
            return;

        if (laminaCelesteDropPrefab == null)
        {
            Debug.LogWarning(
                "Prefab da Lamina Celeste nao foi atribuido."
            );
            return;
        }

        if (pontoDropReliquia == null)
        {
            Debug.LogWarning(
                "Ponto de drop da Lamina Celeste nao foi atribuido."
            );
            return;
        }

        reliquiaSolta = true;

        Instantiate(
            laminaCelesteDropPrefab,
            pontoDropReliquia.position,
            Quaternion.identity
        );
    }

    // =========================
    // SALVAMENTO
    // =========================

    private void RegistrarDerrota()
    {
        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
        {
            Debug.LogWarning(
                "Salenthra derrotada sem slot selecionado. " +
                "A derrota nao sera salva."
            );
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
        {
            Debug.LogError(
                "Nao foi possivel carregar o salvamento " +
                "para registrar a derrota da Salenthra."
            );
            return;
        }

        if (dados.chefesDerrotados == null)
        {
            Debug.LogError(
                "Lista de chefes derrotados indisponivel."
            );
            return;
        }

        if (!dados.chefesDerrotados.Contains("Salenthra"))
        {
            dados.chefesDerrotados.Add("Salenthra");

            if (ContadorTempoJogo.instancia != null)
            {
                ContadorTempoJogo.instancia
                    .AtualizarTempoNosDados(dados);
            }

            if (GerenciadorSalvamento.Salvar(slot, dados))
            {
                Debug.Log(
                    "Salenthra derrotada permanentemente!" +
                    "\nSlot: " + slot +
                    "\nTempo: " +
                    dados.tempoTotalJogado.ToString("F1") +
                    " segundos"
                );
            }
            else
            {
                Debug.LogError(
                    "Falha ao salvar a derrota da Salenthra."
                );
            }
        }
    }

    // =========================
    // CALCULO DE DANO
    // =========================

    public int CalcularDano(int danoBase)
    {
        float porcentagemVida;

        if (faseAtual == 1)
        {
            porcentagemVida =
                (float)vidaAtualFase1 /
                vidaMaximaFase1;
        }
        else
        {
            porcentagemVida =
                (float)vidaAtualFase2 /
                vidaMaximaFase2;
        }

        float multiplicador = 1f;

        if (porcentagemVida <= 0.30f)
        {
            multiplicador = 1.4f;
        }
        else if (porcentagemVida <= 0.60f)
        {
            multiplicador = 1.2f;
        }

        return Mathf.RoundToInt(
            danoBase * multiplicador
        );
    }

    // =========================
    // PORCENTAGEM DE VIDA
    // =========================

    public float ObterPorcentagemVidaAtual()
    {
        if (faseAtual == 1)
        {
            return
                (float)vidaAtualFase1 /
                vidaMaximaFase1;
        }

        return
            (float)vidaAtualFase2 /
            vidaMaximaFase2;
    }
}

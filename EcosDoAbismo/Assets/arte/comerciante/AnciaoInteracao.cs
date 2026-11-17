
using UnityEngine;
using TMPro;

public class AnciaoInteracao : MonoBehaviour
{
    private enum TipoConversa
    {
        Nenhuma,
        Apresentacao,
        Coroa,
        IntroducaoRakthas,
        Calice,
        Retorno
    }

    private enum TipoBencao
    {
        Nenhuma,
        Ascensao,
        InvestidaFlamejante
    }

    [Header("Interface")]
    [SerializeField] private GameObject painelConversa;
    [SerializeField] private TMP_Text textoConversa;

    [Header("Tutoriais")]
    [SerializeField] private GerenciadorTutoriais gerenciadorTutoriais;
    [SerializeField] private AreaTutorial tutorialAscensao;
    [SerializeField] private AreaTutorial tutorialInvestidaFlamejante;

    [Header("Identificadores das Reliquias")]
    [SerializeField] private string idReliquiaCoroa = "CoroaAbismo";
    [SerializeField] private string idReliquiaCalice = "CaliceBrasas";

    [Header("Primeiro Encontro")]
    [SerializeField, TextArea(2, 5)]
    private string[] dialogosPrimeiroEncontro =
    {
        "Ahh... Malditos bichos! Quase me mandaram para debaixo da terra antes da hora.",

        "Você me salvou, rapaz. E olhe que não sou homem de pedir favores... embora minhas pernas já não concordem comigo.",

        "Espere... Chegue um pouco mais perto.",

        "Hm... Que coisa curiosa...",

        "Ah, não ligue para este velho. A idade faz a gente enxergar coisas onde não existem, sabe?",

        "Mas me diga, meu jovem... O que o traz tão longe de casa? Não tem cara de quem anda por estas terras só para passear.",

        "Hm... Entendo. Não parece querer falar muito, não é? Hah! Melhor assim. Já falo pelos cotovelos por nós dois.",

        "Ora... Essa arma, essas marcas de batalha... Não me diga que também está atrás das antigas relíquias.",

        "Ah, então acertei! Hah! Parece que nossos caminhos não se cruzaram por acaso, hein, rapaz?",

        "Também as procuro, sabe? Passei anos estudando os velhos guardiões. Achei que ainda conseguiria chegar até um deles... mas você viu no que deu.",

        "Se está mesmo atrás das relíquias, talvez eu possa lhe ser útil. Conheço certos ritos... coisas que aprendi quando estas mãos ainda não tremiam tanto.",

        "O poder desses artefatos pode ser despertado. Posso conceder-lhe parte dele, mas preciso manter a relíquia comigo para completar o ritual. Entende?",

        "Heh... Vejo que consegui chamar sua atenção. Mas não se anime tanto, meu jovem. Essas coisas nunca são tão simples quanto parecem.",

        "Sabe, eu estava seguindo os rastros de uma antiga coroa. Dizem que repousa nas mãos de Voragul... uma criatura que há muito deixou de ser homem, se é que algum dia foi.",

        "Não cheguei muito longe, como pode ver. Aqueles malditos lá fora trataram de encerrar minha aventura antes mesmo de começar.",

        "Mas você... Você parece ter mais sorte com uma espada do que eu tenho com estas pernas.",

        "Se algum dia encontrar essa coroa, traga-a até mim, está bem? Quero lhe mostrar que este velho ainda serve para alguma coisa.",

        "E tome cuidado por onde anda, rapaz. Nem todo monstro vai cair tão facilmente quanto aqueles três."
    };

    [Header("Retorno com a Coroa")]
    [SerializeField, TextArea(2, 5)]
    private string[] dialogosCoroa =
    {
        "Ora, ora... Veja só quem voltou! E com os dois braços, pelo que vejo. Hah!",

        "Espere... O que é isso que carrega? Não me diga que...",

        "Pelos céus... A Coroa do Abismo! Você realmente enfrentou Voragul?",

        "E venceu... Heh. Devo admitir, rapaz, que não esperava tanto de você.",

        "Venha, deixe-me vê-la. Ah... Ainda consigo sentir seu poder. Nem todos esses anos foram capazes de apagá-lo.",

        "Bem, meu jovem, uma promessa é uma promessa. Lembra-se do que lhe disse? Conheço um jeito de despertar esse poder.",

        "Aproxime-se. Vamos ver se estas mãos velhas ainda se lembram de como realizar uma bênção."
    };

    [Header("Introducao de Rakthas")]
    [SerializeField, TextArea(2, 5)]
    private string[] dialogosIntroducaoRakthas =
    {
        "Ah, rapaz... Antes que siga sua viagem, há uma coisa que preciso lhe contar.",

        "Já ouviu falar de Rakthas? Não? Hm... Sorte sua.",

        "Dizem que o desgraçado vive cercado de fogo. Um sujeito tão violento que nem as próprias chamas conseguem acompanhá-lo.",

        "Guarda um cálice antigo. Há quem diga que o fogo dentro dele nunca se apaga.",

        "Eu mesmo pensei em procurá-lo, sabe? Mas depois daqueles três monstrinhos... Hah! Acho que já tive aventuras suficientes.",

        "Se seus caminhos se cruzarem, tome cuidado. Voragul era pesado, lento... Rakthas não lhe dará tanto tempo para pensar.",

        "E, se conseguir o cálice... bem, já sabe onde encontrar este velho, não sabe?"
    };

    [Header("Retorno com o Calice")]
    [SerializeField, TextArea(2, 5)]
    private string[] dialogosCalice =
    {
        "Ah! Meu jovem! Já estava começando a pensar que tinha virado um monte de cinzas.",

        "O quê? Não me olhe assim. Um velho também tem direito a se preocupar, não é?",

        "Então conseguiu encontrar Rakthas? E voltou vivo para contar a história... Impressionante.",

        "Deixe-me ver esse cálice. Devagar, rapaz! Essas mãos já não aguentam tanto calor.",

        "Heh... Ainda arde. Mesmo longe de seu antigo mestre.",

        "Muito bem. Vamos fazer nossa parte do acordo. Afinal, não seria justo deixá-lo enfrentar aquelas criaturas sem alguma recompensa.",

        "Aproxime-se, meu jovem. Vamos ver o que este fogo pode fazer por você."
    };

    [Header("Visitas Comuns")]
    [SerializeField, TextArea(2, 5)]
    private string[] dialogosRetorno =
    {
        "Ah, meu salvador... Vejo que a morte ainda não conseguiu alcançá-lo.",

        "Ainda não trouxe nenhuma novidade, rapaz? Hah! Não precisa fazer essa cara.",

        "Vá com cuidado. Os guardiões não costumam receber visitas de bom humor."
    };

    private ElianColeta elianPerto;
    private ElianMovimento movimento;
    private Animator animatorElian;

    private bool conversaAberta;
    private bool conversaLiberada = true;
    private bool bencaoPendente;
    private bool tutorialPendente;

    private int indiceDialogo;
    private int slotAtual;

    private TipoConversa tipoConversaAtual;
    private TipoBencao bencaoEmEntrega;
    private TipoBencao tutorialDaBencao;

    private string[] dialogosAtuais;

    void Start()
    {
        slotAtual = MenuSelecaoSave.slotPartidaAtual;

        if (painelConversa != null)
            painelConversa.SetActive(false);
    }

    void Update()
    {
        if (!conversaAberta)
            return;

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AvancarDialogo();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            FecharConversa();
        }
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        ElianColeta coleta =
            outro.GetComponentInParent<ElianColeta>();

        if (coleta == null)
            return;

        elianPerto = coleta;
        elianPerto.DefinirAnciaoPerto(this);
    }

    void OnTriggerExit2D(Collider2D outro)
    {
        ElianColeta coleta =
            outro.GetComponentInParent<ElianColeta>();

        if (coleta == null || coleta != elianPerto)
            return;

        if (conversaAberta)
            FecharConversa();

        elianPerto.DefinirAnciaoPerto(null);
        elianPerto = null;
    }

    public void DefinirConversaLiberada(bool liberada)
    {
        conversaLiberada = liberada;

        if (!liberada && conversaAberta)
            FecharConversa();
    }

    public void AbrirConversa()
    {
        if (!conversaLiberada ||
            conversaAberta ||
            bencaoPendente ||
            tutorialPendente ||
            elianPerto == null)
        {
            return;
        }

        if (painelConversa == null || textoConversa == null)
        {
            Debug.LogWarning(
                "Configure o painel e o texto do Anciao."
            );
            return;
        }

        movimento =
            elianPerto.GetComponent<ElianMovimento>();

        animatorElian =
            elianPerto.GetComponent<Animator>();

        if (movimento == null || animatorElian == null)
            return;

        if (slotAtual < 1 || slotAtual > 3)
        {
            Debug.LogWarning(
                "Anciao: slot invalido."
            );
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slotAtual);

        if (dados == null)
            return;

        bool possuiCoroa =
            dados.reliquiasObtidas != null &&
            dados.reliquiasObtidas.Contains(idReliquiaCoroa);

        bool possuiCalice =
            dados.reliquiasObtidas != null &&
            dados.reliquiasObtidas.Contains(idReliquiaCalice);

        bool possuiAscensao =
            dados.bencaosRecebidas != null &&
            dados.bencaosRecebidas.Contains("Ascensao");

        bool possuiInvestida =
            dados.bencaosRecebidas != null &&
            dados.bencaosRecebidas.Contains(
                "InvestidaFlamejante"
            );

        if (!dados.apresentacaoAnciaoConcluida)
        {
            tipoConversaAtual =
                TipoConversa.Apresentacao;

            dialogosAtuais =
                dialogosPrimeiroEncontro;
        }
        else if (possuiCoroa && !possuiAscensao)
        {
            tipoConversaAtual =
                TipoConversa.Coroa;

            dialogosAtuais = dialogosCoroa;
        }
        else if (possuiAscensao &&
                 !dados.historiaRakthasContada)
        {
            tipoConversaAtual =
                TipoConversa.IntroducaoRakthas;

            dialogosAtuais =
                dialogosIntroducaoRakthas;
        }
        else if (possuiCalice && !possuiInvestida)
        {
            tipoConversaAtual =
                TipoConversa.Calice;

            dialogosAtuais = dialogosCalice;
        }
        else
        {
            tipoConversaAtual =
                TipoConversa.Retorno;

            dialogosAtuais = dialogosRetorno;
        }

        if (dialogosAtuais == null ||
            dialogosAtuais.Length == 0)
        {
            return;
        }

        indiceDialogo = 0;
        conversaAberta = true;

        movimento.podeMover = false;
        animatorElian.SetFloat("velocidade", 0f);

        painelConversa.SetActive(true);
        MostrarDialogo();
    }

    private void MostrarDialogo()
    {
        textoConversa.text =
            dialogosAtuais[indiceDialogo];
    }

    private void AvancarDialogo()
    {
        indiceDialogo++;

        if (indiceDialogo < dialogosAtuais.Length)
        {
            MostrarDialogo();
            return;
        }

        switch (tipoConversaAtual)
        {
            case TipoConversa.Apresentacao:
                RegistrarProgressoConversa(true);
                FecharConversa();
                break;

            case TipoConversa.IntroducaoRakthas:
                RegistrarProgressoConversa(false);
                FecharConversa();
                break;

            case TipoConversa.Coroa:
                IniciarEntregaBencao(TipoBencao.Ascensao);
                break;

            case TipoConversa.Calice:
                IniciarEntregaBencao(
                    TipoBencao.InvestidaFlamejante
                );
                break;

            default:
                FecharConversa();
                break;
        }
    }

    private void IniciarEntregaBencao(TipoBencao tipo)
    {
        if (elianPerto == null)
        {
            FecharConversa();
            return;
        }

        conversaAberta = false;
        bencaoPendente = true;
        tutorialPendente = false;

        bencaoEmEntrega = tipo;
        tutorialDaBencao = TipoBencao.Nenhuma;

        painelConversa.SetActive(false);

        // O Elian executa a animacao Coletar.
        elianPerto.IniciarBencao(this);
    }

    // Chamado pelo evento ExecutarColeta
    public void ConcederBencao()
    {
        if (!bencaoPendente || movimento == null)
            return;

        ElianBencaos bencaos =
            movimento.GetComponent<ElianBencaos>();

        if (bencaos == null)
        {
            Debug.LogError(
                "ElianBencaos nao encontrado."
            );
            bencaoPendente = false;
            return;
        }

        bool sucesso = false;

        switch (bencaoEmEntrega)
        {
            case TipoBencao.Ascensao:
                sucesso = bencaos.ConcederAscensao();
                break;

            case TipoBencao.InvestidaFlamejante:
                sucesso =
                    bencaos.ConcederInvestidaFlamejante();
                break;
        }

        if (sucesso)
        {
            tutorialDaBencao = bencaoEmEntrega;
            tutorialPendente = true;

            Debug.Log(
                "Bencao concedida: " + bencaoEmEntrega
            );
        }

        bencaoPendente = false;
        bencaoEmEntrega = TipoBencao.Nenhuma;
    }

    // Chamado no fim da animacao Coletar
    public void FinalizarRecebimentoBencao()
    {
        bencaoPendente = false;

        if (!tutorialPendente)
            return;

        tutorialPendente = false;

        AreaTutorial tutorial = null;

        switch (tutorialDaBencao)
        {
            case TipoBencao.Ascensao:
                tutorial = tutorialAscensao;
                break;

            case TipoBencao.InvestidaFlamejante:
                tutorial = tutorialInvestidaFlamejante;
                break;
        }

        tutorialDaBencao = TipoBencao.Nenhuma;

        if (gerenciadorTutoriais == null || tutorial == null)
        {
            Debug.LogWarning(
                "Configure o tutorial desta bencao."
            );
            return;
        }

        if (!gerenciadorTutoriais.AbrirTutorial(tutorial))
        {
            Debug.LogWarning(
                "Nao foi possivel abrir o tutorial."
            );
        }
    }

    private void RegistrarProgressoConversa(bool apresentacao)
    {
        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slotAtual);

        if (dados == null)
            return;

        if (apresentacao)
            dados.apresentacaoAnciaoConcluida = true;
        else
            dados.historiaRakthasContada = true;

        if (ContadorTempoJogo.instancia != null)
        {
            ContadorTempoJogo.instancia
                .AtualizarTempoNosDados(dados);
        }

        if (!GerenciadorSalvamento.Salvar(slotAtual, dados))
        {
            Debug.LogError(
                "Falha ao salvar o progresso do dialogo."
            );
        }
    }

    public void FecharConversa()
    {
        if (!conversaAberta)
            return;

        conversaAberta = false;

        if (painelConversa != null)
            painelConversa.SetActive(false);

        if (movimento != null)
        {
            ElianVida vida =
                movimento.GetComponent<ElianVida>();

            if (vida == null || !vida.EstaMorto)
            {
                movimento.podeMover = true;
            }
        }
    }
}

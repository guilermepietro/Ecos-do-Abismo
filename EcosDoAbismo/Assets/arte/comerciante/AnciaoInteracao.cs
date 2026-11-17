
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
        IntroducaoSalenthra,
        Lamina,
        Retorno
    }

    private enum TipoBencao
    {
        Nenhuma,
        Ascensao,
        InvestidaFlamejante,
        InvestidaCeleste
    }

    [Header("Interface")]
    [SerializeField] private GameObject painelConversa;
    [SerializeField] private TMP_Text textoConversa;

    [Header("Tutoriais")]
    [SerializeField] private GerenciadorTutoriais gerenciadorTutoriais;
    [SerializeField] private AreaTutorial tutorialAscensao;
    [SerializeField] private AreaTutorial tutorialInvestidaFlamejante;
    [SerializeField] private AreaTutorial tutorialInvestidaCeleste;

    [Header("Identificadores das Reliquias")]
    [SerializeField] private string idReliquiaCoroa = "CoroaAbismo";
    [SerializeField] private string idReliquiaCalice = "CaliceBrasas";
    [SerializeField] private string idReliquiaLamina = "LaminaCeleste";

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
        "Heh... Veja só. Até parece mais alto agora, rapaz. Ou será que estou encolhendo de vez?",
        "E então? Sentiu alguma diferença? Vamos, não precisa me agradecer. Já estou velho demais para essas cerimônias.",
        "Ah, esse olhar... Hah! Parece que minhas mãos ainda prestam para alguma coisa, afinal.",
        "Sabe, quando era mais novo, eu teria dado tudo para carregar um poder desses. Hoje, mal consigo me levantar sem sentir a coluna estalar.",
        "Mas não pense que aquela coroa é a única maravilha esquecida por estas terras, meu jovem.",
        "Hm? Ah, vejo que isso lhe interessou. Imaginei que interessaria.",
        "Já ouviu falar de um cálice que guarda um fogo incapaz de se apagar? Não? Pois dizem que existe um desses.",
        "O chamam de Cálice de Brasas. Um nome bonito para uma coisa que provavelmente arrancaria a pele destas minhas mãos.",
        "O problema, rapaz, é que ele não está abandonado por aí, esperando alguém recolhê-lo.",
        "Há uma criatura que o guarda. Rakthas. Já ouviu esse nome alguma vez?",
        "Não? Melhor assim. Quem conhece Rakthas de perto geralmente não vive o bastante para falar dele.",
        "Dizem que o desgraçado carrega tanta fúria no peito que nem as chamas ao redor conseguem acompanhá-lo.",
        "Voragul tinha força para esmagar um homem. Rakthas, pelo que contam, não lhe daria sequer tempo de perceber o golpe.",
        "Hah! Já estou vendo essa expressão. Não, rapaz, não estou dizendo que deve sair correndo atrás dele.",
        "Mas você está procurando as relíquias, não está? Cedo ou tarde, imagino que seus caminhos acabem se cruzando.",
        "Se isso acontecer, lembre-se do que lhe digo: não tente acompanhar a fúria de Rakthas. Deixe que ele se perca nela.",
        "E quanto ao cálice... Bem, depois do que fizemos com aquela coroa, talvez eu consiga despertar algo semelhante nele.",
        "Não prometo nada, ouviu? Um velho precisa preservar um pouco da própria reputação. Hah! Agora vá, rapaz. E procure voltar inteiro."
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

    [Header("Introducao de Salenthra")]
    [SerializeField, TextArea(2, 5)]
    private string[] dialogosIntroducaoSalenthra =
    {
        "Hah! Veja só como se move, rapaz. Antes mal conseguia acreditar que voltaria vivo de Voragul.",
        "E agora aparece diante de mim carregando o fogo de Rakthas... Quem diria, hein?",
        "Não me olhe desse jeito. Estou contente por você. Só me custa admitir que um rapaz tão jovem consegue fazer o que não fiz em uma vida inteira.",
        "Sabe... A velocidade sempre me fascinou. Talvez porque estas pernas nunca tenham sido grande coisa. Hah!",
        "Mas há uma diferença entre correr depressa e ser rápido o bastante para que ninguém veja seu movimento.",
        "Já ouviu falar de Salenthra?",
        "Hm... Imaginei. Não é um nome que se escute por aí com muita frequência.",
        "Dizem que ela empunha uma lâmina tão veloz que seus inimigos só compreendem o golpe quando já estão caindo.",
        "Uma espada dessas nas minhas mãos? Hah! Eu provavelmente cortaria a própria barba antes de dar o primeiro passo.",
        "Mas não se engane, meu jovem. Salenthra não é apenas uma criatura veloz. Ela sabe aproveitar cada abertura que encontra.",
        "Voragul podia ser vencido com paciência. Rakthas, deixando sua fúria trabalhar contra ele. Salenthra... bem, eu não contaria com a mesma sorte.",
        "O quê? Parece preocupado? Não há vergonha nenhuma nisso, rapaz. Até os mais corajosos precisam saber quando ter medo.",
        "O curioso é que sua lâmina guarda um poder que vai muito além de ferir. Pelo menos, é o que dizem os velhos escritos.",
        "Heh... Já está pensando no que poderia fazer com esse poder, não é? Não adianta esconder essa expressão de mim.",
        "Se um dia encontrar Salenthra, observe bem seus movimentos. Às vezes, o segredo para vencer alguém está justamente naquilo que mais a torna perigosa.",
        "E se conseguir trazer aquela lâmina até aqui... quem sabe? Talvez estas mãos ainda consigam lhe ensinar a alcançar lugares que seus pés jamais alcançariam.",
        "Agora vá, rapaz. E não se esqueça de voltar. Já me acostumei com sua companhia, sabe?"
    };

    [Header("Retorno com a Lamina")]
    [SerializeField, TextArea(2, 5)]
    private string[] dialogosLamina =
    {
        "Ora, ora... Aí está você novamente. E eu aqui, pensando que finalmente teria uma tarde de sossego.",
        "Espere um instante... Essa espada... Não pode ser.",
        "Você enfrentou Salenthra? E está aqui, diante de mim, como se tivesse acabado de voltar de um passeio?",
        "Hah! Não faça essa cara. Deixe um velho demonstrar surpresa de vez em quando.",
        "Venha, aproxime-se. Quero ver aquela lâmina com meus próprios olhos.",
        "Ah... Que coisa extraordinária...",
        "Hm? O quê? Oh, perdoe-me, rapaz. Às vezes minha memória me leva a lugares onde já não deveria entrar.",
        "Sabe, há coisas que o tempo leva de um homem. A força, a juventude... até os rostos daqueles que amamos.",
        "Mas há outras que permanecem. E o poder desta espada é uma delas.",
        "Bem, meu jovem, não vamos ficar aqui alimentando a nostalgia de um velho.",
        "Você cumpriu sua parte. Agora é minha vez de cumprir a minha.",
        "Aproxime-se. Vamos descobrir até onde essas pernas podem levá-lo."
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
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slotAtual);

        if (dados == null)
            return;

        bool possuiCoroa =
            PossuiReliquia(dados, idReliquiaCoroa);

        bool possuiCalice =
            PossuiReliquia(dados, idReliquiaCalice);

        bool possuiLamina =
            PossuiReliquia(dados, idReliquiaLamina);

        bool possuiAscensao =
            PossuiBencao(dados, "Ascensao");

        bool possuiInvestidaFlamejante =
            PossuiBencao(dados, "InvestidaFlamejante");

        bool possuiInvestidaCeleste =
            PossuiBencao(dados, "InvestidaCeleste");

        if (!dados.apresentacaoAnciaoConcluida)
        {
            tipoConversaAtual = TipoConversa.Apresentacao;
            dialogosAtuais = dialogosPrimeiroEncontro;
        }
        else if (possuiCoroa && !possuiAscensao)
        {
            tipoConversaAtual = TipoConversa.Coroa;
            dialogosAtuais = dialogosCoroa;
        }
        else if (possuiAscensao &&
                 !dados.historiaRakthasContada)
        {
            tipoConversaAtual = TipoConversa.IntroducaoRakthas;
            dialogosAtuais = dialogosIntroducaoRakthas;
        }
        else if (possuiCalice && !possuiInvestidaFlamejante)
        {
            tipoConversaAtual = TipoConversa.Calice;
            dialogosAtuais = dialogosCalice;
        }
        else if (possuiInvestidaFlamejante &&
                 !dados.historiaSalenthraContada)
        {
            tipoConversaAtual = TipoConversa.IntroducaoSalenthra;
            dialogosAtuais = dialogosIntroducaoSalenthra;
        }
        else if (possuiLamina && !possuiInvestidaCeleste)
        {
            tipoConversaAtual = TipoConversa.Lamina;
            dialogosAtuais = dialogosLamina;
        }
        else
        {
            tipoConversaAtual = TipoConversa.Retorno;
            dialogosAtuais = dialogosRetorno;
        }

        if (dialogosAtuais == null || dialogosAtuais.Length == 0)
            return;

        indiceDialogo = 0;
        conversaAberta = true;

        movimento.podeMover = false;
        animatorElian.SetFloat("velocidade", 0f);

        painelConversa.SetActive(true);
        MostrarDialogo();
    }

    private bool PossuiReliquia(DadosSalvamento dados, string id)
    {
        return dados.reliquiasObtidas != null &&
               dados.reliquiasObtidas.Contains(id);
    }

    private bool PossuiBencao(DadosSalvamento dados, string id)
    {
        return dados.bencaosRecebidas != null &&
               dados.bencaosRecebidas.Contains(id);
    }

    private void MostrarDialogo()
    {
        textoConversa.text = dialogosAtuais[indiceDialogo];
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
                RegistrarProgressoConversa(tipoConversaAtual);
                FecharConversa();
                break;

            case TipoConversa.IntroducaoRakthas:
            case TipoConversa.IntroducaoSalenthra:
                RegistrarProgressoConversa(tipoConversaAtual);
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

            case TipoConversa.Lamina:
                IniciarEntregaBencao(
                    TipoBencao.InvestidaCeleste
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
        elianPerto.IniciarBencao(this);
    }

    public void ConcederBencao()
    {
        if (!bencaoPendente || movimento == null)
            return;

        ElianBencaos bencaos =
            movimento.GetComponent<ElianBencaos>();

        if (bencaos == null)
        {
            Debug.LogError("ElianBencaos nao encontrado.");
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
                sucesso = bencaos.ConcederInvestidaFlamejante();
                break;

            case TipoBencao.InvestidaCeleste:
                sucesso = bencaos.ConcederInvestidaCeleste();
                break;
        }

        if (sucesso)
        {
            tutorialDaBencao = bencaoEmEntrega;
            tutorialPendente = true;

            Debug.Log("Bencao concedida: " + bencaoEmEntrega);
        }

        bencaoPendente = false;
        bencaoEmEntrega = TipoBencao.Nenhuma;
    }

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

            case TipoBencao.InvestidaCeleste:
                tutorial = tutorialInvestidaCeleste;
                break;
        }

        tutorialDaBencao = TipoBencao.Nenhuma;

        if (gerenciadorTutoriais == null || tutorial == null)
        {
            Debug.LogWarning("Configure o tutorial desta bencao.");
            return;
        }

        if (!gerenciadorTutoriais.AbrirTutorial(tutorial))
        {
            Debug.LogWarning("Nao foi possivel abrir o tutorial.");
        }
    }

    private void RegistrarProgressoConversa(TipoConversa tipo)
    {
        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slotAtual);

        if (dados == null)
            return;

        switch (tipo)
        {
            case TipoConversa.Apresentacao:
                dados.apresentacaoAnciaoConcluida = true;
                break;

            case TipoConversa.IntroducaoRakthas:
                dados.historiaRakthasContada = true;
                break;

            case TipoConversa.IntroducaoSalenthra:
                dados.historiaSalenthraContada = true;
                break;
        }

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

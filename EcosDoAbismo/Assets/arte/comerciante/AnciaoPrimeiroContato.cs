
using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
public class AnciaoPrimeiroContato : MonoBehaviour
{
    [Header("Interface do Anciao")]
    [SerializeField] private GameObject painelConversa;
    [SerializeField] private TMP_Text textoConversa;

    [Header("Dialogos do Primeiro Contato")]
    [SerializeField, TextArea(2, 5)]
    private string[] dialogos =
    {
        "Ei! Ei, rapaz! Aqui! Por favor, não passe direto!",

        "Ahh... Pelos céus, finalmente alguém que não pretende arrancar minha cabeça.",

        "Você... Você sabe usar essa espada, não sabe? Hah! Que pergunta idiota. Claro que sabe.",

        "Escute, meu jovem... Há umas criaturas rondando por aqui. Quase acabaram comigo.",

        "Estas pernas já não me ajudam muito, sabe? Se puder dar um jeito naqueles desgraçados, ficarei lhe devendo uma.",

        "Vá, rapaz! Eu espero aqui. E tome cuidado... Ainda preciso agradecer direito quando voltar!"
    };

    private ElianMovimento movimento;
    private Rigidbody2D rbElian;
    private Animator animatorElian;

    private bool conversaAberta;
    private int indiceDialogo;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (conversaAberta || outro.isTrigger)
            return;

        Rigidbody2D rb = outro.attachedRigidbody;

        if (rb == null || rb.gameObject.name != "elian")
            return;

        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot < 1 || slot > 3)
            return;

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slot);

        if (dados == null)
            return;

        // Nao repete apos concluir o primeiro contato.
        if (dados.primeiroContatoAnciaoConcluido)
            return;

        // Nao aparece se o Anciao ja foi resgatado
        // ou sua apresentacao ja aconteceu.
        if (dados.resgateAnciaoConcluido ||
            dados.apresentacaoAnciaoConcluida)
        {
            return;
        }

        movimento = rb.GetComponent<ElianMovimento>();
        animatorElian = rb.GetComponent<Animator>();
        rbElian = rb;

        if (movimento == null ||
            animatorElian == null ||
            painelConversa == null ||
            textoConversa == null ||
            dialogos == null ||
            dialogos.Length == 0)
        {
            Debug.LogError(
                "Primeiro contato: referencias incompletas."
            );
            return;
        }

        AbrirConversa();
    }

    private void AbrirConversa()
    {
        conversaAberta = true;
        indiceDialogo = 0;

        movimento.podeMover = false;
        rbElian.linearVelocity = Vector2.zero;

        animatorElian.SetFloat("velocidade", 0f);

        textoConversa.text = dialogos[indiceDialogo];
        painelConversa.SetActive(true);

        Debug.Log(
            "Primeiro contato com o Anciao iniciado."
        );
    }

    private void Update()
    {
        if (!conversaAberta)
            return;

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AvancarDialogo();
        }
    }

    private void AvancarDialogo()
    {
        indiceDialogo++;

        if (indiceDialogo < dialogos.Length)
        {
            textoConversa.text = dialogos[indiceDialogo];
            return;
        }

        FinalizarConversa();
    }

    private void FinalizarConversa()
    {
        conversaAberta = false;
        painelConversa.SetActive(false);

        int slot = MenuSelecaoSave.slotPartidaAtual;

        if (slot >= 1 && slot <= 3)
        {
            DadosSalvamento dados =
                GerenciadorSalvamento.Carregar(slot);

            if (dados != null)
            {
                dados.primeiroContatoAnciaoConcluido = true;

                if (ContadorTempoJogo.instancia != null)
                {
                    ContadorTempoJogo.instancia
                        .AtualizarTempoNosDados(dados);
                }

                if (!GerenciadorSalvamento.Salvar(slot, dados))
                {
                    Debug.LogError(
                        "Falha ao salvar o primeiro contato."
                    );
                }
            }
        }

        if (movimento != null)
        {
            ElianVida vida =
                movimento.GetComponent<ElianVida>();

            if (vida == null || !vida.EstaMorto)
            {
                movimento.podeMover = true;
            }
        }

        Debug.Log(
            "Primeiro contato concluido."
        );
    }
}

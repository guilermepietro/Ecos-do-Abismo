
using UnityEngine;

public class AnciaoInteracao : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private GameObject painelConversa;

    private ElianColeta elianPerto;
    private ElianMovimento movimento;
    private Animator animatorElian;

    private bool conversaAberta;
    private bool conversaLiberada = true;

    public void DefinirConversaLiberada(bool liberada)
    {
        conversaLiberada = liberada;

        if (!liberada && conversaAberta)
        {
            FecharConversa();
        }
    }

    void Start()
    {
        if (painelConversa != null)
        {
            painelConversa.SetActive(false);
        }
    }

    void Update()
    {
        if (conversaAberta && Input.GetKeyDown(KeyCode.Escape))
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
        {
            FecharConversa();
        }

        elianPerto.DefinirAnciaoPerto(null);
        elianPerto = null;
    }

    public void AbrirConversa()
    {
        if (!conversaLiberada ||
            conversaAberta ||
            elianPerto == null)
        {
            return;
        }

        movimento =
            elianPerto.GetComponent<ElianMovimento>();

        animatorElian =
            elianPerto.GetComponent<Animator>();

        if (movimento == null || animatorElian == null)
            return;

        if (painelConversa == null)
        {
            Debug.LogWarning(
                "Configure o Painel Conversa no Anciao."
            );
            return;
        }

        conversaAberta = true;

        movimento.podeMover = false;
        animatorElian.SetFloat("velocidade", 0);

        painelConversa.SetActive(true);
    }

    public void FecharConversa()
    {
        if (!conversaAberta)
            return;

        conversaAberta = false;

        if (painelConversa != null)
        {
            painelConversa.SetActive(false);
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
    }
}

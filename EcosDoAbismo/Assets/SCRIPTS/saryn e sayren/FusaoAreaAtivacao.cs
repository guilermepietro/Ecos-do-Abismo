
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FusaoAreaAtivacao : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private BarraVidaFusao barraVidaFusao;

    [Header("Cutscene")]
    [SerializeField] private string idCutscene =
        "Fusao_Apresentacao";

    private BoxCollider2D area;
    private bool ativada = false;

    private void Awake()
    {
        area = GetComponent<BoxCollider2D>();
        area.isTrigger = true;

        if (movimento == null)
        {
            movimento =
                GetComponentInParent<FusaoMovimento>();
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        VerificarElian(outro);
    }

    private void OnTriggerStay2D(Collider2D outro)
    {
        VerificarElian(outro);
    }

    private void VerificarElian(Collider2D outro)
    {
        if (ativada || movimento == null ||
            outro == null ||
            !movimento.gameObject.activeInHierarchy)
            return;

        // Mantém a identificação pelo nome "elian".
        Transform atual = outro.transform;
        bool encontrouElian = false;

        while (atual != null)
        {
            if (atual.name == "elian")
            {
                encontrouElian = true;
                break;
            }

            atual = atual.parent;
        }

        if (!encontrouElian)
            return;

        ativada = true;

        // Apresentação antes do combate.
        if (GerenciadorCutscenes.Instancia != null)
        {
            bool iniciou =
                GerenciadorCutscenes.Instancia.Reproduzir(
                    idCutscene,
                    IniciarCombate
                );

            if (iniciou)
                return;
        }

        // Segurança caso o vídeo esteja indisponível.
        IniciarCombate();
    }

    private void IniciarCombate()
    {
        if (movimento == null ||
            !movimento.gameObject.activeInHierarchy)
            return;

        movimento.AtivarCombate();

        if (barraVidaFusao != null)
            barraVidaFusao.Exibir();

        Debug.Log(
            "Fusão: apresentação concluída! " +
            "Combate iniciado."
        );
    }
}

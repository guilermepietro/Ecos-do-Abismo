
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class IshkharothAreaAtivacao : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private IshkharothMovimento movimento;
    [SerializeField] private BarraVidaIshkharoth barraVidaIshkharoth;

    [Header("Cutscene")]
    [SerializeField] private string idCutscene =
        "Ishkharoth_Apresentacao";

    private BoxCollider2D area;
    private bool ativada;

    private void Awake()
    {
        area = GetComponent<BoxCollider2D>();
        area.isTrigger = true;

        if (movimento == null)
        {
            movimento =
                GetComponentInParent<IshkharothMovimento>();
        }
    }

    private void FixedUpdate()
    {
        if (ativada || movimento == null)
            return;

        Collider2D[] encontrados =
            Physics2D.OverlapBoxAll(
                area.bounds.center,
                area.bounds.size,
                0f
            );

        foreach (Collider2D encontrado in encontrados)
        {
            VerificarElian(encontrado);

            if (ativada)
                break;
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
        if (ativada || outro == null ||
            movimento == null ||
            !movimento.gameObject.activeInHierarchy)
            return;

        ElianVida vidaElian =
            outro.GetComponentInParent<ElianVida>();

        if (vidaElian == null)
            return;

        AtivarApresentacao();
    }

    private void AtivarApresentacao()
    {
        if (ativada)
            return;

        ativada = true;

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

        Debug.LogWarning(
            "Cutscene do Ishkharoth indisponível."
        );

        IniciarCombate();
    }

    private void IniciarCombate()
    {
        if (movimento == null ||
            !movimento.gameObject.activeInHierarchy)
            return;

        movimento.AtivarCombate();

        // NOVO: bloqueia teletransportes.
        Teletransporte.BloquearDuranteCombate(
            movimento.gameObject
        );

        if (barraVidaIshkharoth != null)
            barraVidaIshkharoth.Exibir();

        Debug.Log(
            "Ishkharoth: apresentação concluída! " +
            "Combate iniciado."
        );
    }
}

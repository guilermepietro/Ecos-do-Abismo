
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class IshkharothAreaAtivacao : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private IshkharothMovimento movimento;
    [SerializeField] private BarraVidaIshkharoth barraVidaIshkharoth;

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

        // Verifica se Elian esta dentro da area.
        // Funciona mesmo que OnTriggerEnter2D
        // nao tenha sido executado.
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
        if (ativada || outro == null || movimento == null)
            return;

        ElianVida vidaElian =
            outro.GetComponentInParent<ElianVida>();

        if (vidaElian == null)
            return;

        AtivarCombate();
    }

    private void AtivarCombate()
    {
        if (ativada)
            return;

        ativada = true;

        movimento.AtivarCombate();

        if (barraVidaIshkharoth != null)
            barraVidaIshkharoth.Exibir();

        Debug.Log(
            "Ishkharoth: combate ativado pela area!"
        );
    }
}

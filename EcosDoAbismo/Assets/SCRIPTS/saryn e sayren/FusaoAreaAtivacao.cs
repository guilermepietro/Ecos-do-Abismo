
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FusaoAreaAtivacao : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private BarraVidaFusao barraVidaFusao;

    private bool ativada;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;

        if (movimento == null)
            movimento = GetComponentInParent<FusaoMovimento>();
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
        if (ativada || movimento == null)
            return;

        Transform atual = outro.transform;

        while (atual != null)
        {
            if (atual.name == "elian")
            {
                ativada = true;
                movimento.AtivarCombate();

                if (barraVidaFusao != null)
                    barraVidaFusao.Exibir();

                return;
            }

            atual = atual.parent;
        }
    }
}

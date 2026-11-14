
using UnityEngine;

public class FusaoAtaque1 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform elian;

    [Header("Projetil")]
    [SerializeField] private GameObject prefabChama;
    [SerializeField] private Transform pontoDisparo;

    private bool emAtaque;
    private float direcaoAtaque = 1f;

    public bool EmAtaque => emAtaque;

    private void Awake()
    {
        if (movimento == null)
            movimento = GetComponent<FusaoMovimento>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (elian == null)
        {
            GameObject objeto = GameObject.Find("elian");

            if (objeto != null)
                elian = objeto.transform;
        }
    }

    public void IniciarAtaque()
    {
        if (emAtaque || movimento == null ||
            !movimento.CombateAtivo || elian == null)
            return;

        emAtaque = true;

        direcaoAtaque =
            elian.position.x >= transform.position.x ? 1f : -1f;

        movimento.OlharParaElian();
        movimento.BloquearMovimento(true);

        if (animator != null)
            animator.SetTrigger("Ataque1");
    }

    // Animation Event no momento do soco.
    public void LancarChama()
    {
        if (!emAtaque || prefabChama == null || pontoDisparo == null)
            return;

        GameObject chama = Instantiate(
            prefabChama,
            pontoDisparo.position,
            Quaternion.identity
        );

        ChamaFusao projetil = chama.GetComponent<ChamaFusao>();

        if (projetil != null)
            projetil.DefinirDirecao(direcaoAtaque);
    }

    // Animation Event no ultimo frame.
    public void FinalizarAtaque1()
    {
        if (!emAtaque)
            return;

        emAtaque = false;

        if (movimento != null)
            movimento.BloquearMovimento(false);
    }
}


using System.Collections;
using UnityEngine;

public class FusaoAtaque3 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D areaChuvaPedras;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] prefabsPedras;

    [Header("Distribuicao")]
    [Min(1)]
    [SerializeField] private int numeroSetores = 6;

    [Min(1)]
    [SerializeField] private int quantidadeOndas = 3;

    [Header("Tempos")]
    [Min(0f)]
    [SerializeField] private float intervaloEntrePedras = 0.15f;

    [Min(0f)]
    [SerializeField] private float intervaloEntreOndas = 0.45f;

    [Min(0f)]
    [SerializeField] private float tempoAposUltimaPedra = 1f;

    [Header("Posicionamento")]
    [Min(0f)]
    [SerializeField] private float margemHorizontal = 0.25f;

    [Min(0f)]
    [SerializeField] private float margemTeto = 0.25f;

    private bool emAtaque;
    private bool chuvaIniciada;
    private bool chuvaTerminou;
    private bool animacaoTerminou;

    public bool EmAtaque => emAtaque;

    private void Awake()
    {
        if (movimento == null)
            movimento = GetComponent<FusaoMovimento>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void IniciarAtaque()
    {
        if (emAtaque || movimento == null ||
            !movimento.CombateAtivo)
            return;

        if (areaChuvaPedras == null ||
            prefabsPedras == null ||
            prefabsPedras.Length == 0)
        {
            Debug.LogWarning(
                "FusaoAtaque3: Configure a area e as pedras."
            );
            return;
        }

        foreach (GameObject prefab in prefabsPedras)
        {
            if (prefab == null)
            {
                Debug.LogWarning(
                    "FusaoAtaque3: Existe um prefab vazio."
                );
                return;
            }
        }

        emAtaque = true;
        chuvaIniciada = false;
        chuvaTerminou = false;
        animacaoTerminou = false;

        movimento.OlharParaElian();
        movimento.BloquearMovimento(true);

        if (animator != null)
        {
            animator.SetBool("ChuvaTerminou", false);
            animator.SetTrigger("Ataque3");
        }
    }

    // Animation Event no inicio do grito.
    public void IniciarChuva()
    {
        if (!emAtaque || chuvaIniciada)
            return;

        chuvaIniciada = true;

        StartCoroutine(ExecutarChuva());
    }

    private IEnumerator ExecutarChuva()
    {
        Bounds limites = areaChuvaPedras.bounds;

        float minimoX = limites.min.x + margemHorizontal;
        float maximoX = limites.max.x - margemHorizontal;

        if (minimoX >= maximoX)
        {
            minimoX = limites.min.x;
            maximoX = limites.max.x;
        }

        float altura = limites.max.y - margemTeto;
        int setores = Mathf.Max(1, numeroSetores);
        int ondas = Mathf.Max(1, quantidadeOndas);

        int[] ordem = new int[setores];

        for (int onda = 0; onda < ondas; onda++)
        {
            for (int i = 0; i < setores; i++)
                ordem[i] = i;

            // Embaralha a ordem dos setores.
            for (int i = 0; i < setores; i++)
            {
                int aleatorio = Random.Range(i, setores);

                int temporario = ordem[i];
                ordem[i] = ordem[aleatorio];
                ordem[aleatorio] = temporario;
            }

            for (int i = 0; i < setores; i++)
            {
                int setor = ordem[i];

                float inicioSetor = Mathf.Lerp(
                    minimoX,
                    maximoX,
                    (float)setor / setores
                );

                float fimSetor = Mathf.Lerp(
                    minimoX,
                    maximoX,
                    (float)(setor + 1) / setores
                );

                float posicaoX = Mathf.Lerp(
                    inicioSetor,
                    fimSetor,
                    Random.Range(0.15f, 0.85f)
                );

                GameObject prefab = prefabsPedras[
                    Random.Range(0, prefabsPedras.Length)
                ];

                Vector3 posicao = new Vector3(
                    posicaoX,
                    altura,
                    transform.position.z
                );

                Instantiate(
                    prefab,
                    posicao,
                    prefab.transform.rotation
                );

                if (intervaloEntrePedras > 0f)
                {
                    yield return new WaitForSeconds(
                        intervaloEntrePedras
                    );
                }
                else
                {
                    yield return null;
                }
            }

            if (onda < ondas - 1 &&
                intervaloEntreOndas > 0f)
            {
                yield return new WaitForSeconds(
                    intervaloEntreOndas
                );
            }
        }

        yield return new WaitForSeconds(tempoAposUltimaPedra);

        chuvaTerminou = true;

        // Libera a transicao do loop para o final.
        if (animator != null)
            animator.SetBool("ChuvaTerminou", true);

        TentarFinalizarAtaque();
    }

    // Animation Event no final de Ataque3Fim.
    public void FinalizarAtaque3()
    {
        if (!emAtaque)
            return;

        animacaoTerminou = true;
        TentarFinalizarAtaque();
    }

    private void TentarFinalizarAtaque()
    {
        if (!emAtaque ||
            !animacaoTerminou ||
            !chuvaTerminou)
            return;

        emAtaque = false;

        if (movimento != null)
            movimento.BloquearMovimento(false);
    }
}

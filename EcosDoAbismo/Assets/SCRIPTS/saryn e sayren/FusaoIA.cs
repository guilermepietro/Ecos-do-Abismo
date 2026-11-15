
using UnityEngine;

public class FusaoIA : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private FusaoAtaque1 ataque1;
    [SerializeField] private FusaoAtaque2 ataque2;
    [SerializeField] private FusaoAtaque3 ataque3;
    [SerializeField] private Transform elian;

    [Header("Configuracoes")]
    [SerializeField] private float distanciaMaximaAtaque = 7f;
    [SerializeField] private float intervaloEntreAtaques = 3.5f;
    [SerializeField] private float atrasoInicial = 1.5f;

    [Header("Distancias")]
    [SerializeField] private float distanciaCurta = 3f;
    [SerializeField] private float distanciaLonga = 5f;

    [Header("Anti-Repeticao")]
    [Range(0f, 1f)]
    [SerializeField] private float penalidadeRetorno = 0.4f;

    private ElianVida vidaElian;

    private float proximoAtaque;
    private bool iniciouCombate;
    private bool aguardandoFimAtaque;

    private int ultimoAtaque;
    private int penultimoAtaque;

    private void Awake()
    {
        if (movimento == null)
            movimento = GetComponent<FusaoMovimento>();

        if (ataque1 == null)
            ataque1 = GetComponent<FusaoAtaque1>();

        if (ataque2 == null)
            ataque2 = GetComponent<FusaoAtaque2>();

        if (ataque3 == null)
            ataque3 = GetComponent<FusaoAtaque3>();

        if (elian == null)
        {
            GameObject objeto = GameObject.Find("elian");

            if (objeto != null)
                elian = objeto.transform;
        }

        if (elian != null)
            vidaElian = elian.GetComponent<ElianVida>();
    }

    private void Update()
    {
        if (movimento == null ||
            ataque1 == null ||
            ataque2 == null ||
            ataque3 == null ||
            elian == null)
            return;

        if (!movimento.CombateAtivo)
        {
            iniciouCombate = false;
            aguardandoFimAtaque = false;
            ultimoAtaque = 0;
            penultimoAtaque = 0;
            return;
        }

        if (!iniciouCombate)
        {
            iniciouCombate = true;
            proximoAtaque = Time.time + atrasoInicial;
        }

        if (vidaElian != null && vidaElian.EstaMorto)
            return;

        if (aguardandoFimAtaque)
        {
            if (ataque1.EmAtaque ||
                ataque2.EmAtaque ||
                ataque3.EmAtaque)
                return;

            aguardandoFimAtaque = false;

            proximoAtaque =
                Time.time + intervaloEntreAtaques;
        }

        if (ataque1.EmAtaque ||
            ataque2.EmAtaque ||
            ataque3.EmAtaque)
            return;

        if (Time.time < proximoAtaque)
            return;

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        if (distancia > distanciaMaximaAtaque)
            return;

        EscolherAtaque(distancia);
    }

    private void EscolherAtaque(float distancia)
    {
        float peso1;
        float peso2;
        float peso3;

        if (distancia < distanciaCurta)
        {
            peso1 = 5f;
            peso2 = 5f;
            peso3 = 2f;
        }
        else if (distancia < distanciaLonga)
        {
            peso1 = 6f;
            peso2 = 4f;
            peso3 = 4f;
        }
        else
        {
            peso1 = 4f;
            peso2 = 4f;
            peso3 = 6f;
        }

        // Proibe repetir o ultimo ataque.
        if (ultimoAtaque == 1)
            peso1 = 0f;

        if (ultimoAtaque == 2)
            peso2 = 0f;

        if (ultimoAtaque == 3)
            peso3 = 0f;

        // Reduz a chance do padrao A-B-A.
        if (penultimoAtaque == 1)
            peso1 *= penalidadeRetorno;

        if (penultimoAtaque == 2)
            peso2 *= penalidadeRetorno;

        if (penultimoAtaque == 3)
            peso3 *= penalidadeRetorno;

        float pesoTotal = peso1 + peso2 + peso3;

        if (pesoTotal <= 0f)
            return;

        float sorteio = Random.Range(0f, pesoTotal);

        int escolhido;

        if (sorteio < peso1)
        {
            escolhido = 1;
        }
        else if (sorteio < peso1 + peso2)
        {
            escolhido = 2;
        }
        else
        {
            escolhido = 3;
        }

        switch (escolhido)
        {
            case 1:
                ataque1.IniciarAtaque();
                break;

            case 2:
                ataque2.IniciarAtaque();
                break;

            case 3:
                ataque3.IniciarAtaque();
                break;
        }

        bool iniciou =
            escolhido == 1 && ataque1.EmAtaque ||
            escolhido == 2 && ataque2.EmAtaque ||
            escolhido == 3 && ataque3.EmAtaque;

        if (!iniciou)
        {
            proximoAtaque = Time.time + 1f;
            return;
        }

        penultimoAtaque = ultimoAtaque;
        ultimoAtaque = escolhido;

        aguardandoFimAtaque = true;
    }
}

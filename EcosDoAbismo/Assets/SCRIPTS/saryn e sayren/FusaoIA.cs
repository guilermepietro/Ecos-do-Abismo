
using UnityEngine;

public class FusaoIA : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FusaoMovimento movimento;
    [SerializeField] private FusaoAtaque1 ataque1;
    [SerializeField] private FusaoAtaque2 ataque2;
    [SerializeField] private Transform elian;

    [Header("Configuracoes")]
    [SerializeField] private float distanciaMaximaAtaque = 7f;
    [SerializeField] private float intervaloEntreAtaques = 3.5f;
    [SerializeField] private float atrasoInicial = 1.5f;

    private ElianVida vidaElian;

    private float proximoAtaque;
    private bool iniciouCombate;
    private bool aguardandoFimAtaque;

    private int ultimoAtaque;

    private void Awake()
    {
        if (movimento == null)
            movimento = GetComponent<FusaoMovimento>();

        if (ataque1 == null)
            ataque1 = GetComponent<FusaoAtaque1>();

        if (ataque2 == null)
            ataque2 = GetComponent<FusaoAtaque2>();

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
        if (movimento == null || ataque1 == null ||
            ataque2 == null || elian == null)
            return;

        if (!movimento.CombateAtivo)
        {
            iniciouCombate = false;
            aguardandoFimAtaque = false;
            ultimoAtaque = 0;
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
            if (ataque1.EmAtaque || ataque2.EmAtaque)
                return;

            aguardandoFimAtaque = false;

            proximoAtaque =
                Time.time + intervaloEntreAtaques;
        }

        if (Time.time < proximoAtaque)
            return;

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        if (distancia > distanciaMaximaAtaque)
            return;

        EscolherAtaque();
    }

    private void EscolherAtaque()
    {
        int escolhido;

        if (ultimoAtaque == 1)
        {
            escolhido = 2;
        }
        else if (ultimoAtaque == 2)
        {
            escolhido = 1;
        }
        else
        {
            escolhido = Random.Range(1, 3);
        }

        if (escolhido == 1)
        {
            ataque1.IniciarAtaque();

            if (!ataque1.EmAtaque)
            {
                proximoAtaque = Time.time + 1f;
                return;
            }
        }
        else
        {
            ataque2.IniciarAtaque();

            if (!ataque2.EmAtaque)
            {
                proximoAtaque = Time.time + 1f;
                return;
            }
        }

        ultimoAtaque = escolhido;
        aguardandoFimAtaque = true;
    }
}

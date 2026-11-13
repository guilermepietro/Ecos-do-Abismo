
using UnityEngine;

[RequireComponent(typeof(IshkharothMovimento))]
[RequireComponent(typeof(Animator))]
public class IshkharothIA : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private Animator animator;
    [SerializeField] private IshkharothMovimento movimento;

    [Header("Combate")]
    [SerializeField] private float distanciaMaximaAtaque = 8f;
    [SerializeField] private float alcanceEspinhosChao = 6f;
    [SerializeField] private float intervaloEntreAtaques = 1.3f;

    [Header("Recarga dos ataques")]
    [SerializeField] private float recargaEsfera = 2.5f;
    [SerializeField] private float recargaEspinhosChao = 4f;
    [SerializeField] private float recargaEspinhosCeu = 5f;

    private bool atacando;
    private float proximaDecisao;

    private readonly float[] proximasRecargas = new float[3];

    private int ultimoAtaque = -1;
    private int penultimoAtaque = -1;

    // Controle especial da esfera.
    private bool aguardandoEsfera;
    private bool animacaoAtaqueTerminou;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movimento == null)
            movimento = GetComponent<IshkharothMovimento>();
    }

    private void Update()
    {
        if (elian == null || movimento == null || animator == null)
            return;

        if (!movimento.CombateAtivo || atacando)
            return;

        if (Time.time < proximaDecisao)
            return;

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        if (distancia > distanciaMaximaAtaque)
            return;

        int ataqueEscolhido = EscolherAtaque(distancia);

        if (ataqueEscolhido >= 0)
            ExecutarAtaque(ataqueEscolhido);
    }

    private int EscolherAtaque(float distancia)
    {
        float[] pesos;

        if (distancia <= 3f)
        {
            pesos = new float[] { 1f, 5f, 3f };
        }
        else if (distancia <= 6f)
        {
            pesos = new float[] { 3f, 4f, 3f };
        }
        else
        {
            pesos = new float[] { 5f, 0f, 4f };
        }

        float somaPesos = 0f;

        for (int i = 0; i < pesos.Length; i++)
        {
            if (Time.time < proximasRecargas[i])
                pesos[i] = 0f;

            if (i == ultimoAtaque)
                pesos[i] = 0f;

            if (i == penultimoAtaque)
                pesos[i] *= 0.35f;

            if (i == 1 && distancia > alcanceEspinhosChao)
                pesos[i] = 0f;

            somaPesos += pesos[i];
        }

        if (somaPesos <= 0f)
            return -1;

        float sorteio = Random.Range(0f, somaPesos);

        for (int i = 0; i < pesos.Length; i++)
        {
            sorteio -= pesos[i];

            if (sorteio <= 0f && pesos[i] > 0f)
                return i;
        }

        return -1;
    }

    private void ExecutarAtaque(int indice)
    {
        atacando = true;

        aguardandoEsfera = false;
        animacaoAtaqueTerminou = false;

        movimento.BloquearMovimento();

        animator.SetBool("Andando", false);

        penultimoAtaque = ultimoAtaque;
        ultimoAtaque = indice;

        switch (indice)
        {
            case 0:
                proximasRecargas[0] = Time.time + recargaEsfera;
                animator.SetTrigger("Ataque1");
                break;

            case 1:
                proximasRecargas[1] = Time.time + recargaEspinhosChao;
                animator.SetTrigger("Ataque2");
                break;

            case 2:
                proximasRecargas[2] = Time.time + recargaEspinhosCeu;
                animator.SetTrigger("Ataque3");
                break;
        }
    }

    // Chamado pelo script de disparo.
    public void AguardarAfastamentoEsfera()
    {
        if (atacando)
            aguardandoEsfera = true;
    }

    // Chamado quando a esfera se afasta do corpo.
    public void EsferaAfastou()
    {
        if (!atacando || !aguardandoEsfera)
            return;

        aguardandoEsfera = false;

        if (animacaoAtaqueTerminou)
            ConcluirAtaque();
    }

    // Animation Event no final de cada ataque.
    public void FinalizarAtaque()
    {
        if (!atacando)
            return;

        animacaoAtaqueTerminou = true;

        if (aguardandoEsfera)
            return;

        ConcluirAtaque();
    }

    private void ConcluirAtaque()
    {
        if (!atacando)
            return;

        atacando = false;
        aguardandoEsfera = false;
        animacaoAtaqueTerminou = false;

        movimento.LiberarMovimento();

        proximaDecisao = Time.time + intervaloEntreAtaques;
    }

    private void OnDisable()
    {
        if (atacando && movimento != null)
            movimento.LiberarMovimento();

        atacando = false;
        aguardandoEsfera = false;
        animacaoAtaqueTerminou = false;
    }
}


using UnityEngine;

public class IshkharothIAFase2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private Animator animator;
    [SerializeField] private IshkharothVida vida;
    [SerializeField] private IshkharothMovimentoFase2 movimento;

    [Header("Combate")]
    [SerializeField] private float distanciaMaximaAtaque = 10f;
    [SerializeField] private float intervaloEntreAtaques = 1f;
    [SerializeField] private float esperaInicial = 1.4f;

    [Header("Recargas")]
    [SerializeField] private float recargaAtaque1 = 2f;
    [SerializeField] private float recargaAtaque2 = 3.5f;
    [SerializeField] private float recargaAtaque3 = 5f;

    private bool combateIniciado;
    private bool atacando;

    private float proximaDecisao;
    private readonly float[] recargas = new float[3];

    private int ultimoAtaque = -1;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (vida == null)
            vida = GetComponent<IshkharothVida>();

        if (movimento == null)
            movimento = GetComponent<IshkharothMovimentoFase2>();
    }

    private void Update()
    {
        if (vida == null || !vida.Fase2Iniciada)
            return;

        if (movimento == null ||
            !movimento.Fase2Ativa ||
            elian == null ||
            animator == null)
            return;

        if (!combateIniciado)
        {
            combateIniciado = true;
            proximaDecisao = Time.time + esperaInicial;
        }

        if (atacando || Time.time < proximaDecisao)
            return;

        float distancia = Mathf.Abs(
            elian.position.x - transform.position.x
        );

        if (distancia > distanciaMaximaAtaque)
            return;

        int ataque = EscolherAtaque(distancia);

        if (ataque >= 0)
            ExecutarAtaque(ataque);
    }

    private int EscolherAtaque(float distancia)
    {
        float[] pesos;

        if (distancia <= 4f)
        {
            // Curta distancia:
            // projetil curto e pilares favorecidos.
            pesos = new float[] { 5f, 2f, 4f };
        }
        else if (distancia <= 7f)
        {
            // Media distancia:
            // distribuicao mais equilibrada.
            pesos = new float[] { 3f, 4f, 5f };
        }
        else
        {
            // Longa distancia:
            // meia-lua favorecida.
            pesos = new float[] { 2f, 5f, 4f };
        }

        float soma = 0f;

        for (int i = 0; i < pesos.Length; i++)
        {
            if (i == ultimoAtaque ||
                Time.time < recargas[i])
            {
                pesos[i] = 0f;
            }

            soma += pesos[i];
        }

        if (soma <= 0f)
            return -1;

        float sorteio = Random.Range(0f, soma);

        for (int i = 0; i < pesos.Length; i++)
        {
            if (pesos[i] <= 0f)
                continue;

            sorteio -= pesos[i];

            if (sorteio <= 0f)
                return i;
        }

        return -1;
    }

    private void ExecutarAtaque(int indice)
    {
        atacando = true;
        ultimoAtaque = indice;

        movimento.BloquearMovimento();
        animator.SetBool("Andando", false);

        switch (indice)
        {
            case 0:
                recargas[0] = Time.time + recargaAtaque1;
                animator.SetTrigger("Ataque1F2");
                break;

            case 1:
                recargas[1] = Time.time + recargaAtaque2;
                animator.SetTrigger("Ataque2F2");
                break;

            case 2:
                recargas[2] = Time.time + recargaAtaque3;
                animator.SetTrigger("Ataque3F2");
                break;
        }
    }

    // Animation Event dos ataques 1 e 2.
    // No ataque 3 sera chamado pelo script dos pilares.
    public void FinalizarAtaqueFase2()
    {
        if (!atacando)
            return;

        atacando = false;

        movimento.LiberarMovimento();

        proximaDecisao =
            Time.time + intervaloEntreAtaques;
    }

    private void OnDisable()
    {
        if (atacando && movimento != null)
            movimento.LiberarMovimento();

        atacando = false;
    }
}

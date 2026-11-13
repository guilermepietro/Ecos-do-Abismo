
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IshkharothAtaque3Fase2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private GameObject prefabPilar;
    [SerializeField] private Animator animator;
    [SerializeField] private IshkharothIAFase2 ia;

    [Header("Limites da Arena")]
    [SerializeField] private Transform limiteEsquerdo;
    [SerializeField] private Transform limiteDireito;
    [SerializeField] private LayerMask camadasChao;

    [Header("Invocacao")]
    [SerializeField, Min(1)] private int quantidadePilares = 6;
    [SerializeField, Min(0f)] private float intervaloPilares = 0.45f;
    [SerializeField] private float margemBordas = 0.6f;

    [Header("Distribuicao")]
    [SerializeField, Min(0)] private int pilaresDirecionados = 2;
    [SerializeField] private float espacamentoMinimo = 1.2f;

    [Header("Posicionamento no chao")]
    [SerializeField] private float alturaBuscaChao = 0.8f;
    [SerializeField] private float profundidadeBuscaChao = 1.4f;

    [Header("Dano")]
    [SerializeField] private int dano = 25;

    private Coroutine rotinaPilares;

    private bool ataqueEmAndamento;
    private bool criacaoConcluida;
    private bool animacaoFinalizada;

    private int pilaresAtivos;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (ia == null)
            ia = GetComponent<IshkharothIAFase2>();
    }

    // Animation Event do Ataque3_F2.
    public void IniciarPilaresF2()
    {
        if (ataqueEmAndamento)
            return;

        ataqueEmAndamento = true;
        criacaoConcluida = false;
        animacaoFinalizada = false;
        pilaresAtivos = 0;

        if (elian == null ||
            prefabPilar == null ||
            limiteEsquerdo == null ||
            limiteDireito == null)
        {
            Debug.LogWarning(
                "Ishkharoth: referencias dos pilares incompletas."
            );

            criacaoConcluida = true;
            return;
        }

        rotinaPilares = StartCoroutine(ExecutarPilares());
    }

    private IEnumerator ExecutarPilares()
    {
        float esquerda = Mathf.Min(
            limiteEsquerdo.position.x,
            limiteDireito.position.x
        ) + margemBordas;

        float direita = Mathf.Max(
            limiteEsquerdo.position.x,
            limiteDireito.position.x
        ) - margemBordas;

        if (direita <= esquerda)
        {
            Debug.LogWarning(
                "Ishkharoth: limites da arena invalidos."
            );

            criacaoConcluida = true;
            rotinaPilares = null;

            VerificarFimAtaque();
            yield break;
        }

        int total = Mathf.Max(1, quantidadePilares);

        int direcionados = Mathf.Clamp(
            pilaresDirecionados,
            0,
            total
        );

        int distribuidos = total - direcionados;

        List<int> ordem = new List<int>();

        // Cada indice representa um setor da arena.
        for (int i = 0; i < distribuidos; i++)
            ordem.Add(i);

        // -1 representa um pilar direcionado ao Elian.
        for (int i = 0; i < direcionados; i++)
            ordem.Add(-1);

        // Embaralha a ordem de invocacao.
        for (int i = ordem.Count - 1; i > 0; i--)
        {
            int sorteado = Random.Range(0, i + 1);

            int temporario = ordem[i];
            ordem[i] = ordem[sorteado];
            ordem[sorteado] = temporario;
        }

        List<float> posicoesUsadas = new List<float>();

        foreach (int setor in ordem)
        {
            if (!ataqueEmAndamento)
                yield break;

            float x;

            if (setor == -1)
            {
                // Mira a posicao ATUAL do Elian.
                x = Mathf.Clamp(
                    elian.position.x,
                    esquerda,
                    direita
                );

                // Evita repetir exatamente a mesma posicao
                // quando outro pilar ja foi invocado ali.
                if (!PosicaoDisponivel(x, posicoesUsadas))
                {
                    float esquerdaAlternativa = Mathf.Clamp(
                        x - espacamentoMinimo,
                        esquerda,
                        direita
                    );

                    float direitaAlternativa = Mathf.Clamp(
                        x + espacamentoMinimo,
                        esquerda,
                        direita
                    );

                    if (Random.value < 0.5f)
                    {
                        if (PosicaoDisponivel(
                            esquerdaAlternativa, posicoesUsadas))
                        {
                            x = esquerdaAlternativa;
                        }
                        else if (PosicaoDisponivel(
                            direitaAlternativa, posicoesUsadas))
                        {
                            x = direitaAlternativa;
                        }
                    }
                    else
                    {
                        if (PosicaoDisponivel(
                            direitaAlternativa, posicoesUsadas))
                        {
                            x = direitaAlternativa;
                        }
                        else if (PosicaoDisponivel(
                            esquerdaAlternativa, posicoesUsadas))
                        {
                            x = esquerdaAlternativa;
                        }
                    }
                }
            }
            else
            {
                float larguraSetor =
                    (direita - esquerda) / distribuidos;

                float inicioSetor =
                    esquerda + setor * larguraSetor;

                float fimSetor =
                    inicioSetor + larguraSetor;

                x = SortearPosicao(
                    inicioSetor,
                    fimSetor,
                    posicoesUsadas
                );
            }

            posicoesUsadas.Add(x);

            CriarPilar(x, esquerda, direita);

            yield return new WaitForSeconds(
                intervaloPilares
            );
        }

        criacaoConcluida = true;
        rotinaPilares = null;

        VerificarFimAtaque();
    }

    private float SortearPosicao(
        float inicio,
        float fim,
        List<float> posicoesUsadas)
    {
        float candidato = (inicio + fim) * 0.5f;

        for (int tentativa = 0; tentativa < 15; tentativa++)
        {
            float x = Random.Range(inicio, fim);

            candidato = x;

            if (PosicaoDisponivel(x, posicoesUsadas))
                return x;
        }

        return candidato;
    }

    private bool PosicaoDisponivel(
        float x,
        List<float> posicoesUsadas)
    {
        foreach (float posicao in posicoesUsadas)
        {
            if (Mathf.Abs(x - posicao) < espacamentoMinimo)
                return false;
        }

        return true;
    }

    private void CriarPilar(
        float x,
        float esquerda,
        float direita)
    {
        float proporcao = Mathf.InverseLerp(
            esquerda,
            direita,
            x
        );

        float alturaBase = Mathf.Lerp(
            limiteEsquerdo.position.y,
            limiteDireito.position.y,
            proporcao
        );

        // Procura o chao perto da altura dos limites.
        // Evita iniciar a busca muito acima da arena.
        Vector2 origemRaio = new Vector2(
            x,
            alturaBase + alturaBuscaChao
        );

        RaycastHit2D contato = Physics2D.Raycast(
            origemRaio,
            Vector2.down,
            alturaBuscaChao + profundidadeBuscaChao,
            camadasChao
        );

        float yChao = contato.collider != null
            ? contato.point.y
            : alturaBase;

        Vector3 posicao = new Vector3(
            x,
            yChao,
            transform.position.z
        );

        GameObject objeto = Instantiate(
            prefabPilar,
            posicao,
            Quaternion.identity
        );

        PilarIshkharothF2 pilar =
            objeto.GetComponent<PilarIshkharothF2>();

        if (pilar == null)
        {
            Debug.LogError(
                "Prefab sem PilarIshkharothF2!"
            );

            Destroy(objeto);
            return;
        }

        pilaresAtivos++;

        pilar.Inicializar(
            dano,
            PilarDestruido
        );
    }

    private void PilarDestruido()
    {
        if (!ataqueEmAndamento)
            return;

        pilaresAtivos = Mathf.Max(
            0,
            pilaresAtivos - 1
        );

        VerificarFimAtaque();
    }

    // Animation Event no ultimo frame
    // do ataque 3 do Ishkharoth.
    public void FinalizarAnimacaoPilaresF2()
    {
        animacaoFinalizada = true;

        if (!ataqueEmAndamento)
        {
            EncerrarAnimacaoEIA();
            return;
        }

        VerificarFimAtaque();
    }

    private void VerificarFimAtaque()
    {
        if (!ataqueEmAndamento)
            return;

        if (!criacaoConcluida)
            return;

        if (!animacaoFinalizada)
            return;

        if (pilaresAtivos > 0)
            return;

        ataqueEmAndamento = false;

        EncerrarAnimacaoEIA();
    }

    private void EncerrarAnimacaoEIA()
    {
        if (animator != null)
            animator.SetTrigger("EncerrarAtaque3F2");

        if (ia != null)
            ia.FinalizarAtaqueFase2();

        Debug.Log(
            "Ishkharoth: pilares da fase 2 concluidos."
        );
    }

    private void OnDisable()
    {
        if (rotinaPilares != null)
        {
            StopCoroutine(rotinaPilares);
            rotinaPilares = null;
        }

        ataqueEmAndamento = false;
        criacaoConcluida = false;
        animacaoFinalizada = false;
    }
}

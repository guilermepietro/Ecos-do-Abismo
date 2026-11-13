
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IshkharothAtaque3 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private GameObject prefabEspinho;
    [SerializeField] private Animator animator;

    [Header("Limites da Arena")]
    [SerializeField] private Transform limiteEsquerdo;
    [SerializeField] private Transform limiteDireito;
    [SerializeField] private LayerMask camadasChao;
    [SerializeField] private float margemBordas = 0.5f;

    [Header("Chuva de Espinhos")]
    [SerializeField] private int quantidadeOndas = 3;
    [SerializeField] private int espinhosPorOnda = 5;
    [SerializeField] private float intervaloEspinhos = 0.2f;
    [SerializeField] private float intervaloOndas = 0.7f;

    [Header("Distribuicao Inteligente")]
    [SerializeField] private float larguraMaximaSetor = 2.8f;
    [SerializeField] private int maximoEspinhosPorOnda = 11;

    [Header("Pressao sobre Elian")]
    [SerializeField, Range(1, 4)]
    private int espinhosDirecionadosPorOnda = 3;

    [SerializeField] private float distanciaPressaoLateral = 0.85f;

    [Header("Variacao de Posicoes")]
    [SerializeField] private float variacaoHorizontal = 0.15f;
    [SerializeField] private float espacamentoMinimo = 1f;

    [Header("Queda")]
    [SerializeField] private float alturaNascimento = 6f;
    [SerializeField] private float velocidadeQueda = 12f;

    [Header("Dano")]
    [SerializeField] private int dano = 20;

    private Coroutine rotinaChuva;

    private bool tempestadeAtiva;
    private bool emissaoConcluida;
    private int espinhosAtivos;

    private enum TipoQueda
    {
        Cobertura,
        Direcionada,
        PressaoLateral
    }

    private struct QuedaPlanejada
    {
        public TipoQueda tipo;
        public int setor;

        public QuedaPlanejada(TipoQueda novoTipo, int novoSetor)
        {
            tipo = novoTipo;
            setor = novoSetor;
        }
    }

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Animation Event quando Ishkharoth levanta os bracos.
    public void IniciarChuva()
    {
        if (tempestadeAtiva)
            return;

        if (elian == null ||
            prefabEspinho == null ||
            limiteEsquerdo == null ||
            limiteDireito == null)
        {
            Debug.LogWarning(
                "Ishkharoth: referencias da chuva incompletas."
            );
            return;
        }

        tempestadeAtiva = true;
        emissaoConcluida = false;
        espinhosAtivos = 0;

        if (animator != null)
            animator.SetTrigger("EntrarLoopAtaque3");

        rotinaChuva = StartCoroutine(ExecutarChuva());
    }

    private IEnumerator ExecutarChuva()
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

            emissaoConcluida = true;
            rotinaChuva = null;

            VerificarFimTempestade();
            yield break;
        }

        float larguraArena = direita - esquerda;

        int limite = Mathf.Max(2, maximoEspinhosPorOnda);

        int quantidadeDirecionada = Mathf.Clamp(
            espinhosDirecionadosPorOnda,
            1,
            limite - 1
        );

        // Garante cobertura proporcional a largura da arena.
        int quantidadeSetores = Mathf.Clamp(
            Mathf.CeilToInt(
                larguraArena / Mathf.Max(0.5f, larguraMaximaSetor)
            ),
            1,
            limite - quantidadeDirecionada
        );

        // Espinhos por onda passa a ser a quantidade minima.
        int quantidadeTotal = Mathf.Min(
            limite,
            Mathf.Max(
                espinhosPorOnda,
                quantidadeSetores + quantidadeDirecionada
            )
        );

        for (int onda = 0; onda < quantidadeOndas; onda++)
        {
            // Cada onda recebe uma ordem independente.
            List<QuedaPlanejada> plano = GerarPlanoOnda(
                quantidadeSetores,
                quantidadeTotal,
                quantidadeDirecionada
            );

            List<float> posicoesUsadas = new List<float>();

            foreach (QuedaPlanejada queda in plano)
            {
                if (!tempestadeAtiva)
                    yield break;

                float x;

                switch (queda.tipo)
                {
                    case TipoQueda.Direcionada:

                        // Captura a posicao atual do Elian,
                        // nao a posicao no inicio da onda.
                        x = Mathf.Clamp(
                            elian.position.x,
                            esquerda,
                            direita
                        );
                        break;

                    case TipoQueda.PressaoLateral:

                        // Pressiona uma possivel rota de fuga.
                        float lado = Random.value < 0.5f
                            ? -1f
                            : 1f;

                        x = Mathf.Clamp(
                            elian.position.x +
                            lado * distanciaPressaoLateral,
                            esquerda,
                            direita
                        );
                        break;

                    default:

                        // Distribuicao por setores da arena.
                        x = SortearPosicaoSetor(
                            queda.setor,
                            quantidadeSetores,
                            esquerda,
                            direita,
                            posicoesUsadas
                        );
                        break;
                }

                posicoesUsadas.Add(x);

                CriarEspinho(x);

                yield return new WaitForSeconds(
                    intervaloEspinhos
                );
            }

            if (onda < quantidadeOndas - 1)
            {
                yield return new WaitForSeconds(
                    intervaloOndas
                );
            }
        }

        emissaoConcluida = true;
        rotinaChuva = null;

        VerificarFimTempestade();
    }

    // Cria uma ordem aleatoria, mas garante:
    // - cobertura dos setores;
    // - ataques direcionados em momentos diferentes;
    // - pressao sobre rotas de fuga.
    private List<QuedaPlanejada> GerarPlanoOnda(
        int quantidadeSetores,
        int quantidadeTotal,
        int quantidadeDirecionada)
    {
        List<QuedaPlanejada> cobertura =
            new List<QuedaPlanejada>();

        // Todos os setores recebem pelo menos um espinho.
        for (int i = 0; i < quantidadeSetores; i++)
        {
            cobertura.Add(
                new QuedaPlanejada(TipoQueda.Cobertura, i)
            );
        }

        // Preenche as posicoes extras com cobertura aleatoria.
        while (cobertura.Count <
            quantidadeTotal - quantidadeDirecionada)
        {
            int setorAleatorio = Random.Range(
                0,
                quantidadeSetores
            );

            cobertura.Add(
                new QuedaPlanejada(
                    TipoQueda.Cobertura,
                    setorAleatorio
                )
            );
        }

        // Embaralha a ordem de cobertura.
        Embaralhar(cobertura);

        // Distribui os ataques direcionados ao longo da onda.
        Dictionary<int, TipoQueda> momentosPressao =
            new Dictionary<int, TipoQueda>();

        for (int i = 0; i < quantidadeDirecionada; i++)
        {
            int inicioFaixa = Mathf.FloorToInt(
                (float)i * quantidadeTotal /
                quantidadeDirecionada
            );

            int fimFaixa = Mathf.FloorToInt(
                (float)(i + 1) * quantidadeTotal /
                quantidadeDirecionada
            ) - 1;

            int indice = Random.Range(
                inicioFaixa,
                fimFaixa + 1
            );

            // Alterna mira direta com pressao lateral.
            TipoQueda tipo = (i % 2 == 1)
                ? TipoQueda.PressaoLateral
                : TipoQueda.Direcionada;

            momentosPressao[indice] = tipo;
        }

        List<QuedaPlanejada> plano =
            new List<QuedaPlanejada>();

        int indiceCobertura = 0;

        for (int i = 0; i < quantidadeTotal; i++)
        {
            if (momentosPressao.TryGetValue(
                i, out TipoQueda tipo))
            {
                plano.Add(
                    new QuedaPlanejada(tipo, -1)
                );
            }
            else
            {
                plano.Add(cobertura[indiceCobertura]);
                indiceCobertura++;
            }
        }

        return plano;
    }

    private void Embaralhar(List<QuedaPlanejada> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int indiceAleatorio = Random.Range(0, i + 1);

            QuedaPlanejada temporario = lista[i];

            lista[i] = lista[indiceAleatorio];
            lista[indiceAleatorio] = temporario;
        }
    }

    // Sorteia dentro de um setor, reduzindo grandes
    // espacos vazios e evitando sobreposicoes excessivas.
    private float SortearPosicaoSetor(
        int setor,
        int quantidadeSetores,
        float esquerda,
        float direita,
        List<float> posicoesExistentes)
    {
        float larguraSetor = (direita - esquerda) /
            quantidadeSetores;

        float inicioSetor = esquerda +
            setor * larguraSetor;

        float fimSetor = inicioSetor + larguraSetor;

        float candidato = (inicioSetor + fimSetor) * 0.5f;

        for (int tentativa = 0; tentativa < 15; tentativa++)
        {
            float proporcao = Random.Range(0.2f, 0.8f);

            float x = Mathf.Lerp(
                inicioSetor,
                fimSetor,
                proporcao
            );

            x += Random.Range(
                -variacaoHorizontal,
                variacaoHorizontal
            );

            x = Mathf.Clamp(
                x,
                inicioSetor,
                fimSetor
            );

            candidato = x;

            bool disponivel = true;

            foreach (float existente in posicoesExistentes)
            {
                if (Mathf.Abs(x - existente) <
                    espacamentoMinimo)
                {
                    disponivel = false;
                    break;
                }
            }

            if (disponivel)
                return x;
        }

        // Se nao houver espaco livre suficiente,
        // mantem o espinho dentro do proprio setor.
        return candidato;
    }

    private void CriarEspinho(float x)
    {
        float alturaBase = (
            limiteEsquerdo.position.y +
            limiteDireito.position.y
        ) / 2f;

        Vector2 origemRaio = new Vector2(
            x,
            alturaBase + alturaNascimento + 1f
        );

        RaycastHit2D contato = Physics2D.Raycast(
            origemRaio,
            Vector2.down,
            alturaNascimento + 3f,
            camadasChao
        );

        float yChao = contato.collider != null
            ? contato.point.y
            : alturaBase;

        Vector3 posicaoNascimento = new Vector3(
            x,
            yChao + alturaNascimento,
            0f
        );

        GameObject objeto = Instantiate(
            prefabEspinho,
            posicaoNascimento,
            Quaternion.identity
        );

        EspinhoCeuIshkharoth espinho =
            objeto.GetComponent<EspinhoCeuIshkharoth>();

        if (espinho == null)
        {
            Debug.LogError(
                "Prefab sem EspinhoCeuIshkharoth!"
            );

            Destroy(objeto);
            return;
        }

        espinhosAtivos++;

        espinho.Inicializar(
            dano,
            velocidadeQueda,
            EspinhoDestruido
        );
    }

    private void EspinhoDestruido()
    {
        if (!tempestadeAtiva)
            return;

        espinhosAtivos = Mathf.Max(
            0,
            espinhosAtivos - 1
        );

        VerificarFimTempestade();
    }

    private void VerificarFimTempestade()
    {
        if (!tempestadeAtiva)
            return;

        if (!emissaoConcluida)
            return;

        if (espinhosAtivos > 0)
            return;

        FinalizarConjuracao();
    }

    private void FinalizarConjuracao()
    {
        if (!tempestadeAtiva)
            return;

        tempestadeAtiva = false;
        emissaoConcluida = false;

        if (animator != null)
        {
            animator.SetTrigger("EncerrarAtaque3");
        }

        // A movimentacao continua bloqueada.
        // O Animation Event FinalizarAtaque
        // sera executado apos baixar os bracos.

        Debug.Log(
            "Ishkharoth: tempestade concluida. " +
            "Finalizando animacao."
        );
    }

    private void OnDisable()
    {
        if (rotinaChuva != null)
        {
            StopCoroutine(rotinaChuva);
            rotinaChuva = null;
        }

        tempestadeAtiva = false;
        emissaoConcluida = false;
    }
}

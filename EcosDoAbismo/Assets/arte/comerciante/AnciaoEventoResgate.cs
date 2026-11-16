
using UnityEngine;

public class AnciaoEventoResgate : MonoBehaviour
{
    [Header("Anciao")]
    [SerializeField] private AnciaoInteracao interacaoAnciao;

    [Header("Monstros do Resgate")]
    [SerializeField] private GameObject[] monstros;

    private bool[] monstrosDerrotados;
    private bool resgateConcluido;
    private bool eventoValido;
    private int slotAtual;

    void Awake()
    {
        if (interacaoAnciao != null)
        {
            interacaoAnciao.DefinirConversaLiberada(false);
        }
    }

    void Start()
    {
        slotAtual = MenuSelecaoSave.slotPartidaAtual;

        if (slotAtual < 1 || slotAtual > 3)
        {
            Debug.LogError(
                "Resgate do Anciao: slot invalido."
            );
            return;
        }

        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slotAtual);

        if (dados == null)
        {
            Debug.LogError(
                "Resgate do Anciao: salvamento nao encontrado."
            );
            return;
        }

        if (interacaoAnciao == null)
        {
            Debug.LogError(
                "Resgate do Anciao: configure a interacao."
            );
            return;
        }

        if (dados.resgateAnciaoConcluido)
        {
            resgateConcluido = true;

            DesativarMonstros();

            interacaoAnciao.DefinirConversaLiberada(true);

            Debug.Log(
                "Anciao ja foi resgatado neste slot."
            );

            return;
        }

        if (monstros == null || monstros.Length != 3)
        {
            Debug.LogError(
                "O resgate precisa de exatamente 3 monstros."
            );
            return;
        }

        monstrosDerrotados = new bool[monstros.Length];

        for (int i = 0; i < monstros.Length; i++)
        {
            if (monstros[i] == null)
            {
                Debug.LogError(
                    "Monstro nao configurado: " + i
                );
                return;
            }

            bool possuiVida =
                monstros[i].GetComponentInChildren<SombraVida>(true) != null ||
                monstros[i].GetComponentInChildren<CarcacaVida>(true) != null;

            if (!possuiVida)
            {
                Debug.LogError(
                    "Monstro sem SombraVida ou CarcacaVida: " +
                    monstros[i].name
                );
                return;
            }
        }

        eventoValido = true;

        interacaoAnciao.DefinirConversaLiberada(false);
    }

    void Update()
    {
        if (!eventoValido || resgateConcluido)
            return;

        bool todosDerrotados = true;

        for (int i = 0; i < monstros.Length; i++)
        {
            if (!monstrosDerrotados[i])
            {
                if (monstros[i] == null)
                {
                    monstrosDerrotados[i] = true;
                }
                else
                {
                    SombraVida sombra =
                        monstros[i].GetComponentInChildren<SombraVida>(true);

                    CarcacaVida carcaca =
                        monstros[i].GetComponentInChildren<CarcacaVida>(true);

                    if (sombra != null && sombra.EstaMorto)
                    {
                        monstrosDerrotados[i] = true;
                    }
                    else if (carcaca != null && carcaca.EstaMorto)
                    {
                        monstrosDerrotados[i] = true;
                    }
                }
            }

            if (!monstrosDerrotados[i])
            {
                todosDerrotados = false;
            }
        }

        if (todosDerrotados)
        {
            ConcluirResgate();
        }
    }

    private void ConcluirResgate()
    {
        DadosSalvamento dados =
            GerenciadorSalvamento.Carregar(slotAtual);

        if (dados == null)
            return;

        dados.resgateAnciaoConcluido = true;

        if (ContadorTempoJogo.instancia != null)
        {
            ContadorTempoJogo.instancia
                .AtualizarTempoNosDados(dados);
        }

        if (!GerenciadorSalvamento.Salvar(slotAtual, dados))
        {
            Debug.LogError(
                "Nao foi possivel salvar o resgate."
            );
            return;
        }

        resgateConcluido = true;

        interacaoAnciao.DefinirConversaLiberada(true);

        Debug.Log(
            "Anciao resgatado! Evento salvo permanentemente."
        );
    }

    private void DesativarMonstros()
    {
        if (monstros == null)
            return;

        foreach (GameObject monstro in monstros)
        {
            if (monstro != null)
            {
                monstro.SetActive(false);
            }
        }
    }
}

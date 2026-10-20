
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuOpcoes : MonoBehaviour
{
    [Header("Cursor das Opções")]
    public RectTransform cursorOpcoes;

    [Header("Pontos de Seleção")]
    public RectTransform[] pontosSelecao;

    [Header("Movimento do Cursor")]
    public float tempoDeslizamento = 0.15f;

    [Header("Menu Principal")]
    public MenuManager menuManager;

    [Header("Indicadores de Controle")]
    public GameObject teclaContinuar;
    public GameObject teclaAplicar;

    [Header("Preenchimentos das Barras")]
    public Image barraVolumeGeral;
    public Image barraMusica;
    public Image barraEfeitosSonoros;

    [Header("Ajuste de Volume")]
    [Range(0.01f, 1f)]
    public float passoVolume = 0.05f;

    [Header("Audio Mixer")]
    public AudioMixer mixerEcosDoAbismo;

    private int indiceSelecionado = 0;

    private Vector3 destinoCursor;
    private Vector3 velocidadeCursor;

    private bool editandoOpcao = false;
    private bool volumesInicializados = false;

    private float[] volumesConfirmados = new float[3];
    private float volumeAnterior;

    private readonly string[] parametrosVolume =
    {
        "VolumeGeral",
        "VolumeMusica",
        "VolumeEfeitosSonoros"
    };

    private readonly string[] chavesSalvamento =
    {
        "VolumeGeralSalvo",
        "VolumeMusicaSalvo",
        "VolumeEfeitosSonorosSalvo"
    };

    void Awake()
    {
        volumesConfirmados[0] = barraVolumeGeral != null
            ? barraVolumeGeral.fillAmount : 1f;

        volumesConfirmados[1] = barraMusica != null
            ? barraMusica.fillAmount : 1f;

        volumesConfirmados[2] = barraEfeitosSonoros != null
            ? barraEfeitosSonoros.fillAmount : 1f;
    }

    void OnEnable()
    {
        if (!volumesInicializados)
        {
            CarregarVolumes();
        }

        indiceSelecionado = 0;
        velocidadeCursor = Vector3.zero;
        editandoOpcao = false;

        AtualizarIndicadores();
        AtualizarBarras();
        AtualizarTodosVolumes();

        if (cursorOpcoes != null &&
            pontosSelecao != null &&
            pontosSelecao.Length > 0 &&
            pontosSelecao[0] != null)
        {
            destinoCursor = pontosSelecao[0].position;
            cursorOpcoes.position = destinoCursor;
        }
    }

    void Update()
    {
        if (pontosSelecao == null || pontosSelecao.Length == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (editandoOpcao)
            {
                CancelarEdicao();
            }
            else
            {
                if (menuManager != null)
                    menuManager.VoltarAoMenu();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (editandoOpcao)
                AplicarEdicao();
            else
                IniciarEdicao();

            return;
        }

        if (editandoOpcao)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                AlterarVolume(passoVolume);

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                AlterarVolume(-passoVolume);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                indiceSelecionado++;

                if (indiceSelecionado >= pontosSelecao.Length)
                    indiceSelecionado = 0;

                AtualizarDestino();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                indiceSelecionado--;

                if (indiceSelecionado < 0)
                    indiceSelecionado = pontosSelecao.Length - 1;

                AtualizarDestino();
            }
        }

        if (cursorOpcoes != null)
        {
            cursorOpcoes.position = Vector3.SmoothDamp(
                cursorOpcoes.position,
                destinoCursor,
                ref velocidadeCursor,
                tempoDeslizamento
            );
        }
    }

    public void InicializarVolumes()
    {
        if (!volumesInicializados)
        {
            CarregarVolumes();
        }

        AtualizarBarras();
        AtualizarTodosVolumes();
    }

    void CarregarVolumes()
    {
        for (int i = 0; i < volumesConfirmados.Length; i++)
        {
            volumesConfirmados[i] = Mathf.Clamp01(
                PlayerPrefs.GetFloat(
                    chavesSalvamento[i],
                    volumesConfirmados[i]
                )
            );
        }

        volumesInicializados = true;
    }

    void SalvarVolume(int indice)
    {
        if (indice < 0 || indice >= volumesConfirmados.Length)
            return;

        PlayerPrefs.SetFloat(
            chavesSalvamento[indice],
            volumesConfirmados[indice]
        );

        PlayerPrefs.Save();
    }

    void IniciarEdicao()
    {
        editandoOpcao = true;

        Image barra = ObterBarraSelecionada();

        if (barra != null)
            volumeAnterior = barra.fillAmount;

        AtualizarIndicadores();
    }

    void AlterarVolume(float quantidade)
    {
        Image barra = ObterBarraSelecionada();

        if (barra == null)
            return;

        barra.fillAmount = Mathf.Clamp01(
            barra.fillAmount + quantidade
        );

        AtualizarVolumeMixer(
            indiceSelecionado,
            barra.fillAmount
        );
    }

    void AplicarEdicao()
    {
        Image barra = ObterBarraSelecionada();

        if (barra != null &&
            indiceSelecionado < volumesConfirmados.Length)
        {
            volumesConfirmados[indiceSelecionado] = barra.fillAmount;

            SalvarVolume(indiceSelecionado);
        }

        editandoOpcao = false;
        AtualizarIndicadores();
    }

    void CancelarEdicao()
    {
        Image barra = ObterBarraSelecionada();

        if (barra != null)
        {
            barra.fillAmount = volumeAnterior;

            AtualizarVolumeMixer(
                indiceSelecionado,
                volumeAnterior
            );
        }

        editandoOpcao = false;
        AtualizarIndicadores();
    }

    void AtualizarVolumeMixer(int indice, float volume)
    {
        if (mixerEcosDoAbismo == null)
            return;

        if (indice < 0 || indice >= parametrosVolume.Length)
            return;

        float decibeis;

        if (volume <= 0.0001f)
        {
            decibeis = -80f;
        }
        else
        {
            decibeis = Mathf.Clamp(
                20f * Mathf.Log10(volume),
                -80f,
                0f
            );
        }

        mixerEcosDoAbismo.SetFloat(
            parametrosVolume[indice],
            decibeis
        );
    }

    void AtualizarTodosVolumes()
    {
        for (int i = 0; i < volumesConfirmados.Length; i++)
        {
            AtualizarVolumeMixer(
                i,
                volumesConfirmados[i]
            );
        }
    }

    Image ObterBarraSelecionada()
    {
        switch (indiceSelecionado)
        {
            case 0:
                return barraVolumeGeral;

            case 1:
                return barraMusica;

            case 2:
                return barraEfeitosSonoros;

            default:
                return null;
        }
    }

    void AtualizarBarras()
    {
        if (barraVolumeGeral != null)
            barraVolumeGeral.fillAmount = volumesConfirmados[0];

        if (barraMusica != null)
            barraMusica.fillAmount = volumesConfirmados[1];

        if (barraEfeitosSonoros != null)
            barraEfeitosSonoros.fillAmount = volumesConfirmados[2];
    }

    void AtualizarIndicadores()
    {
        if (teclaContinuar != null)
            teclaContinuar.SetActive(!editandoOpcao);

        if (teclaAplicar != null)
            teclaAplicar.SetActive(editandoOpcao);
    }

    void AtualizarDestino()
    {
        if (pontosSelecao[indiceSelecionado] != null)
            destinoCursor = pontosSelecao[indiceSelecionado].position;
    }
}

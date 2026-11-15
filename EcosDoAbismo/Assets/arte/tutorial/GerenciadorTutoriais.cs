
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GerenciadorTutoriais : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private GameObject tutorialBase;
    [SerializeField] private TMP_Text textoTitulo;
    [SerializeField] private TMP_Text textoDescricao;
    [SerializeField] private Image imagemTutorial1;
    [SerializeField] private Image imagemTutorial2;

    [Header("Controles do Elian")]
    [SerializeField] private MonoBehaviour[] scriptsControleElian;

    private bool[] estadosAnteriores;
    private bool tutorialAberto;
    private float escalaTempoAnterior = 1f;
    private int quadroAbertura;

    private void Awake()
    {
        if (tutorialBase != null)
            tutorialBase.SetActive(false);
    }

    private void Update()
    {
        if (!tutorialAberto)
            return;

        if (Time.frameCount == quadroAbertura)
            return;

        if (EnterPressionado())
            FecharTutorial();
    }

    public bool AbrirTutorial(AreaTutorial area)
    {
        if (tutorialAberto || area == null)
            return false;

        if (tutorialBase == null ||
            textoTitulo == null ||
            textoDescricao == null ||
            imagemTutorial1 == null ||
            imagemTutorial2 == null)
        {
            Debug.LogError("Referencias do tutorial nao configuradas!");
            return false;
        }

        textoTitulo.text = area.Titulo;
        textoDescricao.text = area.Descricao;

        imagemTutorial1.sprite = area.Imagem1;
        imagemTutorial2.sprite = area.Imagem2;

        imagemTutorial1.gameObject.SetActive(area.Imagem1 != null);
        imagemTutorial2.gameObject.SetActive(area.Imagem2 != null);

        escalaTempoAnterior = Time.timeScale;

        tutorialAberto = true;
        quadroAbertura = Time.frameCount;

        BloquearControlesElian();

        tutorialBase.SetActive(true);
        Time.timeScale = 0f;

        return true;
    }

    private void BloquearControlesElian()
    {
        if (scriptsControleElian == null)
            return;

        estadosAnteriores = new bool[scriptsControleElian.Length];

        for (int i = 0; i < scriptsControleElian.Length; i++)
        {
            MonoBehaviour script = scriptsControleElian[i];

            if (script == null || script == this)
                continue;

            estadosAnteriores[i] = script.enabled;
            script.enabled = false;
        }
    }

    private void RestaurarControlesElian()
    {
        if (scriptsControleElian == null ||
            estadosAnteriores == null)
            return;

        for (int i = 0; i < scriptsControleElian.Length; i++)
        {
            MonoBehaviour script = scriptsControleElian[i];

            if (script == null || script == this)
                continue;

            script.enabled = estadosAnteriores[i];
        }

        estadosAnteriores = null;
    }

    private void FecharTutorial()
    {
        tutorialAberto = false;

        tutorialBase.SetActive(false);
        Time.timeScale = escalaTempoAnterior;

        RestaurarControlesElian();
    }

    private bool EnterPressionado()
    {
        bool pressionado = false;

#if ENABLE_INPUT_SYSTEM
        Keyboard teclado = Keyboard.current;

        if (teclado != null)
        {
            pressionado =
                teclado.enterKey.wasPressedThisFrame ||
                teclado.numpadEnterKey.wasPressedThisFrame;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        pressionado |=
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter);
#endif

        return pressionado;
    }

    private void OnDisable()
    {
        if (!tutorialAberto)
            return;

        tutorialAberto = false;

        Time.timeScale = escalaTempoAnterior;
        RestaurarControlesElian();

        if (tutorialBase != null)
            tutorialBase.SetActive(false);
    }
}

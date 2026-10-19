
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TelaInicial : MonoBehaviour
{
    [Header("Cena de Destino")]
    public string nomeCenaCarregamento = "Carregamento";

    [Header("Transição")]
    public TransicaoTela transicaoTela;

    private bool transicaoIniciada = false;

    void Update()
    {
        if (transicaoIniciada)
            return;

        bool tecladoPressionado =
            Keyboard.current != null &&
            Keyboard.current.anyKey.wasPressedThisFrame;

        bool controlePressionado = false;

        if (Gamepad.current != null)
        {
            foreach (var botao in Gamepad.current.allControls)
            {
                if (botao is UnityEngine.InputSystem.Controls.ButtonControl tecla &&
                    tecla.wasPressedThisFrame)
                {
                    controlePressionado = true;
                    break;
                }
            }
        }

        if (tecladoPressionado || controlePressionado)
        {
            transicaoIniciada = true;
            StartCoroutine(IniciarTransicao());
        }
    }

    IEnumerator IniciarTransicao()
    {
        if (transicaoTela != null)
            yield return StartCoroutine(transicaoTela.Escurecer());

        SceneManager.LoadScene(nomeCenaCarregamento);
    }
}

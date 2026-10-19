
using System.Collections;
using UnityEngine;

public class FadeEntradaMenu : MonoBehaviour
{
    [Header("Transição")]
    public TransicaoTela transicaoTela;

    void Start()
    {
        if (transicaoTela != null)
            StartCoroutine(IniciarFade());
    }

    IEnumerator IniciarFade()
    {
        yield return StartCoroutine(transicaoTela.Clarear());
    }
}

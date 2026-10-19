
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorCarregamento : MonoBehaviour
{
    [Header("Cena de Destino")]
    public string nomeCenaDestino = "Menu";

    [Header("Tempo de Carregamento")]
    public float tempoMinimo = 1.5f;

    void Start()
    {
        StartCoroutine(CarregarCena());
    }

    IEnumerator CarregarCena()
    {
        float inicio = Time.realtimeSinceStartup;

        AsyncOperation carregamento =
            SceneManager.LoadSceneAsync(nomeCenaDestino);

        if (carregamento == null)
        {
            Debug.LogError("Não foi possível iniciar o carregamento da cena.");
            yield break;
        }

        carregamento.allowSceneActivation = false;

        while (carregamento.progress < 0.9f)
        {
            yield return null;
        }

        float tempoDecorrido = Time.realtimeSinceStartup - inicio;
        float tempoRestante = tempoMinimo - tempoDecorrido;

        if (tempoRestante > 0f)
        {
            yield return new WaitForSecondsRealtime(tempoRestante);
        }

        carregamento.allowSceneActivation = true;
    }
}

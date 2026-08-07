using UnityEngine;
using System.Collections;

public class HitStop : MonoBehaviour
{
    public static HitStop instancia;

    void Awake()
    {
        instancia = this;
    }

    public void Executar(float tempo)
    {
        StartCoroutine(Pausa(tempo));
    }

    IEnumerator Pausa(float tempo)
    {
        
        
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(tempo);

        
        
        Time.timeScale = 1f;
    }
}
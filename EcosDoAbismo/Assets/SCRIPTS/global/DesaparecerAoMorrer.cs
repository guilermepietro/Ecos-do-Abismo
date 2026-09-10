using UnityEngine;

public class DesaparecerAoMorrer : MonoBehaviour
{
    [Header("Tempo até desaparecer")]
    public float tempoParaDesaparecer = 2f;

    public void IniciarMorte()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        Destroy(gameObject, tempoParaDesaparecer);
    }
}
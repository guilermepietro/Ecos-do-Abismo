using UnityEngine;

public class IgnorarColisaoElian : MonoBehaviour
{
    void Start()
    {
        GameObject elian = GameObject.FindGameObjectWithTag("Player");

        if (elian == null)
        {
            Debug.LogWarning("Elian com Tag Player não encontrado.");
            return;
        }

        Collider2D[] colisoresElian =
            elian.GetComponentsInChildren<Collider2D>();

        Collider2D[] colisoresInimigo =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D colisorElian in colisoresElian)
        {
            if (colisorElian.isTrigger)
                continue;

            foreach (Collider2D colisorInimigo in colisoresInimigo)
            {
                if (colisorInimigo.isTrigger)
                    continue;

                Physics2D.IgnoreCollision(
                    colisorElian,
                    colisorInimigo,
                    true
                );
            }
        }
    }
}
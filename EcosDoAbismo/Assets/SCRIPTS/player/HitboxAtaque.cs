using UnityEngine;
using System.Collections.Generic;

public class HitboxAtaque : MonoBehaviour
{
    public int danoAtual = 20;

    private BoxCollider2D boxCollider;
    private List<GameObject> inimigosAtingidos = new List<GameObject>();

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void LimparInimigosAtingidos()
    {
        inimigosAtingidos.Clear();
    }

    public void DefinirDano(int dano)
    {
        danoAtual = dano;
    }

    public void VerificarAcerto()
    {
        Collider2D[] inimigos = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f
        );

        foreach (Collider2D inimigo in inimigos)
        {
            if (inimigo.CompareTag("Inimigo"))
            {
                if (!inimigosAtingidos.Contains(inimigo.gameObject))
                {
                    inimigosAtingidos.Add(inimigo.gameObject);

                    VidaInimigo vida = inimigo.GetComponent<VidaInimigo>();

                    if (vida != null)
                    {
                        vida.ReceberDano(danoAtual);
                    }
                }
            }
        }
    }
}
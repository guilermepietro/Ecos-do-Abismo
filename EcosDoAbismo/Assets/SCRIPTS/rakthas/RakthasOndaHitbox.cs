
using UnityEngine;
using System.Collections.Generic;

public class RakthasOndaHitbox : MonoBehaviour
{
    public int dano = 20;

    private BoxCollider2D boxCollider;

    private HashSet<GameObject> atingidos = new HashSet<GameObject>();

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        atingidos.Clear();
    }

    public void VerificarDano()
    {
        Collider2D[] alvos = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f
        );

        foreach (Collider2D alvo in alvos)
        {
            CausarDano(alvo);
        }
    }

    public void ReiniciarAcertos()
{
    atingidos.Clear();
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        CausarDano(other);
    }

    private void CausarDano(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (atingidos.Contains(other.gameObject))
            return;

        ElianVida vida = other.GetComponent<ElianVida>();

        if (vida != null)
        {
            atingidos.Add(other.gameObject);
            vida.ReceberDano(dano);
        }
    }
}
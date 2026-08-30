using UnityEngine;

public class EsqueleticoHitboxAtaque : MonoBehaviour
{
    public int dano = 20;
    public bool ElianDentro { get; private set; }

    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void VerificarAcerto()
    {
        Collider2D[] atingidos = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f
        );

        foreach (Collider2D atingido in atingidos)
        {
            ElianVida vida = atingido.GetComponentInParent<ElianVida>();

            if (vida != null && !vida.EstaMorto)
            {
                vida.ReceberDano(dano);
                return;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
{
    ElianVida vida = other.GetComponentInParent<ElianVida>();

    if (vida != null)
    {
        ElianDentro = true;
    }
}

private void OnTriggerExit2D(Collider2D other)
{
    ElianVida vida = other.GetComponentInParent<ElianVida>();

    if (vida != null)
    {
        ElianDentro = false;
    }
}
}
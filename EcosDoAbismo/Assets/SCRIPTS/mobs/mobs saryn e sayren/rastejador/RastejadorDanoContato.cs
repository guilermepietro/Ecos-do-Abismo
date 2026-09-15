using UnityEngine;

public class RastejadorDanoContato : MonoBehaviour
{
    public int dano = 20;
    public float tempoEntreDanos = 1f;

    private float proximoDano = 0f;
    private RastejadorVida vidaRastejador;

    void Start()
    {
        vidaRastejador = GetComponent<RastejadorVida>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (vidaRastejador != null && vidaRastejador.EstaMorto)
            return;

        ElianVida vida = collision.gameObject.GetComponentInParent<ElianVida>();

        if (vida == null || vida.EstaMorto)
            return;

        if (Time.time < proximoDano)
            return;

        vida.ReceberDano(dano);

        proximoDano = Time.time + tempoEntreDanos;
    }
}
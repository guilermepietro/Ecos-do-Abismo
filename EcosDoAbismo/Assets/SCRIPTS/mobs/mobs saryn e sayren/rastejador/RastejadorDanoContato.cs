
using UnityEngine;

public class RastejadorDanoContato : MonoBehaviour
{
    public int dano = 20;
    public float tempoEntreDanos = 1f;

    private float proximoDano = 0f;
    private RastejadorVida vidaRastejador;

    // NOVO: áudio universal do mob
    private AudioMob audioMob;

    void Start()
    {
        vidaRastejador = GetComponent<RastejadorVida>();

        // NOVO
        audioMob = GetComponentInParent<AudioMob>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (vidaRastejador != null &&
            vidaRastejador.EstaMorto)
        {
            return;
        }

        ElianVida vida =
            collision.gameObject
                .GetComponentInParent<ElianVida>();

        if (vida == null || vida.EstaMorto)
            return;

        if (Time.time < proximoDano)
            return;

        vida.ReceberDano(dano);

        // NOVO: som da tentativa de ataque por contato
        if (audioMob != null)
        {
            audioMob.TocarAtaque();
        }

        proximoDano =
            Time.time + tempoEntreDanos;
    }
}


using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PilarFusao : MonoBehaviour
{
    [Header("Dano")]
    [SerializeField] private int dano = 25;

    private BoxCollider2D areaDano;
    private bool danoAtivo;
    private bool atingiuElian;

    private void Awake()
    {
        areaDano = GetComponent<BoxCollider2D>();

        areaDano.isTrigger = true;
        areaDano.enabled = false;
    }

    // Animation Event no inicio do golpe.
    public void AtivarDano()
    {
        danoAtivo = true;
        areaDano.enabled = true;
    }

    // Animation Event quando o golpe termina.
    public void DesativarDano()
    {
        danoAtivo = false;
        areaDano.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        VerificarDano(outro);
    }

    private void OnTriggerStay2D(Collider2D outro)
    {
        VerificarDano(outro);
    }

    private void VerificarDano(Collider2D outro)
    {
        if (!danoAtivo || atingiuElian)
            return;

        ElianVida vidaElian =
            outro.GetComponentInParent<ElianVida>();

        if (vidaElian == null)
            return;

        atingiuElian = true;
        vidaElian.ReceberDano(dano);
    }
}


using UnityEngine;

public class AudioEfeitoFusao : MonoBehaviour
{
    public enum TipoEfeito
    {
        Pilar,
        Pedra
    }

    [Header("Configuracao")]
    [SerializeField] private TipoEfeito tipoEfeito;
    [SerializeField] private LayerMask camadaChao;

    [Header("Pedras")]
    [SerializeField] private bool tocarPedraAoNascer = false;

    private AudioFusao audioFusao;
    private bool somTocado;

    private void Awake()
    {
        audioFusao = FindFirstObjectByType<AudioFusao>();
    }

    private void Start()
    {
        if (audioFusao == null)
            return;

        if (tipoEfeito == TipoEfeito.Pilar)
        {
            audioFusao.SomPilar();
            somTocado = true;
        }
        else if (tipoEfeito == TipoEfeito.Pedra &&
                 tocarPedraAoNascer)
        {
            audioFusao.SomPedra();
            somTocado = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (somTocado ||
            tipoEfeito != TipoEfeito.Pedra ||
            tocarPedraAoNascer ||
            audioFusao == null)
        {
            return;
        }

        bool tocouChao =
            (camadaChao.value &
            (1 << outro.gameObject.layer)) != 0;

        if (!tocouChao)
            return;

        somTocado = true;
        audioFusao.SomPedra();
    }
}

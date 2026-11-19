using UnityEngine;

public class PararAoMorrerElian : MonoBehaviour
{
    [Header("Scripts que controlam o inimigo")]
    [SerializeField] private MonoBehaviour[] scriptsParaDesativar;

    [Header("Física")]
    [SerializeField] private bool pararRigidbody = true;

    private ElianVida elianVida;
    private Rigidbody2D rb;

    private bool jaParou = false;

    void Start()
    {
        elianVida = FindFirstObjectByType<ElianVida>();
        rb = GetComponent<Rigidbody2D>();

        if (elianVida == null)
        {
            Debug.LogWarning(
                name + ": ElianVida não encontrado."
            );
        }
    }

    void Update()
    {
        if (jaParou)
            return;

        if (elianVida == null)
            return;

        if (elianVida.EstaMorto)
        {
            PararInimigo();
        }
    }

    void PararInimigo()
    {
        jaParou = true;

        if (scriptsParaDesativar != null)
        {
            foreach (MonoBehaviour script in scriptsParaDesativar)
            {
                if (script != null && script != this)
                {
                    script.enabled = false;
                }
            }
        }

        if (pararRigidbody && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
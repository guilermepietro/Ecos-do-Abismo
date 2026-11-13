
using System.Collections.Generic;
using UnityEngine;

public class IshkharothAtaque2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoxCollider2D hitboxEspinhos;
    [SerializeField] private SpriteRenderer visual;

    [Header("Dano")]
    [SerializeField] private int dano = 22;

    private bool ondaAtiva;
    private Vector3 escalaOriginalHitbox;

    private readonly HashSet<ElianVida> atingidos =
        new HashSet<ElianVida>();

    private readonly List<Collider2D> sobreposicoes =
        new List<Collider2D>();

    private void Awake()
    {
        if (visual == null)
            visual = GetComponent<SpriteRenderer>();

        if (hitboxEspinhos == null)
        {
            Debug.LogError(
                "Ishkharoth: Hitbox dos espinhos nao configurada!"
            );
            enabled = false;
            return;
        }

        escalaOriginalHitbox = hitboxEspinhos.transform.localScale;

        hitboxEspinhos.isTrigger = true;
        hitboxEspinhos.enabled = false;
    }

    // Animation Event: quando os espinhos comecam a surgir.
    public void IniciarOnda()
    {
        if (hitboxEspinhos == null)
            return;

        atingidos.Clear();

        // Acompanha o flipX do sprite do Ishkharoth.
        // As Keys devem ser feitas na direcao original do sprite.
        float espelhamento =
            visual != null && visual.flipX ? -1f : 1f;

        hitboxEspinhos.transform.localScale = new Vector3(
            escalaOriginalHitbox.x * espelhamento,
            escalaOriginalHitbox.y,
            escalaOriginalHitbox.z
        );

        ondaAtiva = true;
        hitboxEspinhos.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!ondaAtiva || hitboxEspinhos == null)
            return;

        sobreposicoes.Clear();

        Physics2D.OverlapCollider(
            hitboxEspinhos,
            ContactFilter2D.noFilter,
            sobreposicoes
        );

        foreach (Collider2D alvo in sobreposicoes)
        {
            if (alvo == null)
                continue;

            ElianVida vidaElian =
                alvo.GetComponentInParent<ElianVida>();

            if (vidaElian == null)
                continue;

            // Cada Elian so recebe dano uma vez por onda.
            if (atingidos.Add(vidaElian))
            {
                vidaElian.ReceberDano(dano);
            }
        }
    }

    // Animation Event: quando os espinhos deixam de causar dano.
    public void EncerrarOnda()
    {
        ondaAtiva = false;

        if (hitboxEspinhos != null)
            hitboxEspinhos.enabled = false;

        atingidos.Clear();
    }

    private void OnDisable()
    {
        EncerrarOnda();
    }
}

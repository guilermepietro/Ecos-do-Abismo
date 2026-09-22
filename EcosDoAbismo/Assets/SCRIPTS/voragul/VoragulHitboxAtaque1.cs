
using UnityEngine;

public class VoragulHitboxAtaque1 : MonoBehaviour
{
    public int dano = 20;

    [Header("Posição da Hitbox")]
    public float posicaoDireita = 0.5f;
    public float posicaoEsquerda = -0.5f;

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteVoragul;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteVoragul = GetComponentInParent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (spriteVoragul == null)
            return;

        Vector3 posicao = transform.localPosition;

        posicao.x = spriteVoragul.flipX
            ? posicaoEsquerda
            : posicaoDireita;

        transform.localPosition = posicao;
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
            if (atingido.CompareTag("Player"))
            {
                ElianVida vida = atingido.GetComponent<ElianVida>();

                if (vida != null)
                {
                    vida.ReceberDano(dano);
                }
            }
        }
    }
}
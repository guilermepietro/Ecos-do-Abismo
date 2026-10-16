using UnityEngine;

public class VoragulHitboxAtaque2 : MonoBehaviour
{
    [Header("Posição da Hitbox")]
    public float posicaoDireita = 0.5f;
    public float posicaoEsquerda = -0.5f;

    public int dano = 25;

    private SpriteRenderer spriteVoragul;
    private BoxCollider2D boxCollider;

    private bool danoAtivo = false;
    private bool elianJaAtingido = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteVoragul = GetComponentInParent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (spriteVoragul != null)
        {
            Vector3 posicao = transform.localPosition;

            posicao.x = spriteVoragul.flipX
                ? posicaoEsquerda
                : posicaoDireita;

            transform.localPosition = posicao;
        }

        if (danoAtivo && !elianJaAtingido)
        {
            VerificarAcerto();
        }
    }

    public void AtivarDano()
    {
        danoAtivo = true;
        elianJaAtingido = false;
    }

    public void DesativarDano()
    {
        danoAtivo = false;
    }

    private void VerificarAcerto()
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
                    elianJaAtingido = true;
                }

                return;
            }
        }
    }
}
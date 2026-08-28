using UnityEngine;

public class RakthasAtaques : MonoBehaviour
{
    public GameObject hitboxRaio;

    private bool atacando = false;
    private SpriteRenderer spriteRenderer;

    [Header("Posição da Hitbox do Raio")]
    public float posicaoXDireita = 0.352f;
    public float posicaoXEsquerda = -1.054f;

    public bool EstaAtacando => atacando;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        AtualizarPosicaoHitboxRaio();

        // TEMPORÁRIO PARA TESTAR O RAIO
        if (Input.GetKeyDown(KeyCode.M) && !atacando)
        {
            atacando = true;

            Animator animator = GetComponent<Animator>();

            animator.speed = 1f;
            animator.SetBool("estaCorrendo", false);
            animator.SetTrigger("Raio");
        }
    }

    private void AtualizarPosicaoHitboxRaio()
    {
        if (hitboxRaio == null || spriteRenderer == null)
            return;

        Vector3 posicao = hitboxRaio.transform.localPosition;

        if (spriteRenderer.flipX)
            posicao.x = posicaoXEsquerda;
        else
            posicao.x = posicaoXDireita;

        hitboxRaio.transform.localPosition = posicao;
    }

    public void AtivarHitboxRaio()
{
    if (hitboxRaio != null)
    {
        hitboxRaio.SetActive(true);

        RakthasRaioHitbox raio = hitboxRaio.GetComponent<RakthasRaioHitbox>();

        if (raio != null)
            raio.VerificarDano();
    }
}

    public void DesativarHitboxRaio()
    {
        if (hitboxRaio != null)
            hitboxRaio.SetActive(false);
    }

    public void FinalizarRaio()
    {
        atacando = false;
    }
}
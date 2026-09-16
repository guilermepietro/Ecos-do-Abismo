using UnityEngine;

public class CriaturaAtaque : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    public Animator animator;
    public BoxCollider2D hitboxAtaque;
    

    [Header("Ataque")]
    public float tempoEntreAtaques = 1.5f;

    private float proximoAtaque = 0f;
    private bool atacando = false;
    private bool morto = false;
    public int dano = 20;

    

    void Update()
    {
       
        if (morto || atacando)
            return;

        Collider2D[] atingidos = Physics2D.OverlapBoxAll(
            hitboxAtaque.bounds.center,
            hitboxAtaque.bounds.size,
            0f
        );

        foreach (Collider2D alvo in atingidos)
        {
            if (alvo.CompareTag("Player"))
            {
                if (Time.time >= proximoAtaque)
                {
                    Atacar();
                }

                break;
            }
        }
    }

    void Start()
{
    VirarParaElian();
}

    void VirarParaElian()
{
    if (elian == null || morto)
        return;

    if (elian.position.x > transform.position.x)
    {
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
    else
    {
        transform.localScale = new Vector3(
            -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
}

    void Atacar()
    {
        atacando = true;

        animator.SetTrigger("Attack");

        proximoAtaque = Time.time + tempoEntreAtaques;
    }

    public void FinalizarAtaque()
    {
        atacando = false;
    }

    public void DarDano()
{
    Collider2D[] atingidos = Physics2D.OverlapBoxAll(
        hitboxAtaque.bounds.center,
        hitboxAtaque.bounds.size,
        0f
    );

    foreach (Collider2D alvo in atingidos)
    {
        if (alvo.CompareTag("Player"))
        {
            alvo.gameObject.SendMessage(
                "ReceberDano",
                dano,
                SendMessageOptions.DontRequireReceiver
            );

            break;
        }
    }
}

    public void Morrer()
    {
        morto = true;
        atacando = false;

        animator.ResetTrigger("Attack");
    }
}
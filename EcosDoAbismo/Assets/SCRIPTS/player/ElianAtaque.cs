using UnityEngine;

public class ElianAtaque : MonoBehaviour
{
    private Animator animator;

    private bool atacando = false;
    private bool comboAtaque2 = false;
    private bool comboAtaque3 = false;
    private bool ataqueAreaDisponivel = true;
    public float tempoCooldownArea = 5f;

    

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
{
    // ATAQUE EM ÁREA
    if (Input.GetKeyDown(KeyCode.K) && animator.GetBool("estaNoChao") && !atacando && ataqueAreaDisponivel)
{
    animator.SetTrigger("AtaqueArea");
    ataqueAreaDisponivel = false;
}

    // COMBO NORMAL
    if (Input.GetKeyDown(KeyCode.J) && animator.GetBool("estaNoChao"))
    {
        if (!atacando)
        {
            animator.ResetTrigger("Ataque2");
            animator.ResetTrigger("Ataque3");

            animator.SetTrigger("Atacar");
            atacando = true;
        }
        else if (!comboAtaque2)
        {
            comboAtaque2 = true;
            animator.SetTrigger("Ataque2");
        }
        else if (!comboAtaque3)
        {
            comboAtaque3 = true;
            animator.SetTrigger("Ataque3");
        }
    }
}




public void AtivarHitboxArea()
{
    Transform hitbox = transform.Find("HitboxArea");

    if (hitbox != null)
    {
        hitbox.gameObject.SetActive(true);
    }
}

public void DesativarHitboxArea()
{
    Transform hitbox = transform.Find("HitboxArea");

    if (hitbox != null)
    {
        hitbox.gameObject.SetActive(false);
    }
}

public void VerificarDanoArea()
{
    Transform hitbox = transform.Find("HitboxArea");

    if (hitbox != null)
    {
        HitboxArea area = hitbox.GetComponent<HitboxArea>();

        if (area != null)
        {
            area.VerificarAcerto();
        }
    }
}

public void VerificarAcertoArea()
{
    Transform hitbox = transform.Find("HitboxArea");

    if (hitbox != null)
    {
        HitboxArea area = hitbox.GetComponent<HitboxArea>();

        if (area != null)
        {
            area.VerificarAcerto();
        }
    }
}
    public void FinalizarAtaque1()
    {
        if (!comboAtaque2)
        {
            atacando = false;
        }
    }

    public void FinalizarAtaque2()
    {
        if (!comboAtaque3)
        {
            atacando = false;
            comboAtaque2 = false;
        }
    }

    public void FinalizarAtaque3()
    {
        atacando = false;
        comboAtaque2 = false;
        comboAtaque3 = false;

        animator.ResetTrigger("Ataque2");
        animator.ResetTrigger("Ataque3");
    }

    private void ReativarAtaqueArea()
{
    ataqueAreaDisponivel = true;
}

public void IniciarCooldownArea()
{
    Invoke(nameof(ReativarAtaqueArea), tempoCooldownArea);
}
}
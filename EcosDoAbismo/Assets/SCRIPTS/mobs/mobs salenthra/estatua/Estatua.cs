using UnityEngine;

public class Estatua : MonoBehaviour
{
    [Header("Referências")]
    public Transform elian;
    public Animator animator;
    public GameObject HitboxAtaque;
    private bool recebendoDano;

    [Header("Ataque")]
    public float distanciaAtaque = 2f;
    public float tempoEntreAtaques = 1.5f;

    private float tempoUltimoAtaque;
    private bool morto;
    

    void Update()
    {
        if (morto || recebendoDano || elian == null)
    return;

        float distancia = Vector2.Distance(transform.position, elian.position);

        if (distancia <= distanciaAtaque)
        {
            if (Time.time >= tempoUltimoAtaque + tempoEntreAtaques)
            {
                animator.SetTrigger("Atacar");
                tempoUltimoAtaque = Time.time;
            }
        }
    }

    public void AtivarHitbox()
{
    HitboxAtaque.SetActive(true);
}

public void DesativarHitbox()
{
    HitboxAtaque.SetActive(false);
}

public void Morrer()
{
    morto = true;

    if (HitboxAtaque != null)
    {
        HitboxAtaque.SetActive(false);
    }
}

public void IniciarDano()
{
    recebendoDano = true;

    if (HitboxAtaque != null)
    {
        HitboxAtaque.SetActive(false);
    }
}

public void FinalizarDano()
{
    recebendoDano = false;
}
}
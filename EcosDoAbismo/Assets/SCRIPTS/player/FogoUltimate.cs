using UnityEngine;
using System.Collections.Generic;

public class FogoUltimate : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 9f;
    public float tempoDeVida = 4f;

    [Header("Crescimento")]
    public Vector3 escalaInicial = new Vector3(2.821599f, 2.821599f, 1f);
    public Vector3 escalaFinal = new Vector3(7f, 7f, 1f);
    public float tempoCrescimento = 0.7f;

    [Header("Dano")]
    public int dano = 100;
    public float forcaKnockback = 10f;

    private float direcao = 1f;
    private float tempoAtual = 0f;

    private BoxCollider2D boxCollider;

    private HashSet<GameObject> inimigosAtingidos =
        new HashSet<GameObject>();

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            Debug.LogError(
                "FogoUltimate: BoxCollider2D não encontrado!"
            );
        }

        transform.localScale = escalaInicial;

        Destroy(gameObject, tempoDeVida);
    }

    void Update()
    {
        Movimento();
        Crescimento();
        VerificarAcertos();
    }

    void Movimento()
    {
        transform.position +=
            Vector3.right *
            direcao *
            velocidade *
            Time.deltaTime;
    }

    void Crescimento()
    {
        if (tempoAtual >= tempoCrescimento)
        {
            return;
        }

        tempoAtual += Time.deltaTime;

        float progresso =
            Mathf.Clamp01(
                tempoAtual / tempoCrescimento
            );

        transform.localScale =
            Vector3.Lerp(
                escalaInicial,
                escalaFinal,
                progresso
            );
    }

    void VerificarAcertos()
    {
        if (boxCollider == null)
        {
            return;
        }

        Collider2D[] acertos =
            Physics2D.OverlapBoxAll(
                boxCollider.bounds.center,
                boxCollider.bounds.size,
                0f
            );

        foreach (Collider2D acerto in acertos)
        {
            if (acerto.gameObject == gameObject)
            {
                continue;
            }

            GameObject inimigo =
                EncontrarInimigo(acerto.transform);

            if (inimigo == null)
            {
                continue;
            }

            if (inimigosAtingidos.Contains(inimigo))
            {
                continue;
            }

            inimigosAtingidos.Add(inimigo);

            // =================================================
            // DANO
            // =================================================

            inimigo.BroadcastMessage(
                "ReceberDano",
                dano,
                SendMessageOptions.DontRequireReceiver
            );

            Debug.Log(
                "Ultimate atingiu: " +
                inimigo.name +
                " | Dano: " +
                dano
            );

            // =================================================
            // KNOCKBACK
            // =================================================

            KnockbackInimigo knockback =
                inimigo.GetComponent<KnockbackInimigo>();

            if (knockback == null)
            {
                knockback =
                    inimigo.GetComponentInChildren<KnockbackInimigo>();
            }

            if (knockback != null)
            {
                knockback.AplicarEmpurrao(
                    forcaKnockback,
                    direcao
                );
            }
        }
    }

    GameObject EncontrarInimigo(Transform objeto)
    {
        Transform atual = objeto;

        while (atual != null)
        {
            if (atual.CompareTag("Inimigo"))
            {
                return atual.gameObject;
            }

            atual = atual.parent;
        }

        return null;
    }

    public void DefinirDirecao(float novaDirecao)
    {
        direcao = novaDirecao;

        SpriteRenderer spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX =
                direcao < 0f;
        }
    }
}
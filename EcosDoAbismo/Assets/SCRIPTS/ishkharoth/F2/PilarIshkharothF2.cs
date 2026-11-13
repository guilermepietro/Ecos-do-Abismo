
using System;
using System.Collections.Generic;
using UnityEngine;

public class PilarIshkharothF2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoxCollider2D hitboxPilar;

    [Header("Dano")]
    [SerializeField] private int dano = 25;

    [Header("Seguranca")]
    [SerializeField] private float tempoMaximo = 6f;

    private bool inicializado;
    private bool danoAtivo;
    private bool finalizando;

    private Action aoDestruir;

    private readonly HashSet<ElianVida> atingidos =
        new HashSet<ElianVida>();

    private void Awake()
    {
        if (hitboxPilar == null)
        {
            hitboxPilar =
                GetComponentInChildren<BoxCollider2D>();
        }

        if (hitboxPilar != null)
        {
            hitboxPilar.isTrigger = true;
            hitboxPilar.enabled = false;
        }
    }

    public void Inicializar(
        int novoDano,
        Action callbackDestruicao)
    {
        dano = novoDano;
        aoDestruir = callbackDestruicao;

        atingidos.Clear();

        inicializado = true;
        danoAtivo = false;
        finalizando = false;

        Destroy(gameObject, tempoMaximo);
    }

    // Animation Event:
    // quando o pilar comeca a ficar perigoso.
    public void AtivarDanoPilar()
    {
        if (!inicializado || hitboxPilar == null)
            return;

        danoAtivo = true;
        hitboxPilar.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!inicializado ||
            !danoAtivo ||
            finalizando ||
            hitboxPilar == null)
        {
            return;
        }

        // Utiliza o tamanho atual do collider,
        // definido pelas Keys da animacao.
        Bounds area = hitboxPilar.bounds;

        Collider2D[] alvos = Physics2D.OverlapBoxAll(
            area.center,
            area.size,
            0f
        );

        foreach (Collider2D alvo in alvos)
        {
            if (alvo == null)
                continue;

            ElianVida vidaElian =
                alvo.GetComponentInParent<ElianVida>();

            if (vidaElian == null)
                continue;

            // Cada pilar causa dano uma vez por execucao.
            if (atingidos.Add(vidaElian))
            {
                vidaElian.ReceberDano(dano);
            }
        }
    }

    // Animation Event:
    // ultimo frame da animacao do pilar.
    public void FinalizarPilar()
    {
        if (finalizando)
            return;

        finalizando = true;
        danoAtivo = false;

        if (hitboxPilar != null)
            hitboxPilar.enabled = false;

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (inicializado && aoDestruir != null)
        {
            aoDestruir.Invoke();
        }
    }
}

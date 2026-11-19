
using System;
using System.Collections;
using UnityEngine;

public class DesaparecerAoMorrer : MonoBehaviour
{
    [Header("Referências")]
    public Animator animator;

    [Header("Configurações")]
    public float tempoExtra = 0.2f;
    public float tempoSemAnimacao = 1f;

    [Header("Nomes das Animações de Morte")]
    public string[] nomesMorte =
    {
        "Morte",
        "Morrer",
        "Morrendo",
        "Death",
        "Dead",
        "Die"
    };

    private bool morteDetectada = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (morteDetectada || animator == null)
            return;

        if (DetectarMorte())
            IniciarMorte();
    }

    // Pode ser chamado por qualquer script de vida
    public void IniciarMorte()
    {
        if (morteDetectada)
            return;

        morteDetectada = true;

        StartCoroutine(RotinaDesaparecer());
    }

    private bool DetectarMorte()
    {
        if (animator == null)
            return false;

        AnimatorStateInfo estado =
            animator.GetCurrentAnimatorStateInfo(0);

        foreach (string nome in nomesMorte)
        {
            if (string.IsNullOrEmpty(nome))
                continue;

            if (estado.shortNameHash ==
                Animator.StringToHash(nome))
                return true;
        }

        AnimatorClipInfo[] clips =
            animator.GetCurrentAnimatorClipInfo(0);

        foreach (AnimatorClipInfo info in clips)
        {
            if (info.clip == null)
                continue;

            foreach (string nome in nomesMorte)
            {
                if (string.IsNullOrEmpty(nome))
                    continue;

                if (info.clip.name.IndexOf(
                    nome,
                    StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
        }

        return false;
    }

    private IEnumerator RotinaDesaparecer()
    {
        // Remove colisões imediatamente
        Collider2D[] colisoes =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D colisao in colisoes)
            colisao.enabled = false;

        // Impede que o corpo seja empurrado
        Rigidbody2D[] corpos =
            GetComponentsInChildren<Rigidbody2D>();

        foreach (Rigidbody2D corpo in corpos)
            corpo.simulated = false;

        float tempoRestante = tempoSemAnimacao;

        if (animator != null)
        {
            // Aguarda a entrada na animação de morte
            float espera = 0.35f;

            while (!DetectarMorte() && espera > 0f)
            {
                yield return null;
                espera -= Time.deltaTime;
            }

            if (DetectarMorte())
            {
                AnimatorStateInfo estado =
                    animator.GetCurrentAnimatorStateInfo(0);

                float progresso =
                    Mathf.Clamp01(estado.normalizedTime);

                float velocidade =
                    Mathf.Max(
                        0.01f,
                        Mathf.Abs(animator.speed));

                tempoRestante =
                    estado.length * (1f - progresso) /
                    velocidade;

                tempoRestante =
                    Mathf.Clamp(tempoRestante, 0f, 5f);
            }
        }

        yield return new WaitForSeconds(
            tempoRestante + tempoExtra);

        Destroy(gameObject);
    }
}


using UnityEngine;

public class IshkharothAtaque1 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform elian;
    [SerializeField] private GameObject prefabEsfera;
    [SerializeField] private IshkharothVida vida;
    [SerializeField] private IshkharothIA ia;
    [SerializeField] private Collider2D colisorCorpo;

    [Header("Ponto de nascimento")]
    [SerializeField] private Transform pontoNascimentoEsfera;
    [SerializeField] private float alturaAlvo = 0.7f;

    [Header("Tamanho conforme a vida")]
    [SerializeField] private float tamanhoVidaCheia = 1f;
    [SerializeField] private float tamanhoVidaBaixa = 3f;

    [Header("Dano")]
    [SerializeField] private int dano = 18;

    private void Awake()
    {
        if (vida == null)
            vida = GetComponent<IshkharothVida>();

        if (ia == null)
            ia = GetComponent<IshkharothIA>();

        if (colisorCorpo == null)
            colisorCorpo = GetComponent<CapsuleCollider2D>();
    }

    // Animation Event no momento em que a esfera sai da mao.
    public void DispararEsfera()
    {
        if (elian == null ||
            prefabEsfera == null ||
            pontoNascimentoEsfera == null)
            return;

        int direcao = elian.position.x >= transform.position.x
            ? 1
            : -1;

        // Usa a posicao configurada na Scene.
        // Espelha o deslocamento horizontal conforme a direcao.
        Vector3 posicaoLocal = transform.InverseTransformPoint(
            pontoNascimentoEsfera.position
        );

        posicaoLocal.x = Mathf.Abs(posicaoLocal.x) * direcao;

        Vector3 origem = transform.TransformPoint(posicaoLocal);

        Vector3 destino =
            elian.position + Vector3.up * alturaAlvo;

        Vector2 direcaoDisparo =
            (destino - origem).normalized;

        float porcentagemVida =
            vida != null ? vida.PorcentagemVida : 1f;

        float escalaFinal = Mathf.Lerp(
            tamanhoVidaBaixa,
            tamanhoVidaCheia,
            porcentagemVida
        );

        GameObject objetoEsfera = Instantiate(
            prefabEsfera,
            origem,
            Quaternion.identity
        );

        EsferaIshkharoth esfera =
            objetoEsfera.GetComponent<EsferaIshkharoth>();

        if (esfera == null)
        {
            Debug.LogError(
                "Prefab sem o script EsferaIshkharoth!"
            );

            Destroy(objetoEsfera);
            return;
        }

        if (ia != null)
            ia.AguardarAfastamentoEsfera();

        esfera.Inicializar(
            direcaoDisparo,
            dano,
            escalaFinal,
            ia,
            colisorCorpo
        );
    }
}

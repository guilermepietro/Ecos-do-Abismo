
using UnityEngine;

public class IshkharothAtaque1Fase2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject prefabProjetil;
    [SerializeField] private Transform pontoNascimento;
    [SerializeField] private SpriteRenderer visualIshkharoth;

    [Header("Direcao")]
    [SerializeField] private bool spriteOriginalOlhaParaEsquerda = true;

    [Header("Dano")]
    [SerializeField] private int dano = 25;

    private void Awake()
    {
        if (visualIshkharoth == null)
            visualIshkharoth = GetComponent<SpriteRenderer>();
    }

    // Animation Event do Ataque1_F2.
    public void DispararAtaque1F2()
    {
        if (prefabProjetil == null || pontoNascimento == null)
            return;

        int direcao = ObterDirecao();

        Vector3 posicaoLocal =
            transform.InverseTransformPoint(
                pontoNascimento.position
            );

        posicaoLocal.x =
            Mathf.Abs(posicaoLocal.x) * direcao;

        Vector3 origem =
            transform.TransformPoint(posicaoLocal);

        GameObject objeto = Instantiate(
            prefabProjetil,
            origem,
            Quaternion.identity
        );

        ProjetilIshkharothFase2 projetil =
            objeto.GetComponent<ProjetilIshkharothFase2>();

        if (projetil != null)
        {
            projetil.Inicializar(direcao, dano);
        }
        else
        {
            Debug.LogError(
                "Prefab do ataque 1 sem ProjetilIshkharothFase2!"
            );

            Destroy(objeto);
        }
    }

    private int ObterDirecao()
    {
        if (visualIshkharoth == null)
            return -1;

        if (spriteOriginalOlhaParaEsquerda)
            return visualIshkharoth.flipX ? 1 : -1;

        return visualIshkharoth.flipX ? -1 : 1;
    }
}

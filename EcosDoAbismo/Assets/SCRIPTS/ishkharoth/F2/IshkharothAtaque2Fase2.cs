
using UnityEngine;

public class IshkharothAtaque2Fase2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject prefabMeiaLua;
    [SerializeField] private Transform pontoNascimento;
    [SerializeField] private SpriteRenderer visualIshkharoth;

    [Header("Direcao")]
    [SerializeField] private bool spriteOriginalOlhaParaEsquerda = true;

    [Header("Dano")]
    [SerializeField] private int dano = 30;

    private void Awake()
    {
        if (visualIshkharoth == null)
            visualIshkharoth = GetComponent<SpriteRenderer>();
    }

    // Animation Event do Ataque2_F2.
    public void DispararAtaque2F2()
    {
        if (prefabMeiaLua == null || pontoNascimento == null)
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
            prefabMeiaLua,
            origem,
            Quaternion.identity
        );

        ProjetilIshkharothFase2 meiaLua =
            objeto.GetComponent<ProjetilIshkharothFase2>();

        if (meiaLua != null)
        {
            meiaLua.Inicializar(direcao, dano);
        }
        else
        {
            Debug.LogError(
                "Prefab da meia-lua sem ProjetilIshkharothFase2!"
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

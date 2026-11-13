
using UnityEngine;

public class IshkharothVida : MonoBehaviour
{
    [Header("Vida da primeira fase")]
    [SerializeField] private int vidaMaxima = 400;
    [SerializeField] private int vidaAtual;

    public int VidaAtual => vidaAtual;

    public float PorcentagemVida =>
        vidaMaxima > 0 ? (float)vidaAtual / vidaMaxima : 0f;

    private void Awake()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int dano)
    {
        if (dano <= 0 || vidaAtual <= 0)
            return;

        vidaAtual = Mathf.Max(0, vidaAtual - dano);

        Debug.Log("Ishkharoth - Vida: " + vidaAtual);
    }

    [ContextMenu("TESTE - Perder 100 de vida")]
    private void TestarDano()
    {
        if (Application.isPlaying)
            ReceberDano(100);
    }
}

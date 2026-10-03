using UnityEngine;
using System.Collections;

public class ChuvaEspadas : MonoBehaviour
{
    [Header("Referências")]
    public GameObject espadaPrefab;
    public SalenthraMovimento salenthra;
    public SalenthraVida vidaSalenthra;

    [Header("Área")]
    public float larguraArea = 12f;

    [Header("Chuva")]
    public int quantidadeEspadas = 10;
    public float intervaloEntreEspadas = 0.25f;

    private int espadasAtivas = 0;
    private bool terminouDeGerar = false;

    public void IniciarChuva()
    {
        espadasAtivas = 0;
        terminouDeGerar = false;

        StartCoroutine(GerarChuva());
    }

    private IEnumerator GerarChuva()
    {
        for (int i = 0; i < quantidadeEspadas; i++)
        {
            float xAleatorio = Random.Range(
                transform.position.x - larguraArea / 2f,
                transform.position.x + larguraArea / 2f
            );

            Vector3 posicaoSpawn = new Vector3(
                xAleatorio,
                transform.position.y,
                0f
            );

            GameObject novaEspada = Instantiate(
                espadaPrefab,
                posicaoSpawn,
                Quaternion.identity
            );

            EspadaChuva espada = novaEspada.GetComponent<EspadaChuva>();

            if (espada != null)
            {
                espada.chuvaEspadas = this;
                espada.vidaSalenthra = vidaSalenthra;

                espadasAtivas++;
            }

            yield return new WaitForSeconds(intervaloEntreEspadas);
        }

        terminouDeGerar = true;

        VerificarFimChuva();
    }

    public void EspadaDesapareceu()
    {
        espadasAtivas--;

        if (espadasAtivas < 0)
            espadasAtivas = 0;

        VerificarFimChuva();
    }

    private void VerificarFimChuva()
    {
        if (!terminouDeGerar)
            return;

        if (espadasAtivas > 0)
            return;

        if (salenthra != null)
        {
            salenthra.FinalizarAtaqueChuva();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(larguraArea, 0.5f, 0f)
        );
    }
}
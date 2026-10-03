using UnityEngine;

public class SalenthraDanoContato : MonoBehaviour
{
    [Header("Dano")]
    public int dano = 20;

    private SalenthraVida vidaSalenthra;
    private SalenthraMovimento movimento;

    private void Awake()
    {
        vidaSalenthra = GetComponent<SalenthraVida>();
        movimento = GetComponent<SalenthraMovimento>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (vidaSalenthra != null && vidaSalenthra.EstaMorta)
            return;

        ElianVida vidaElian = collision.gameObject.GetComponent<ElianVida>();

        if (vidaElian != null)
        {
           int danoFinal = dano;

if (vidaSalenthra != null)
{
    danoFinal = vidaSalenthra.CalcularDano(dano);
}

vidaElian.ReceberDano(danoFinal);

            if (movimento != null)
            {
                movimento.PararDashAoAcertar();
            }
        }
    }
}
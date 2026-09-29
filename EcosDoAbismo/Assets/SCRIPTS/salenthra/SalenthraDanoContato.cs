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
            vidaElian.ReceberDano(dano);

            if (movimento != null)
            {
                movimento.PararDashAoAcertar();
            }
        }
    }
}
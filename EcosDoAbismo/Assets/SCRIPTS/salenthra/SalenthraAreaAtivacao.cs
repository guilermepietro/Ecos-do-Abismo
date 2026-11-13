
using UnityEngine;

public class SalenthraAreaAtivacao : MonoBehaviour
{
    public SalenthraMovimento movimento;
    public BarraVidaSalenthra barraVidaSalenthra;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (movimento == null || movimento.elian == null)
            return;

        if (other.transform == movimento.elian ||
            other.transform.IsChildOf(movimento.elian))
        {
            movimento.Ativar();

            if (barraVidaSalenthra != null)
                barraVidaSalenthra.Exibir();
        }
    }
}

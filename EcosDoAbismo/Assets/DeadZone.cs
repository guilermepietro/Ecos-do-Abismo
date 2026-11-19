
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [Header("Configurações")]
    public int danoMortal = 1000000;

    [Header("Verificação")]
    public float intervaloTentativas = 0.2f;

    private Dictionary<EntityId, float> proximasTentativas =
        new Dictionary<EntityId, float>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        TentarMatar(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TentarMatar(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EntityId id = ObterID(other);
        proximasTentativas.Remove(id);
    }

    private EntityId ObterID(Collider2D other)
    {
        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.GetEntityId();

        return other.GetEntityId();
    }

    private void TentarMatar(Collider2D other)
    {
        EntityId id = ObterID(other);

        if (proximasTentativas.TryGetValue(
            id, out float proximaTentativa))
        {
            if (Time.time < proximaTentativa)
                return;
        }

        proximasTentativas[id] =
            Time.time + Mathf.Max(0.05f, intervaloTentativas);

        other.gameObject.SendMessageUpwards(
            "ReceberDano",
            danoMortal,
            SendMessageOptions.DontRequireReceiver
        );
    }
}

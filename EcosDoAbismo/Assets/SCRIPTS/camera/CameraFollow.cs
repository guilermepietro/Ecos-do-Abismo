using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform alvo;

    public float velocidade = 5f;

    void LateUpdate()
    {
        if (alvo == null)
            return;

        Vector3 novaPosicao = new Vector3(
            alvo.position.x,
            alvo.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            novaPosicao,
            velocidade * Time.deltaTime
        );
    }
}
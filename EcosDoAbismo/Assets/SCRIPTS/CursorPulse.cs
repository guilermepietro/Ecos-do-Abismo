using UnityEngine;
using UnityEngine.UI;

public class CursorPulse : MonoBehaviour
{
    [Header("Pulsação")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.05f;

    [Header("Rotação")]
    public float rotationSpeed = 1.5f;
    public float rotationAmount = 2f;

    [Header("Brilho")]
    public float alphaSpeed = 2f;
    public float minAlpha = 0.5f;
    public float maxAlpha = 1f;

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Image image;

    void Start()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;

        image = GetComponent<Image>();
    }

    void Update()
    {
        // Pulsação
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scale;

        // Rotação
        float angle = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
        transform.localRotation =
            originalRotation * Quaternion.Euler(0, 0, angle);

        // Transparência
        if (image != null)
        {
            Color color = image.color;

            float alpha = Mathf.Lerp(
                minAlpha,
                maxAlpha,
                (Mathf.Sin(Time.time * alphaSpeed) + 1f) / 2f
            );

            color.a = alpha;
            image.color = color;
        }
    }
}
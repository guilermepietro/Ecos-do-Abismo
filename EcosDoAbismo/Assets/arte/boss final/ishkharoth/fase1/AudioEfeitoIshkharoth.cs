
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEfeitoIshkharoth : MonoBehaviour
{
    [Header("Efeito Sonoro")]
    [SerializeField] private AudioClip somAoNascer;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.7f;

    private AudioSource fonteAudio;

    private void Awake()
    {
        fonteAudio = GetComponent<AudioSource>();

        fonteAudio.playOnAwake = false;
        fonteAudio.loop = false;
        fonteAudio.spatialBlend = 0f;
    }

    private void Start()
    {
        ReproduzirSom();
    }

    public void ReproduzirSom()
    {
        if (fonteAudio == null ||
            somAoNascer == null)
        {
            return;
        }

        fonteAudio.PlayOneShot(
            somAoNascer,
            volume
        );
    }
}

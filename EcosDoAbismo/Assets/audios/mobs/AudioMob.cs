
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioMob : MonoBehaviour
{
    [Header("Efeitos Sonoros")]
    public AudioClip somAtaque;
    public AudioClip somDano;
    public AudioClip somMorte;

    private AudioSource audioMob;

    void Awake()
    {
        audioMob = GetComponent<AudioSource>();

        audioMob.playOnAwake = false;
        audioMob.loop = false;
    }

    public void TocarAtaque()
    {
        ReproduzirSom(somAtaque);
    }

    public void TocarDano()
    {
        ReproduzirSom(somDano);
    }

    public void TocarMorte()
    {
        ReproduzirSom(somMorte);
    }

    private void ReproduzirSom(AudioClip som)
    {
        if (audioMob != null && som != null)
        {
            audioMob.PlayOneShot(som);
        }
    }
}

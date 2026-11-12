
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSalenthra : MonoBehaviour
{
    [Header("Fase 1")]
    public AudioClip somDashFase1;
    public AudioClip somPuxao;
    public AudioClip somChuvaEspadas;

    [Header("Espadas Individuais da Chuva")]
    public AudioClip somQuedaEspada;

    [Range(0f, 1f)]
    public float volumeQuedaEspada = 0.35f;

    [Header("Fase 2")]
    public AudioClip somEspadaFase2;
    public AudioClip somDashFase2;
    public AudioClip somProjetilLua;
    public AudioClip somRaioCelestial;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    // =========================
    // FASE 1
    // =========================

    public void TocarDashFase1()
    {
        ReproduzirSom(somDashFase1);
    }

    public void TocarPuxao()
    {
        ReproduzirSom(somPuxao);
    }

    public void TocarChuvaEspadas()
    {
        ReproduzirSom(somChuvaEspadas);
    }

    // NOVO: uma vez por espada criada
    public void TocarQuedaEspada()
    {
        if (audioSource != null &&
            somQuedaEspada != null)
        {
            audioSource.PlayOneShot(
                somQuedaEspada,
                volumeQuedaEspada
            );
        }
    }

    // =========================
    // FASE 2
    // =========================

    public void TocarEspadaFase2()
    {
        ReproduzirSom(somEspadaFase2);
    }

    public void TocarDashFase2()
    {
        ReproduzirSom(somDashFase2);
    }

    public void TocarProjetilLua()
    {
        ReproduzirSom(somProjetilLua);
    }

    public void TocarRaioCelestial()
    {
        ReproduzirSom(somRaioCelestial);
    }

    // =========================
    // REPRODUÇÃO
    // =========================

    private void ReproduzirSom(AudioClip som)
    {
        if (audioSource != null && som != null)
        {
            audioSource.PlayOneShot(som);
        }
    }
}


using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonsInterface : MonoBehaviour
{
    public static SonsInterface Instancia { get; private set; }

    [Header("Efeitos Sonoros da Interface")]
    public AudioClip somNavegacao;
    public AudioClip somConfirmacao;
    public AudioClip somVoltar;
    public AudioClip somAjusteVolume;
    public AudioClip somExcluirSalvamento;
    public AudioClip somIniciarJogo;

    private AudioSource audioInterface;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        audioInterface = GetComponent<AudioSource>();
        audioInterface.playOnAwake = false;
        audioInterface.loop = false;
        audioInterface.spatialBlend = 0f;
    }

    public void TocarNavegacao()
    {
        Reproduzir(somNavegacao);
    }

    public void TocarConfirmacao()
    {
        Reproduzir(somConfirmacao);
    }

    public void TocarVoltar()
    {
        Reproduzir(somVoltar);
    }

    public void TocarAjusteVolume()
    {
        Reproduzir(somAjusteVolume);
    }

    public void TocarExcluirSalvamento()
    {
        Reproduzir(somExcluirSalvamento);
    }

    public void TocarIniciarJogo()
    {
        Reproduzir(somIniciarJogo);
    }

    private void Reproduzir(AudioClip som)
    {
        if (audioInterface != null && som != null)
        {
            audioInterface.PlayOneShot(som);
        }
    }

    void OnDestroy()
    {
        if (Instancia == this)
        {
            Instancia = null;
        }
    }
}

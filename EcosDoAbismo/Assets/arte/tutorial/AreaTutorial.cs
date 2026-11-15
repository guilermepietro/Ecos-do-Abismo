
using UnityEngine;

public class AreaTutorial : MonoBehaviour
{
    [Header("Gerenciador")]
    [SerializeField] private GerenciadorTutoriais gerenciador;

    [Header("Conteudo do Tutorial")]
    [SerializeField] private string titulo;

    [SerializeField, TextArea(3, 8)]
    private string descricao;

    [SerializeField] private Sprite imagem1;
    [SerializeField] private Sprite imagem2;

    private bool jaExibido;

    public string Titulo => titulo;
    public string Descricao => descricao;
    public Sprite Imagem1 => imagem1;
    public Sprite Imagem2 => imagem2;

    private void OnTriggerEnter2D(Collider2D outro)
    {
        TentarAtivarTutorial(outro);
    }

    private void OnTriggerStay2D(Collider2D outro)
    {
        TentarAtivarTutorial(outro);
    }

    private void TentarAtivarTutorial(Collider2D outro)
    {
        if (jaExibido || gerenciador == null)
            return;

        bool ehElian = outro.gameObject.name == "elian";

        if (!ehElian && outro.attachedRigidbody != null)
        {
            ehElian =
                outro.attachedRigidbody.gameObject.name == "elian";
        }

        if (!ehElian)
            return;

        if (gerenciador.AbrirTutorial(this))
            jaExibido = true;
    }
}


using System;
using UnityEngine;

public enum ComandoElian
{
    Ataque,
    AtaqueArea,
    Ultimate,
    Defender,
    Curar,
    Interagir,
    Pular,
    Dash,
    AndarEsquerda,
    AndarDireita
}

public static class ControlesElian
{
    private const string Prefixo =
        "EcosDoAbismo_Tecla_";

    public static KeyCode Obter(ComandoElian comando)
    {
        string chave = Prefixo + comando;

        KeyCode padrao = ObterPadrao(comando);

        int valor = PlayerPrefs.GetInt(
            chave,
            (int)padrao
        );

        if (!Enum.IsDefined(typeof(KeyCode), valor))
            return padrao;

        return (KeyCode)valor;
    }

    public static bool Pressionou(ComandoElian comando)
    {
        return Input.GetKeyDown(Obter(comando));
    }

    public static bool Segurando(ComandoElian comando)
    {
        return Input.GetKey(Obter(comando));
    }

    public static float EixoHorizontal
    {
        get
        {
            bool esquerda = Segurando(
                ComandoElian.AndarEsquerda
            );

            bool direita = Segurando(
                ComandoElian.AndarDireita
            );

            return (direita ? 1f : 0f) -
                   (esquerda ? 1f : 0f);
        }
    }

    public static bool TentarDefinir(
        ComandoElian comando,
        KeyCode novaTecla)
    {
        if (!TeclaValida(novaTecla))
            return false;

        foreach (
            ComandoElian outro in
            Enum.GetValues(typeof(ComandoElian))
        )
        {
            if (
                outro != comando &&
                Obter(outro) == novaTecla
            )
            {
                return false;
            }
        }

        PlayerPrefs.SetInt(
            Prefixo + comando,
            (int)novaTecla
        );

        PlayerPrefs.Save();

        return true;
    }

    // Altera as duas teclas de movimento juntas.
    public static bool TentarDefinirAndar(
        KeyCode esquerda,
        KeyCode direita)
    {
        if (
            !TeclaValida(esquerda) ||
            !TeclaValida(direita) ||
            esquerda == direita
        )
        {
            return false;
        }

        foreach (
            ComandoElian outro in
            Enum.GetValues(typeof(ComandoElian))
        )
        {
            if (
                outro == ComandoElian.AndarEsquerda ||
                outro == ComandoElian.AndarDireita
            )
            {
                continue;
            }

            if (
                Obter(outro) == esquerda ||
                Obter(outro) == direita
            )
            {
                return false;
            }
        }

        PlayerPrefs.SetInt(
            Prefixo + ComandoElian.AndarEsquerda,
            (int)esquerda
        );

        PlayerPrefs.SetInt(
            Prefixo + ComandoElian.AndarDireita,
            (int)direita
        );

        PlayerPrefs.Save();

        return true;
    }

    private static bool TeclaValida(KeyCode tecla)
    {
        return tecla != KeyCode.None &&
               tecla != KeyCode.Escape &&
               tecla != KeyCode.Return &&
               tecla != KeyCode.KeypadEnter &&
               tecla != KeyCode.UpArrow &&
               tecla != KeyCode.DownArrow &&
               tecla != KeyCode.LeftArrow &&
               tecla != KeyCode.RightArrow;
    }

    public static KeyCode ObterPadrao(
        ComandoElian comando)
    {
        switch (comando)
        {
            case ComandoElian.Ataque:
                return KeyCode.J;

            case ComandoElian.AtaqueArea:
                return KeyCode.K;

            case ComandoElian.Ultimate:
                return KeyCode.R;

            case ComandoElian.Defender:
                return KeyCode.Q;

            case ComandoElian.Curar:
                return KeyCode.L;

            case ComandoElian.Interagir:
                return KeyCode.E;

            case ComandoElian.Pular:
                return KeyCode.Space;

            case ComandoElian.Dash:
                return KeyCode.LeftControl;

            case ComandoElian.AndarEsquerda:
                return KeyCode.A;

            case ComandoElian.AndarDireita:
                return KeyCode.D;

            default:
                return KeyCode.None;
        }
    }
}

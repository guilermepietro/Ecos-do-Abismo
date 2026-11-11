
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class CadastroTilesPassosEditor
{
    [MenuItem("Tools/Ecos do Abismo/Cadastrar Tiles de Vegetacao")]
    public static void CadastrarTiles()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning(
                "Pare o Play Mode antes de cadastrar os Tiles."
            );
            return;
        }

        GameObject prefabPalette =
            Selection.activeObject as GameObject;

        if (prefabPalette == null ||
            !PrefabUtility.IsPartOfPrefabAsset(prefabPalette))
        {
            Debug.LogWarning(
                "Selecione o prefab da Tile Palette na aba Project."
            );
            return;
        }

        ElianMovimento elian =
            Object.FindFirstObjectByType<ElianMovimento>();

        if (elian == null)
        {
            Debug.LogWarning(
                "ElianMovimento não encontrado. Abra a cena Gameplay."
            );
            return;
        }

        string caminhoPrefab =
            AssetDatabase.GetAssetPath(prefabPalette);

        GameObject raizTemporaria = null;

        HashSet<TileBase> tilesEncontrados =
            new HashSet<TileBase>();

        try
        {
            raizTemporaria =
                PrefabUtility.LoadPrefabContents(caminhoPrefab);

            Tilemap[] tilemaps =
                raizTemporaria.GetComponentsInChildren<Tilemap>(true);

            foreach (Tilemap tilemap in tilemaps)
            {
                int quantidade = tilemap.GetUsedTilesCount();

                if (quantidade <= 0)
                    continue;

                TileBase[] tiles =
                    new TileBase[quantidade];

                int encontrados =
                    tilemap.GetUsedTilesNonAlloc(tiles);

                for (int i = 0; i < encontrados; i++)
                {
                    if (tiles[i] != null)
                        tilesEncontrados.Add(tiles[i]);
                }
            }
        }
        finally
        {
            if (raizTemporaria != null)
            {
                PrefabUtility.UnloadPrefabContents(
                    raizTemporaria
                );
            }
        }

        if (tilesEncontrados.Count == 0)
        {
            Debug.LogWarning(
                "Nenhum Tile encontrado no prefab selecionado."
            );
            return;
        }

        // Preserva os Tiles que já estavam cadastrados.
        if (elian.tilesVegetacao != null)
        {
            foreach (TileBase tile in elian.tilesVegetacao)
            {
                if (tile != null)
                    tilesEncontrados.Add(tile);
            }
        }

        Undo.RecordObject(
            elian,
            "Cadastrar Tiles de Vegetacao"
        );

        elian.tilesVegetacao =
            new List<TileBase>(tilesEncontrados).ToArray();

        EditorUtility.SetDirty(elian);

        PrefabUtility.RecordPrefabInstancePropertyModifications(
            elian
        );

        EditorSceneManager.MarkSceneDirty(
            elian.gameObject.scene
        );

        Debug.Log(
            "Ecos do Abismo: " +
            elian.tilesVegetacao.Length +
            " Tiles cadastrados para passos de vegetação."
        );
    }
}

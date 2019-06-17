/* 
 * Oscar Forra Carbonell
*/
using UnityEditor;
using System.Collections;
using UnityEngine;

public class SpriteProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        string l_lowerCaseAssetPath = assetPath.ToLower();
        bool l_isInSpritesDirectory = l_lowerCaseAssetPath.IndexOf("/art/sprites/") != -1;

        if (l_isInSpritesDirectory)
        {
            TextureImporter l_texutreImporter = (TextureImporter)assetImporter;
            l_texutreImporter.textureType = TextureImporterType.Sprite;
        }
    }
}

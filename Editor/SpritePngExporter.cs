using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpritePngExporter : EditorWindow
{
    private Texture2D texture2D;
    private string outputFolderPath = "Assets/06.Downloads/00.Using/Sprites/Thumbnail";

    [MenuItem("Tools/Sprite Exporter")]
    public static void ShowWindow()
    {
        SpritePngExporter window = GetWindow<SpritePngExporter>("Sprite Exporter");
        // 중앙에 배치
        Vector2 windowSize = new Vector2(600, 200);
        Rect mainWindowRect = EditorGUIUtility.GetMainWindowPosition();
        window.position = new Rect(
            mainWindowRect.x + (mainWindowRect.width - windowSize.x) / 2,
            mainWindowRect.y + (mainWindowRect.height - windowSize.y) / 2,
            windowSize.x, windowSize.y
        );
    }

    void OnGUI()
    {
        GUILayout.Label("Sprite Exporter", EditorStyles.boldLabel);

        texture2D = (Texture2D)EditorGUILayout.ObjectField("Texture2D", texture2D, typeof(Texture2D), false);
        outputFolderPath = EditorGUILayout.TextField("Output Folder Path", outputFolderPath);

        if (GUILayout.Button("Export Sprites"))
        {
            ExportSpritesToPNG();
        }
    }

    void ExportSpritesToPNG()
    {
        if (texture2D == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a Texture2D.", "OK");
            return;
        }

        Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(texture2D)).OfType<Sprite>().ToArray();

        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        foreach (Sprite sprite in sprites)
        {
            Texture2D spriteTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] pixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
            spriteTexture.SetPixels(pixels);
            spriteTexture.Apply();

            byte[] pngData = spriteTexture.EncodeToPNG();
            if (pngData != null)
            {
                string filePath = Path.Combine(outputFolderPath, sprite.name + ".png");
                File.WriteAllBytes(filePath, pngData);
                //Debug.Log("Saved: " + filePath);
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "Sprites exported successfully.", "OK");
    }
}

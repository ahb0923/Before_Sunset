using UnityEngine;
using UnityEditor;
using System.IO;

public class CursorResizerWindow : EditorWindow
{
    private int targetWidth = 4;
    private int targetHeight = 4;

    [MenuItem("Tools/Cursor Resizer")]
    public static void ShowWindow()
    {
        GetWindow<CursorResizerWindow>("Cursor Resizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("커서 리사이즈 설정", EditorStyles.boldLabel);

        targetWidth = EditorGUILayout.IntField("너비 (px)", targetWidth);
        targetHeight = EditorGUILayout.IntField("높이 (px)", targetHeight);

        if (GUILayout.Button("선택된 Sprite 리사이즈"))
        {
            ResizeSelectedSprite(targetWidth, targetHeight);
        }
    }

    private void ResizeSelectedSprite(int width, int height)
    {
        if (!(Selection.activeObject is Sprite sprite))
        {
            return;
        }

        Rect rect = sprite.rect;
        Texture2D source = sprite.texture;

        if (!source.isReadable)
        {
            return;
        }

        Texture2D resized = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int srcX = Mathf.FloorToInt(rect.x + (x + 0.5f) * rect.width / width);
                int srcY = Mathf.FloorToInt(rect.y + (y + 0.5f) * rect.height / height);

                Color color = source.GetPixel(srcX, srcY);
                resized.SetPixel(x, y, color);
            }
        }

        resized.Apply();

        string path = Application.dataPath + $"/Cursor_{width}x{height}.png";
        File.WriteAllBytes(path, resized.EncodeToPNG());
        AssetDatabase.Refresh();

        Debug.Log($"{width}x{height} 커서 PNG 저장 완료: {path}");
    }
}
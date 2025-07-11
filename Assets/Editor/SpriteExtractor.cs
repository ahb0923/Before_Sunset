using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteExtractor
{
    [MenuItem("Tools/Extract Selected Sprite To PNG")]
    public static void ExtractSprite()
    {
        if (Selection.activeObject is Sprite sprite)
        {
            // 텍스처 생성
            Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var source = sprite.texture;
            Color[] pixels = source.GetPixels(
                (int)sprite.rect.x,
                (int)sprite.rect.y,
                (int)sprite.rect.width,
                (int)sprite.rect.height);
            tex.SetPixels(pixels);
            tex.Apply();

            // PNG로 저장
            byte[] png = tex.EncodeToPNG();
            string path = Application.dataPath + "/ExtractedCursor.png";
            File.WriteAllBytes(path, png);

            AssetDatabase.Refresh();
            Debug.Log("Saved to: " + path);
        }
        else
        {
            Debug.LogWarning("선택된 항목이 Sprite가 아닙니다!");
        }
    }
}
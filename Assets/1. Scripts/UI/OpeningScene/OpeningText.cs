using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class OpeningText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public RectTransform rect;

    private float originSize = 1f;
    
    public IEnumerator C_DOTextMesh(float duration)
    {
        textMesh.maxVisibleCharacters = 0;

        Tween tween = DOTween.To(x => textMesh.maxVisibleCharacters = (int)x,
            0f, textMesh.text.Length, duration).SetEase(Ease.Linear);
        yield return tween.WaitForCompletion();
        
        textMesh.maxVisibleCharacters = int.MaxValue;
    }
    
    public void MoveText(float moveAmount, float duration)
    {
        Vector3 ps = rect.anchoredPosition;
        ps.y += moveAmount;
        
        Color startColor = textMesh.color;
        Color endColor = new Color(startColor.r * 0.6f, startColor.g * 0.6f, startColor.b * 0.6f, 1f);

        originSize *= 0.8f;
        
        Sequence seq = DOTween.Sequence();
        seq.Join(rect.DOAnchorPos(ps, duration).SetEase(Ease.OutCubic));
        seq.Join(textMesh.DOColor(endColor, duration).SetEase(Ease.OutCubic));
        seq.Join(rect.DOScale(originSize, duration).SetEase(Ease.OutCubic));

        seq.Play();
    }
}

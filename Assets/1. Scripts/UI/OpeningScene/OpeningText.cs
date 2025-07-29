using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class OpeningText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public RectTransform rect;

    private float _originScale = 1f;

    private void Reset()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
    }

    public IEnumerator C_DOTextMesh(float duration)
    {
        textMesh.maxVisibleCharacters = 0;

        Tween tween = DOTween.To(x => textMesh.maxVisibleCharacters = (int)x,
            0f, textMesh.text.Length, duration).SetEase(Ease.Linear).SetAutoKill(false);

        yield return new WaitForSeconds(0.7f);
        bool isSkipped = false;
        while (tween.IsPlaying() && !isSkipped)
        {
            if (Input.GetMouseButtonDown(0) ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return))
            {
                tween.Complete();
                isSkipped = true;
            }
            yield return null;
        }

        if (!isSkipped)
        {
            yield return tween.WaitForCompletion();
        }
        
        textMesh.maxVisibleCharacters = int.MaxValue;
        tween.Kill();
    }
    
    public void MoveText(float moveAmount, float duration)
    {
        Vector3 ps = rect.anchoredPosition;
        ps.y += moveAmount;
        
        Color startColor = textMesh.color;
        Color endColor = new Color(startColor.r * 0.6f, startColor.g * 0.6f, startColor.b * 0.6f, 1f);

        _originScale *= 0.8f;
        
        Sequence seq = DOTween.Sequence();
        seq.Join(rect.DOAnchorPos(ps, duration).SetEase(Ease.OutCubic));
        seq.Join(textMesh.DOColor(endColor, duration).SetEase(Ease.OutCubic));
        seq.Join(rect.DOScale(_originScale, duration).SetEase(Ease.OutCubic));

        seq.Play();
    }

    public void ResetOpeningText()
    {
        textMesh.text = string.Empty;
        textMesh.color = Color.white;
        rect.localScale = Vector3.one;
        rect.anchoredPosition = Vector2.zero;
        _originScale = 1f;
    }
}
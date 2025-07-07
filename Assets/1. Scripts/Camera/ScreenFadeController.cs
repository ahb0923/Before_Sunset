using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{
    public static ScreenFadeController Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public IEnumerator FadeInOut(System.Action onMiddleAction)
    {
        yield return StartCoroutine(Fade(0f, 1f, 0f));

        onMiddleAction?.Invoke();

        yield return StartCoroutine(Fade(1f, 0f, 2f));
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha, float waittime)
    {
        if (waittime > 0f)
            yield return new WaitForSeconds(waittime);

        fadeImage.gameObject.SetActive(true);

        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = toAlpha;
        fadeImage.color = color;

        if (toAlpha == 0f)
            fadeImage.gameObject.SetActive(false);
    }
}

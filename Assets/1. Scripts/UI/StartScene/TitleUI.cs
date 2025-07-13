using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class TitleUI : MonoBehaviour
{
    [Header("Tweening")]
    [SerializeField] private Vector3 _vector3 = Vector3.zero;
    [SerializeField] private float _duration = 3f;

    [Header("Buttons")]
    [SerializeField] private List<GameObject> _buttons;
    
    private const string SINGLE_BUTTON = "SingleButton";
    private const string MULTI_BUTTON = "MultiButton";
    private const string LIKE_BUTTON = "LikeButton";
    private const string MUSIC_BUTTON = "MusicButton";
    private const string HOME_BUTTON = "HomeButton";
    private const string STAR_BUTTON = "StarButton";
    private const string SETTING_BUTTON = "SettingButton";
    private const string QUIT_BUTTON = "QuitButton";

    private void Reset()
    {
        _buttons.Add(GameObject.Find(SINGLE_BUTTON));
        _buttons.Add(GameObject.Find(MULTI_BUTTON));
        _buttons.Add(GameObject.Find(LIKE_BUTTON));
        _buttons.Add(GameObject.Find(MUSIC_BUTTON));
        _buttons.Add(GameObject.Find(HOME_BUTTON));
        _buttons.Add(GameObject.Find(STAR_BUTTON));
        _buttons.Add(GameObject.Find(SETTING_BUTTON));
        _buttons.Add(GameObject.Find(QUIT_BUTTON));
    }

    // private void Start()
    // {
    //     StartCoroutine(C_StartSequence());
    // }
    //
    // private IEnumerator C_StartSequence()
    // {
    //     Tween titleTween = this.transform.DOLocalMove(endValue: _vector3, duration: _duration);
    //     yield return titleTween.WaitForCompletion();
    //
    //     foreach (GameObject button in _buttons)
    //     {
    //         Image image = button.GetComponent<Image>();
    //         if (image != null)
    //         {
    //             image.DOFade(1f, 1f);
    //         }
    //     }
    // }
}

using System.Collections;
using UnityEngine;

public class StartSceneManager : MonoSingleton<StartSceneManager>
{
    [Header("카메라")]
    [SerializeField] private GameObject _mainCamera;
    
    private Vector2 _bottomLeft = new Vector2(31.5f, -4.5f);
    private Vector2 _topRight = new Vector2(84.5f, 17.5f);
    public Coroutine CameraCoroutine;
    public Coroutine ShakeCameraCoroutine;
    
    public StartSceneUI StartSceneUI { get; private set; }
    public StartSceneAnimation StartSceneAnimation { get; private set; }

    private void Reset()
    {
        _mainCamera = GameObject.Find("Main Camera");
    }

    protected override void Awake()
    {
        base.Awake();
        StartSceneUI = GameObject.Find("Canvas").GetComponent<StartSceneUI>();
        StartSceneAnimation = GameObject.Find("StartSceneAnimation").GetComponent<StartSceneAnimation>();
    }
    
    public void ShakeCamera(float duration = 0.2f, float intensity = 0.1f, float delay = 0f)
    {
        ShakeCameraCoroutine = StartCoroutine(C_Shake(duration, intensity, delay));
    }
    
    private IEnumerator C_Shake(float duration, float intensity, float delay)
    {
        Vector3 original = _mainCamera.transform.position;
        yield return new WaitForSeconds(delay);
        while (duration > 0f)
        {
            _mainCamera.transform.position = original + new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0f);
            duration -= Time.deltaTime;
            yield return null;
        }
        _mainCamera.transform.position = original;
    }
    
    public void CameraAction()
    {
        
        Vector2 startPos = new Vector2(
            Random.Range(_bottomLeft.x, _topRight.x),
            Random.Range(_bottomLeft.y, _topRight.y));
        _mainCamera.transform.position = new Vector3(startPos.x, startPos.y, -10f);

        Vector2[] dirs =
        {
            new Vector2(1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, 1f), new Vector2(-1f, -1f)
        };
        Vector2 dir = dirs[Random.Range(0, dirs.Length)].normalized;

        CameraCoroutine = StartCoroutine(C_BounceCamera(dir));
    }

    private IEnumerator C_BounceCamera(Vector2 dir)
    {
        while (true)
        {
            Vector3 pos = _mainCamera.transform.position;
            
            Vector3 nextPos = pos + (Vector3)(dir * 2f * Time.deltaTime);

            if (nextPos.x < _bottomLeft.x || nextPos.x > _topRight.x)
            {
                dir.x *= -1;
                nextPos.x = Mathf.Clamp(nextPos.x, _bottomLeft.x, _topRight.x);
            }

            if (nextPos.y < _bottomLeft.y || nextPos.y > _topRight.y)
            {
                dir.y *= -1;
                nextPos.y = Mathf.Clamp(nextPos.y, _bottomLeft.y, _topRight.y);
            }
            _mainCamera.transform.position = nextPos;
            yield return null;
        }
    }
    
    public void StopCamera()
    {
        if (CameraCoroutine == null)
        {
            return;
        }

        StopCoroutine(CameraCoroutine);
        CameraCoroutine = null;
    }
}
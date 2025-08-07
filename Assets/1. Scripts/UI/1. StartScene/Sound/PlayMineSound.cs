using UnityEngine;

public class PlayMineSound : MonoBehaviour
{
    private void MineSound()
    {
        int i = Random.Range(3, 4);
        AudioManager.Instance.PlaySFX($"HittingARock{i}");
    }
}

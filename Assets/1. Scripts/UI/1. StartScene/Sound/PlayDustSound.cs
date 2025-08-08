using UnityEngine;

public class PlayDustSound : MonoBehaviour
{
    private void DustSound()
    {
        AudioManager.Instance.PlaySFX("Dust");
    }
}

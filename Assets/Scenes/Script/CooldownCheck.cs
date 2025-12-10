using UnityEngine;

public class CooldownCheck : MonoBehaviour
{
    private void OnDisable()
    {
        Debug.Log($"[CooldownBG DISABLED by something] {name}", this);
    }
}

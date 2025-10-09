using UnityEngine;

namespace UMod.Examples
{
    public class KeepAlive : MonoBehaviour
    {
        // Methods
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

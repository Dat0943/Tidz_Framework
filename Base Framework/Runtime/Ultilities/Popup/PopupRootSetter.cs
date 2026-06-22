using UnityEngine;

namespace Tidz
{
    public class PopupRootSetter : MonoBehaviour
    {
        private void Awake()
        {
            PopupManager.SetRoot(transform);
        }
    }
}

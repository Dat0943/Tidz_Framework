using Sirenix.OdinInspector;
using UnityEngine;

namespace Tidz
{
    public class LFactory : ScriptableObjectSingleton<LFactory>
    {
        [Title("Scene Loader")]
        [SerializeField] GameObject _sceneLoaderPrefab;

        public static GameObject sceneLoaderPrefab => instance._sceneLoaderPrefab;
    }
}
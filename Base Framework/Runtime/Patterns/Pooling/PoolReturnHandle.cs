using UnityEngine;

namespace Tidz
{
    public sealed class PoolReturnHandle : MonoBehaviour
    {
        private int _spawnVersion;

        public int MarkSpawn()
        {
            _spawnVersion++;
            return _spawnVersion;
        }

        public bool IsVersion(int v) => _spawnVersion == v;
    }
}

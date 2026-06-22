using UnityEngine;

namespace Tidz
{
    public class FactorySfx : ScriptableObjectSingleton<FactorySfx>
    {
        [SerializeField] private AudioConfig _click;

        public static AudioConfig click => instance._click;
    }
}

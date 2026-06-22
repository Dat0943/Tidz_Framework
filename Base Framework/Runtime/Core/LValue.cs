using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tidz
{
    [Serializable]
    public class LValue<T>
    {
        [SerializeField] T _value;

        public T value
        {
            get { return _value; }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    eventValueChanged?.Invoke(_value);
                }
            }
        }

        public event Action<T> eventValueChanged;

        public LValue() { }

        public LValue(T defaultValue)
        {
            _value = defaultValue;
        }
    }
}
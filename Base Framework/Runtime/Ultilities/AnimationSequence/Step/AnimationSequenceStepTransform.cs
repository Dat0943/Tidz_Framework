using Sirenix.OdinInspector;
using UnityEngine;

namespace Tidz
{
    public abstract class AnimationSequenceStepTransform : AnimationSequenceStepAction<Transform>
    {
        [ShowIf("@_changeStartValue")]
        [SerializeField] protected Vector3 _valueStart;

        [SerializeField] protected Vector3 _value;
    }
}
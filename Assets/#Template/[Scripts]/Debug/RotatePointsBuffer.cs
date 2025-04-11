using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DancingLineFanmade.Debugging
{
    [CreateAssetMenu]
    public class RotatePointsBuffer : ScriptableObject
    {
        [TableList]
        public List<RotatePoints> SavedPoints;
    }
}

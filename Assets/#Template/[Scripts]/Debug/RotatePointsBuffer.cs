using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingLineFanmade.Debugging
{
    [CreateAssetMenu]
    public class RotatePointsBuffer : ScriptableObject
    {
        public List<RotatePoints> SavedPoints;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DancingLineFanmade.Gameplay.Skin
{
    public interface ISkin
    {
        public void OnReadying();
        public void OnRotate();
        public void OnLanding();
        public void OnFlying();
        public void OnDie();
        public void OnWin();
    }
}

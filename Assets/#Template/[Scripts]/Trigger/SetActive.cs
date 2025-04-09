using DancingLineFanmade.Gameplay;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DancingLineFanmade.Triggers
{
    [Serializable]
    public struct SingleActive
    {
        public GameObject target;
        public bool active;
        public bool dontRevive;

        public SingleActive(GameObject target, bool active, bool dontRevive)
        {
            this.target = target;
            this.active = active;
            this.dontRevive = dontRevive;
        }

        public void SetActive()
        {
            target.SetActive(active);
        }
    }

    [DisallowMultipleComponent]
    public class SetActive : MonoBehaviour
    {
        [SerializeField] internal bool activeOnAwake;
        [SerializeField, TableList] internal List<SingleActive> actives = new();

        private readonly List<SingleActive> revives = new();
        //internal int index;

        private void Start()
        {
            if (!activeOnAwake) 
                return;
            foreach (var s in actives)
            {
                s.SetActive();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || activeOnAwake)
                return;
            //index = Player.Instance.Checkpoints.Count;
            foreach (var s in actives)
            {
                s.SetActive();
            }
        }

        internal void AddRevives()
        {
            for (var a = 0; a < actives.Count; a++)
            {
                revives.Add(new SingleActive(actives[a].target, actives[a].target.activeSelf, actives[a].dontRevive));
            }
        }

        /*internal void Revive()
        {
            LevelManager.CompareCheckpointIndex(index, () =>
            {
                foreach (var s in revives.Where(s => !s.dontRevive))
                {
                    s.SetActive();
                }
            });
        }*/
    }
}
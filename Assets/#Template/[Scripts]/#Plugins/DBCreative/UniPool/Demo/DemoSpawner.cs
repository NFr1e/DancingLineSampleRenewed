using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniPoolNS {

    public class DemoSpawner : MonoBehaviour
    {

        [SerializeField]
        Color[] colorList;

        [SerializeField]
        List<GameObject> prefabList;

        [Range(1,30)]
        [SerializeField]
        int multiplier = 5;

        [SerializeField]
        int fillAmount = 0;

        // Start is called before the first frame update
        void Start()
        {
            for(int i=0, len=prefabList.Count; i<len; ++i){
                UniPool.Register<DemoSpawnObj>(prefabList[i]);
            }

            if(fillAmount > 0){

                for(int i=0, len=prefabList.Count; i<len; ++i){
                    UniPool.Fill(prefabList[i], fillAmount);
                }
                
            }
            
        }


        void DoSpawn(){
            
            Color color = colorList[ Random.Range(0, colorList.Length) ];
            //Vector3 direction = Random.insideUnitSphere.normalized;
            Vector3 direction = Random.insideUnitCircle.normalized;
            float speed = Random.Range(10f, 50f);

            //
            PoolCell cell = UniPool.Get( prefabList[ Random.Range(0, prefabList.Count) ] );
            cell.transform.position = transform.position;

            DemoSpawnObj spawn = cell.GetComponent<DemoSpawnObj>();
            spawn.Setup(direction, speed, color);
            
            cell.gameObject.SetActive(true);
            //
            UniPool.Release(cell, Random.Range(3f, 5f) );
        }

        void FixedUpdate(){
            
            for(int i=0; i<multiplier; ++i){
                DoSpawn();
            }

        }

        
    }

}
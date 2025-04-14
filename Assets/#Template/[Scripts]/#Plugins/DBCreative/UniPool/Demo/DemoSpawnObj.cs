using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniPoolNS {

    public class DemoSpawnObj : MonoBehaviour
    {
        Renderer meshRenderer = null;
        //Rigidbody rb = null;

        bool initPassed = false;

        Vector3 velocity = Vector3.up;

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        void Init(){
            meshRenderer = GetComponent<Renderer>();
            //rb = GetComponent<Rigidbody>();
            initPassed = true;
        }

        public void Setup(Vector3 direction, float speed, Color color){
            if(!initPassed) Init();

            transform.forward = direction;
            velocity = direction * speed;
            meshRenderer.material.color = color;
            
        }

        public void SetActive(bool status){
            gameObject.SetActive(status);
        }

        void Update(){
            velocity.y += -15f * Time.deltaTime;
            //transform.position += velocity * Time.deltaTime;
            transform.Translate(velocity * Time.deltaTime);
        }

        
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniPoolNS {
// You can remove namespace to use it in the open.
// (if you don't have the following class names taken: UniPool, PoolCell, PoolManager)

    public static class UniPool {
    // Wrapper static class for ease of access.
    // access example: UniPool.Get(prefab);

        // Static reference to the first created pool manager.
        public static PoolManager Pool = null;

        public static PoolCell Get(GameObject prefab){
            return Pool.Get(prefab);
        }

        public static PoolCell Get<T>(GameObject prefab, bool componentInChildren = false){
            return Pool.Get<T>(prefab, componentInChildren);
        }

        public static PoolCell Get(string name){
            return Pool.Get(name);
        }

        public static PoolCell Get<T>(string name){
            return Pool.Get(name);
        }
        //

        public static void Release(PoolCell cell, float delay){
            Pool.Release(cell, delay);
        }
        public static void Release(GameObject actor, float delay){
            Pool.Release(actor, delay);
        }
        public static void Release(Transform actorT, float delay){
            Pool.Release(actorT, delay);
        }
        public static void Release(object com, float delay){
            Pool.Release(com, delay);
        }

        public static void ReleaseAll(GameObject prefab, float delay){
            Pool.ReleaseAll(prefab, delay);
        }

        public static void ReleaseAll(string name, float delay){
            Pool.ReleaseAll(name, delay);
        }


        //

        public static void Release(PoolCell cell){
            Pool.Release(cell);
        }
        public static void Release(GameObject actor){
            Pool.Release(actor);
        }
        public static void Release(Transform actorT){
            Pool.Release(actorT);
        }
        public static void Release(object com){
            Pool.Release(com);
        }

        //

        public static void ReleaseAll(GameObject prefab){
            Pool.ReleaseAll(prefab);
        }

        public static void ReleaseAll(string name){
            Pool.ReleaseAll(name);
        }
        

        //

        public static bool Register(GameObject prefab, Transform container = null){
            return Pool.Register(prefab, container);
        }

        public static bool Register(GameObject prefab, string name, Transform container = null){
            return Pool.Register(prefab, name, container);
        }

        public static bool Register<T>(GameObject prefab, Transform container = null, bool componentInChildren = false){
            return Pool.Register<T>(prefab, container, componentInChildren);
        }

        public static bool Register<T>(GameObject prefab, string name, Transform container = null, bool componentInChildren = false){
            return Pool.Register<T>(prefab, name, container, componentInChildren);
        }

        //

        // Note: Unregistering also destroys all instances of GameObject
        public static void Unregister(GameObject prefab){
            Pool.Unregister(prefab);
        }

        public static void Unregister(string name){
            Pool.Unregister(name);
        }

        //

        public static void Fill(GameObject prefab, int amount){
            Pool.Fill(prefab, amount);
        }

        public static void Fill(string name, int amount){
            Pool.Fill(name, amount);
        }

        //

        // Remove from pooling scheme. May choose to not destroy GameObject.
        public static void Remove(PoolCell cell, bool destroy = true){
            Pool.Remove(cell, destroy);
        }

        public static void Remove(GameObject actor, bool destroy = true){
            Pool.Remove(actor, destroy);
        }

        public static void Remove(Transform actorT, bool destroy = true){
            Pool.Remove(actorT, destroy);
        }

        public static void Remove(object com, bool destroy = true){
            Pool.Remove(com, destroy);
        }

        //

        // Destroy all instances of this GameObject.
        // To unregister - use corresponding Unregister method.
        public static void DestroyAll(GameObject prefab){
            Pool.DestroyAll(prefab);
        }

        public static void DestroyAll(string name){
            Pool.DestroyAll(name);
        }

        // Clears / Destroys ALL instances and starts like new.
        public static void DestroyAll(){
            Pool.DestroyAll();
        }


    }

}

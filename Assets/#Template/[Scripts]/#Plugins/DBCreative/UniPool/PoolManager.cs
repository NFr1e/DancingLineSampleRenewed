using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main class of UniPool pooling solution. This is where all the features are created.

namespace UniPoolNS {

    public class PoolManager : MonoBehaviour
    {

        [Tooltip("Default container. On Release return pooled objects to this container")]
        [SerializeField]
        Transform Container = null;// default container

        Dictionary<GameObject, PoolStack> PrefabCellStackMap = new Dictionary<GameObject, PoolStack>();
        Dictionary<PoolCell, PoolStack> CellStackMap = new Dictionary<PoolCell, PoolStack>();
        Dictionary<string, PoolStack> NameCellStackMap = new Dictionary<string, PoolStack>();

        Dictionary<GameObject, PoolCell> GameObjectCellMap = new Dictionary<GameObject, PoolCell>();
        Dictionary<Transform, PoolCell> TransformCellMap = new Dictionary<Transform, PoolCell>();
        Dictionary<object, PoolCell> ComponentCellMap = new Dictionary<object, PoolCell>();

        void Init(){
            if(Container == null) Container = transform;

            UniPool.Pool = this;
        }

        void Awake(){
            Init();
        }

        public PoolCell Get(GameObject prefab){

            PoolCell cell = null;
            PoolStack cellStack = null;

            if(PrefabCellStackMap.TryGetValue(prefab, out cellStack)){

                
                if(!cellStack.TryPop(out cell)){
                // create cell;

                    cell = AddPoolCell(cellStack);

                }

                //
                // fallback in case gameObject was somehow destroyed
                while(cell.gameObject == null){
                    Remove(cell);
                    if(!cellStack.TryPop(out cell)){
                    // create cell;

                        cell = AddPoolCell(cellStack);

                    }
                }

                
            }else{
            // create new prefab entry

                cellStack = new PoolStack();

                cellStack.activeOnGet = prefab.activeSelf;
                cellStack.prefabScale = prefab.transform.localScale;
                cellStack.Container = Container;

                PrefabCellStackMap[prefab] = cellStack;

                cellStack.CellGenerator = CreatePoolCell(prefab, Container);

                cell = AddPoolCell(cellStack);

            }
            
            cell.isPooled = false;
            if(cellStack.activeOnGet) cell.gameObject.SetActive(true);

            return cell;

        }

        public PoolCell Get<T>(GameObject prefab, bool componentInChildren = false){

            PoolCell cell = null;
            PoolStack cellStack = null;

            if(PrefabCellStackMap.TryGetValue(prefab, out cellStack)){

                if(!cellStack.TryPop(out cell)){
                // create cell;

                    cell = AddPoolCell(cellStack);

                }

                //
                // fallback in case gameObject was somehow destroyed
                while(cell.gameObject == null){
                    Remove(cell);
                    if(!cellStack.TryPop(out cell)){
                    // create cell;

                        cell = AddPoolCell(cellStack);

                    }
                }
                
            }else{
                // create new prefab entry
                cellStack = new PoolStack();

                cellStack.activeOnGet = prefab.activeSelf;
                cellStack.prefabScale = prefab.transform.localScale;
                cellStack.Container = Container;

                PrefabCellStackMap[prefab] = cellStack;

                cellStack.CellGenerator = CreatePoolCell<T>(prefab, Container, componentInChildren);

                cell = AddPoolCell(cellStack);
            }

            cell.isPooled = false;
            if(cellStack.activeOnGet) cell.gameObject.SetActive(true);

            return cell;

        }

        // string Name version
        // Note: must register prefab by name, before using Get via name

        public PoolCell Get(string name){

            PoolCell cell = null;
            PoolStack cellStack = null;

            if(!NameCellStackMap.TryGetValue(name, out cellStack)){
                return null;
            }

            if(!cellStack.TryPop(out cell)){
            // create cell;

                cell = AddPoolCell(cellStack);

            }

            //
            // fallback in case gameObject was somehow destroyed
            while(cell.gameObject == null){
                Remove(cell);
                if(!cellStack.TryPop(out cell)){
                // create cell;

                    cell = AddPoolCell(cellStack);

                }
            }
            
            cell.isPooled = false;
            if(cellStack.activeOnGet) cell.gameObject.SetActive(true);

            return cell;

        }

        public PoolCell Get<T>(string name){
        // for call consistency
            return Get(name);
        }

        //

        IEnumerator ReleaseWithDelay(PoolCell cell, float delay){
            yield return new WaitForSeconds(delay);
            Release(cell);
        }
        IEnumerator ReleaseWithDelay(GameObject actor, float delay){
            yield return new WaitForSeconds(delay);
            Release(actor);
        }
        IEnumerator ReleaseWithDelay(Transform actorT, float delay){
            yield return new WaitForSeconds(delay);
            Release(actorT);
        }
        IEnumerator ReleaseWithDelay(object com, float delay){
            yield return new WaitForSeconds(delay);
            Release(com);
        }
        //
        IEnumerator ReleaseAllWithDelay(GameObject prefab, float delay){
            yield return new WaitForSeconds(delay);
            ReleaseAll(prefab);
        }
        IEnumerator ReleaseAllWithDelay(string name, float delay){
            yield return new WaitForSeconds(delay);
            ReleaseAll(name);
        }
        //

        public void Release(PoolCell cell, float delay){
            StartCoroutine(ReleaseWithDelay(cell, delay));
        }
        public void Release(GameObject actor, float delay){
            StartCoroutine(ReleaseWithDelay(actor, delay));
        }
        public void Release(Transform actorT, float delay){
            StartCoroutine(ReleaseWithDelay(actorT, delay));
        }
        public void Release(object com, float delay){
            StartCoroutine(ReleaseWithDelay(com, delay));
        }
        //
        public void ReleaseAll(GameObject prefab, float delay){
            StartCoroutine(ReleaseAllWithDelay(prefab, delay));
        }
        public void ReleaseAll(string name, float delay){
            StartCoroutine(ReleaseAllWithDelay(name, delay));
        }
        //

        void ProcessRelease(PoolCell cell){
            if(cell.isPooled) return;

            PoolStack cellStack = CellStackMap[cell];
            cellStack.Push(cell);

            cell.isPooled = true;

            cell.gameObject.SetActive(false);
            cell.transform.parent = cellStack.Container;
            cell.transform.localScale = cellStack.prefabScale;
            //cell.transform.localPosition = Vector3.zero;
        }

        public void Release(PoolCell cell){
            if(cell.isPooled) return;

            PoolStack cellStack;
            if(CellStackMap.TryGetValue(cell, out cellStack)){

                cellStack.Push(cell);

                cell.isPooled = true;

                cell.gameObject.SetActive(false);
                cell.transform.parent = cellStack.Container;
                cell.transform.localScale = cellStack.prefabScale;
                //cell.transform.localPosition = Vector3.zero;
            }
        }
        public void Release(GameObject actor){
            PoolCell cell;
            if(GameObjectCellMap.TryGetValue(actor, out cell)){
                ProcessRelease(cell);
            }else{
                GameObject.Destroy( actor );
            }
        }
        public void Release(Transform actorT){
            PoolCell cell;
            if(TransformCellMap.TryGetValue(actorT, out cell)){
                ProcessRelease(cell);
            }else{
                GameObject.Destroy( actorT.gameObject );
            }
        }
        public void Release(object com){
            PoolCell cell;
            if(ComponentCellMap.TryGetValue(com, out cell)){
                ProcessRelease(cell);
            }else{
                GameObject.Destroy( ((MonoBehaviour)com).gameObject );
            }
        }

        //

        public void ReleaseAll(GameObject prefab){
            PoolStack cellStack = null;
            PoolCell cell = null;
            if(PrefabCellStackMap.TryGetValue(prefab, out cellStack)){
                List<PoolCell> cellList = cellStack.CellRegistry;
                for(int i=0, len=cellList.Count; i<len; ++i){
                    cell = cellList[i];
                    if(cell.isPooled == false){
                        
                        cellStack.Push(cell);

                        cell.isPooled = true;

                        cell.gameObject.SetActive(false);
                        cell.transform.parent = cellStack.Container;
                        cell.transform.localScale = cellStack.prefabScale;

                    }
                }
            }
        }

        public void ReleaseAll(string name){
            PoolStack cellStack = null;
            PoolCell cell = null;
            if(NameCellStackMap.TryGetValue(name, out cellStack)){
                List<PoolCell> cellList = cellStack.CellRegistry;
                for(int i=0, len=cellList.Count; i<len; ++i){
                    cell = cellList[i];
                    if(cell.isPooled == false){
                        
                        cellStack.Push(cell);

                        cell.isPooled = true;

                        cell.gameObject.SetActive(false);
                        cell.transform.parent = cellStack.Container;
                        cell.transform.localScale = cellStack.prefabScale;

                    }
                }
            }
        }

        //

        public bool Register(GameObject prefab, Transform container = null){
            if(PrefabCellStackMap.ContainsKey(prefab) ) return false;

            PoolStack cellStack = new PoolStack();

            cellStack.activeOnGet = prefab.activeSelf;
            cellStack.prefabScale = prefab.transform.localScale;

            if(container != null){
                cellStack.Container = container;
            }else{
                cellStack.Container = Container;
            }

            cellStack.CellGenerator = CreatePoolCell(prefab, cellStack.Container);

            PrefabCellStackMap[prefab] = cellStack;

            return true;   
        }

        public bool Register(GameObject prefab, string name, Transform container = null){
            if(NameCellStackMap.ContainsKey(name) || PrefabCellStackMap.ContainsKey(prefab) ) return false;

            PoolStack cellStack = new PoolStack();

            cellStack.activeOnGet = prefab.activeSelf;
            cellStack.prefabScale = prefab.transform.localScale;

            if(container != null){
                cellStack.Container = container;
            }else{
                cellStack.Container = Container;
            }

            cellStack.CellGenerator = CreatePoolCell(prefab, cellStack.Container);

            PrefabCellStackMap[prefab] = cellStack;
            
            NameCellStackMap[name] = cellStack;

            return true;   
        }

        //

        public bool Register<T>(GameObject prefab, Transform container = null, bool componentInChildren = false){
            if(PrefabCellStackMap.ContainsKey(prefab) ) return false;

            PoolStack cellStack = new PoolStack();

            cellStack.activeOnGet = prefab.activeSelf;
            cellStack.prefabScale = prefab.transform.localScale;

            if(container != null){
                cellStack.Container = container;
            }else{
                cellStack.Container = Container;
            }

            cellStack.CellGenerator = CreatePoolCell<T>(prefab, cellStack.Container, componentInChildren);

            PrefabCellStackMap[prefab] = cellStack;

            return true;
        }

        public bool Register<T>(GameObject prefab, string name, Transform container = null, bool componentInChildren = false){
            if(NameCellStackMap.ContainsKey(name) || PrefabCellStackMap.ContainsKey(prefab) ) return false;

            PoolStack cellStack = new PoolStack();

            cellStack.activeOnGet = prefab.activeSelf;
            cellStack.prefabScale = prefab.transform.localScale;
            
            if(container != null){
                cellStack.Container = container;
            }else{
                cellStack.Container = Container;
            }

            cellStack.CellGenerator = CreatePoolCell<T>(prefab, cellStack.Container, componentInChildren);

            PrefabCellStackMap[prefab] = cellStack;
            
            NameCellStackMap[name] = cellStack;

            return true;
        }

        //

        // Note: Unregister also destroys all instances of the object, both active and pooled
        public void Unregister(GameObject prefab){
            if(PrefabCellStackMap.ContainsKey(prefab)){
                DestroyAll(prefab);
                PrefabCellStackMap.Remove(prefab);
            }
        }

        public void Unregister(string name){
            PoolStack cellStack = null;
            GameObject prefab = null;
            if(NameCellStackMap.TryGetValue(name, out cellStack)){
                foreach(var kvp in PrefabCellStackMap){
                    if(kvp.Value == cellStack){
                        prefab = kvp.Key;
                        break;
                    }
                }
                if(prefab != null){
                    DestroyAll(prefab);

                    PrefabCellStackMap.Remove(prefab);
                    NameCellStackMap.Remove(name);
                }
            }

            
        }


        //

        public void Fill(GameObject prefab, int amount){
            PoolStack cellStack;
            if(PrefabCellStackMap.TryGetValue(prefab, out cellStack)){
                PoolCell cell = null;
                for(int i=0; i<amount; ++i){
                    cell = AddPoolCell(cellStack);
                    cellStack.Push(cell);
                    cell.isPooled = true;
                }
            }
        }
        public void Fill(string name, int amount){
            PoolStack cellStack;
            if(NameCellStackMap.TryGetValue(name, out cellStack)){
                PoolCell cell = null;
                for(int i=0; i<amount; ++i){
                    cell = AddPoolCell(cellStack);
                    cellStack.Push(cell);
                    cell.isPooled = true;
                }
            }
        }

        //

        // Remove from pooling scheme. May choose to not destroy GameObject.
        public void Remove(PoolCell cell, bool destroy = true){
            if(cell.isPooled){
                CellStackMap[cell].Remove(cell);
            }
            CellStackMap.Remove(cell);
            TransformCellMap.Remove(cell.transform);
            if(cell.component != null) ComponentCellMap.Remove(cell.component);
            GameObjectCellMap.Remove(cell.gameObject);
            //
            if(cell.gameObject != null && destroy == true){
                GameObject.Destroy(cell.gameObject);
            }
            
        }

        public void Remove(GameObject actor, bool destroy = true){
            PoolCell cell;
            if(GameObjectCellMap.TryGetValue(actor, out cell)){
                Remove(cell, destroy);
            }
        }

        public void Remove(Transform actorT, bool destroy = true){
            PoolCell cell;
            if(TransformCellMap.TryGetValue(actorT, out cell)){
                Remove(cell, destroy);
            }
        }

        public void Remove(object com, bool destroy = true){
            PoolCell cell;
            if(ComponentCellMap.TryGetValue(com, out cell)){
                Remove(cell, destroy);
            }
        }


        public void DestroyAll(GameObject prefab){
            PoolStack cellStack = null;
            PoolCell cell = null;
            
            if(PrefabCellStackMap.TryGetValue(prefab, out cellStack)){
                List<PoolCell> cellList = cellStack.CellRegistry;
                
                for(int i=0, len=cellList.Count; i<len; ++i){
                    cell = cellList[i];
                    CellStackMap.Remove(cell);
                    TransformCellMap.Remove(cell.transform);
                    if(cell.component != null) ComponentCellMap.Remove(cell.component);
                    GameObjectCellMap.Remove(cell.gameObject);
                    //
                    if(cell.gameObject != null){
                        GameObject.Destroy(cell.gameObject);
                    }
                }
                cellStack.RemoveAll();
            }
        }

        public void DestroyAll(string name){
            PoolStack cellStack = null;
            PoolCell cell = null;

            if(NameCellStackMap.TryGetValue(name, out cellStack)){
                List<PoolCell> cellList = cellStack.CellRegistry;
                
                for(int i=0, len=cellList.Count; i<len; ++i){
                    cell = cellList[i];
                    CellStackMap.Remove(cell);
                    TransformCellMap.Remove(cell.transform);
                    if(cell.component != null) ComponentCellMap.Remove(cell.component);
                    GameObjectCellMap.Remove(cell.gameObject);
                    //
                    if(cell.gameObject != null){
                        GameObject.Destroy(cell.gameObject);
                    }
                }
                cellStack.RemoveAll();
            }
        }

        public void DestroyAll(){
            foreach(PoolCell cell in CellStackMap.Keys){
                GameObject.Destroy(cell.gameObject);
            }
            PoolStack cellStack = null;
            foreach(var kvp in PrefabCellStackMap){
                cellStack = kvp.Value;
                cellStack.RemoveAll();
            }
            CellStackMap = new Dictionary<PoolCell, PoolStack>();
            GameObjectCellMap = new Dictionary<GameObject, PoolCell>();
            TransformCellMap = new Dictionary<Transform, PoolCell>();
            ComponentCellMap = new Dictionary<object, PoolCell>();
        }


        //


        IEnumerator CreatePoolCell<T>(GameObject prefab, Transform container, bool componentInChildren){
            PoolCell cell = null;
            Transform _container = container;
            //
            for(;;){
                cell = new PoolCell(prefab, _container);
                cell.SetComponent<T>(componentInChildren);

                yield return cell;
            }
        }
        IEnumerator CreatePoolCell(GameObject prefab, Transform container){
            Transform _container = container;
            //
            for(;;){
                yield return new PoolCell(prefab, _container);
            }
        }

        PoolCell AddPoolCell(PoolStack cellStack){
            
            PoolCell cell = cellStack.GenerateCell();

            CellStackMap[cell] = cellStack;
            GameObjectCellMap[cell.gameObject] = cell;
            TransformCellMap[cell.transform] = cell;
            
            if(cell.component != null){
                ComponentCellMap[cell.component] = cell;
            }

            return cell;
        }


        protected class PoolStack
        {
            int Index = -1;
            int Count = 0;
            List<PoolCell> CellStack = new List<PoolCell>();

            // tracks all cells in this stack
            public List<PoolCell> CellRegistry { get; protected set; } = new List<PoolCell>();


            public IEnumerator CellGenerator;

            public bool activeOnGet = false;

            public Vector3 prefabScale = Vector3.one;

            public Transform Container = null;
            
            public void Push(PoolCell cell){

                ++Index;
                if(Index < Count){
                    
                    CellStack[Index] = cell;
                
                }else{
                // expand
                    CellStack.Add(cell);
                    ++Count;
                }
                
            }

            public bool TryPop(out PoolCell cell){

                if(Index > -1){
                    cell = CellStack[Index];
                    CellStack[Index] = null;
                    --Index;
                    return true;
                }else{
                    cell = null;
                    return false;
                }
            }

            public void Remove(PoolCell cell){
                for(int i=0; i<=Index; ++i){
                    if(CellStack[i] == cell){

                        for(;i<Index; ++i){
                        // shift cells down
                            CellStack[i] = CellStack[i+1];
                        }
                        
                        CellStack[Index] = null;
                        --Index;
                    }
                }
                //

                CellRegistry.Remove(cell);
            }

            public void RemoveAll(){
            // RemoveAll does not destroy the cells or the objects
                Count = 0;
                Index = -1;
                CellStack = new List<PoolCell>();
                CellRegistry = new List<PoolCell>();
            }

            public void RegisterCell(PoolCell cell){
                CellRegistry.Add(cell);
            }

            public PoolCell GenerateCell(){
                CellGenerator.MoveNext();
                PoolCell cell = (PoolCell)(CellGenerator.Current);
                RegisterCell(cell);
                return cell;
            }

        }
        

    }



    //

    public class PoolCell
    {
        public bool isPooled = false;// for pooling manager to set

        public GameObject gameObject = null;
        public Transform transform = null;
        public object component = null;

        public PoolCell(GameObject prefab, Transform container){
            gameObject = GameObject.Instantiate(prefab, container);
            gameObject.SetActive(false);
            transform = gameObject.transform;
        }
        
        public void SetComponent<T>(bool componentInChildren = false){
            if(componentInChildren == false){
                component = gameObject.GetComponent<T>();
            }else{
                component = gameObject.GetComponentInChildren<T>();
            }
        }
        public T GetComponent<T>(){
            if(component == null){
                return default(T);
            }
            return (T)component;
        }
        
    }

}

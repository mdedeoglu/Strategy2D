using UnityEngine;
using System.Collections.Generic;

namespace Strategy2D
{
    public class Pool : MonoBehaviour
    {
        public GameObject PoolCell;                                     // One cell of the pool

        private Stack<GameObject> _poolStack = new Stack<GameObject>(); // Stack for pool
        private GameObject _parent;                                     // Root for unused obj
        private int _poolObjectCount = 40;


        // Initiliazes the pool with given name
        public void InitPool(Transform parent)
        {
            _parent = new GameObject("Pool");
            _parent.transform.SetParent(parent);

            // Filling the pool with poolCellPrefab object
            for (int i = 0; i < _poolObjectCount; i++)
                PushObject(GameObject.Instantiate(PoolCell));
        }

        // Returns object to the pool
        public void ReturnObjectToPool(Transform go)
        {
            GameObject poolObject = go.gameObject;
            PushObject(poolObject);
        }

        // Pushes object to the pool stack
        private void PushObject(GameObject poolObject)
        {
            poolObject.SetActive(false);
            poolObject.name = "Pool Cell";
            _poolStack.Push(poolObject);
            poolObject.transform.SetParent(_parent.transform, false);
        }

        // Pops object from the pool stack
        public GameObject PopObject()
        {
            GameObject poolObject = _poolStack.Pop().gameObject;
            poolObject.SetActive(true);
            return poolObject;
        }
    }
}
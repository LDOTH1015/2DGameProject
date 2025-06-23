using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly List<T> pool = new List<T>();
    private readonly Transform parent;

    public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateNewObject();
            pool.Add(obj);
        }
    }

    private T CreateNewObject()
    {
        T newObj = Object.Instantiate(prefab);

        newObj.transform.SetParent(parent, false);
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        foreach (var obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                PrepareObject(obj, position, rotation);
                return obj;
            }
        }

        T newObj = CreateNewObject();
        pool.Add(newObj);
        PrepareObject(newObj, position, rotation);
        return newObj;
    }

    private void PrepareObject(T obj, Vector3 position, Quaternion rotation)
    {
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
    }
}
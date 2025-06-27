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
        // Instantiate는 항상 prefab을 Active 상태로 저장해 두고 사용해야 함
        T newObj = Object.Instantiate(prefab);

        // 🔹 부모 설정 시에도 Scale, Rotation 영향을 받지 않게 identity 설정
        if (parent != null)
        {
            newObj.transform.SetParent(parent);
        }
        else
        {
            newObj.transform.parent = null; // 부모가 없으면 null로 고정
        }

        newObj.gameObject.SetActive(false); // 풀링 시 비활성화
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
        obj.transform.parent = null; // 부모 강제 해제
        obj.gameObject.SetActive(true);
    }


    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
    }
}

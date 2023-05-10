using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
	public static ObjectPooler current;
	
	[SerializeField] private bool logging;
	[SerializeField] private GameObject bulletObj;
	[SerializeField] private int bulletCount;
	[SerializeField] private GameObject molotovObj;
	[SerializeField] private int molotovCount;
	[SerializeField] private GameObject textObj;
	[SerializeField] private int textCount;

	private List<List<GameObject>> mainPool;

	private void Awake()
	{
		current = this;
		mainPool = new List<List<GameObject>>();
		AddObject<Bullet>(bulletObj, bulletCount);
		AddObject<Molotov>(molotovObj, molotovCount);
		AddObject<FloatingText>(textObj, textCount);
	}

	public void AddObject<T>(GameObject prefab, int count)
	{
		/* If pooled object does not exist in main pool, add pooled object to main list. */
		if (!GetPooledObject<T>())
		{
			List<GameObject> newPooledList = new List<GameObject>();
			GameObject newPooledObj;

			for (int i = 0; i < count; i++)
			{
				newPooledObj = Instantiate(prefab);
				newPooledObj.hideFlags = HideFlags.HideInHierarchy;
				newPooledObj.SetActive(false);
				newPooledList.Add(newPooledObj);
			}

			mainPool.Add(newPooledList);
		}
	}

	public GameObject GetPooledObject<T>()
	{
		foreach (var pool in mainPool)
		{
			GameObject pooledObj = pool[0];
			var pooledObjType = pooledObj.GetComponent<T>();

			if (pooledObjType != null)
			{
				/* Finds the first inactive pooled object from its own pool. */
				for (int i = 0; i < pool.Count; i++)
				{
					if (!pool[i].activeSelf)
					{
						return pool[i];
					}
				}
			}
		}

		return null;
	}
}

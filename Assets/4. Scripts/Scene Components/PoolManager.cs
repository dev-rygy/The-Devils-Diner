using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class PoolManager : MonoBehaviour
{
    public static PoolManager main;

    public static int UID_LENGTH = 8;

    [System.Serializable]
    private struct PrefabPool
    {
        public GameObject prefab;
        public int size;
    }
    [Header("Settings")]
    [SerializeField]
    private Transform poolsHolder;
    [SerializeField]
    private PrefabPool[] prefabPools;

    private Dictionary<string, Transform> transformDictionary = new Dictionary<string, Transform>();
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    [Header("Asset Management")]
    [SerializeField]
    private string prefabDirectory;

    [Header("Debugs")]
    [SerializeField]
    private bool isInitialized;

    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        foreach (PrefabPool prefabPool in prefabPools)
        {
            var uid = prefabPool.prefab.GetComponent<PoolObject>().UID;
            var poolTransform = new GameObject($"UID = {uid}").transform;
            poolTransform.parent = poolsHolder;
            poolTransform.gameObject.AddComponent<PoolMonitor>();
            transformDictionary[uid] = poolTransform;

            var poolQueue = new Queue<GameObject>();
            for (int i = 0; i < prefabPool.size; i++)
            {
                GameObject poolObject = Instantiate(prefabPool.prefab, poolTransform);
                poolObject.SetActive(false);
                poolQueue.Enqueue(poolObject);
            }

            poolDictionary[uid] = poolQueue;
        }

        isInitialized = true;
    }

    public GameObject Spawn(GameObject prefab)
    {
        var uid = prefab.GetComponent<PoolObject>().UID;
        var poolQueue = poolDictionary[uid];
        if(poolQueue.Count > 0)
        {
            var gameObject = poolQueue.Dequeue();
            gameObject.SetActive(true);
            return gameObject;
        }
        else
        {
            var gameObject = Instantiate(prefab, transformDictionary[uid]);
            gameObject.SetActive(true);
            return gameObject;
        }        
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var gameObject = Spawn(prefab);

        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        if (parent != null)
            gameObject.transform.parent = parent;

        return gameObject;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Transform parent = null)
    {
        return Spawn(prefab, position, Quaternion.identity, parent);
    }

    public void Despawn(GameObject gameObject)
    {
        var uid = gameObject.GetComponent<PoolObject>().UID;
        gameObject.SetActive(false);
        poolDictionary[uid].Enqueue(gameObject);
    }

#if UNITY_EDITOR

    [ContextMenu("Generate Prefab UIDs")]
    private void GeneratePrefabUIDs()
    {
        // Find all prefabs in the specified folder and its subdirectories
        var prefabPaths = AssetDatabase.FindAssets("t:Prefab", new string[] { prefabDirectory });
        var usedUIDs = new HashSet<string>();

        foreach (string prefabPath in prefabPaths)
        {
            string prefabAssetPath = AssetDatabase.GUIDToAssetPath(prefabPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);
            if (prefab.TryGetComponent(out PoolObject poolObject) )
            {
                if (poolObject.UID != "")
                {
                    // Regenerate UID if found duplicate
                    if (usedUIDs.Contains(poolObject.UID))
                        poolObject.UID = GenerateUIDNoCollision(usedUIDs);

                    usedUIDs.Add(poolObject.UID);
                }
                else
                {
                    poolObject.UID = GenerateUIDNoCollision(usedUIDs);
                    usedUIDs.Add(poolObject.UID);
                }

                EditorUtility.SetDirty(prefab);
            }
            else
            {
                Debug.LogWarning($"Prefab {prefab.name} has no PoolObject", prefab);
            }
        }

        AssetDatabase.SaveAssets();
        /*var currentScene = SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(currentScene);
        EditorSceneManager.SaveScene(currentScene);*/

        Debug.Log("Prefab UID generation complete.");
    }

    private string GenerateUIDNoCollision(HashSet<string> usedUIDs)
    {
        string result;
        do
        {
            result = GenerateRandomUID();
        }
        while (usedUIDs.Contains(result));
        return result;
    }

    public static string GenerateRandomUID()
    {
        uint randomUInt = (uint)Random.Range(0, (int)Mathf.Pow(10, UID_LENGTH));
        string randomUIntString = randomUInt.ToString();

        while (randomUIntString.Length < UID_LENGTH)
        {
            randomUIntString = "0" + randomUIntString;
        }

        return randomUIntString;
    }

#endif
}

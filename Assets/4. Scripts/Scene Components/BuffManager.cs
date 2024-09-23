using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuffManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool canSpawnBuff = true;
    [SerializeField]
    private float spawnCdr = 0.1f;
    [SerializeField]
    private BoxCollider2D playArea;
    [SerializeField]
    private Transform[] shootingPoints;    

    [Header("Prefabs")]
    [SerializeField]
    private GameObject speedBuffPrefab;
    [SerializeField]
    private GameObject slowDeBuffPrefab;

    [Header("Debugs")]
    [SerializeField]
    private bool isInitialized = false;
    [SerializeField]
    private int speedBuffCount = 0;
    [SerializeField]
    private int slowHazardCount = 0;

    private Bounds bounds;
    private PoolManager poolManager;
    private GameManager gameManager;

    private int currentIndex = 0;

    private HashSet<Vector3> positionCheck = new HashSet<Vector3>();

    public bool IsInitialized => isInitialized;

    private void Start()
    {
        bounds = playArea.bounds;
        poolManager = PoolManager.main;
        gameManager = GameManager.main;
    }

    public void Initialize(NightPreset nightPreset)
    {
        gameManager.everyTenSeconds.AddListener(TriggerEvent);

        speedBuffCount = nightPreset.speedBuff.toggle ? nightPreset.speedBuff.value : 0;
        slowHazardCount = nightPreset.slowHazard.toggle ? nightPreset.slowHazard.value : 0;

        isInitialized = true;
    }

    public void TriggerEvent()
    {
        if (!canSpawnBuff|| !isInitialized || gameManager.IsPaused) return;

        StartCoroutine(EventCoroutine());
    }

    public IEnumerator EventCoroutine()
    {
        if (!poolManager.IsInitialized) yield break;

        for (int i = 0; i < speedBuffCount; i++)
        {
            var candicate = bounds.GetRandomPoint().SnapToGrid();
            while (positionCheck.Contains(candicate))
            {
                candicate = bounds.GetRandomPoint().SnapToGrid();
                yield return null;
            }
            positionCheck.Add(candicate);
            var go = poolManager.Spawn(speedBuffPrefab, candicate);
            go.GetComponent<Effector>().Initialize(shootingPoints[currentIndex].position);
            ChangeShootingPosition();

            yield return new WaitForSeconds(spawnCdr);

        }

        for (int i = 0; i < slowHazardCount; i++)
        {
            var candicate = bounds.GetRandomPoint().SnapToGrid();
            while (positionCheck.Contains(candicate))
            {
                candicate = bounds.GetRandomPoint().SnapToGrid();
                yield return null;
            }
            positionCheck.Add(candicate);
            var go = poolManager.Spawn(slowDeBuffPrefab, candicate);
            go.GetComponent<Effector>().Initialize(shootingPoints[currentIndex].position);
            ChangeShootingPosition();

            yield return new WaitForSeconds(spawnCdr);
        }

        

        positionCheck.Clear();
    }

    private void ChangeShootingPosition()
    {
        currentIndex++;
        if (currentIndex > shootingPoints.Length - 1)
            currentIndex = 0;
    }


}

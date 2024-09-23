using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    private struct DemonTypePrefab
    {
        public DemonType type;
        public GameObject prefab;
    }

    public const int MAX_STAR_COUNT = 5;

    public static GameManager main;

    [Header("General Settings")]
    [SerializeField]
    private bool canLoseStar = true;
    [SerializeField]
    [Range(0, MAX_STAR_COUNT)]
    private int startingStarsCount = 5;
    [SerializeField]
    [Range(0.01f, 5)]
    private float timeScale = 1;
    [SerializeField]
    private GameSave gameSave;

    [Header("Night Settings")]
    [SerializeField]
    private Difficulty difficultyPreset;
    [SerializeField]
    private DemonTypePrefab[] demonTypePrefabs;    

    [Header("Dish Settings")]
    [SerializeField]
    private int dishCapacity = 10;
    [HideInInspector]
    public UnityEvent<int> OnDishCountChange;

    [Header("Transition Settings")]
    [SerializeField]
    private bool skipTransition = false;
    [SerializeField]
    private TransitionUI transitionUI;

    [Header("Required Components")]
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private BuffManager buffManager;
    [SerializeField]
    private ServingQueue[] servingQueues;

    [Header("General Debugs")]
    [SerializeField]
    private bool isPaused;
    [SerializeField]
    private int currentNight = 0;
    [SerializeField]
    private int currentStar = 5;
    [SerializeField]
    private int currentScore = 0;
    [SerializeField]
    private int currentDishCount;

    [Header("Demon Debugs")]
    [SerializeField]
    private int demonSpawn = 0;
    [SerializeField]
    private int demonDespawn = 0;

    private StarsUI starUI;

    private bool isInitialized = false;
    private bool isEndingNight = false;

    private float nextTen;

    private NightPreset currentNightPreset;

    Dictionary<ServingQueue, Dictionary<DemonType, int>> demonInQueues = new Dictionary<ServingQueue, Dictionary<DemonType, int>>();
    Dictionary<DemonType, int> demonPopulation = new Dictionary<DemonType, int>();
    Dictionary<DemonType, int> demonAppearance = new Dictionary<DemonType, int>();
    Dictionary<DemonType, GameObject> demonTypeToPrefab = new Dictionary<DemonType, GameObject>();

    [HideInInspector]
    public UnityEvent everyTenSeconds;

    public bool IsPaused => isPaused;
    public float TimeScale => timeScale;

    public int CurrentStar => currentStar;
    public int CurrentScore => currentScore;
    public int CurrentDishCount => currentDishCount;

    public int DishCapacity => dishCapacity;    

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        // Load from save file
        currentNight = gameSave.currentNight;
        currentScore = gameSave.currentScore;

        // Initialize demonTypeToPrefab
        foreach(var demon in demonTypePrefabs)
        {
            demonTypeToPrefab[demon.type] = demon.prefab;
        }

        // Initialize StartUI
        starUI = StarsUI.main;        

        StartCoroutine(IntroductionSequence());
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (Time.time > nextTen)
        {
            nextTen = Time.time + 10f * 1 / TimeScale;

            everyTenSeconds?.Invoke();
            SpawnDemon();
        }
    }

    private IEnumerator IntroductionSequence()
    {
        // Pause the game until the introduction sequence is completed
        isPaused = true;
        isInitialized = false;

        // Cache the current night
        if (currentNight >= difficultyPreset.nightPresets.Length) currentNight = 0;
        currentNightPreset = difficultyPreset.nightPresets[currentNight];

        // Initialize demonPopulation and demonAppearance
        var demonTypeValues = System.Enum.GetValues(typeof(DemonType));
        foreach (var demonTypeValue in demonTypeValues)
        {
            var demon = (DemonType)demonTypeValue;
            demonPopulation[demon] = 0;
            demonAppearance[demon] = 0;
        }

        // Initialize demonInQueues
        foreach (var servingQueue in servingQueues)
        {
            var queuePopulation = new Dictionary<DemonType, int>();
            foreach (var demonTypeValue in demonTypeValues)
            {
                queuePopulation[(DemonType)demonTypeValue] = 0;
            }
            demonInQueues[servingQueue] = queuePopulation;
        }

        // Reset StartUI
        currentStar = startingStarsCount;
        starUI.UpdateStar(currentStar);

        // Transition
        if (!skipTransition)
        {
            yield return StartCoroutine(transitionUI.IntroCoroutine(currentNightPreset.name));
        }        

        // Show menu
        yield return new WaitUntil(() => RecipeManager.main.IsInitialized);

        // Initialize BuffManager
        buffManager.Initialize(difficultyPreset.nightPresets[currentNight]);

        isInitialized = true;
        isPaused = false;
        playerController.IsMoveable = true;
    }

    #region Spawn Demon

    private void SpawnDemon()
    {
        DemonSpawnSetting candidateSetting;
        if (demonSpawn >= currentNightPreset.numberOfDemon && currentNightPreset.numberOfDemon != 0)
        {
            if (!isEndingNight)
            {
                isEndingNight = true;
                // End of night code go here
            }

            return;
        }

        if (demonSpawn == currentNightPreset.numberOfDemon - 1 && GetDemonTypeByEndOfNight(out candidateSetting))
        {
            AddDemonTypeToQueue(candidateSetting);
        }
        else if (GetDemonSpawnSettingByEveryXDemon(out candidateSetting))
        {
            AddDemonTypeToQueue(candidateSetting);
        }
        else if (GetDemonSpawnSettingBySpawnChance(out candidateSetting))
        {
            AddDemonTypeToQueue(candidateSetting);
        }
        else
        {
            Debug.LogWarning("No demon type was selected, currentNight = " + currentNight);
        }
    }

    private bool GetDemonTypeByEndOfNight(out DemonSpawnSetting candidateSetting)
    {
        candidateSetting = currentNightPreset.demonSpawnSettings[0];

        var potentialSettings = GetPotentialSpawnSettings(currentNightPreset.demonSpawnSettings, DemonSpawnType.EndOfNight);

        // If population is full or no population has a spawn type of DemonSpawnType.EndOfNight, return false
        if (potentialSettings.Count == 0)
            return false;

        // Randomly get a demon
        candidateSetting = potentialSettings[Random.Range(0, potentialSettings.Count)];
        return true;                   
    }
    private bool GetDemonSpawnSettingByEveryXDemon(out DemonSpawnSetting candidateSetting)
    {
        candidateSetting = currentNightPreset.demonSpawnSettings[0];

        var potentialSettings = GetPotentialSpawnSettings(currentNightPreset.demonSpawnSettings, DemonSpawnType.EveryXDemon);

        // If population is full or no population has a spawn type of emonSpawnType.EveryXDemon, return
        if (potentialSettings.Count == 0) 
            return false;

        // Randomly go through the demon types and select one demon from a population that is not full
        // If there are multiple demon that can spawn at X demon, a random demon will be chosen
        while (potentialSettings.Count > 0)
        {
            var spawnSetting = potentialSettings[Random.Range(0, potentialSettings.Count)];
            if (demonSpawn != 0 && ((demonSpawn + 1) % spawnSetting.spawnEveryXDemon) == 0)
            {
                candidateSetting = spawnSetting;
                return true;
            }
            else
            {
                potentialSettings.Remove(spawnSetting);
            }
        }

        return false;
    }

    private bool GetDemonSpawnSettingBySpawnChance(out DemonSpawnSetting candidateSetting)
    {
        candidateSetting = currentNightPreset.demonSpawnSettings[0];
        var foundType = false;

        var potentialSettings = GetPotentialSpawnSettings(currentNightPreset.demonSpawnSettings, DemonSpawnType.SpawnChance);

        // If population is full or no population has a spawn type of spawnChance, return
        if (potentialSettings.Count == 0) 
            return false;

        // Randomly go through the demon types and select one demon from a population that is not full
        while (potentialSettings.Count > 0)
        {
            var spawnSetting = potentialSettings[Random.Range(0, potentialSettings.Count)];

            if (Random.value < spawnSetting.spawnChance)
            {
                candidateSetting = spawnSetting;             
                foundType = true;
                break;
            }
            else
            {
                potentialSettings.Remove(spawnSetting);
            }
        }

        // If the randomizer can't select a demon, select the first available demon from the population
        // Guarantee at least one type of demon will be selected if the population is not full
        if (!foundType)
        {
            potentialSettings = GetPotentialSpawnSettings(currentNightPreset.demonSpawnSettings, DemonSpawnType.SpawnChance);

            var spawnSetting = potentialSettings[Random.Range(0, potentialSettings.Count)];
            candidateSetting = spawnSetting;
            foundType = true;            
        }

        return foundType;
    }

    private List<DemonSpawnSetting> GetPotentialSpawnSettings(DemonSpawnSetting[] settings, DemonSpawnType type)
    {
        var potentialSettings = new List<DemonSpawnSetting>();
        potentialSettings.AddRange(settings);
        potentialSettings.RemoveAll(s => demonAppearance[s.demonType] >= s.maxPerNight && s.maxPerNight != 0);
        potentialSettings.RemoveAll(s => demonPopulation[s.demonType] >= s.maxPerMap && s.maxPerMap != 0);
        potentialSettings.RemoveAll(s => s.demonSpawnType != type);

        return potentialSettings;
    }

    private void AddDemonTypeToQueue(DemonSpawnSetting candidateSetting)
    {
        var candidateType = candidateSetting.demonType;
        ServingQueue candidateQueue = null;
        var foundQueue = false;

        // Get lowestCount in all queues
        var lowestCount = 99;
        for(int i = 0; i < servingQueues.Length; i++)
        {
            if (servingQueues[i].DemonCount < lowestCount)
                lowestCount = servingQueues[i].DemonCount;
        }

        // Get a list of queues that has demonCount lower or equal to lowestCount
        List<ServingQueue> potentialQueues = new List<ServingQueue>();
        potentialQueues.AddRange(servingQueues);
        potentialQueues.RemoveAll(n => n.IsFull);
        potentialQueues.RemoveAll(n => n.DemonCount > lowestCount);

        // Prioritize queue that has low demonCount
        while (potentialQueues.Count > 0)
        {
            // Get a random candidate queue from the potential queues
            candidateQueue = potentialQueues[Random.Range(0, potentialQueues.Count)];

            // Check if the number of canditate demon exceed limit
            if (candidateQueue.DemonCount <= lowestCount && 
                (demonInQueues[candidateQueue][candidateType] < candidateSetting.maxPerQueue || candidateSetting.maxPerQueue == 0))
            {
                foundQueue = true;
                break;
            }
            else
            {
                potentialQueues.Remove(candidateQueue);
                candidateQueue = null;
            }
        }

        if (!foundQueue)
        {
            // Get a list of queues that are not full
            potentialQueues = new List<ServingQueue>();
            potentialQueues.AddRange(servingQueues);
            potentialQueues.RemoveAll(n => n.IsFull);

            while (potentialQueues.Count > 0)
            {
                // Get a random candidate queue from the potential queues
                candidateQueue = potentialQueues[Random.Range(0, potentialQueues.Count)];

                // Check if the number of canditate demon exceed limit
                if (demonInQueues[candidateQueue][candidateType] < candidateSetting.maxPerQueue || candidateSetting.maxPerQueue == 0)
                {
                    foundQueue = true;
                    break;
                }
                else
                {
                    potentialQueues.Remove(candidateQueue);
                    candidateQueue = null;
                }
            }
        }

        if (foundQueue)
        {
            // Add the candidate demon to the candidate queue                
            candidateQueue.AddDemon(demonTypeToPrefab[candidateType]);
            demonAppearance[candidateType]++;
            demonPopulation[candidateType]++;
            demonInQueues[candidateQueue][candidateType]++;
            demonSpawn++;
            //Debug.Log($"candidateQueue = {candidateQueue.name}, candidateType = {candidateType}");
        }
    }

    #endregion

    #region Demon Manager
    public void NotifyRemoveDemon(ServingQueue servingQueue, DemonType demonType)
    {
        demonInQueues[servingQueue][demonType]--;
        demonPopulation[demonType]--;
    }

    public void NotifyDemonDespawn()
    {
        demonDespawn++;

        // Check if the last demon have left the restaurant
        if(demonDespawn == currentNightPreset.numberOfDemon)
        {
            isPaused = true;
            StartCoroutine(transitionUI.NightEndInCoroutine(currentNightPreset.endNightText,
                "Current Score: " + currentScore.ToString("0000"), 
                "Tap anywhere to continue"));
        }
    }

    #endregion

    public void NextNight()
    {
        if(currentNight + 1 < difficultyPreset.nightPresets.Length)
        {
            gameSave.currentNight++;
            gameSave.currentScore = currentScore;
            SceneManager.LoadScene((int)SceneID.Main);
        }
        else
        {
            gameSave.currentNight = 0;
            gameSave.currentScore = 0;
            SceneManager.LoadScene((int)SceneID.Title);
        }
    }

    public void GainPoint(int value)
    {
        currentScore += value;
    }

    #region Gain/Lose Star

    [ContextMenu("Gain 1 Star")]
    public void GainStar()
    {
        currentStar++;
        if(currentStar > 5)
            currentStar = 5;

        starUI.UpdateStar(currentStar);
    }

    [ContextMenu("Lose 1 Star")]
    public void LoseStar()
    {
        if (!canLoseStar) return;

        currentStar--;
        if (currentStar <= 0)
        {
            // Debug.Log("Game Over");
            LeaderboardManager.main.SubmitScore(currentScore);
            SceneManager.LoadScene((int)SceneID.GameOver);
        }
        else
        {
            starUI.UpdateStar(currentStar);
        }
    }

    #endregion    

    #region Pause/Resume Game

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }

    #endregion

    #region Dish handling
    public bool TakeOneDish()
    {
        if (currentDishCount < dishCapacity)
        {
            currentDishCount++;
            OnDishCountChange?.Invoke(currentDishCount);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void WashOneDish()
    {
        if(currentDishCount <= 0)
        {
            currentDishCount = 0;
            return;
        }

        currentDishCount--;
        OnDishCountChange?.Invoke(currentDishCount);
    }
    #endregion

    public void PrintDemonPopulation(Dictionary<DemonType, int> demonPopulation)
    {
        string output = "\n";

        foreach (KeyValuePair<DemonType, int> demon in demonPopulation)
        {
            output += $"Demon Type: {demon.Key}, Population: {demon.Value}\n";
        }

        Debug.Log(output);
    }

    public void ResetGameSave()
    {
        gameSave.Reset();
    }
}

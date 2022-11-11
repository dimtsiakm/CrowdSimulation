using System;
using UnityEngine;

public class CrowdManager : Singleton<CrowdManager>
{
    public GameObject[] humanPrefab;
    public GameObject[] attractiveLocations;

    [SerializeReference] public GameObject entrance;

    private GameObject[] humans;

    [Range(0.5f, 5f)]
    public float periodInstantiate = 1f;

    [Range(10, 1000)]
    public int maxCrowdCapacity = 10;

    [Header("Human Remaining Time (minutes)")]
    [SerializeField]
    private float minRemainingTimeInAttractionMinutes = 10f;
    [SerializeField]
    private float maxRemainingTimeInAttractionMinutes = 15f;

    [Header("Attraction Influence (%)")]
    [SerializeField]
    private float minAttractivenessPerc = 5f;
    [SerializeField]
    private float maxAttractivenessPerc = 25f;

    [Header("Time Scale")]
    public float timeScale = 1f;

    private int crowdCount = 0;

    [Header("Debug options")]
    public bool showLogs = true;
    public Action<int> CrowdChanged;
    public Action<int> TimeChanged;

    /// <summary>
    /// It declares the time as a percentage of a day. E.g., in the morning is 0 and in the night is 1.
    /// </summary>
    public float TimeOfDay 
    {
        get
        {
            return (Time.time - startTime) / (hours * 60 * 60);
        }
    }
    private int timeOClock = -1;
    private float startTime;
    [Header("Hours per Day")]
    public float hours = 12;
    private int daysCount;
    
    private void Awake()
    {
        daysCount = 0;
        Time.timeScale = timeScale;
        ParametersLimitation();
    }
    void Start()
    {
        attractiveLocations = GameObject.FindGameObjectsWithTag("attraction");
        NewDay();
        InvokeRepeating("CreatePeople", 1f, periodInstantiate);
    }
    void NewDay()
    {
        daysCount++;
        Debug.Log("New Day" + daysCount);
        startTime = Time.time;
        foreach(var attr in attractiveLocations)
        {
            attr.GetComponent<Location>().SetInfluence(UnityEngine.Random.Range(minAttractivenessPerc, maxAttractivenessPerc));
        }
    }
    void CreatePeople()
    {
        float minProb = 0.05f;
        float range = Mathf.Cos(Mathf.PI + 2 * Mathf.PI * TimeOfDay);
        float transformed = (range + 1) / 2; // scaling from [-1, 1] to [0, 1]
        
        ///0.05 + normal_distribution * 0.6 => max: 0.605
        float prob = minProb + transformed * 0.6f;
        float randomNumber = UnityEngine.Random.value;

        if(showLogs)
            Debug.Log("Random num: " + randomNumber + ", prob of creating people: " + prob + ", time: " + (Mathf.FloorToInt(TimeOfDay*12)) + ", people count: " + crowdCount);

        if (randomNumber < prob && crowdCount <= maxCrowdCapacity)
        {
            Instantiate(humanPrefab[(int)UnityEngine.Random.Range(0f, humanPrefab.Length - 1)], entrance.transform.position, Quaternion.identity);
            IncreaseCrowdByOne();
        }
    }

    void Update()
    {
        int time = Mathf.FloorToInt(TimeOfDay * 12);
        if (time != timeOClock)
        {
            timeOClock = time;
            TimeChanged?.Invoke(time);
        }
        if (TimeOfDay > 1f)
            NewDay();

    }
    private void ParametersLimitation()
    {
        minRemainingTimeInAttractionMinutes = minRemainingTimeInAttractionMinutes < 0 ? 0 : minRemainingTimeInAttractionMinutes;
        maxRemainingTimeInAttractionMinutes = maxRemainingTimeInAttractionMinutes < 0 ? 0 : maxRemainingTimeInAttractionMinutes;
        minAttractivenessPerc = minAttractivenessPerc < 0 ? 0 : minAttractivenessPerc;
        maxAttractivenessPerc = maxAttractivenessPerc < 0 ? 0 : maxAttractivenessPerc;
    }

    /// <summary>
    /// Get the boundaries of the approved time in Locations.
    /// </summary>
    /// <returns>(min, max) time in seconds.</returns>
    public (float, float) GetMinMaxTimeInAttraction()
    {
        float min = minRemainingTimeInAttractionMinutes * 60 / Time.timeScale;
        float max = maxRemainingTimeInAttractionMinutes * 60 / Time.timeScale;
        return (min, max);
    }

    /// <summary>
    /// Get the attractiveness.
    /// </summary>
    /// <returns>(min, max) attractiveness (%).</returns>
    public (float, float) GetAttractivenessPercentage()
    {
        return (minAttractivenessPerc, maxAttractivenessPerc);
    }

    public void IncreaseCrowdByOne()
    {
        crowdCount++;
        CrowdChanged?.Invoke(crowdCount);
    }
    public void DecreaseCrowdByOne()
    {
        crowdCount--;
        CrowdChanged?.Invoke(crowdCount);
        if (crowdCount < 0)
        {
            throw new System.ArgumentException("Parameter cannot be zero", nameof(crowdCount));
        }
    }
}

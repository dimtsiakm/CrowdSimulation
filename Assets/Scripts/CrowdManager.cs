using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEditor.PlayerSettings;

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

    [HideInInspector]
    public bool isWeekend;

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
    public float hours = 0.1f;
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
        isWeekend = false;
        daysCount++;
        Debug.Log("New Day" + daysCount);
        startTime = Time.time;
        if(daysCount % 4 == 0)
        {
            isWeekend = true;
            Debug.Log("It's a weekend!");
        }
        foreach (var attr in attractiveLocations)
        {
            attr.GetComponent<Location>().SetInfluence(UnityEngine.Random.Range(minAttractivenessPerc, maxAttractivenessPerc));
            if (isWeekend)
                attr.GetComponent<Location>().SetWeekend(true);
        }
    }
    void CreatePeople()
    {
        float maxProb = 0.7f;
        float minProb = 0.1f;
        float value = 0;
        //06:00 to 11:00
        if (TimeOfDay < 0.2f)
            value = IncreaseCos(minProb, maxProb, TimeOfDay / 0.2f);
        //11:00 to 16:00
        else if (TimeOfDay >= 0.2f && TimeOfDay < 0.4f)
            value = DecreaseCos((maxProb / 2), maxProb, (TimeOfDay - 0.2f) / 0.2f);
        //16:00 to 18:00
        else if (TimeOfDay >= 0.4f && TimeOfDay < 0.483f)
            value = IncreaseCos((maxProb / 2), maxProb, (TimeOfDay - 0.4f) / 0.083f);
        //18:00 to 06:00
        else if (TimeOfDay >= 0.483f && TimeOfDay < 1f)
            value = DecreaseCos(minProb, maxProb, (TimeOfDay - 0.483f) / 0.517f);
        else
            Debug.LogError("Something is wrong!");


        //0.05 + normal_distribution * 0.6 => max: 0.605
        float randomNumber = UnityEngine.Random.value;
        
        if(showLogs)
            Debug.Log("Random num: " + randomNumber + ", prob of creating people: " + value + ", time: " + (Mathf.FloorToInt(TimeOfDay*12)) + ", people count: " + crowdCount);
            

        if (randomNumber < value && crowdCount <= maxCrowdCapacity)
        {
            Instantiate(humanPrefab[(int)UnityEngine.Random.Range(0f, humanPrefab.Length - 1)], entrance.transform.position, Quaternion.identity);
            IncreaseCrowdByOne();
        }
    }

    float IncreaseCos(float min, float max, float normalizedTime)
    {
        float value = Mathf.Cos(Mathf.PI + Mathf.PI * normalizedTime);
        
        //scale from one range to another. 
        float newValue = ((value + 1) * (max - min) / 2) + min;
        return newValue;
    }
    float DecreaseCos(float min, float max, float normalizedTime)
    {
        float value = Mathf.Cos(Mathf.PI * normalizedTime);
        //scale from one range to another. 
        float newValue = ((value + 1) * (max - min) / 2) + min;
        return newValue;
    }

    void Update()
    {
        int time = Mathf.FloorToInt(TimeOfDay * 24);
        if (time != timeOClock)
        {
            timeOClock = time;
            TimeChanged?.Invoke(time);
            CheckLocationsAreClosedRoutine();
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

    void CheckLocationsAreClosedRoutine()
    {
        //Debug.Log("CheckLocationsAreClosedRoutine: " + TimeOfDay);
        foreach (var attr in attractiveLocations)
        {
            int isClosed = attr.GetComponent<Location>().checkLocationAvailability(0.008f, TimeOfDay);
            if (isClosed == -1)
            {
                List<GameObject> locations = new List<GameObject>();
                foreach(var go in attractiveLocations)
                {
                    if(attr.GetComponent<Location>().Name == go.GetComponent<Location>().Name && attr != go)
                    {
                        locations.Add(go);
                    }
                }
                foreach (var go in locations)
                {
                    go.GetComponent<Location>().AddInfluence(attr.GetComponent<Location>().maxInfluence / locations.Count);
                    Debug.Log("Location " + go.GetComponent<Location>().Name + " increased their maxInfluence.");
                }
            }
            else if (isClosed == 1)
            {
                List<GameObject> locations = new List<GameObject>();
                foreach (var go in attractiveLocations)
                {
                    if (attr.GetComponent<Location>().Name == go.GetComponent<Location>().Name && attr != go)
                    {
                        locations.Add(go);
                    }
                }
                foreach (var go in locations)
                {
                    go.GetComponent<Location>().DecreaseInfluence(attr.GetComponent<Location>().maxInfluence / locations.Count);
                    Debug.Log("Location " + go.GetComponent<Location>().Name + " decreased their maxInfluence.");
                }
            }
        }
    }
}

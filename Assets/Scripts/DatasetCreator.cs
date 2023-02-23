using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatasetCreator : MonoBehaviour
{
    private Location[] attractions;
    private AIControlFAIR[] crowd;
    private GameObject entrance;
    private GameObject exit;
    
    string path = "Assets/dataset.csv";
    public bool isRecording;

    int rows = 0;
    public int totalRows;

    public int samplingFrequencyMinutes;

    private float nextActionTime = 0.0f;
    private float period;

    public static Action<int> DatasetRecordsChanged;
    
    void Start()
    {
        float frequencyPerHour = 60f / samplingFrequencyMinutes; //If the sampling occurs every half hour this should be 2;
        float totalSamplesPerDay = 24f / (1f / frequencyPerHour);
        float simulatedDayTimeInSecs = CrowdManager.Instance.hours * 60f * 60f;
        period = simulatedDayTimeInSecs / totalSamplesPerDay;

        attractions = FindObjectsOfType<Location>();
        entrance = GameObject.FindGameObjectWithTag("entrance");
        exit = GameObject.FindGameObjectWithTag("exit");
        SetHead();
    }
    private void SetHead()
    {
        string head = "entr,exit,";
        int numIncr = 0;
        foreach (Location attr in attractions)
        {
            head += "attr" + numIncr + ",currInfl" + numIncr + ",";
            numIncr++;
        }
        //head += "timestamp";
        head += "dayTimeNormalized,";
        head += "weekend";
        head += "\n";
        File.AppendAllText(path, head);
    }

    private void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            GetRecord();
        }
    }

    void GetRecord()
    {
        if (isRecording)
        {
            Debug.Log("Record!");
            crowd = FindObjectsOfType<AIControlFAIR>();
            string data = "";

            int entranceNum = 0;
            int exitNum = 0;
            foreach (AIControlFAIR human in crowd)
            {
                if (Vector3.Distance(human.transform.position, entrance.transform.position) < 4f)
                {
                    entranceNum++;
                }
                if (Vector3.Distance(human.transform.position, exit.transform.position) < 4f)
                {
                    exitNum++;
                }
            }
            data += entranceNum.ToString() + "," + exitNum.ToString() + ",";
            //for each attraction, calculate the number of people who are there and their currect influence.
            foreach (Location attr in attractions)
            {
                int crowdNumberInAttraction = 0;
                float currInfl = 0;
                foreach (AIControlFAIR human in crowd)
                {
                    if (Vector3.Distance(human.transform.position, attr.transform.position) < 2.5f)
                    {
                        crowdNumberInAttraction++;
                    }
                }
                currInfl = attr.CalculateInfluence(CrowdManager.Instance.TimeOfDay);
                data += crowdNumberInAttraction.ToString() + "," + currInfl.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
            }
            //data += DateTime.Now.ToString();
            float t = CrowdManager.Instance.TimeOfDay;
            data += t.ToString("0.00000", System.Globalization.CultureInfo.InvariantCulture) + ",";
            data += CrowdManager.Instance.isWeekend ? 1 : 0;
            data += "\n";
            rows++;
            DatasetRecordsChanged?.Invoke(rows);

            File.AppendAllText(path, data);

            // Stop the application when the total number of data is recorded.
            if (rows > totalRows)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }
}

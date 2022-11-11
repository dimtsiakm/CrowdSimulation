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

    public int samplingFrequencyMinutes = 5;
    private int frequencySeconds;
    
    void Start()
    {
        frequencySeconds = samplingFrequencyMinutes * 60;
        attractions = FindObjectsOfType<Location>();
        entrance = GameObject.FindGameObjectWithTag("entrance");
        exit = GameObject.FindGameObjectWithTag("exit");
        SetHead();
        InvokeRepeating("GetRecord", 1f, frequencySeconds);

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
        head += "timestamp";
        head += "\n";
        File.AppendAllText(path, head);
    }
    private void GetRecord()
    {
        if (!isRecording)
            return;
        Debug.Log("GetRecord " + rows);
        crowd = FindObjectsOfType<AIControlFAIR>();
        string data = "";

        int entranceNum = 0;
        int exitNum = 0;
        foreach (AIControlFAIR human in crowd)
        {
            if (Vector3.Distance(human.transform.position, entrance.transform.position) < 10f)
            {
                entranceNum++;
            }
            if (Vector3.Distance(human.transform.position, exit.transform.position) < 10f)
            {
                exitNum++;
            }
        }
        data += entranceNum.ToString() + "," + exitNum.ToString() + ",";
        //for each attraction, calculate the number of people who are there and the currect influence.
        foreach (Location attr in attractions)
        {
            int crowdNumberInAttraction = 0;
            float currInfl = 0;
            foreach (AIControlFAIR human in crowd)
            {
                if (Vector3.Distance(human.transform.position, attr.transform.position) < 8f)
                {
                    crowdNumberInAttraction++;
                }
            }
            currInfl = attr.CalculateInfluence(CrowdManager.Instance.TimeOfDay);
            data += crowdNumberInAttraction.ToString() + "," + currInfl.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
        }
        data += DateTime.Now.ToString();
        data += "\n";
        rows++;
        File.AppendAllText(path, data);
        if (rows > totalRows)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}

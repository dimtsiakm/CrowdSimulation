using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeField;
    [SerializeField] TextMeshProUGUI crowdNumberField;
    [SerializeField] TextMeshProUGUI dataRecordsField;

    private void Start()
    {
        CrowdManager.Instance.CrowdChanged += ChangeCrowdNumber;
        CrowdManager.Instance.TimeChanged += ChangeTime;
        DatasetCreator.DatasetRecordsChanged += ChangeDatasetRecords;

        dataRecordsField.text = "Dataset Records: NO";
    }
    void ChangeTime(int time)
    {
        time += 8;
        string timeString = time.ToString();
        if (time < 10)
            timeString = "0" + time;
        timeField.text = "Time: " + timeString + ":00";
    }
    void ChangeCrowdNumber(int number)
    {
        crowdNumberField.text = "People Count: " + number;
    }
    void ChangeDatasetRecords(int number)
    {
        dataRecordsField.text = "Dataset Records: " + number;
    }

}

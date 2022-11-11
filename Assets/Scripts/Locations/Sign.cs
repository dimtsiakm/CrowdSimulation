using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Location))]
public class Sign : MonoBehaviour
{
    public Location location;

    private void Start()
    {
        gameObject.GetComponent<TextMeshPro>().text = location.Name;
    }
}

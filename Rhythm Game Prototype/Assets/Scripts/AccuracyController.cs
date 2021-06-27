using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyController : MonoBehaviour
{
    public float displayTime = .15f;

    // Update is called once per frame
    void Update()
    {
        displayTime -= Time.deltaTime;
        if(displayTime < 0)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}

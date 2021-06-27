using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyController : MonoBehaviour
{
    public float displayTime = .15f;
    private float removeTime = 0;

    private void Start()
    {
        this.removeTime = displayTime;
    }

    // Update is called once per frame
    void Update()
    {
        removeTime -= Time.deltaTime;
        float s = 1 + (1 - (removeTime / displayTime)) * .25f;
        this.transform.localScale = new Vector2(s, s);
        if(removeTime < 0)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}

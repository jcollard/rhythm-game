using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyHelper : MonoBehaviour
{
    public AccuracyController Good;
    public AccuracyController Great;
    public AccuracyController Perfect;
    public AccuracyController Miss;

    public readonly Dictionary<HitType, AccuracyController> accuracy = new Dictionary<HitType, AccuracyController>();

    public void Start()
    {
        accuracy.Add(HitType.Perfect, Perfect);
        accuracy.Add(HitType.Good, Good);
        accuracy.Add(HitType.Great, Great);
        accuracy.Add(HitType.Miss, Miss);
        Good.gameObject.SetActive(false);
        Great.gameObject.SetActive(false);
        Perfect.gameObject.SetActive(false);
        Miss.gameObject.SetActive(false);
    }
}

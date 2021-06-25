using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{

    public GameObject gameObjectRef;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public float startTime;
    public float endTime;
    public BeatMapper beatMapper;
    public Tuple<Note, Beat> model;

    void Update()
    {
        if (beatMapper == null)
        {
            return;
        }

        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);
        float rotation = 360f * percentage;
        
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.position = Vector2.LerpUnclamped(startPosition, endPosition, percentage);

        if (percentage > 1.5 || percentage < 0)
        {
            beatMapper.removeNote(model, this);
        }
    }

}


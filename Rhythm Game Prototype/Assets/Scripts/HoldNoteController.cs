﻿using System;
using UnityEngine;

public class HoldNoteController : NoteController
{

    public GameObject startObject;
    public GameObject endObject;

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

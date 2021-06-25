using System;
using UnityEngine;

/// <summary>
/// A HoldNoteController is a View for a HoldNote, Beat model.
/// </summary>
public class HoldNoteController : NoteController
{

    public GameObject startObject;
    public GameObject endObject;
    public float secondStartTime;
    public float secondEndTime;

    void Update()
    {
        if (beatMapper == null)
        {
            return;
        }

        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);
        float percentage2 = (beatMapper.songPosition - secondStartTime) / (secondEndTime - secondStartTime);

        startObject.transform.position = Vector2.Lerp(startPosition, endPosition, percentage);
        endObject.transform.position = Vector2.Lerp(startPosition, endPosition, percentage2);

        if (percentage2 > 1 || percentage < 0)
        {
            beatMapper.removeNoteController(model);
        }
    }
}

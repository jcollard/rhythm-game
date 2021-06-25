using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A DoubleNoteController is a View for a Double, Beat model.
/// </summary>
public class DoubleNoteController : NoteController
{
    public GameObject note0;
    public GameObject note1;

    public Vector2 endPosition2;

    void Update()
    {
        if (beatMapper == null)
        {
            return;
        }

        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);

        note0.transform.position = Vector2.LerpUnclamped(startPosition, endPosition, percentage);
        note1.transform.position = Vector2.LerpUnclamped(startPosition, endPosition2, percentage);

        if (percentage > 1.5 || percentage < 0)
        {
            beatMapper.removeNoteController(model);
        }
    }
}

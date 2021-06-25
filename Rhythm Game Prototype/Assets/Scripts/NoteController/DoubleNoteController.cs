using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleNoteController : NoteController
{
    public GameObject note0;
    public GameObject note1;

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

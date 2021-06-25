using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScratchNoteController is a View for a ScratchNote, Beat model.
/// </summary>
public class ScratchNoteController : NoteController
{
    public Vector2 startScale = new Vector2(0.25f, 0.25f);
    public Vector2 endScale = new Vector2(3.25f, 3.25f);

    void Update()
    {
        if (beatMapper == null)
        {
            return;
        }

        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);
        float rotation = -90f * percentage;

        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.position = startPosition;
        transform.localScale = Vector2.LerpUnclamped(startScale, endScale, percentage);

        if (percentage > 1.5 || percentage < 0)
        {
            beatMapper.removeNoteController(model);
        }
    }

}


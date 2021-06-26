using System;
using UnityEngine;

/// <summary>
/// A HoldNoteController is a View for a HoldNote, Beat model.
/// </summary>
public class HoldNoteController : NoteController
{

    public GameObject startObject;
    public GameObject endObject;
    private float secondStartTime;
    private float secondEndTime;

    protected override void DoCalculateStartAndEndTime()
    {
        base.DoCalculateStartAndEndTime();
        HoldNote n = (HoldNote)model.Item1;
        Beat b = model.Item2;
        secondStartTime = beatMapper.beatMap.CursorPositionToSeconds(b.position + n.duration - BeatMap.BEAT * beatMapper.beatsVisible);
        secondEndTime = beatMapper.beatMap.CursorPositionToSeconds(b.position + n.duration);
    }

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

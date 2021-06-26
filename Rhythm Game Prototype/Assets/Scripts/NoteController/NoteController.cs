using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A NoteController is the displayed portion of a Note, Beat combination.
/// <br/>
/// This class keeps track of Normal Notes.
/// </summary>
public class NoteController : MonoBehaviour
{
    //TODO: calculate startTime / endTime based on model?
    public Vector2 startPosition;
    public Vector2 endPosition;
    protected float startTime;
    protected float endTime;
    private int bpm = -1;
    public BeatMapper beatMapper;
    public Tuple<Note, Beat> model;

    public void Start()
    {
        CalculateStartAndEndTime();
    }

    public void CalculateStartAndEndTime()
    {
        if (beatMapper == null || bpm == beatMapper.beatMap.getBPM())
        {
            return;
        }
        DoCalculateStartAndEndTime();
        bpm = beatMapper.beatMap.getBPM();
    }

    protected virtual void DoCalculateStartAndEndTime()
    {
        Beat b = model.Item2;
        startTime = beatMapper.beatMap.CursorPositionToSeconds(b.position - BeatMap.BEAT * beatMapper.beatsVisible);
        endTime = beatMapper.beatMap.CursorPositionToSeconds(b.position);
    }

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
            beatMapper.removeNoteController(model);
        }
    }

}


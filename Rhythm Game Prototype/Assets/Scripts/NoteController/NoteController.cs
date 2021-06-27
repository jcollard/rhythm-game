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
    public HitType isHit = HitType.Null;
    private AccuracyController ac = null;
    private bool drawHit = true;

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

    public virtual void Reset()
    {
        isHit = HitType.Null;
        drawHit = true;
    }

    public virtual void CheckHit(Dictionary<NoteInput, long> inputs)
    {
        Note n = model.Item1;
        Beat b = model.Item2;
        long currPosition = beatMapper.beatMap.getCursor();
        int withinTolerance = 249;
        bool checkMiss = currPosition > b.position;
        long diff = Math.Abs(currPosition - b.position);
        if (diff < withinTolerance)
        {

            if (inputs.ContainsKey(n.input))
            {
                long pressedAt = inputs[n.input];
                float accuracy = (withinTolerance - Math.Abs(b.position - pressedAt)) / (float)withinTolerance;
                if (accuracy > 0)
                {
                    HitType hit = HitType.Null;
                    if (accuracy > 0.75)
                    {
                        hit = HitType.Perfect;
                    }
                    else if (accuracy > 0.50)
                    {
                        hit = HitType.Great;
                    }
                    else
                    {
                        hit = HitType.Good;
                    }
                    isHit = hit;
                }
            }
        }
        if (checkMiss && diff > withinTolerance)
        {
            isHit = HitType.Miss;
        }
    }

    protected virtual void Render()
    {
        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);
        float rotation = 360f * percentage;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.position = Vector2.LerpUnclamped(startPosition, endPosition, percentage);
    }

    protected virtual bool CheckRemove()
    {
        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);
        return percentage > 1.5 || percentage < 0;
    }

    protected virtual AccuracyController RenderHit()
    {
        AccuracyController ac = UnityEngine.Object.Instantiate<AccuracyController>(beatMapper.accuracyHelper.accuracy[isHit]);
        ac.transform.position = endPosition;
        ac.transform.parent = beatMapper.accuracyHelper.transform;
        ac.gameObject.SetActive(true);
        return ac;
    }

    void Update()
    {
        if (beatMapper == null)
        {
            return;
        }

        if (isHit == HitType.Null || isHit == HitType.Miss)
        {
            Render();
        } else
        {
            transform.position = new Vector2(-20, -20);
        }

        //TODO: drawHit feels hacky
        //print("isHit: " + isHit + ", ac: " + ac + ", drawHit: " + drawHit);
        if (isHit != HitType.Null && ac == null && drawHit)
        {
            this.ac = RenderHit();
            drawHit = false;
        }

        if (CheckRemove())
        {
            beatMapper.removeNoteController(model);
        }
    }

}


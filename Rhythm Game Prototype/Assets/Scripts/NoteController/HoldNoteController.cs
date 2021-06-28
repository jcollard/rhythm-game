using System;
using System.Collections.Generic;
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

    protected HitType isFirstHit = HitType.Null;
    protected AccuracyController firstAC = null;
    protected bool firstDrawHit = true;

    protected override void DoCalculateStartAndEndTime()
    {
        base.DoCalculateStartAndEndTime();
        HoldNote n = (HoldNote)model.Item1;
        Beat b = model.Item2;
        secondStartTime = beatMapper.beatMap.CursorPositionToSeconds(b.position + n.duration - BeatMap.BEAT * beatMapper.beatsVisible);
        secondEndTime = beatMapper.beatMap.CursorPositionToSeconds(b.position + n.duration);
    }

    protected override bool CheckRemove()
    {
        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);
        float percentage2 = (beatMapper.songPosition - secondStartTime) / (secondEndTime - secondStartTime);
        return percentage2 > 1.25 || percentage < 0;
    }

    protected override void Render()
    {
        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);
        float percentage2 = (beatMapper.songPosition - secondStartTime) / (secondEndTime - secondStartTime);
        startObject.transform.position = Vector2.Lerp(startPosition, endPosition, percentage);
        endObject.transform.position = Vector2.Lerp(startPosition, endPosition, percentage2);
    }

    protected override AccuracyController RenderHit()
    {
        AccuracyController ac1 = UnityEngine.Object.Instantiate<AccuracyController>(beatMapper.accuracyHelper.accuracy[isFirstHit]);
        ac1.transform.position = endPosition;
        ac1.transform.parent = beatMapper.accuracyHelper.transform;
        ac1.gameObject.SetActive(true);
        return ac1;
    }

    private AccuracyController RenderHit2()
    {
        AccuracyController ac1 = UnityEngine.Object.Instantiate<AccuracyController>(beatMapper.accuracyHelper.accuracy[isHit]);
        ac1.transform.position = endPosition;
        ac1.transform.parent = beatMapper.accuracyHelper.transform;
        ac1.gameObject.SetActive(true);
        return ac1;
    }

    public override void CheckHit(Dictionary<NoteInput, long> inputs)
    {
        
        HoldNote n = (HoldNote)model.Item1;
        Beat b = model.Item2;
        long currPosition = beatMapper.beatMap.getCursor();
        int withinTolerance = 249;

        // Check Initial Hit
        bool checkMiss = currPosition > b.position;
        long diff = Math.Abs(currPosition - b.position);
        if (isFirstHit == HitType.Null)
        {
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
                        isFirstHit = hit;
                    }
                }
            }
            if (checkMiss && diff > withinTolerance)
            {
                isFirstHit = HitType.Miss;
            }
            return;
        }

        //Check Hold
        if(isFirstHit == HitType.Miss)
        {
            isHit = HitType.Miss;
            return;
        }

        long endAt = n.duration + b.position;
        if(currPosition >= b.position && currPosition <= endAt + withinTolerance)
        {
            if (!inputs.ContainsKey(n.input))
            { 
                long releasedAt = currPosition;
                float accuracy = (withinTolerance - Math.Abs(endAt - releasedAt)) / (float)withinTolerance;
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
                else
                {
                    isHit = HitType.Miss;
                }
            }
        }

        // Was not released in time
        if(currPosition > endAt + withinTolerance)
        {
            isHit = HitType.Miss;
        }

    }

    void Update()
    {
        if (beatMapper == null)
        {
            return;
        }

        if (isFirstHit != HitType.Miss && isHit != HitType.Miss)
        {
            Render();
        }
        else
        {
            transform.position = new Vector2(-20, -20);
        }

        //TODO: drawHit feels hacky
        //print("isHit: " + isHit + ", ac: " + ac + ", drawHit: " + drawHit);
        if (isFirstHit != HitType.Null && firstAC == null && firstDrawHit)
        {
            this.firstAC = RenderHit();
            firstDrawHit = false;
        }

        if (isHit != HitType.Null && ac == null && drawHit)
        {
            this.firstAC = RenderHit2();
            drawHit = false;
        }

        if (CheckRemove())
        {
            beatMapper.removeNoteController(model);
        }
    }
}

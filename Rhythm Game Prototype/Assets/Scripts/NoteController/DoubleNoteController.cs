using System;
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

    protected override void Render()
    {
        float percentage = (beatMapper.songPosition - startTime) / (endTime - startTime);

        note0.transform.position = Vector2.LerpUnclamped(startPosition, endPosition, percentage);
        note1.transform.position = Vector2.LerpUnclamped(startPosition, endPosition2, percentage);
    }

    protected override AccuracyController RenderHit()
    {
        AccuracyController ac0 = UnityEngine.Object.Instantiate<AccuracyController>(beatMapper.accuracyHelper.accuracy[isHit]);
        ac0.transform.position = endPosition;
        ac0.transform.parent = beatMapper.accuracyHelper.transform;
        AccuracyController ac1 = UnityEngine.Object.Instantiate<AccuracyController>(beatMapper.accuracyHelper.accuracy[isHit]);
        ac1.transform.position = endPosition2;
        ac1.transform.parent = beatMapper.accuracyHelper.transform;
        ac0.gameObject.SetActive(true);
        ac1.gameObject.SetActive(true);
        return ac0;
    }

    public override void CheckHit(Dictionary<NoteInput, long> inputs)
    {
        DoubleNote n = (DoubleNote)model.Item1;
        Beat b = model.Item2;
        long currPosition = beatMapper.beatMap.getCursor();
        int withinTolerance = 249;
        bool checkMiss = currPosition > b.position;
        long diff = Math.Abs(currPosition - b.position);
        if (diff < withinTolerance)
        {

            if (inputs.ContainsKey(n.input) && inputs.ContainsKey(n.input2))
            {
                long pressedAt0 = inputs[n.input];
                long pressedAt1 = inputs[n.input2];
                float accuracy0 = (withinTolerance - Math.Abs(b.position - pressedAt0)) / (float)withinTolerance;
                float accuracy1 = (withinTolerance - Math.Abs(b.position - pressedAt0)) / (float)withinTolerance;
                // Take the worst of the two accuracies
                float accuracy = Math.Min(accuracy0, accuracy1);
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
}

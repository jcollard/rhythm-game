using System;
using System.Collections.Generic;
using UnityEngine;

public class BeatSync: MonoBehaviour
{

    private readonly BeatMap _beatMap;

    private readonly Dictionary<DoubleRange, List<Tuple<double, long>>> _beatSynch = new Dictionary<DoubleRange, List<Tuple<double, long>>>();

   

    public BeatSync(BeatMap beatMap)
    {
        _beatMap = beatMap;
        SetBeatAtSecond(0, 0);
    }

    public long SecondsToBeat(double second, Tuple<double, long> before, Tuple<double, long> after)
    {
        double totalSeconds = after.Item1 - before.Item1;
        long totalBeats = after.Item2 - before.Item2;
        double distanceFromBeginning = second - before.Item1;
        double percentFromFirstBeat = distanceFromBeginning / totalSeconds;
        return (long)(totalBeats * percentFromFirstBeat) + before.Item2;
    }

    private long SecondsToBeat(double second, List<Tuple<double, long>> lookup)
    {
        return 0;
    }

    public List<Tuple<double, long>> GetTuplesNearSecond(double second)
    {
        DoubleRange before = new DoubleRange(0, 1);
        DoubleRange current = new DoubleRange((int)Math.Floor(second), (int)Math.Ceiling(second));
        DoubleRange after = new DoubleRange(Int32.MaxValue, Int32.MaxValue);
        foreach(DoubleRange range in _beatSynch.Keys)
        {
            print(range);
            //TODO: Check if range is less than current
            //      if it is, if it is greater than before, update before
            if(range.MIN < current.MIN && range.MIN > before.MIN)
            {
                before = range;
            }


            //TODO: Check if range is greater than current
            //      if it is, if it is less than after, update after

            if (range.MIN > current.MIN && range.MIN < after.MIN)
            {
                after = range;
            }
        }

        List<Tuple<double, long>> result = new List<Tuple<double, long>>();

        //Edge Case: No Range Before (immediately before)

        //Edge Case: No Range at this second

        if (_beatSynch.ContainsKey(before))
        {
            result.AddRange(_beatSynch[before]);
        }

        if (_beatSynch.ContainsKey(current))
        {
            result.AddRange(_beatSynch[current]);
        }

        if (_beatSynch.ContainsKey(after))
        {
            result.AddRange(_beatSynch[after]);
        }

        //Edge Case: No Range After (immediately after)
        //Simplest solution, get ALL tuples and put them in a list
        return result;
    }

    /// <summary>
    /// Given a time in seconds, return the nearest Beat value
    /// </summary>
    /// <param name="second">The time in seconds to calculate</param>
    /// <returns>the nearest Beat valu</returns>
    public long SecondsToBeat(double second)
    {
        //TODO: ensure second is > 0
        DoubleRange secondRange = new DoubleRange((int)Math.Floor(second), (int)Math.Ceiling(second));
        //TODO: If the secondRange is not in _beatSynch, we need to find the range that comes before and after it.
        if (!_beatSynch.ContainsKey(secondRange))
        {
            // GetRange Before and After this one
        }
        if (_beatSynch.ContainsKey(secondRange))
        {
            List<Tuple<double, long>> beatRange = _beatSynch[secondRange];

            Tuple<double, long> closestBefore = null;
            Tuple<double, long> closestAfter = null;

            if (beatRange[0].Item1 > second) {
                //TODO: The last beat in the previous DoubleRange, is the closestBefore
            }
            //TODO: Loop through and find closestBefore / closestAfter
            foreach(Tuple<double, long> tuple in beatRange) {
                if(closestBefore == null) {
                    //TODO: Check if the current double is less than second, if so, update closestBefore
                    if(tuple.Item1 < second)
                    {
                        closestBefore = tuple;
                    }
                } else if (closestAfter == null)
                {
                    //TODO: Check if the current double is greater than second, if so, update closestAfter
                    if (tuple.Item1 > second)
                    {
                        closestAfter = tuple;
                    }
                } else
                {
                    break;
                }
            }

            //TODO: If we get through the loop and closestBefore is not found, then closetBefore = <0.0, 0> (maybe solved by constructor)
            if(closestBefore == null)
            {
                closestBefore = new Tuple<double, long>(0, 0);
            }

            //TODO: If we loop through everything and closestAfter is not found closestAfter must be first in next DoubleRange
            // If we are at the end of the song and there is no next Double Range, what do we do?
            // We could interpret it using the last 2 entries in the entire beatSynch
            if(closestAfter == null)
            {
                DoubleRange nextRange = new DoubleRange((int)Math.Floor(second) + 1, (int)Math.Ceiling(second) + 1);
                if(nextRange != null)
                {
                    List<Tuple<double, long>> bextBeatRange = _beatSynch[nextRange];
                    closestAfter = bextBeatRange[0];
                }
                else
                {
                    closestAfter = beatRange[beatRange.Count - 1];
                }
               
            }

            double totalSeconds = closestAfter.Item1 - closestBefore.Item1;
            long totalBeats = closestAfter.Item2 - closestBefore.Item2;
            double distanceFromBeginning = second - closestBefore.Item1;
            double percentFromFirstBeat = distanceFromBeginning / totalSeconds;
            return (long)(totalBeats * percentFromFirstBeat) + closestBefore.Item2;
        }
        else
        {
            throw new Exception("Second not in BeatSync range " + second);
        }
    }

    /// <summary>
    /// Given a beat value, return the time in seconds
    /// </summary>
    /// <param name="beat"></param>
    /// <returns></returns>
    public double BeatToSeconds(long beat) {
        return 0;
    }

    public void SetBeatAtSecond(long beat, double second)
    {
        //TODO: Convert second to a DoubleRange
        DoubleRange secondRange = new DoubleRange((int)Math.Floor(second), (int)Math.Ceiling(second));
        //TODO: If the DoubleRange is not present in _beatSynch, we need to initialize it
        if (!_beatSynch.ContainsKey(secondRange))
        {
            _beatSynch[secondRange] = new List<Tuple<double, long>>();
        }
        //TODO: Create a tuple from second to beat
        Tuple<double, long> tuple = new Tuple<double, long>(second, beat);
        //TODO: Add the Tuple to the list in _beatSynch
        _beatSynch[secondRange].Add(tuple);
    }


    


}

public class DoubleRange
{
    public readonly int MIN;
    public readonly int MAX;

    public DoubleRange(int min, int max)
    {
        if (min == max)
        {
            max = min + 1;
        }
        this.MIN = min;
        this.MAX = max;
    }

    public override bool Equals(object obj)
    {
        return obj is DoubleRange range &&
               MIN == range.MIN &&
               MAX == range.MAX;
    }

    public override int GetHashCode()
    {
        int hashCode = -745437496;
        hashCode = hashCode * -1521134295 + MIN.GetHashCode();
        hashCode = hashCode * -1521134295 + MAX.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return "DoubleRange(" + MIN + ", " + MAX + ")";
    }
}

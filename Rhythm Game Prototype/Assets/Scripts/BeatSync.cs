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

    public long SecondsToBeat(double second, List<Tuple<double, long>> lookup)
    {
        Tuple<double, long> before = new Tuple<double, long>(Double.NegativeInfinity, long.MinValue);
        double secondCurrent = second;
        Tuple<double, long> after = new Tuple<double, long>(Double.PositiveInfinity, long.MaxValue);

        lookup.ForEach((Tuple<double, long> item) =>
        {
            if(item.Item1 <= secondCurrent && item.Item1 > before.Item1)
            {
                before = item;
            }


            if (item.Item1 > secondCurrent && item.Item1 < after.Item1)
            {
                after = item;
            }

        });

        return CalculateSecondsToBeat(second, before, after);
    }

    public long CalculateSecondsToBeat(double second, Tuple<double, long> before, Tuple<double, long> after)
    {
        double totalSeconds = after.Item1 - before.Item1;
        long totalBeats = after.Item2 - before.Item2;
        double distanceFromBeginning = second - before.Item1;
        double percentFromFirstBeat = distanceFromBeginning / totalSeconds;
        return (long)(totalBeats * percentFromFirstBeat) + before.Item2;
    }

    public List<Tuple<double, long>> GetTuplesNearSecond(double second)
    {
        DoubleRange before = new DoubleRange(0, 1);
        DoubleRange current = new DoubleRange((int)Math.Floor(second), (int)Math.Ceiling(second));
        DoubleRange after = new DoubleRange(Int32.MaxValue, Int32.MaxValue);
        foreach(DoubleRange range in _beatSynch.Keys)
        {
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
        List<Tuple<double, long>> list = GetTuplesNearSecond(second);
        long beat = SecondsToBeat(second, list);
        return beat;
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


    public override string ToString()
    {
        int totalBeats = 0;
        foreach(List<Tuple<double, long>> ls in _beatSynch.Values)
        {
            totalBeats += ls.Count;
        }

        return "BeatSync: Seconds - " + _beatSynch.Count + " , Beats - " + totalBeats;
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

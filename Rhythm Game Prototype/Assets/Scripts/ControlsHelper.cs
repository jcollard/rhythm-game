using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsHelper : MonoBehaviour
{
    public Button prevBeat;
    public Button prevHalfBeat;
    public Button prevQuarterBeat;
    public Button nextBeat;
    public Button nextHalfBeat;
    public Button nextQuarterBeat;
    public BeatMapper beatMapper;

    void Start()
    {
        prevBeat.onClick.AddListener(() => { print("prevBeat"); beatMapper.beatMap.prevBeat(); });
        prevHalfBeat.onClick.AddListener(() => beatMapper.beatMap.prevHalf());
        prevQuarterBeat.onClick.AddListener(() => beatMapper.beatMap.prevQuarter());

        nextBeat.onClick.AddListener(() => { print("nextBeat"); beatMapper.beatMap.nextBeat(); });
        nextHalfBeat.onClick.AddListener(() => beatMapper.beatMap.nextHalf());
        nextQuarterBeat.onClick.AddListener(() => beatMapper.beatMap.nextQuarter());
    }
}

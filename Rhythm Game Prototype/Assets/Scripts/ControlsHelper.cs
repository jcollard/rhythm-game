using System;
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

    public Button playButton;
    public Button stopButton;
    public Button pauseButton;

    public InputField currentPosition;
    public BeatMapper beatMapper;

    void Start()
    {
        prevBeat.onClick.AddListener(() => beatMapper.beatMap.prevBeat());
        prevHalfBeat.onClick.AddListener(() => beatMapper.beatMap.prevHalf());
        prevQuarterBeat.onClick.AddListener(() => beatMapper.beatMap.prevQuarter());

        nextBeat.onClick.AddListener(() => beatMapper.beatMap.nextBeat());
        nextHalfBeat.onClick.AddListener(() => beatMapper.beatMap.nextHalf());
        nextQuarterBeat.onClick.AddListener(() => beatMapper.beatMap.nextQuarter());

        currentPosition.onEndEdit.AddListener((String newCursor) =>
        {
            float nC = float.Parse(newCursor);
            beatMapper.beatMap.setCursor((long)(nC * BeatMap.BEAT));
        });

        playButton.onClick.AddListener(() => beatMapper.isPlaying = true);
        pauseButton.onClick.AddListener(() => beatMapper.isPlaying = false);
        stopButton.onClick.AddListener(() =>
        {
            beatMapper.isPlaying = false;
            beatMapper.beatMap.setCursor(0);
        });


    }
}

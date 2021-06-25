using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsHelper : MonoBehaviour
{
    [Header("Position Buttons")]
    public InputField currentPosition;
    public Button prevBeat;
    public Button prevHalfBeat;
    public Button prevQuarterBeat;
    public Button nextBeat;
    public Button nextHalfBeat;
    public Button nextQuarterBeat;

    [Header("Play Controls")]
    public Button playButton;
    public Button stopButton;
    public Button pauseButton;

    [Header("Add Controls")]
    public Button upButton;
    public Button leftButton;
    public Button downButton;
    public Button triangleButton;
    public Button circleButton;
    public Button xButton;

    [Header("Factory Controls")]
    public Button nextFactory;
    public Button prevFactory;

    [Header("Beat Mapper")]
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


        upButton.onClick.AddListener(() => beatMapper.addNote(NoteInput.Up));
        leftButton.onClick.AddListener(() => beatMapper.addNote(NoteInput.Left));
        downButton.onClick.AddListener(() => beatMapper.addNote(NoteInput.Down));
        triangleButton.onClick.AddListener(() => beatMapper.addNote(NoteInput.Triangle));
        circleButton.onClick.AddListener(() => beatMapper.addNote(NoteInput.Circle));
        xButton.onClick.AddListener(() => beatMapper.addNote(NoteInput.X));

        nextFactory.onClick.AddListener(() => beatMapper.nextFactory());
        prevFactory.onClick.AddListener(() => beatMapper.prevFactory());
    }
}

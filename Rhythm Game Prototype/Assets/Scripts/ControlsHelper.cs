using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// A Helper Class for specifying how UI controls work.
/// </summary>
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
    public Button setTrackButton;
    public AudioSource trackSource;

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

    [Header("Save/Load")]
    public Button saveButton;
    public Button loadButton;

    [Header("Beat Mapper")]
    public BeatMapper beatMapper;
    public InputField bpm;

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
            beatMapper.beatMap.setCursor((long)(nC * BeatMap.BEAT), true);
        });

        bpm.onEndEdit.AddListener((String newBPM) =>
        {
            int bpm = int.Parse(newBPM);
            beatMapper.setBPM(bpm);
        });

        playButton.onClick.AddListener(() => beatMapper.Play());
        pauseButton.onClick.AddListener(() => beatMapper.Stop());
        stopButton.onClick.AddListener(() =>
        {
            beatMapper.Stop();
            beatMapper.beatMap.setCursor(0, true);
        });


        upButton.onClick.AddListener(() => beatMapper.handleUserInput(NoteInput.Up));
        leftButton.onClick.AddListener(() => beatMapper.handleUserInput(NoteInput.Left));
        downButton.onClick.AddListener(() => beatMapper.handleUserInput(NoteInput.Down));
        triangleButton.onClick.AddListener(() => beatMapper.handleUserInput(NoteInput.Triangle));
        circleButton.onClick.AddListener(() => beatMapper.handleUserInput(NoteInput.Circle));
        xButton.onClick.AddListener(() => beatMapper.handleUserInput(NoteInput.X));

        nextFactory.onClick.AddListener(() => beatMapper.nextFactory());
        prevFactory.onClick.AddListener(() => beatMapper.prevFactory());

        loadButton.onClick.AddListener(() =>
        {
            String file = EditorUtility.OpenFilePanel("Open File", "", "");

            if (file == "")
            {
                return;
            }

            try
            {
                beatMapper.beatMap = BeatMap.Deserialize(file);
                if(beatMapper.beatMap.pathToTrack != null)
                {
                    beatMapper.LoadTrack(beatMapper.beatMap.pathToTrack);
                }
                beatMapper.beatMap.registerObserver(beatMapper);
                beatMapper.drawBeats();
                beatMapper.doUpdate();
                beatMapper.setBPM(beatMapper.beatMap.getBPM());
                EditorUtility.DisplayDialog("Load Complete", "File Loaded", "Continue");
            }
            catch
            {
                EditorUtility.DisplayDialog("Unable to Open File", "Could not Open File", "Continue");
            }
            //beatMapper.beatMap.Serialize("test.file");
        });

        saveButton.onClick.AddListener(() =>
        {
            String file = EditorUtility.SaveFilePanel("Save File", "", beatMapper.beatMap.name, "beatMap");
            if (file == "")
            {
                return;
            }

            try
            {

                String[] path = file.Split(Path.DirectorySeparatorChar);
                beatMapper.beatMap.name = path[path.Length - 1];
                beatMapper.beatMap.Serialize(file);
                EditorUtility.DisplayDialog("File Saved", "File Saved", "Continue");

            }
            catch
            {
                EditorUtility.DisplayDialog("Unable to Save File", "Could not Open File", "Continue");
            }

        });

        setTrackButton.onClick.AddListener(() =>
        {
            string file = EditorUtility.OpenFilePanel("Open File", "", "mp3");
            if (file == "")
            {
                return;
            }

            beatMapper.LoadTrack(file);

        });
    }

    
}

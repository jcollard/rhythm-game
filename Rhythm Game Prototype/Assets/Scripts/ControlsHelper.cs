using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using OxOD;

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
    public Button syncButton;
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
    public FileDialog fileDialog;

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
            print("Edit End: " + newBPM);
            int bpm = int.Parse(newBPM);
            beatMapper.setBPM(bpm);
        });

        //TODO: Go through UserInputManager?
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

        loadButton.onClick.AddListener(() => StartCoroutine(LoadFile()));

        saveButton.onClick.AddListener(() => StartCoroutine(SaveFile()));

        setTrackButton.onClick.AddListener(() => StartCoroutine(LoadTrack()));

        syncButton.onClick.AddListener(() => beatMapper.StartSynching());
    }

    public IEnumerator LoadFile()
    {

        yield return fileDialog.Open();

        if (fileDialog.result != null)
        {
            try
            {
                beatMapper.beatMap = BeatMap.Deserialize(fileDialog.result);
                if (beatMapper.beatMap.pathToTrack != null)
                {
                    beatMapper.LoadTrack(beatMapper.beatMap.pathToTrack);
                }
                beatMapper.beatMap.registerObserver(beatMapper);
                beatMapper.drawBeats();
                beatMapper.doUpdate();
                beatMapper.setBPM(beatMapper.beatMap.getBPM());
                //TODO: Add Dialog to display success
            }
            catch
            {
                //TODO: Add Dialog to display failure message
            }
        }
    }

    public IEnumerator SaveFile()
    {
        yield return fileDialog.Save();
        if (fileDialog.result != null)
        {

            try
            {

                String[] path = fileDialog.result.Split(Path.DirectorySeparatorChar);
                beatMapper.beatMap.name = path[path.Length - 1];
                beatMapper.beatMap.Serialize(fileDialog.result);
                //TODO: Add Dialog to display success

            }
            catch
            {
                //TODO: Add Dialog to display failure message
            }
        }
    }

    public IEnumerator LoadTrack()
    {
        yield return fileDialog.Open();
        if (fileDialog.result != null)
        {
            beatMapper.LoadTrack(fileDialog.result);
            //TODO: Add Dialog to display success
        }
    }


}

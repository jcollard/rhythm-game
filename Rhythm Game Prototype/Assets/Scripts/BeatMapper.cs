using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using UnityEditor;

/// <summary>
/// A BeatMapper manages the View/Controller for a BeatMap
/// </summary>
public class BeatMapper : MonoBehaviour, Observer
{
    /// <summary>
    /// The BeatMap model that is being used
    /// </summary>
    public BeatMap beatMap = new BeatMap(120);

    /// <summary>
    /// The number of beats that should be rendered to the screen
    /// </summary>
    public int beatsVisible = 4;

    /// <summary>
    /// Determines if the BeatMap's cursor should be updated based on
    /// Time.deltaTime
    /// </summary>
    public bool isPlaying = false;

    /// <summary>
    /// The current position of the track in seconds
    /// </summary>
    public float songPosition = 0;

    public GameObject notes;
    /// <summary>
    /// The audio file to be used for the metronome sound
    /// </summary>
    public AudioSource metronome;
    public bool isMetronomeOn = true;
    public InputField bpmField;

    /// <summary>
    /// A reference to a PositionHelper
    /// </summary>
    public PositionHelper positions;

    /// <summary>
    /// A lookup table for noteController's that should currently be displayed
    /// </summary>
    public readonly Dictionary<Tuple<Note, Beat>, NoteController> noteControllers = new Dictionary<Tuple<Note, Beat>, NoteController>();

    /// <summary>
    /// A lookup table for where NoteInput's are displayed on the screen
    /// </summary>
    public readonly Dictionary<NoteInput, Vector2> noteToPosition = new Dictionary<NoteInput, Vector2>();

    /// <summary>
    /// The InputField for the current position
    /// </summary>
    public InputField currentPosition;

    [Header("Note Factory")]

    /// <summary>
    /// The text which should be updated based on the currently selected factory
    /// </summary>
    public Text noteFactoryName;

    /// <summary>
    /// The index of the currently selected NoteFactory
    /// </summary>
    public int factoryIndex = 0;

    /// <summary>
    /// The NoteFactoryHelper
    /// </summary>
    public FactoryHelper noteFactoryHelper;

    /// <summary>
    /// The currently selected NoteFactory
    /// </summary>
    private NoteFactory noteFactory;

    /// <summary>
    /// A GameObject that is at the position where the currently selected NoteFactory
    /// should be rendered
    /// </summary>
    public GameObject factoryPosition;

    /// <summary>
    /// The AudioSource of the track
    /// </summary>
    public AudioSource trackSource;

    void Start()
    {
        // Register self as an observer on the beatMap and set the cursor to 0
        beatMap.registerObserver(this);
        beatMap.setCursor(0);

        // Register each of the note positions for easy lookup
        noteToPosition.Add(NoteInput.Up, positions.UP.position);
        noteToPosition.Add(NoteInput.Left, positions.LEFT.position);
        noteToPosition.Add(NoteInput.Down, positions.DOWN.position);
        noteToPosition.Add(NoteInput.Triangle, positions.TRIANGLE.position);
        noteToPosition.Add(NoteInput.Circle, positions.CIRCLE.position);
        noteToPosition.Add(NoteInput.X, positions.X.position);

        // Select the current factory
        newFactory(noteFactoryHelper.factories[factoryIndex]);

    }

    void Update()
    {
        if (isPlaying)
        {
            // Update the songPosition based on the Time.deltaTime
            // TODO: When a musical track is selected, this likely will be queried
            if (trackSource.clip == null)
            {
                songPosition += Time.deltaTime;
            }
            else
            {
                songPosition = trackSource.time;
            }

            if (trackSource.clip != null && !trackSource.isPlaying)
            {
                isPlaying = false;
            }

        }

        // Calculate the cursor position
        // (BPM * BEAT DURATION * songPosition in seconds / 60 seconds per minute) = Cursor Position
        // TODO: write a helper method for this calculation
        long newCursorPosition = beatMap.SecondsToCursorPosition(songPosition);

        
        long nextClick = ((beatMap.getCursor() / 1000) + 1) * 1000;

        
        if (isMetronomeOn && isPlaying && newCursorPosition >= nextClick)
        {
            metronome.Play();
        }
        if (newCursorPosition != beatMap.getCursor())
        {
            beatMap.setCursor(newCursorPosition);
            SetCursorText();
        }
        drawBeats();
    }

    /// <summary>
    /// Draws the beats that from 2 BEAT after they should hit to beatsVisible
    /// BEAT before they should be hit
    /// </summary>
    public void drawBeats()
    {
        drawBeats(beatMap.getCursor() - 2 * BeatMap.BEAT, beatMap.getCursor() + beatsVisible * BeatMap.BEAT);
    }

    /// <summary>
    /// Given a startBeat cursor and an endBeat cursor, draw all of the beats
    /// that fall between those two points.
    /// </summary>
    /// <param name="startBeat"></param>
    /// <param name="endBeat"></param>
    public void drawBeats(long startBeat, long endBeat)
    {
        // TODO: This currently loops through all of the beats. There is
        //       probably a simpler way to do this to increase performance
        //       for BeatMap's that have a large number of Beats
        //       beats is a sorted list so we could do a binary search
        //       to find the start / end efficiently then loop
        //       based on index.
        List<Beat> beats = beatMap.getBeats().FindAll((Beat b) =>
        {
            return b.position >= startBeat && b.position <= endBeat;
        });

        foreach (Beat b in beats)
        {
            foreach (Note n in b.notes)
            {
                Tuple<Note, Beat> tuple = new Tuple<Note, Beat>(n, b);
                // If this Note, Beat combo is already present, we don't need
                // to add it again
                if (noteControllers.ContainsKey(tuple))
                {
                    continue;
                }

                NoteController newNote = NoteFactory.getNoteController(n, b, this);
                noteControllers.Add(new Tuple<Note, Beat>(n, b), newNote);

            }
        }
    }

    /// <summary>
    /// Given a Note, Beat tuple, removes the associated NoteController from the scene
    /// </summary>
    /// <param name="tuple">The Note, Beat tuple to remove</param>
    public void removeNoteController(Tuple<Note, Beat> tuple)
    {
        if (noteControllers.ContainsKey(tuple))
        {
            UnityEngine.Object.Destroy(noteControllers[tuple].gameObject);
            noteControllers.Remove(tuple);
        }
    }

    /// <summary>
    /// Given a Note and a Beat, remove the NoteController associated with it
    /// </summary>
    /// <param name="n">The Note to remove</param>
    /// <param name="b">The Beat with that Note</param>
    public void removeNoteController(Note n, Beat b)
    {
        removeNoteController(new Tuple<Note, Beat>(n, b));
    }

    /// <summary>
    /// Given a Note and a Beat, add a NoteController to the scene. If
    /// the Note, Beat combination is already present, the old value is first
    /// removed before replacing it with the new Tuple
    /// </summary>
    /// <param name="n">The Note to Add</param>
    /// <param name="b">The Beat to add</param>
    public void addNoteController(Note n, Beat b)
    {
        NoteController newNote = NoteFactory.getNoteController(n, b, this);
        Tuple<Note, Beat> key = new Tuple<Note, Beat>(n, b);
        if (noteControllers.ContainsKey(key))
        {
            removeNoteController(n, b);
        }
        noteControllers.Add(key, newNote);
        drawBeats();
    }

    /// <summary>
    /// Called by the BeatMap when a change is made. This is used to update the position text.
    /// </summary>
    public void doUpdate()
    {
        SetCursorText();
        songPosition = beatMap.CursorPositionToSeconds(beatMap.getCursor());
        bpmField.text = "" + beatMap.getBPM();
        trackSource.time = songPosition;
    }

    private void SetCursorText()
    {
        currentPosition.text = "" + beatMap.getCursor() / BeatMap.BEAT + "." + ("" + beatMap.getCursor() % 1000).PadRight(3, '0');

    }

    /// <summary>
    /// Processes userInput for a specific NoteInput. 
    /// </summary>
    /// <param name="type">The NoteInput that was selected by the user</param>
    public void handleUserInput(NoteInput type)
    {

        noteFactory.handleUserInput(type, this);

    }

    /// <summary>
    /// Goes to the nextFactory in the factories array (loops around)
    /// </summary>
    public void nextFactory()
    {
        factoryIndex = (factoryIndex + 1) % noteFactoryHelper.factories.Length;
        newFactory(noteFactoryHelper.factories[factoryIndex]);
    }

    /// <summary>
    /// Goes to the prevFactory in the factories array (loops around)
    /// </summary>
    public void prevFactory()
    {
        factoryIndex = factoryIndex > 0 ? factoryIndex - 1 : noteFactoryHelper.factories.Length - 1;
        newFactory(noteFactoryHelper.factories[factoryIndex]);
    }

    /// <summary>
    /// Helper method that is used to set the factory. This moves the factory to be
    /// displayed on the screen and calls the factories initialize method to clear out any
    /// previous information.
    /// </summary>
    /// <param name="newFactory">The factory to be used</param>
    private void newFactory(NoteFactory newFactory)
    {
        if (noteFactory != null)
        {
            noteFactory.transform.position = new Vector2(-20, 0);
        }
        noteFactory = newFactory;
        noteFactory.transform.position = factoryPosition.transform.position;
        noteFactoryName.text = noteFactory.displayName;
        noteFactory.initialize();
    }

    /// <summary>
    /// Sets the BPM of the underlying BeatMap model and redraws the screen.
    /// </summary>
    /// <param name="bpm">The new BPM</param>
    public void setBPM(int bpm)
    {
        beatMap.setBPM(bpm);
        foreach (NoteController n in noteControllers.Values)
        {
            UnityEngine.Object.Destroy(n.gameObject);
        }
        noteControllers.Clear();
        drawBeats();
    }

    public void Play()
    {
        if (trackSource.clip != null)
        {
            trackSource.Play();
            trackSource.time = songPosition;
            
        }
        isPlaying = true;
    }

    public void Stop()
    {
        if (trackSource.clip != null)
        {
            trackSource.Stop();
        }
        isPlaying = false;
    }

    public void LoadTrack(String pathToTrack)
    {
        try
        {
            Coroutine cr = StartCoroutine(GetAudioClip(pathToTrack));
            isMetronomeOn = false;
        }
        catch
        {
            EditorUtility.DisplayDialog("Unable to Load File", "Could not Load Audio File", "Continue");
        }
    }

    IEnumerator GetAudioClip(String filePath)
    {
        String file = "file://" + filePath;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                EditorUtility.DisplayDialog("Could not load track", www.error, "Continue");
            }
            else
            {
                try
                {
                    trackSource.clip = DownloadHandlerAudioClip.GetContent(www);
                    trackSource.time = (beatMap.getCursor() * 60) / ((float)(BeatMap.BEAT * beatMap.getBPM()));
                    beatMap.pathToTrack = filePath;
                }
                catch
                {
                    EditorUtility.DisplayDialog("Could not load track", "Could not load " + filePath, "Continue");
                    beatMap.pathToTrack = null;
                }
            }
        }
    }


}



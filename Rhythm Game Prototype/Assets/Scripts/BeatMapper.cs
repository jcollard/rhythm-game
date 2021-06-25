using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class BeatMapper : MonoBehaviour, Observer
{
    public BeatMap beatMap = new BeatMap(120);
    public int beatsVisible = 4;

    public bool isPlaying = false;
    public float songPosition = 0;

    
    public GameObject normalNote;
    public GameObject holdNote;
    public GameObject notes;
    public AudioSource metronome;

    public PositionHelper positions;

    public readonly Dictionary<Tuple<Note, Beat>, NoteController> noteControllers = new Dictionary<Tuple<Note,Beat>, NoteController>();
    public readonly Dictionary<NoteInput, Vector2> noteToPosition = new Dictionary<NoteInput, Vector2>();

    public InputField currentPosition;

    [Header("Note Factory")]
    public Text noteFactoryName;
    public int factoryIndex = 0;
    public FactoryHelper noteFactoryHelper;
    private NoteFactory noteFactory;
    public GameObject factoryPosition;

    // Use this for initialization
    void Start()
    {

        beatMap.registerObserver(this);
        beatMap.setCursor(0);

        noteToPosition.Add(NoteInput.Up, positions.UP.position);
        noteToPosition.Add(NoteInput.Left, positions.LEFT.position);
        noteToPosition.Add(NoteInput.Down, positions.DOWN.position);
        noteToPosition.Add(NoteInput.Triangle, positions.TRIANGLE.position);
        noteToPosition.Add(NoteInput.Circle, positions.CIRCLE.position);
        noteToPosition.Add(NoteInput.X, positions.X.position);

        newFactory(noteFactoryHelper.factories[factoryIndex]);

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            songPosition += Time.deltaTime;

        }

        // Calculate the cursor position
        // (BPM * BEAT DURATION * songPosition in seconds / 60 seconds per minute) = Cursor Position

        long newCursorPosition = (long)((beatMap.getBPM() * BeatMap.BEAT * songPosition)/60);
        long nextClick = ((beatMap.getCursor() / 1000) + 1) * 1000;
        if (isPlaying && newCursorPosition >= nextClick)
        {
            metronome.Play();
        }

        beatMap.setCursor(newCursorPosition);
        drawBeats();
    }

    public void drawBeats()
    {
        drawBeats(beatMap.getCursor() - 2 * BeatMap.BEAT, beatMap.getCursor() + beatsVisible * BeatMap.BEAT);
    }

    public void drawBeats(long startBeat, long endBeat)
    {
        List<Beat> beats = beatMap.getBeats().FindAll((Beat b) =>
        {
            return b.position >= startBeat && b.position <= endBeat;
        });
        //print("Position: " + beatMap.getCursor());
        //print("Upcomming Notes: ");
        foreach(Beat b in beats)
        {
            foreach(Note n in b.notes)
            {
                Tuple<Note, Beat> tuple = new Tuple<Note, Beat>(n, b);
                if (noteControllers.ContainsKey(tuple))
                {
                    continue;
                }

                //TODO: Key needs to be beat + note
                NoteController newNote = NoteFactory.getNoteController(n, b, this);
                noteControllers.Add(new Tuple<Note, Beat>(n, b), newNote);

            }
        }
    }

    public void removeNote(Tuple<Note, Beat> tuple, NoteController controller)
    {
        if (noteControllers.ContainsKey(tuple))
        {
            UnityEngine.Object.Destroy(controller.gameObjectRef);
            noteControllers.Remove(tuple);
        }
    }

    public void doUpdate()
    {
        currentPosition.text =  "" + beatMap.getCursor() / BeatMap.BEAT + "." + ("" + beatMap.getCursor() % 1000).PadRight(3, '0');
        songPosition = (beatMap.getCursor() * 60) / ((float)(BeatMap.BEAT * beatMap.getBPM()));
    }

    public void addNote(NoteInput type)
    {
        
        NoteFactory nextNoteFactory = noteFactory.addNote(type, beatMap);
        
        if(nextNoteFactory != noteFactory)
        {
            noteFactory.transform.position = new Vector2(-20, 0);
            noteFactory = nextNoteFactory;
            noteFactory.transform.position = factoryPosition.transform.position;
        }
    }

    public void nextFactory()
    {
        factoryIndex = (factoryIndex + 1) % noteFactoryHelper.factories.Length;
        newFactory(noteFactoryHelper.factories[factoryIndex]);
    }

    public void prevFactory()
    {
        factoryIndex = factoryIndex > 0 ? factoryIndex - 1 : noteFactoryHelper.factories.Length - 1;
        newFactory(noteFactoryHelper.factories[factoryIndex]);
    }

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

    public void setBPM(int bpm)
    {
        beatMap.setBPM(bpm);
        foreach(NoteController n in noteControllers.Values)
        {
            UnityEngine.Object.Destroy(n.gameObjectRef);
        }
        noteControllers.Clear();
        drawBeats();
    }

    

}

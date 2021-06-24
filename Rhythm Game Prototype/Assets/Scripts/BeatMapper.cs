using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BeatMapper : MonoBehaviour, Observer
{
    public BeatMap beatMap = new BeatMap(60);
    public int beatsVisible = 4;
    public int initialBPM = 240;

    public bool isPlaying = false;
    public float songPosition = 0;

    public GameObject normalNote;
    public GameObject notes;
    public AudioSource metronome;

    public PositionHelper positions;

    public readonly Dictionary<Note, NoteController> noteControllers = new Dictionary<Note, NoteController>();
    public readonly Dictionary<NoteInput, Vector2> noteToPosition = new Dictionary<NoteInput, Vector2>();

    public InputField currentPosition;

    // Use this for initialization
    void Start()
    {
        beatMap.bpm = initialBPM;
        beatMap.setCursor(BeatMap.BEAT * 5);
        beatMap.addNote(new Note(NoteInput.Up));
        beatMap.nextBeat();
        beatMap.addNote(new Note(NoteInput.Left));
        beatMap.nextBeat();
        beatMap.addNote(new Note(NoteInput.Down));
        beatMap.nextBeat();
        beatMap.addNote(new Note(NoteInput.Triangle));
        beatMap.nextBeat();
        beatMap.addNote(new Note(NoteInput.Circle));
        beatMap.nextBeat();
        beatMap.addNote(new Note(NoteInput.X));

        beatMap.registerObserver(this);
        beatMap.setCursor(0);

        noteToPosition.Add(NoteInput.Up, positions.UP.position);
        noteToPosition.Add(NoteInput.Left, positions.LEFT.position);
        noteToPosition.Add(NoteInput.Down, positions.DOWN.position);
        noteToPosition.Add(NoteInput.Triangle, positions.TRIANGLE.position);
        noteToPosition.Add(NoteInput.Circle, positions.CIRCLE.position);
        noteToPosition.Add(NoteInput.X, positions.X.position);

        

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

        long newCursorPosition = (long)((beatMap.bpm * BeatMap.BEAT * songPosition)/60);
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
                if (noteControllers.ContainsKey(n))
                {
                    continue;
                }
                //TODO: Get note Factory based on note type
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(normalNote);
                NoteController newNote = gameObject.GetComponent<NoteController>();
                noteControllers.Add(n, newNote);
                newNote.model = n;
                newNote.gameObjectRef = gameObject;
                newNote.transform.parent = notes.transform;
                newNote.name = "" + n.input + " @" + b.position;
                newNote.startPosition = positions.CENTER.position;
                newNote.endPosition = noteToPosition[n.input];

                newNote.startTime = ((b.position - BeatMap.BEAT * beatsVisible)*60) / ((float)(BeatMap.BEAT * beatMap.bpm));
                newNote.endTime = (b.position * 60)/((float)(BeatMap.BEAT * beatMap.bpm));
                newNote.beatMapper = this;
                

            }
        }
    }

    public void removeNote(Note n, NoteController controller)
    {
        if (noteControllers.ContainsKey(n))
        {
            UnityEngine.Object.Destroy(controller.gameObjectRef);
            noteControllers.Remove(n);
        }
    }

    public void doUpdate()
    {
        currentPosition.text =  "" + beatMap.getCursor() / BeatMap.BEAT + "." + ("" + beatMap.getCursor() % 1000).PadRight(3, '0');
        songPosition = (beatMap.getCursor() * 60) / ((float)(BeatMap.BEAT * beatMap.bpm));
    }
}

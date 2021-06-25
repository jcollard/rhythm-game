using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A NoteFactory is a Singleton class used to add Notes and NoteController's to the game.
/// <br/>
/// Given a Note and a Beat, the static method getNoteController will select
/// the correct NoteFactory to use based on the specified Note's subclass.
/// <br/>
/// To subclass NoteFactory, the virtual methods addNote, createNoteController
/// and initialize should be overridden. Then, in the subclasses Start() method,
/// the class should be registered on the NoteFactory using the NoteFactory.registerFactory method. For example<br/>
/// <pre>
///     NoteFactory.registerFactory(typeof(Note), this);
/// </pre>
/// <br/>
/// This will make all Notes of that type use that factory during calls to getNoteController.
/// </summary>
public class NoteFactory : MonoBehaviour
{

    public String displayName = "Normal";

    /// <summary>
    /// The NoteController to Instantiate when creating a Note
    /// </summary>
    public GameObject noteController;

    private static readonly Dictionary<Type, NoteFactory> factories = new Dictionary<Type, NoteFactory>();

    private void Start()
    {
        NoteFactory.registerFactory(typeof(Note), this);
    }

    /// <summary>
    /// Registers a Factory to be used for a specific Type. This method is intended
    /// to be used with subclasses of the Note type.
    /// </summary>
    /// <param name="type">Note.GetType() of the subclass to register</param>
    /// <param name="factory">The NoteFactory to be used for the specified Note type</param>
    public static void registerFactory(Type type, NoteFactory factory)
    {
        NoteFactory.factories.Add(type, factory);
    }

    /// <summary>
    /// Given a Note, a Beat, and the BeatMapper being used, retrieves the NoteController
    /// associated with that Note, Beat combination. If one does not exist, a new one is
    /// created and then returned.
    /// </summary>
    /// <param name="n">The Note to be associated with the returned NoteController</param>
    /// <param name="b">The Beat at which this Note occurs</param>
    /// <param name="beatMapper">The BeatMapper where the NoteController will be added</param>
    /// <returns>The NoteController associated with the (Note, Beat) Tuple</returns>
    public static NoteController getNoteController(Note n, Beat b, BeatMapper beatMapper)
    {
        return NoteFactory.factories[n.GetType()].createNoteController(n, b, beatMapper);
    }

    /// <summary>
    /// Given the NoteInput and BeatMap, attempts to create and add a new Note at
    /// the BeatMap's current cursor position. If an equivalent Note exists at the
    /// current beat, the BeatMap is not modified. If the BeatMap was modified, it
    /// notifies its observers.
    /// </summary>
    /// <param name="type">The type of input for the added Note</param>
    /// <param name="beatMap">The BeatMap to be modified</param>
    /// <returns>Returns true if the Note was added and False otherwise</returns>
    public virtual void handleUserInput(NoteInput type, BeatMapper beatMapper)
    {
        Note n = new Note(type);
        Beat b = beatMapper.beatMap.getBeat();
        if(b != null && b.notes.Contains(n))
        {
            beatMapper.beatMap.removeNote(n);
            beatMapper.removeNoteController(n, b);
        } else
        {
            beatMapper.beatMap.addNote(n);
            beatMapper.addNoteController(n, beatMapper.beatMap.getBeat());
        }
    }

    /// <summary>
    /// Given a Note, a Beat, and a BeatMapper, create a NoteController
    /// for the given Note.
    /// </summary>
    /// <param name="n">The modeled Note</param>
    /// <param name="b">The Beat at which the Note is added</param>
    /// <param name="beatMapper">The BeatMapper where the NoteController will be added</param>
    /// <returns></returns>
    public virtual NoteController createNoteController(Note n, Beat b, BeatMapper beatMapper)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(noteController);
        NoteController newNote = gameObject.GetComponent<NoteController>();

        newNote.model = new Tuple<Note, Beat>(n, b);
        newNote.gameObjectRef = gameObject;
        newNote.transform.parent = beatMapper.notes.transform;
        newNote.name = "Normal: " + n.input + " @" + b.position;
        newNote.startPosition = beatMapper.positions.CENTER.position;
        newNote.endPosition = beatMapper.noteToPosition[n.input];

        newNote.startTime = ((b.position - BeatMap.BEAT * beatMapper.beatsVisible) * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));
        newNote.endTime = (b.position * 60) / ((float)(BeatMap.BEAT * beatMapper.beatMap.getBPM()));
        newNote.beatMapper = beatMapper;

        return newNote;
    }

    /// <summary>
    /// Initializes this factory. This method is called when this factory is selected.
    /// It can be used to setup the user input for Note Types which require multiple
    /// clicks to add a Note
    /// </summary>
    public virtual void initialize()
    {
        
    }

}

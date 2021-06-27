using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFormController : MonoBehaviour
{

    public BeatMapper beatMapper;
    public GameObject cursor;
    public SpriteRenderer spriteRenderer;


    private void OnMouseDown()
    {
        UpdateCursor();
    }

    private void OnMouseDrag()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (beatMapper.trackSource.clip == null)
        {
            return;
        }
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        float position = (mousePosition.x - this.gameObject.transform.position.x);
        //TODO: 8 Should be the width of the box
        float percentage = position / 8;
        float length = beatMapper.trackSource.clip.length;
        float songPosition = length * percentage;
        long cursorPosition = beatMapper.beatMap.SecondsToCursorPosition(songPosition);
        beatMapper.beatMap.setCursor(cursorPosition);
        beatMapper.beatMap.prevQuarter();
    }

    public void UpdateCursorPosition()
    {
        if(beatMapper.trackSource.clip == null)
        {
            return;
        }
        
        float length = beatMapper.trackSource.clip.length;
        float percentage = beatMapper.songPosition / length;
        Vector3 position = new Vector3((8 * percentage) - 4, this.gameObject.transform.position.y + 0.5f, -1);
        //TODO Fix magic numbers (8 and 4)
        cursor.transform.position = position;
    }

}

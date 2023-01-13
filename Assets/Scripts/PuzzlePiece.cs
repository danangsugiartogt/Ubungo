using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [SerializeField] private float offsetX = 0;
    [SerializeField] private float offsetY = 0;

    private Vector2 defaultPosition;

    private void Awake()
    {
        defaultPosition = transform.position;
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseDrag()
    {
        var mousePosition = GetMousePosition();
        transform.position = mousePosition;
    }

    public void SetPuzzleToDefaultPosition()
    {
        transform.position = defaultPosition;
    }

    private void OnMouseUp()
    {
        var mousePosition = GetMousePosition();

        float postX = GetDesiredValue(mousePosition.x, offsetX);
        float postY = GetDesiredValue(mousePosition.y, offsetY);

        transform.position = new Vector2(postX, postY);

        var childsPost = GetChildsPosition();
        GameManagerEvent.NotifyOnDropAnswer(this, childsPost);
    }

    private float GetDesiredValue(float value, float offset)
    {
        if(offset > 0)
        {
            var isNegative = IsNegative(value);
            var devider = isNegative ? -1 : 1;
            var remaining = value % devider;

            if (remaining != 0)
            {
                value -= remaining;
                return isNegative ? value += -offset : value += offset;
            }

            return value + offset;
        }

        return Mathf.RoundToInt(value);
    }

    private Vector2[] GetChildsPosition()
    {
        Vector2[] childsPost = new Vector2[transform.childCount];
        int index = 0;

        foreach(Transform tf in transform)
        {
            childsPost[index] = tf.position;
            index++;
        }

        return childsPost;
    }

    private bool IsNegative(float number)
    {
        return number < 0;
    }

    private Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}

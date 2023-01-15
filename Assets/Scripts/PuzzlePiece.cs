using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PuzzlePiece : MonoBehaviour
{
    [SerializeField] private float offsetX = 0;
    [SerializeField] private float offsetY = 0;
    [SerializeField] private bool isRotateAble;
    [SerializeField] private ParticleSystem particle;

    private Vector2 defaultPosition;
    private Vector2 latestPosition;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRender;

    public bool IsRotateAble => isRotateAble;

    private void Awake()
    {
        defaultPosition = transform.position;
        latestPosition = defaultPosition;
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        GameManagerEvent.NotifyOnSelectPuzzle(this);
    }

    private void OnMouseDrag()
    {
        var mousePosition = GetMousePosition();
        transform.position = mousePosition;
    }

    public void UpdateLatestPosition(Vector2 position)
    {
        latestPosition = position;
    }

    public void MovePuzzleToDefaultPosition(float duration, Action onComplete = null)
    {
        transform.DOMove(defaultPosition, duration).OnComplete(() => {
            onComplete?.Invoke();
        });
    }

    public void MovePuzzleToLatestPosition(float duration, Action onComplete = null)
    {
        EnableInteraction(false);
        transform.DOMove(latestPosition, duration).OnComplete(() => {
            EnableInteraction(true);
            onComplete?.Invoke();
        });
    }

    public void SetPuzzleLayer(string layerName)
    {
        spriteRender.sortingLayerName = layerName;
    }

    public void SetPuzzleColor(Color color)
    {
        spriteRender.color = color;
    }

    public void EnableInteraction(bool isEnabled = true)
    {
        boxCollider.enabled = isEnabled;
    }

    public void PlayParticle()
    {
        var spawnnedParticle = Instantiate(particle, transform);
        var duration = spawnnedParticle.main.duration + spawnnedParticle.main.startLifetimeMultiplier;
        spawnnedParticle.Play();
        Destroy(spawnnedParticle.gameObject, duration);
    }

    public GameObject[] GetChilds()
    {
        GameObject[] childs = new GameObject[transform.childCount];
        int index = 0;

        foreach (Transform tf in transform)
        {
            childs[index] = tf.gameObject;
            index++;
        }

        return childs;
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

    public Vector2[] GetChildsPosition()
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

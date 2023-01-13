using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Level level;

    private Dictionary<PuzzlePiece, Vector2[]> puzzlePieceOnSlotDict = new Dictionary<PuzzlePiece, Vector2[]>();

    private void OnEnable()
    {
        GameManagerEvent.OnDropAnswer += OnDropAnswer;
    }

    private void OnDisable()
    {
        GameManagerEvent.OnDropAnswer -= OnDropAnswer;
    }

    private void OnDropAnswer(PuzzlePiece puzzle, Vector2[] positions)
    {
        // Remove puzzle from dictionary
        if (puzzlePieceOnSlotDict.TryGetValue(puzzle, out var childsPosition))
        {
            level.RemoveClaimedTiles(childsPosition);
            puzzlePieceOnSlotDict.Remove(puzzle);
        }

        // Check answer
        var isCorrectAnswer = level.TryToClaimTiles(positions);

        if (isCorrectAnswer)
        {
            puzzlePieceOnSlotDict.Add(puzzle, positions);
            GameManagerEvent.NotifyOnCorrectAnswer();
        }
        else
        {
            puzzle.SetPuzzleToDefaultPosition();
            GameManagerEvent.NotifyOnWrongAnswer();
        }
    }
}

public static class GameManagerEvent
{
    public static Action<PuzzlePiece, Vector2[]> OnDropAnswer;

    public static void NotifyOnDropAnswer(PuzzlePiece puzzle, Vector2[] positions)
    {
        OnDropAnswer?.Invoke(puzzle, positions);
    }

    public static Action OnCorrectAnswer;

    public static void NotifyOnCorrectAnswer()
    {
        OnCorrectAnswer?.Invoke();
    }

    public static Action OnWrongAnswer;

    public static void NotifyOnWrongAnswer()
    {
        OnWrongAnswer?.Invoke();
    }
}

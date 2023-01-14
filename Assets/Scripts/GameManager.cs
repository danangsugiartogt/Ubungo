using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Level level;
    [SerializeField] private List<PuzzlePiece> puzzlePieceList;

    private Dictionary<GameObject, Vector2> puzzlePiecesPositionDict = new Dictionary<GameObject, Vector2>();

    private PuzzlePiece selectedPuzzle;

    private void OnEnable()
    {
        GameManagerEvent.OnDropAnswer += OnDropAnswer;
        GameManagerEvent.OnSelectPuzzle += OnSelectPuzzle;
    }

    private void OnDisable()
    {
        GameManagerEvent.OnDropAnswer -= OnDropAnswer;
        GameManagerEvent.OnSelectPuzzle -= OnSelectPuzzle;
    }

    private void Awake()
    {
        InitializePuzzlePieces();
    }

    private void InitializePuzzlePieces()
    {
        for(int i = 0; i < puzzlePieceList.Count; i++)
        {
            var childs = puzzlePieceList[i].GetChilds();
            for(int j = 0; j < childs.Length; j++)
            {
                puzzlePiecesPositionDict.Add(childs[j], childs[j].transform.position);
            }
        }
    }

    public void CheckForCompletion()
    {
        Debug.LogError(IsCompleted());
        if (IsCompleted())
        {
            var levelName = SceneManager.GetActiveScene().name;
            if (levelName == "Level1")
                SceneManager.LoadScene("Level2");
            else
                SceneManager.LoadScene("MainMenu");
        }
        else
        {

        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Rotate()
    {
        if (!selectedPuzzle.IsRotateAble) return;

        var currentRotation = selectedPuzzle.transform.rotation;
        var rotateAmount = currentRotation.eulerAngles.z - 90;

        selectedPuzzle.transform.rotation = Quaternion.Euler(0, 0, rotateAmount);
        ResetPuzzlePiecesPositionDict(selectedPuzzle);

        var isValidMove = IsValidPositions(selectedPuzzle.GetChildsPosition());
        if (isValidMove)
        {
            UpdatePuzzlePiecesPositionDict(selectedPuzzle);
            return;
        }

        TryMoveToValidPosition();
    }

    private void TryMoveToValidPosition()
    {
        bool isValidMove = false;
        for (int i = 1; i < 100; i++)
        {
            // try move -x
            isValidMove = _IsValidMove(new Vector2(selectedPuzzle.transform.position.x - i, selectedPuzzle.transform.position.y));
            if (isValidMove)
            {
                UpdatePuzzlePiecesPositionDict(selectedPuzzle);
                break;
            }

            // try move +x
            isValidMove = _IsValidMove(new Vector2(selectedPuzzle.transform.position.x + i, selectedPuzzle.transform.position.y));
            if (isValidMove)
            {
                UpdatePuzzlePiecesPositionDict(selectedPuzzle);
                break;
            }

            // try move -y
            isValidMove = _IsValidMove(new Vector2(selectedPuzzle.transform.position.x, selectedPuzzle.transform.position.y - i));
            if (isValidMove)
            {
                UpdatePuzzlePiecesPositionDict(selectedPuzzle);
                break;
            }

            // try move +y
            isValidMove = _IsValidMove(new Vector2(selectedPuzzle.transform.position.x, selectedPuzzle.transform.position.y + i));
            if (isValidMove)
            {
                UpdatePuzzlePiecesPositionDict(selectedPuzzle);
                break;
            }
        }

        bool _IsValidMove(Vector2 post)
        {
            ResetPuzzlePiecesPositionDict(selectedPuzzle);
            selectedPuzzle.transform.position = post;
            isValidMove = IsValidPositions(selectedPuzzle.GetChildsPosition());
            if (isValidMove)
            {
                UpdatePuzzlePiecesPositionDict(selectedPuzzle);
                return true;
            }

            return false;
        }
    }

    private void OnSelectPuzzle(PuzzlePiece puzzle)
    {
        selectedPuzzle = puzzle;
        foreach(var puz in puzzlePieceList)
        {
            if(puz == puzzle)
                puzzle.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, .7f);
            else
                puz.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
    }

    private void OnDropAnswer(PuzzlePiece puzzle, Vector2[] positions)
    {
        // Remove puzzle from dictionary
        ResetPuzzlePiecesPositionDict(puzzle);

        // Check collision with other puzzle
        var isValidDrop = IsValidPositions(positions);
        if (!isValidDrop)
        {
            puzzle.SetPuzzleToDefaultPosition();
        }

        UpdatePuzzlePiecesPositionDict(puzzle);
    }

    private void UpdatePuzzlePiecesPositionDict(PuzzlePiece puzzle)
    {
        var puzzleChilds = puzzle.GetChilds();
        for (int i = 0; i < puzzleChilds.Length; i++)
        {
            var puzzleChild = puzzleChilds[i];
            if (puzzlePiecesPositionDict.TryGetValue(puzzleChild, out var puzzlePost))
            {
                puzzlePiecesPositionDict[puzzleChild] = puzzleChild.transform.position;
            }
        }
    }

    private void ResetPuzzlePiecesPositionDict(PuzzlePiece puzzle)
    {
        var puzzleChilds = puzzle.GetChilds();
        for (int i = 0; i < puzzleChilds.Length; i++)
        {
            var puzzleChild = puzzleChilds[i];
            if (puzzlePiecesPositionDict.TryGetValue(puzzleChild, out var puzzlePost))
            {
                puzzlePiecesPositionDict[puzzleChild] = new Vector2(1000, 1000);
            }
        }
    }

    private bool IsValidPositions(Vector2[] positions)
    {
        for(int i = 0; i < positions.Length; i++)
        {
            var piece = puzzlePiecesPositionDict.FirstOrDefault(piece => piece.Value == positions[i]).Key;
            if(piece != null)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsCompleted()
    {
        for(int i = 0; i < puzzlePieceList.Count; i++)
        {
            var childs = puzzlePieceList[i].GetChilds();
            Vector2[] positions = new Vector2[childs.Length];
            for (int j = 0; j < childs.Length; j++)
            {
                positions[j] = childs[j].transform.position;
            }

            if (level.IsTilesOverlapping(positions) == false)
                return false;
        }

        return true;
    }
}

public static class GameManagerEvent
{
    public static Action<PuzzlePiece, Vector2[]> OnDropAnswer;

    public static void NotifyOnDropAnswer(PuzzlePiece puzzle, Vector2[] positions)
    {
        OnDropAnswer?.Invoke(puzzle, positions);
    }

    public static Action<PuzzlePiece> OnSelectPuzzle;

    public static void NotifyOnSelectPuzzle(PuzzlePiece puzzle)
    {
        OnSelectPuzzle?.Invoke(puzzle);
    }
}

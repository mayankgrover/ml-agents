using System.Collections;
using UnityEngine;

public class LudoPiece: MonoBehaviour
{
    [SerializeField] private LudoGame game;

    private Vector2 localPositionOffset;
    private int boardPosition = 0;
    private Vector2 gridPosition;

    public bool IsFinished { get { return boardPosition == game.gridSize; } }
    public int CurrentPosition { get { return boardPosition; } }

    private void Awake()
    {
        gridPosition = Vector2.zero;
        localPositionOffset = transform.localPosition;
    }

    public void MoveTo(int boardPosition, float duration = 0f)
    {
        //if (this.boardPosition != boardPosition)
        {
            this.boardPosition = boardPosition;
            gridPosition = game.BoardToGridPosition(boardPosition);
            Vector2 position = game.GridToLocalPosition(gridPosition);
            position += localPositionOffset;
            //transform.localPosition = position;
            iTween.MoveTo(gameObject, position, duration);
        }
    }

    public void Reset()
    {
        boardPosition = 0;
        gridPosition = Vector2.zero;
        transform.localPosition = localPositionOffset;
    }

    public bool CanDie()
    {
        return !IsFinished && !IsSafe();
    }

    public bool IsSafe()
    {
        return CurrentPosition % (game.gridSize / 4) == 0;
    }

    public bool CanMove(int roll)
    {
        return (boardPosition + roll <= game.gridSize);
    }

    public void MoveForward(int roll)
    {
        StartCoroutine(MoveAimation(boardPosition, roll, 0.5f));
        //MoveTo(boardPosition + roll);
    }

    private IEnumerator MoveAimation(int startPos, int roll, float duration)
    {
        //float step = duration/roll;
        int endPos = startPos + roll;
        while(boardPosition < endPos) {
            MoveTo(boardPosition + 1, duration);
            yield return new WaitForSeconds(duration);
        }
    }
}

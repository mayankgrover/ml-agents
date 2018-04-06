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

    public void MoveTo(int boardPosition)
    {
        //if (this.boardPosition != boardPosition)
        {
            this.boardPosition = boardPosition;
            gridPosition = game.BoardToGridPosition(boardPosition);
            Vector2 position = game.GridToLocalPosition(gridPosition);
            position += localPositionOffset;
            transform.localPosition = position;
        }
    }

    public void Reset()
    {
        boardPosition = 0;
        gridPosition = Vector2.zero;
        transform.localPosition = localPositionOffset;
    }

    public bool CanMove(int roll)
    {
        return (boardPosition + roll <= game.gridSize);
    }

    public void MoveForward(int roll)
    {
        MoveTo(boardPosition + roll);
    }
}

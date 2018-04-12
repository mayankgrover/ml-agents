using System.Collections;
using UnityEngine;

public class LudoAgent : Agent {

    [Header("Ludo")]
    public LudoGame game;
    public int index; // for LudoUI
    public LudoPiece piece1;
    public LudoPiece piece2;

    private int lastDiceRoll;

    public override void InitializeAgent()
    {
        game = GetComponentInParent<LudoGame>();
        ResetPieces();
    }

    public void MovePiece(int pieceIndex, int position)
    {
        LudoPiece piece = pieceIndex == 1 ? piece1 : piece2;
        piece.MoveTo(position);
    }

    public override void CollectObservations()
    {
        lastDiceRoll = game.GetDiceRoll();
        //AddVectorObs(game.gridSizeX);
        //AddVectorObs(game.gridSizeY);
        AddVectorObs(game.gridSize);
        AddVectorObs(lastDiceRoll);
        AddVectorObs(piece1.CurrentPosition);
        //AddVectorObs(piece1.CanMove(lastDiceRoll) ? 1 : 0);
        AddVectorObs(piece2.CurrentPosition);
        //AddVectorObs(piece2.CanMove(lastDiceRoll) ? 1 : 0);
        AddVectorObs(game.GetOtherAgent(this).piece1.CurrentPosition);
        AddVectorObs(game.GetOtherAgent(this).piece2.CurrentPosition);
        //Debug.LogFormat("[Obs][A]{0} [D]:{1}", gameObject.name, lastDiceRoll);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        bool gameFinished = false;
        if(lastDiceRoll != 0 && brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            int action = Mathf.FloorToInt(vectorAction[0]);
            LudoPiece piece = null;
            LudoPiece otherPiece = null;

            // move piece 1
            if (action == 0) {
                piece = piece1;
                otherPiece = piece2;
            }
            // move piece 2
            else if(action == 1) {
                piece = piece2;
                otherPiece = piece1;
            }

            if (piece != null) {
                if (piece.CanMove(lastDiceRoll)) {
                    AddReward(0.01f);
                    piece.MoveForward(lastDiceRoll);
                    gameFinished = game.UpdateGameState(this, piece);
                    Debug.LogFormat("[A]:{0} [D]:{1} [P]:{2}:{3} [OP]:{4}:{5}",
                        gameObject.name, lastDiceRoll,
                        piece.name, piece.CurrentPosition,
                        otherPiece.name, otherPiece.CurrentPosition);
                    // UI
                    game.UpdateAgent(this, piece);
                    game.UpdateDice(lastDiceRoll);
                }
                else {
                    if (otherPiece.CanMove(lastDiceRoll))
                    {
                        AddReward(-0.05f);
                        Debug.LogWarningFormat("[NA:NP]:{0} [D]:{1} [P]:{4}:{5} [OP]:{2}:{3}",
                            gameObject.name, lastDiceRoll,
                            piece.name, piece.CurrentPosition,
                            otherPiece.name, otherPiece.CurrentPosition);

                        if (piece.IsFinished)
                        {
                            AddReward(-0.05f);
                            Debug.LogWarningFormat("[NA:F]:{0} [D]:{1} [P]:{2}:{3} [OP]:{4}:{5}",
                                gameObject.name, lastDiceRoll,
                                piece.name, piece.CurrentPosition,
                                otherPiece.name, otherPiece.CurrentPosition);
                        }
                    }

                }
            }
        }

        if (!gameFinished) {
            if (brain.brainType == BrainType.External) {
                game.RequestNextDecision(this);
            }
            else { //if (brain.brainType == BrainType.Internal)
                StartCoroutine(RequestNextDecisionIn(1f));
            }
        }
    }

    private IEnumerator RequestNextDecisionIn(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        game.RequestNextDecision(this);
    }

    public override void AgentReset()
    {
        //Debug.LogWarning("Agent reset " + gameObject.name);
        ResetPieces();
    }

    private void ResetPieces()
    {
        piece1.MoveTo(0);
        piece2.MoveTo(0);
    }
}

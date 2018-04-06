using System;
using UnityEngine;
using UnityEngine.UI;

public class LudoGame : MonoBehaviour
{
    [Header("Board")]
    public int boardSizeX;
    public int boardSizeY;
    public int gridSizeX;
    public int gridSizeY;

    [Header("Agents")]
    public LudoAgent[] agents;

    [Header("Stats")]
    public Text currrentAgent;
    public Text diceRoll;

    public int gridSize { get { return 2 * (gridSizeX + gridSizeY - 2); } }

    private LudoAcademy academy;

    private void Awake() {
        academy = GameObject.FindObjectOfType<LudoAcademy>();
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }

    /*
    IEnumerator Start()
    {
        for (int x = 0; x < 2 * (gridSizeX + gridSizeY - 2); x++)
        {
            agents[0].MovePiece(1, x);
            agents[0].MovePiece(2, x);
            agents[1].MovePiece(1, x);
            agents[1].MovePiece(2, x);
            //BoardToLocalPosition(x);
            yield return new WaitForSeconds(0.5f);
        }
    }
    */

    private int agentIndex = 0;
    public void StartGame()
    {
        Debug.Log("Starting game");
        agents[agentIndex].RequestDecision();
    }

    public bool UpdateGameState(LudoAgent agent, LudoPiece piece)
    {
        LudoAgent otherAgent = GetOtherAgent(agent);

        // Game completed
        if(agent.piece1.IsFinished && agent.piece2.IsFinished) {
            agent.AddReward(1f);
            agent.Done();
            otherAgent.AddReward(-1f);
            otherAgent.Done();
            LudoUI.Instance.IncrementWin(agent.index);
            Debug.LogWarningFormat("[GO] {0}:{1} {2}:{3}",
                agent.name, agent.GetReward(),
                otherAgent.name, otherAgent.GetReward());
            // reset academy
            academy.AcademyReset();
            return true;
        }

        // didn't finish the game -0.01
        //agent.AddReward(-0.01f);

        // Killed another player's piece
        // provided both of the pieces of other agent were not on the same position
        LudoPiece otherPiece = null;
        if (!otherAgent.piece1.IsFinished && piece.CurrentPosition == otherAgent.piece1.CurrentPosition)
            otherPiece = otherAgent.piece1;
        if(!otherAgent.piece2.IsFinished && piece.CurrentPosition == otherAgent.piece2.CurrentPosition)
            otherPiece = otherAgent.piece2;

        if(otherPiece != null && otherAgent.piece1.CurrentPosition != otherAgent.piece2.CurrentPosition) {
            Debug.LogWarningFormat("[Kill] {0}:{1} {2}:{3}",
                agent.name, piece.CurrentPosition,
                otherAgent.name, otherPiece.CurrentPosition);

            agent.AddReward(1f * otherPiece.CurrentPosition/gridSize);
            LudoUI.Instance.IncrementKill(agent.index);

            otherPiece.MoveTo(0);
            otherAgent.AddReward(-0.25f);
        }

        return false;
    }

    public void RequestNextDecision(LudoAgent ludoAgent)
    {
        LudoAgent other = GetOtherAgent(ludoAgent);
        other.RequestDecision();
        //Debug.LogFormat("[ReqDecision] A:{0}", other.name);
    }

    public int GetDiceRoll() {
        return UnityEngine.Random.Range(1, 7);
    }

    public LudoAgent GetOtherAgent(LudoAgent ludoAgent) {
        return ludoAgent.Equals(agents[0]) ? agents[1] : agents[0];
    }

    public void UpdateAgent(LudoAgent agent, LudoPiece piece) {
        currrentAgent.text = agent.name + "-" + piece.name;
    }

    public void UpdateDice(int roll) {
        diceRoll.text = "Dice: " + roll;
    }

    public Vector2 BoardToLocalPosition(int boardPosition)
    {
        Vector2 gridPos = BoardToGridPosition(boardPosition);
        Vector2 localPos = GridToLocalPosition(gridPos);
        Debug.LogFormat("[BoardToLocal] B:{0} G:{1} L:{2}", boardPosition, gridPos.ToString(), localPos.ToString());
        return localPos;
    }
    public Vector2 GridToLocalPosition(Vector2 gridPos)
    {
        return new Vector2(gridPos.x * boardSizeX / gridSizeX, gridPos.y * boardSizeY / gridSizeY);
    }
    public Vector2 BoardToGridPosition(int boardPosition)
    {
        Vector2 gridPos = Vector2.zero;
        if(boardPosition >= 0 && boardPosition < gridSizeY-1) {
            gridPos.Set(0, boardPosition);
        }
        else if(boardPosition >= gridSizeY-1 && boardPosition < (gridSizeX + gridSizeY - 2)) {
            gridPos.Set(boardPosition - gridSizeY + 1, gridSizeY - 1);
        }
        else if(boardPosition >= (gridSizeX + gridSizeY - 2) && boardPosition < (gridSizeX + 2*gridSizeY - 3)) {
            gridPos.Set(gridSizeX - 1, 2*gridSizeY + gridSizeX - boardPosition - 3);
        }
        else if(boardPosition >= (gridSizeX + 2*gridSizeY - 3) && boardPosition < 2*(gridSizeX + gridSizeY + 2)) {
            gridPos.Set(2*(gridSizeX + gridSizeY) - boardPosition - 4, 0);
        }

        //Debug.Log("[BoardToGrid] B:" + boardPosition + ", G:" + gridPos);
        return gridPos;
    }
}


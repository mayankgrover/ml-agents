using UnityEngine;

public class LudoAcademy : Academy
{
    private bool startGame = true;
    private LudoGame game;

    public override void InitializeAcademy()
    {
        game = FindObjectOfType<LudoGame>();
    }

    public override void AcademyStep()
    {
        if (startGame) {
            startGame = false;
            game.StartGame();
        }
    }

    public override void AcademyReset()
    {
        Debug.LogWarning("Academy reset");
        startGame = true;
    }

}

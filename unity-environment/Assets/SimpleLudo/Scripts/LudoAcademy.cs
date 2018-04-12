using UnityEngine;

public class LudoAcademy : Academy
{
    private bool startGame = true;
    private LudoGame game;
    //private float safePoints = 0;

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
        //float newSafePoint;
        //resetParameters.TryGetValue("safe_points", out newSafePoint);
        //if (safePoints != newSafePoint) {
        //    Debug.LogError("Updating safe points to: " +newSafePoint);
        //    safePoints = newSafePoint;
        //}
    }

}

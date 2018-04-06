using Commons.Singleton;
using System.Collections.Generic;
using UnityEngine.UI;

public class LudoUI : MonoSingleton<LudoUI>
{
    public Text[] wins;
    public Text[] kills;

    private List<int> winScore;
    private List<int> killScore;

    protected override void Awake()
    {
        base.Awake();
        winScore = new List<int>();
        for (int i = 0; i < wins.Length; i++) winScore.Add(0);

        killScore = new List<int>();
        for (int i = 0; i < kills.Length; i++) killScore.Add(0);
    }

    public void IncrementWin(int player)
    {
        winScore[player]++;
        wins[player].text = "Wins: " + winScore[player];
    }

    public void IncrementKill(int player)
    {
        killScore[player]++;
        kills[player].text = "Kills: " + killScore[player];
    }
}

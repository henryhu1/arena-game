using System;

[Serializable]
public class SubmitScoreData
{
    public string name { get; }
    public float score { get; }
    public int roundsCleared { get; }
    public int enemiesDefeated { get; }
    public float inGameTime { get; }

    public SubmitScoreData(string name, float score, int roundsCleared, int enemiesDefeated, float inGameTime)
    {
        this.name = name;
        this.score = score;
        this.roundsCleared = roundsCleared;
        this.enemiesDefeated = enemiesDefeated;
        this.inGameTime = inGameTime;
    }
}

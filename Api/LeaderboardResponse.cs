using System;
using System.Linq;

[Serializable]
public class Timestamp
{
    public double _seconds;
    public double _nanoseconds;
}

[Serializable]
public class LeaderboardScoreData
{
    public string name;
    public float score;
    public int roundsCleared;
    public int enemiesDefeated;
    public float inGameTime;
    public Timestamp createdAt;

    public override string ToString()
    {
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(createdAt._seconds).ToLocalTime();
        return $"name: {name}, score: {score}, roundsCleared: {roundsCleared}, enemiesDefeated: {enemiesDefeated}, inGameTime: {inGameTime}, createdAt: {dateTime}";
    }
}

[Serializable]
public class LeaderboardResponse
{
    public int position;
    public string id;
    public LeaderboardScoreData data;
    public override string ToString()
    {
        return $"position: {position} // id: {id} - {data}";
    }
}

[Serializable]
public class LeaderboardList
{
    public LeaderboardResponse[] scores;
    public override string ToString()
    {
        return string.Join("\n", scores.Select(score => score.ToString()));
    }
}

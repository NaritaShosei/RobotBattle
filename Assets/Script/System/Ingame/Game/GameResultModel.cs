using UnityEngine;

public class GameResultModel
{
    public ResultType ResultType { get; private set; } = ResultType.None;
    public GameOverType GameOverType { get; private set; } = GameOverType.None;
    public float ClearTime { get; private set; }
    public int Score { get; private set; }
    public bool HasResult => ResultType != ResultType.None;
    public bool IsGameClear => ResultType == ResultType.GameClear;
    public bool IsGameOver => ResultType == ResultType.GameOver;

    public void SetGameOver(GameOverType gameOverType)
    {
        ResultType = ResultType.GameOver;
        GameOverType = gameOverType;
    }

    public void SetGameClear()
    {
        ResultType = ResultType.GameClear;
    }

    public void SetTime(float time)
    {
        ClearTime = time;
    }
    public void SetScore(int score)
    {
        Score = score;
    }

    public void Reset()
    {
        ResultType = ResultType.None;
        GameOverType = GameOverType.None;
    }

}
public enum ResultType
{
    None,
    GameClear,
    GameOver,
}
public enum GameOverType
{
    None,
    TimeOver,
    Death,
}

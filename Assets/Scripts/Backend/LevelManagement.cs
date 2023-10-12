using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

[System.Serializable]
public class LevelManagement
{
    private int currentLevel;
    public int totalScore;
    public int totalHints;
    private int maxTotalHints = 5;
    
    public static LevelManagement instance = null;

    private Dictionary<int, Level> levels;

    public Dictionary<int, Level> GetLevels()
    {
        return levels;
    }

    public LevelManagement()
    {
        currentLevel = 1;
        totalHints = 5;
        totalScore = 0;
        levels = new Dictionary<int, Level>();
        Level level1 = new Level(1);
        level1.CanPlay = true;
        levels.Add(1, level1);
        for (int i = 2; i < 65; i++)
        {
            levels.Add(i, new Level(i));
        }
        if (instance == null) {
            instance = this;
        }
    }

    public void AddNewLevel(bool CanPlay)
    {
        if (!NextLevelExists()) {
            Level newLevel = new Level(currentLevel + 1);
            newLevel.CanPlay = CanPlay;
            levels.Add(currentLevel + 1, newLevel);
        }
    }

    public bool NextLevelExists()
    {
        return levels.ContainsKey(currentLevel + 1);
    }

    public bool CanPlayNextLevel()
    {
        if (levels.ContainsKey(currentLevel + 1)) {
            return levels[currentLevel + 1].CanPlay;
        }
        return false;
    }

    public void EnableNextLevel(int completedLevelScore)
    {
        if (levels.ContainsKey(currentLevel + 1)) {
            LevelCompleted();
            // RecalculateScoreAndHints(completedLevelScore);
            levels[currentLevel + 1].CanPlay = true;
            SaveLevel();
        }
    }

    public bool GoNextLevel()
    {
        if (!levels.ContainsKey(currentLevel + 1)) {
            GetLevel(currentLevel);
            return false;
        }
        GetLevel(currentLevel + 1);
        return true;
    }

    public Level GetCurrentLevel()
    {
        Level _level = levels[currentLevel];
        return _level;
    }

    private void SetCurrentLevel(int level)
    {
        currentLevel = level;
        SaveLevel();
    }

    public Level GetLevel(int level)
    {
        Level _level = levels[level];
        SetCurrentLevel(level);
        return _level;
    }

    public int GetNextIncompletedLevel()
    {
        foreach (KeyValuePair<int, Level> item in levels)
        {
            if (!item.Value.Completed) {
                return item.Key;
            }
        }
        return -1;
    }

    public void LevelCompleted()
    {
        if (levels.ContainsKey(currentLevel)) {
            Level _level = levels[currentLevel];
            _level.Completed = true;
        }
    }

    public void IncreaseScore(int score)
    {
        totalScore += score;
        Text scoreText = GameObject.Find("Score").GetComponent<Text>();
        scoreText.text = "Score: " + totalScore;
        // Debug.Log("Increase Score:"+score);
    }

    public void IncreaseHints(int number)
    {
        if (totalHints < maxTotalHints) {
            totalHints += number;
            Text hintsText = GameObject.Find("Hints").GetComponent<Text>();
            hintsText.text = "" + totalHints;
            // Debug.Log("Increase Hints:"+number+":Total:"+totalHints);
        }
    }

    public bool ForceIncreaseHints(int number)
    {
        totalHints += number;
        SaveLevel();
        Text hintsText = GameObject.Find("Hints").GetComponent<Text>();
        hintsText.text = "" + totalHints;
        // Debug.Log("Force Increase Hints:"+number+":Total:"+totalHints);
        return true;
    }

    public void CollectHint()
    {
        GetCurrentLevel().MaxCollectableHints -= 1;
        if (GetCurrentLevel().MaxCollectableHints < 1) {
            HideAdHintButton();
        }
    }

    public void ShowAdHint()
    {
        // Debug.Log("MaxCollectableHints:"+ GetCurrentLevel().MaxCollectableHints);
        if (GetCurrentLevel().MaxCollectableHints > 0) {
            ShowAdHintButton();
        } else {
            HideAdHintButton();
        }
    }

    public void SaveScore()
    {
        if (Social.localUser.authenticated) {
            Social.ReportScore(totalScore, "CgkIjKPc_tMNEAIQAg", (bool success) => {
                // handle success or failure
            });
        }
        // Debug.Log("Score:"+totalScore);
        SaveLevel();
    }

    private void RecalculateScoreAndHints(int completedLevelScore)
    {
        totalScore += completedLevelScore;
        // Score leaderboard
        if (Social.localUser.authenticated) {
            Social.ReportScore(totalScore, "CgkIjKPc_tMNEAIQAg", (bool success) => {
                // handle success or failure
            });
        }
        if (totalHints < maxTotalHints) {
            totalHints += 1;
        }
    }

    public void SaveLevel()
    {
        SaveGame.SaveLevel(this);
        // Level Leaderboard
        // Social.ReportScore(currentLevel, "CgkIjKPc_tMNEAIQAQ", (bool success) => {
            // handle success or failure
        // });
    }

    private void HideAdHintButton()
    {
        // Image hintsText = GameObject.Find("AdHint").GetComponent<Image>();
        // hintsText.gameObject.SetActive(false);
    }

    private void ShowAdHintButton()
    {
        // Image hintsText = GameObject.Find("AdHint").GetComponent<Image>();
        // hintsText.gameObject.SetActive(true);
    }
}
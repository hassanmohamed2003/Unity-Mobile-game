using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngineInternal;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class Leaderoard : MonoBehaviour
{
    // Create a leaderboard with this ID in the Unity Dashboard
    const string LeaderboardId = "under_construction";

    string VersionId { get; set; }
    int Offset { get; set; }
    int Limit { get; set; } = 10;
    int RangeLimit { get; set; }
    List<string> FriendIds { get; set; }

    string playerName;
    string input;

    [Header("Events")]
    public UnityEvent getScore;

    [Header("Name and Score")]
    public List<Text> Names;
    public List<Text> Scores;


    async void Awake()
    {
        await UnityServices.InitializeAsync();

        await SignInAnonymously();

        getScore.Invoke();
    }

    public void ChangeName(InputField input)
    {
        playerName = input.text;
        Debug.Log(playerName);
        AddScore();
    }

    public void ReadStringInput(string s)
    {
        input = s;
        Debug.Log(input);
    }
    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };
        if(AuthenticationService.Instance.AccessToken == null)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void AddScore()
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, PlayerPrefs.GetInt("HighScore",0));
        var nameResponse = await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        GetPaginatedScores(0);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        Debug.Log(JsonConvert.SerializeObject(nameResponse));
    }

    public async void GetScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPaginatedScores(int number)
    {
        Offset += number;

        if(Offset < 0)
        {
            Offset = 0;
        }

        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Offset = Offset, Limit = Limit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse.Results));
        Debug.Log(scoresResponse.Results.Count);

        for (int i = 0; i < Limit; i++)
        {
            if(scoresResponse.Results.Count > i && scoresResponse.Results[i] != null)
            {
                Scores[i].text = "Score: " + scoresResponse.Results[i].Score.ToString();
                Names[i].text = $"{scoresResponse.Results[i].Rank + 1}. " + scoresResponse.Results[i].PlayerName.ToString();
            }
            else
            {
                Scores[i].text = "Score: ";
                Names[i].text = $"{i + 1 + Offset}. ";
            }

        }

    }

    public async void GetPlayerScore()
    {
        var scoreResponse =
            await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        Debug.Log(scoreResponse);

    }

    public async void GetVersionScores()
    {
        var versionScoresResponse =
            await LeaderboardsService.Instance.GetVersionScoresAsync(LeaderboardId, VersionId);
        Debug.Log(JsonConvert.SerializeObject(versionScoresResponse));
    }
}
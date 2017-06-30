using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReaderBoadAffecterFriend : MonoBehaviour {

    //example
    public Menu_Home home;
    public string LEADERBOARD_ID = "CgkIipfs2qcGEAIQAA";
    public Texture2D defaultTexture;
    private GPLeaderBoard loadedLeaderBoard = null;
    private GPCollectionType displayCollection = GPCollectionType.FRIENDS;
    private GPBoardTimeSpan displayTime = GPBoardTimeSpan.ALL_TIME;

    public  LeaderboardFiledsHolder[]  lines;


    //--------------------------------------
    // INITIALIZATION
    //--------------------------------------

    public void UpdateScores(string ID) {
        this.LEADERBOARD_ID=ID;
        displayCollection = GPCollectionType.GLOBAL;
        //ここで回転を始める。
        StartWait();
        //Same events, one with C# actions, one with FLE
        GooglePlayManager.ActionScoresListLoaded += ActionScoreRequestReceived;
        if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
            //checking if player already connected
            LoadScore();
        } else{
           
            //コネクトされていない
            OnNoData();
        }

    }

    public void StartWait(){
        foreach(LeaderboardFiledsHolder line in lines){
            line.Disable();
            line.ShowWait();
        }
    }
    public void StopWait(){
        foreach(LeaderboardFiledsHolder line in lines){
            line.StopWait();
        }
    }
    public void OnNoData(){
        Debug.Log("OnNoData");
        foreach(LeaderboardFiledsHolder line in lines){
            line.OnNoData();
        }
    }

    //--------------------------------------
    // METHODS
    //--------------------------------------



    public void LoadScore() {

        GooglePlayManager.Instance.LoadPlayerCenteredScores(LEADERBOARD_ID, displayTime, displayCollection, 6);
    }

    public void OpenUI() {
        GooglePlayManager.Instance.ShowLeaderBoardById(LEADERBOARD_ID);
    }



    public void ShowGlobal() {
        displayCollection = GPCollectionType.GLOBAL;
        StartWait();
        UpdateScoresDisaplay();
    }

    public void ShowFriend() {
        displayCollection = GPCollectionType.FRIENDS;
        StartWait();
        UpdateScoresDisaplay();
    }


    public void ShowAllTime() {
        displayTime = GPBoardTimeSpan.ALL_TIME;
        StartWait();
        UpdateScoresDisaplay();
    }

    public void ShowWeek() {
        displayTime = GPBoardTimeSpan.WEEK;
        StartWait();
        UpdateScoresDisaplay();
    }

    public void ShowDay() {
        displayTime = GPBoardTimeSpan.TODAY;
        StartWait();
        UpdateScoresDisaplay();
    }

    //--------------------------------------
    // UNITY
    //--------------------------------------

    void UpdateScoresDisaplay() {
        if(loadedLeaderBoard != null) {
            //Getting current player score
            int displayRank;

            GPScore currentPlayerScore = loadedLeaderBoard.GetCurrentPlayerScore(displayTime, displayCollection);


            if(currentPlayerScore == null) {
                //Player does not have rank at this collection / time
                //so let's show the top score
                //since we used loadPlayerCenteredScores function. we should have top scores loaded if player have no scores at this collection / time
                //https://developer.android.com/reference/com/google/android/gms/games/leaderboard/Leaderboards.html#loadPlayerCenteredScores(com.google.android.gms.common.api.GoogleApiClient, java.lang.String, int, int, int)
                //Asynchronously load the player-centered page of scores for a given leaderboard. If the player does not have a score on this leaderboard, this call will return the top page instead.
                displayRank = 1;

            } else {
                //Let's show 5 results before curent player Rank
                displayRank = Mathf.Clamp(currentPlayerScore.Rank - 5, 1, currentPlayerScore.Rank);
                //let's check if displayRank we what to display before player score is exists
                while(loadedLeaderBoard.GetScore(displayRank, displayTime, displayCollection) == null) {
                    displayRank++;
                }
            }


            Debug.Log("Start Display at rank: " + displayRank);


            int i = displayRank;

            foreach(LeaderboardFiledsHolder line in lines) {
                if(line.isPlayerRow)break;
                GPScore score = loadedLeaderBoard.GetScore(i, displayTime, displayCollection);
                if(score != null) {
                    //順位　i.ToString();
                    //スコア　score.LongScore.ToString();
                    GooglePlayerTemplate player = GooglePlayManager.Instance.GetPlayerById(score.PlayerId);
                    if(player != null) {
                        //名前　player.name;
                        //この行は表示する
                        if(player.hasIconImage) {
                            //アイコン有り
                            line.Set(i,score.LongScore,player.name,player.icon);
                        } else {
                            //アイコン無し
                            line.Set(i,score.LongScore,player.name,defaultTexture);
                        }

                    } else {

                        //プレイヤはnull
                        //名前　無し
                        //アイコン無し
                        line.Set(i,score.LongScore,"Angler",defaultTexture);
                    }


                } else {
                    //この行は表示しない
                    line.StopWait();
                }

                i++;
            }

        } else {
            //リーダボードは存在しない
            OnNoData();

        }
    }

    //--------------------------------------
    // EVENTS
    //--------------------------------------

    private void ActionScoreRequestReceived (GooglePlayResult obj) {
        GooglePlayManager.ActionScoresListLoaded -= ActionScoreRequestReceived;
        loadedLeaderBoard = GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID);
        if(loadedLeaderBoard == null) {
            OnNoData();
            Debug.Log("No Leaderboard found");
            return;
        }

        List<GPScore> scoresLB =  loadedLeaderBoard.GetScoresList(GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS);

        foreach(GPScore score in scoresLB) {
            Debug.Log("OnScoreUpdated " + score.Rank + " " + score.PlayerId + " " + score.LongScore);
        }
        int i = 1;

        foreach(LeaderboardFiledsHolder line in lines) {
            if(line.isPlayerRow)break;
            GPScore score = loadedLeaderBoard.GetScore(i, displayTime, displayCollection);
            if(score != null) {
                //順位　i.ToString();
                //スコア　score.LongScore.ToString();
                GooglePlayerTemplate player = GooglePlayManager.Instance.GetPlayerById(score.PlayerId);
                if(player != null) {
                    //名前　player.name;
                    //この行は表示する
                    if(player.hasIconImage) {
                        //アイコン有り
                        line.Set(i,score.LongScore,player.name,player.icon);
                    } else {
                        //アイコン無し
                        line.Set(i,score.LongScore,player.name,defaultTexture);
                    }

                } else {

                    //プレイヤはnull
                    //名前　無し
                    //アイコン無し
                    line.Set(i,score.LongScore,"Angler",defaultTexture);
                }


            } else {
                //この行は表示しない
                line.StopWait();
            }

            i++;
        }

        GPScore currentPlayerScore = loadedLeaderBoard.GetCurrentPlayerScore(displayTime, displayCollection);

        Debug.Log("currentPlayerScore: " + currentPlayerScore.LongScore + " rank:" + currentPlayerScore.Rank);
        if(currentPlayerScore == null) {
            lines[4].OnNoData();

        } else {
            lines[4].Set(currentPlayerScore.Rank,currentPlayerScore.LongScore,"YOU",defaultTexture);
        }

        //UpdateScoresDisaplay();

    }

    void OnDestroy() {
        GooglePlayManager.ActionScoresListLoaded -= ActionScoreRequestReceived;

    }
}

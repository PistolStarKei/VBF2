using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SetPlayerStats : MonoBehaviour {

    public UITexture playerIcon;
    public UILabel rankLb;
    public UILabel expToNext;
    public UILabel totalWins;
    public UISprite progress;
    void SetProgress(int exp,int expToNext){
        progress.fillAmount=((float)exp/(float)expToNext);
        this.expToNext.text=(expToNext-exp).ToString();
    }
    public void SetDatas(int rank,int curretExp,int expToNext,int wins){
        if( rank>=Constants.Params.maxLevel){
            rankLb.text="MAX";
            this.expToNext.text="";
            progress.fillAmount=1.0f;
            totalWins.text="×"+wins.ToString();
        }else{
            rankLb.text=rank.ToString();
            this.expToNext.text=(expToNext-curretExp).ToString();
            SetProgress(curretExp,expToNext);
            totalWins.text="×"+wins.ToString();
        }

       
    }

    public void SetPlayerTex(Texture2D tex){
        playerIcon.mainTexture=tex;
    }

    public void StartProgressAnimations(Menu_Home menu){
        StartCoroutine(InvokeAddRank(menu));

    }

    bool isInvokingAdd=false;
    public RainbowLabel ranklable;
    IEnumerator InvokeAdd(int start,int end,int ExpToRankUp){
        if(isInvokingAdd)yield break;
        isInvokingAdd=true;
        float timer=0.0f;
        AudioController.Play("loop");


        while(true)
        {
            timer+=Time.deltaTime*2.0f;
            SetProgress((int)Mathf.Lerp(start, end, timer),ExpToRankUp);
            if(timer>=1.0f){
                
                break;
            }
            yield return null;
        }
        if(end>=ExpToRankUp){
            DataManger.Instance.GAMEDATA.expOnLevel=0;
            if(DataManger.Instance.GAMEDATA.playerLevel<=Constants.Params.maxLevel-1){
                rankLb.gameObject.GetComponent<Animation>().Play();
                AudioController.Stop("loop");
                AudioController.Play("levelup");
                ranklable.Invoke();

                DataManger.Instance.GAMEDATA.playerLevel++;
                WaitAndCover.Instance.ShowInfoList(Localization.Get("Rank_Up")+DataManger.Instance.GAMEDATA.playerLevel.ToString());

                rankLb.text=DataManger.Instance.GAMEDATA.playerLevel.ToString();
                this.expToNext.text=(Constants.Params.expToNext(DataManger.Instance.GAMEDATA.playerLevel)).ToString();
            }else{
                rankLb.text="MAX";
                this.expToNext.text="";
                progress.fillAmount=1.0f;
            }



            yield return new WaitForSeconds(2.0f);
            ranklable.SetToDefaultColor();
        }

        AudioController.Stop("loop");
        isInvokingAdd=false;
    }


    IEnumerator InvokeAddRank(Menu_Home menu){
        
        int pending=DataManger.Instance.GetExp();
        Coroutine coroutine;
        List<int> bunkatsu_start=new List<int>();
        List<int> bunkatsu_end=new List<int>();
        List<int> bunkatsu_rankup=new List<int>();

        int expNow=DataManger.Instance.GAMEDATA.expOnLevel;
        int level=DataManger.Instance.GAMEDATA.playerLevel;
        int rankup=Constants.Params.expToNext(level);
        while(true){

           
            bunkatsu_start.Add(expNow);
            bunkatsu_rankup.Add(rankup);

            if(GetAmari( expNow,rankup,pending)==0){
                
                bunkatsu_end.Add(expNow + pending);
                pending=0;
            }else{
                int amari=GetAmari(expNow,rankup,pending);
                pending-=amari;
                bunkatsu_end.Add(expNow + pending);
                pending=amari;
            }
            if(pending==0)break;
            expNow=0;
            level++;
            if( level>=Constants.Params.maxLevel){
                yield break;
            }
            rankup=Constants.Params.expToNext(level);
            yield return null;
        }
        for(int i=0;i<bunkatsu_start.Count;i++){
            coroutine = StartCoroutine(InvokeAdd(bunkatsu_start[i],bunkatsu_end[i],bunkatsu_rankup[i]));
            yield return coroutine;
        }

        yield return null;
        ranklable.SetToDefaultColor();
        DataManger.Instance.ClearExp();
        DataManger.Instance.SaveData();
        menu.OnShowComplete();
    }
    int GetAmari(int now ,int max,int add){
        if(now+add>max){
            return (now+add)-max;
        }
        return 0;
    }
}

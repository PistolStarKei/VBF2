using UnityEngine;
using System.Collections;

public class LeaderboardFiledsHolder : MonoBehaviour {

    public bool isFriends=false;
	public UILabel rank;
	public UILabel score;
	public UILabel playerName;
    public UITexture avatar;

    public bool isPlayerRow=false;
    public void OnNoData(){
        this.playerName.text = isPlayerRow?"YOU":"Load failled";
        if(!isPlayerRow) this.avatar.enabled = false;

        this.rank.text = "";
        this.score.text = "";
        StopWait();
    }
    public void Set(int rank,long score,string name,Texture2D tex ) {
        this.playerName.text = isPlayerRow?"YOU":name;
        if(!isPlayerRow) this.avatar.enabled = true;
        this.rank.text = rank.ToString();
        this.score.text = score.ToString();
        if(!isPlayerRow) this.avatar.mainTexture=tex;
        StopWait();
    }
	public void Disable() {
		rank.text = "";
		score.text = "";
        this.playerName.text = isPlayerRow?"YOU":"";

        if(!isPlayerRow) avatar.enabled = false;
	}

    public void ShowWait(){
        col=StartCoroutine(WaitInvoke());
    }
    public Transform wait;
    public bool  isWaitInvoking=false;
    void ShowWaitSP(bool isOn){
        if(wait.gameObject.activeSelf!=isOn)NGUITools.SetActive(wait.gameObject,isOn);
    }
    Coroutine col;
    IEnumerator WaitInvoke(){
        if(isWaitInvoking)yield break;
        isWaitInvoking=true;
        ShowWaitSP(true);
        while(isWaitInvoking){
            wait.Rotate(Vector3.forward * Time.deltaTime*-600.0f);
            yield return null;
        }
        isWaitInvoking=false;
        ShowWaitSP(false);
        col=null;
    }
    public void StopWait(){
        if(col!=null)StopCoroutine(col);
        ShowWaitSP(false);
        col=null;
        isWaitInvoking=false;
    }
}

using UnityEngine;
using System.Collections;

public class LoadingMenuManager : PS_SingletonBehaviour<LoadingMenuManager>  {

    public UIProgressBar progress;
    public UILabel lbl;

    int progressNum=0;
    public int maxProgressNum=0;
    public void SetProgress(int progress,string text){
        progressNum+=progress;
        lbl.text=text;
        this.progress.value=(float)progressNum/(float)maxProgressNum;
    }
    public void SetProgressToDefault(){
        progressNum=0;
        lbl.text="";
        this.progress.value=0.0f;
    }
}

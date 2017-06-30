using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class MenuManager : PS_SingletonBehaviour<MenuManager> {


   
    public Menu_Tab tab;
    public Menu_Tab tacleTab;

    void Start(){
        WaitAndCover.Instance.CoverAll(true);

        tab.SetTabs(Constants.Params.Tab_MAIN);
        tacleTab.SetTabs(Constants.Params.Tab_SHOP);
        tab.onTabChanged+= OnTabChanged;
        UserName.Instance.SetPosition(true);
        if(DataManger.Instance!=null){
            UserName.Instance.UpdateCurrencyNums();
            UserName.Instance.SetNames(GPGSListener.Instance.userName_string);
        }
          

        //時間を更新する　天候を進める
        DataManger.Instance.UpdateTimedata(false);


        //DataManger.Instance.currentCategory=0;
        //DataManger.Instance.previousCategory=0;
        bool isAwardable=false;

        if(DataManger.Instance!=null){
            if(DataManger.Instance.GAMEDATA.currentEntry==null && DataManger.Instance.GAMEDATA.prevEntry==null){
                Debug.Log("新規スタート後");
                //ランキングは通常カテゴリでロードする。
                //前月の受け取りは無し
                //プレイヤのランクは無し
            }else{
                //今月あり
                if(DataManger.Instance.GAMEDATA.prevEntry!=null){
                    Debug.Log("前月のエントリー  あり");
                    //アワード判定を行う
                    isAwardable=true;

                    if(DataManger.Instance.GAMEDATA.currentEntry==null){
                        //ありえないデータ
                        Debug.LogError("今月のエントリー  無しはありえない！！！！！！");
                    }else{
                        //ある内は、今月のトーナメントに参加させない。
                        Debug.Log("今月のエントリー  あり");
                    }
                }else{
                    
                    Debug.Log("前月のエントリー  無し");
                    Debug.Log("今月のエントリー  あり");
                    //以前にエントリーしたデータがない
                    //ランキングは今月のカテゴリでロードする。
                    //前月の受け取りは無し

                }
            }
        }

       // if(Application.isEditor)DataManger.Instance.AddExp(1000);
        //前月データは必ず破棄する。ある内は、今月のトーナメントに参加させない。

        if(isAwardable){
            //Show Award
            //アワード可能なのは、１　前月が前月のもので、今が受け取り期間中で、前月にプッシュしていた場合のみ。
            //1 可能なら、通常のアワード
            //2 受け取り期間終了なら、その旨（前月出ないデータ　受け取り期間外のアクセス）
            //3 有効な条件を満たさず、規定に達していない場合。
            //降格　昇格を決める。　カテゴリの更新　
            InitMenus();
        }else{
            InitMenus();
        }
        //前月データを破棄するまでは入れない。

       
        if(!TimeManager.Instance.isSameDayLogin){
            //ログインボーナスを渡す
            //DataManger.Instance.GAMEDATA.earnedLoginBonus=true;
            //ES2.Save(DataManger.Instance.GAMEDATA.earnedLoginBonus,DataManger.Instance.DataFilename+"?tag=earnedLoginBonus");
        }
    }




    public void InitMenus(){
        
        AudioController.PlayMusic("music1");
        Show(menus[0]);


    }
    public BackHomeBtn backToHome;

    public void BackHome(){
        Debug.Log("BackHome");
        OnTabChanged("MAIN");
    }

    public void OnTabChanged(string menu){
        if(menu=="SHOP"){
            Debug.Log(" OnTabChanged "+menu);
            MenuManager.Instance.tab.SetTabState("SHOP");
            Show(menus[3]);
        }else if(menu=="TACLE"){
            Show(menus[1]);
        }else if(menu=="FISHNG"){
            Show(menus[2]);
        }else if(menu=="MAIN"){
            MenuManager.Instance.tab.ClearState();
            Show(menus[0]);

        }
    }

    public void OnTapLoginBonus(){
        OnTabChanged("SHOP");
    }


    public MenuContents currentShowingMenu;
    public MenuContents prevShowingMenu;
    public void Show(MenuContents menu){
        if(menu.Equals(currentShowingMenu)){
            Debug.Log("same menu return");
            return;
        }
        Debug.Log("Show"+menu.name);
        WaitAndCover.Instance.CoverAll(false);
        prevShowingMenu=currentShowingMenu;
        currentShowingMenu=menu;
        ShowCurrent();
    }
    public MenuContents[] menus;

    public void ShowCurrent(){
        if(prevShowingMenu!=null){
            
            prevShowingMenu.Hide();
        }else{
            currentShowingMenu.Show();
        }
    }

    public void OnHideCurrent(MenuContents menu){
        currentShowingMenu.Show();
    }

    public void OnShowedContents(MenuContents menu){
        Debug.Log("OnShowedContents");
        WaitAndCover.Instance.UnCoverAll();

    }




   
   
  
}


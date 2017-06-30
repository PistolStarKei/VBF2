using UnityEngine;
using System.Collections;

namespace Constants
{
    public static class BassBihaviour
    {
        public static readonly float[] bassSpeed_Stay=new float[4]{0.01f,0.03f,1.0f,3.0f};
        public static readonly float[] bassSpeed_Back=new float[4]{0.03f,0.12f,1.0f,3.0f};
        public static readonly float[] bassSpeed_Attention=new float[4]{0.09f,0.12f,3.0f,7.0f};
        public static readonly float[] bassSpeed_Chase=new float[4]{0.09f,0.12f,3.0f,7.0f};
        public static readonly float[] bassSpeed_Bite=new float[4]{0.3f,0.4f,3.0f,7.0f};
        public static readonly float posOffsetWhenScaleOne=1.3f;
        public static readonly float sizeScallingFactor=0.01f;
        public static readonly float minDistanceWhenChaseScaleOne=2.0f;
        public static readonly float maxDistanceWhenChaseScaleOne=3.0f;
        public static readonly float DistanceWhenChaseScale=1.2f;

        //STOP_GO,GO_STOP,MOVE_FLOAT,LIFT_FALL,
        //FLOAT_SUIMEN,FLOAT_GO,SINKING_BTM,SINKING_GO,
        //JUST_STILL,JUST_MOVE,JUST_FLOAT,JUST_SINKING,
        //NONE};
        //ストップ＆GO  GO＆ストップ　動いてからの浮き上がり リフト&フォール　
        //浮いて水面到着　浮いてからGO 着底　沈んでGO 
        //静止　動き中　浮き上がり中　沈み中

        //単純>変化　
        //ムーブ＞沈み>浮き>
        public static readonly float[] lureApealFactor=new float[13]{
            1.1f,1.08f,1.05f,1.2f,
            1.02f,1.08f,1.02f,1.12f,
            0.02f,0.8f,0.6f,0.8f,
            0.02f};

    }
	public static class Params
	{
        public static readonly string[] HeavyString=new string[7]{"ul","l","ml","m","mh","h","xh"};
        //重さ　UL L ML MH H XH (ルアーのロッドの)
        //しなりがある為、ノーシンカーワームなどの超軽量ルアーも投げやすい。小さい魚の引きを楽しめる。
        //ダウンショットリグやスモラバなどの軽量ルアーを繊細に動かすような釣りに最適。
        //ミノーやシャッド、ポッパーなどの軽量ハードルアーを使う際に最適。若干、反発するのでトゥイッチやジャークなどのアクションを付けると、ルアーが良い動きをする。
        //3/8oz程度のスピナーベイトやバイブレーションなど、ただ巻きするルアーを使う際に違和感なく使える最適。
        //1/2oz程度のスピナーベイトなど、ただ巻きするルアーを使う際に違和感なく使える。
        //テキサスリグやラバージグなどでハードカバーやウィードからすばやく魚を引き離す際に力を発揮する。
        //ビッグベイトなどの重いルアーを投げるような強さが必要な時に最適な硬さ。または、フロッグなどの強い合わせが必要なルアーを使う際に。

        //離れすぎると全部が落ちる　適合サイズは除外する　

        //飛距離;感度;適合サイズ;フッキング;強度
            
        public static readonly bool isShowDepth=true;


        public static readonly float kaishuDistance=3.0f;
        public static readonly float bassVisibleInDepth=-4f;
        public static readonly string fbPageURL="";
        public static readonly string tokushouPageURL="";
        public static readonly string StoreURL="";
        public static readonly string FeedBackURL="";
        public static readonly string tapjoyID="eeOJdXkoRiWbTGtiK570oAEC0pBN4W3iY6et6UOraxdRWNTY9IKiT3uwRy3y";
        public static readonly string tapjoyID_iOS="2wnxC2RSRs2JJIuG2XyGiQEB4dJp9UgPnpZQ6Ol0Ge8Q-61w6JV61qx7repf";

        //100G=110円
        //100000$=110円
        public static readonly long[] purchaseGold = new long[] { 330L,1200L,2500L,6500L};
        
        public static readonly string[] skus = new string[] { "com.pistolstarweb.info.fbf.pekatsuku.gold330","com.pistolstarweb.info.fbf.pekatsuku.gold1200",
            "com.pistolstarweb.info.fbf.pekatsuku.gold2500","com.pistolstarweb.info.fbf.pekatsuku.gold6500"};
        
        public static readonly int[] shoukin=new int[4]{5000,8000,80000,500000};
        public static readonly int maxLevel=99;
        public static readonly string[] Tab_SHOP=new string[]{"ROD","LINE","SOFT","HARD"};
        public static readonly string[] Tab_MAIN=new string[]{"TACLE","FISHNG","SHOP"};
        public static readonly int LikeBonus=1000;
        public static readonly int PVBonus=2;
        //1000dol =1Gold           
        public static readonly int DoltoGold=1000;
        public static readonly int lureEarlyAccess=4;
        public static readonly int rodEarlyAccess=10;
        //Sunny, Cloudy,Rain,HeavyRain
        public static readonly int[] WeightsOfWeather = new int[4] {65, 15, 15,5};

        public static readonly int[] MinStartKion = new int[12] {5,6,10,16,18,22,20,18,15,10,6,5};
        public static readonly int[] MaxStartKion = new int[12] {7,11,15,20,23,27,24,23,20,15,11,7};

        public static readonly float[] MaxWindPower = new float[9] {0.0f,0.5f,1.0f,1.5f,2.0f,2.5f,3.0f,3.5f,4.0f};
        public static readonly int[] WeightsOfMaxWindPower = new int[9] {40,17,10,10,10,7,3,2,1};
        //WeatherType{Sunny,Cloudy,Rain,HeavyRain}
        public static readonly int[] KionSa = new int[12] {12,11,10,9,8,7,6,7,8,9,10,12};
        public static readonly float[] KionFactor = new float[4] {1.0f,0.3f,0.2f,0.2f};

        //日の出　早朝　午前　昼　午後　日没
        public static readonly int[] TimeSpan1 = new int[6] {7,8,11,14,17,21};
        public static readonly int[] TimeSpan2 = new int[6] {6,8,11,14,17,22};
        public static readonly int[] TimeSpan3 = new int[6] {5,8,11,14,18,23};
        public static readonly int[] TimeSpan4 = new int[6] {5,8,11,14,19,24};
        public static readonly int[] TimeSpan5 = new int[6] {4,8,11,14,19,24};
        public static readonly int[] TimeSpan6 = new int[6] {4,8,11,14,18,24};
        public static readonly int[] TimeSpan7 = new int[6] {5,8,11,14,19,24};
        public static readonly int[] TimeSpan8 = new int[6] {5,8,11,14,18,23};
        public static readonly int[] TimeSpan9 = new int[6] {5,8,11,14,17,22};
        public static readonly int[] TimeSpan10 = new int[6] {6,8,11,14,17,22};
        public static readonly int[] TimeSpan11 = new int[6] {7,8,11,14,17,22};
        public static readonly int[] TimeSpan12 = new int[6] {7,8,11,14,16,21};

        public static int[] GetTimeSpan(int month){
            int[] timeSpan=Constants.Params.TimeSpan1;
            switch(month){
            case 1:
                timeSpan=Constants.Params.TimeSpan1;
                break;
            case 2:
                timeSpan=Constants.Params.TimeSpan2;
                break;
            case 3:
                timeSpan=Constants.Params.TimeSpan3;
                break;
            case 4:
                timeSpan=Constants.Params.TimeSpan4;
                break;
            case 5:
                timeSpan=Constants.Params.TimeSpan5;
                break;
            case 6:
                timeSpan=Constants.Params.TimeSpan6;
                break;
            case 7:
                timeSpan=Constants.Params.TimeSpan7;
                break;
            case 8:
                timeSpan=Constants.Params.TimeSpan8;
                break;
            case 9:
                timeSpan=Constants.Params.TimeSpan9;
                break;
            case 10:
                timeSpan=Constants.Params.TimeSpan10;
                break;
            case 11:
                timeSpan=Constants.Params.TimeSpan11;
                break;
            case 12:
                timeSpan=Constants.Params.TimeSpan12;
                break;
            }

            return timeSpan;
        }

        public static  int expToNext(int currrent){
            return currrent*100;
        } 
	}
    public enum Heavy{ul,l,ml,m,mh,h,xh};

    public static class LureDatas{
        public static readonly string[] itemTittles = new string[48]{"viba g","viba p","viba b","viba n","viba ex","vibo mr","vibo y","vibo p","vibo mg","vibo msp","crank c","crank cl","crank bp","crank ob","crank ex","crank mp","crank gsp","crank mg","minnow n","minnow t","minnow ex","minnow wy","krot  g","krot  y","krot  r","krot  sp","s-prus c","s-prus r","s-prus ex","s-prus wsp","shad m","shad t","shad r","shad ex","poppy a","poppy b","poppy o","poppy r","crank-s m","crank-s g","crank-s a","crank-s p","harrier x","harrier n","harrier g","harrier m","sonar x","sonar w"};
        public static readonly string[] itemSprites = new string[48]{"Bive1","Bive2","Bive3","Bive4","Bive6","DBive1","DBive2","DBive3","DBive4","DBived5","Dcrank1","Dcrank2","Dcrank3","Dcrank4","Dcrank5","Dcrank6","Dcrank7","Dcrank8","dMinor1","dMinor2","dMinor3","dMinor4","Flog1","Flog2","Flog3","Flog4","Jitter1","Jitter2","Jitter3","Jitter4","Minor1","Minor2","Minor3","Minor4","Popper1","Popper2","Popper3","Popper4","Scrank1","Scrank2","Scrank3","Scrank4","Shad1","Shad2","Shad3","Shad4","Spinner1","Spinner2"};
        public static readonly Heavy[] heavyCategory = new Heavy[48]{Heavy.m,Heavy.m,Heavy.m,Heavy.m,Heavy.m,Heavy.ml,Heavy.ml,Heavy.ml,Heavy.ml,Heavy.ml,Heavy.h,Heavy.h,Heavy.h,Heavy.h,Heavy.h,Heavy.h,Heavy.h,Heavy.h,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.h,Heavy.h,Heavy.h,Heavy.h,Heavy.ml,Heavy.ml,Heavy.ml,Heavy.ml,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.m,Heavy.m,Heavy.m,Heavy.m,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.mh,Heavy.xh,Heavy.xh};
        public static readonly int[] rangePivot = new int[48]{2,2,2,2,2,1,1,1,1,1,2,2,2,2,2,2,2,2,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0};
        public static readonly float[] rangeSize = new float[48]{0.73f,0.73f,0.73f,0.73f,0.73f,0.33f,0.33f,0.33f,0.33f,0.33f,0.38f,0.38f,0.38f,0.38f,0.38f,0.38f,0.38f,0.38f,0.44f,0.44f,0.44f,0.44f,0.07f,0.07f,0.07f,0.07f,0.07f,0.07f,0.07f,0.07f,0.35f,0.35f,0.35f,0.35f,0.07f,0.07f,0.07f,0.07f,0.37f,0.37f,0.37f,0.37f,0.24f,0.24f,0.24f,0.24f,1.00f,1.00f};
        public static readonly int[] itemNumsMax = new int[48]{3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,1,1,1,1,2,2,2,2,2,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,1,1};
        public static readonly int[] itemUnlockAt = new int[48]{23,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        public static readonly int[] itemPrices = new int[48]{15,15,15,15,15,10,10,10,10,10,12,12,12,12,12,12,12,12,15,15,15,15,11,11,11,11,15,15,15,15,16,16,16,16,12,12,12,12,14,14,14,14,15,15,15,15,17,17};
        public static readonly float[] graph_1 = new float[48]{40.00f,40.00f,40.00f,40.00f,40.00f,40.00f,40.00f,40.00f,40.00f,40.00f,60.00f,60.00f,60.00f,60.00f,60.00f,60.00f,65.00f,65.00f,50.00f,50.00f,50.00f,50.00f,100.00f,100.00f,100.00f,100.00f,50.00f,50.00f,50.00f,50.00f,30.00f,30.00f,30.00f,30.00f,40.00f,40.00f,40.00f,40.00f,55.00f,55.00f,55.00f,55.00f,50.00f,50.00f,50.00f,50.00f,80.00f,80.00f};
        public static readonly float[] graph_2 = new float[48]{65.00f,65.00f,65.00f,65.00f,65.00f,40.00f,40.00f,40.00f,40.00f,40.00f,80.00f,80.00f,80.00f,80.00f,80.00f,80.00f,80.00f,80.00f,65.00f,65.00f,65.00f,65.00f,60.00f,60.00f,60.00f,60.00f,70.00f,70.00f,70.00f,70.00f,60.00f,60.00f,60.00f,60.00f,65.00f,65.00f,65.00f,65.00f,50.00f,50.00f,50.00f,50.00f,65.00f,65.00f,65.00f,65.00f,100.00f,100.00f};
        public static readonly float[] graph_3 = new float[48]{0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,20.00f,20.00f,20.00f,20.00f,20.00f,20.00f,20.00f,20.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,20.00f,20.00f,20.00f,20.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,30.00f,30.00f};
        public static readonly float[] graph_4 = new float[48]{60.00f,60.00f,60.00f,60.00f,60.00f,90.00f,90.00f,90.00f,90.00f,90.00f,40.00f,40.00f,40.00f,40.00f,40.00f,40.00f,40.00f,40.00f,50.00f,50.00f,50.00f,50.00f,20.00f,20.00f,20.00f,20.00f,45.00f,45.00f,45.00f,45.00f,80.00f,80.00f,80.00f,80.00f,65.00f,65.00f,65.00f,65.00f,60.00f,60.00f,60.00f,60.00f,50.00f,50.00f,50.00f,50.00f,10.00f,10.00f};
        public static readonly float[] graph_5 = new float[48]{40.00f,40.00f,40.00f,40.00f,40.00f,50.00f,50.00f,50.00f,50.00f,50.00f,65.00f,65.00f,65.00f,65.00f,65.00f,65.00f,65.00f,65.00f,50.00f,50.00f,50.00f,50.00f,50.00f,50.00f,50.00f,50.00f,55.00f,55.00f,55.00f,55.00f,55.00f,50.00f,50.00f,50.00f,40.00f,40.00f,40.00f,40.00f,60.00f,60.00f,60.00f,60.00f,50.00f,50.00f,50.00f,50.00f,70.00f,70.00f};
        public static readonly string[] avility = new string[48]{"001000","001000","001001","001000","001000","000001","000000","000000","000001","000001","100000","100000","100000","100000","100000","100001","100001","100001","000000","000000","000000","000000","010001","010001","010001","010001","000100","000100","000100","000100","001001","001000","001000","001000","001100","001100","001100","001100","000001","000000","000000","000001","100000","100000","100000","100000","110001","110001"};
        public static readonly float[] colorRange = new float[48]{0.55f,0.48f,-0.69f,0.00f,-0.44f,1.00f,0.63f,0.98f,0.21f,1.00f,-0.24f,0.61f,-0.22f,0.58f,1.00f,-0.31f,-0.42f,-0.20f,0.00f,0.00f,-0.18f,0.68f,-0.36f,0.39f,0.51f,0.07f,0.00f,0.61f,-0.37f,0.00f,-0.59f,0.00f,0.63f,-0.49f,0.00f,0.00f,1.00f,0.41f,-0.62f,0.00f,0.00f,0.29f,-0.38f,0.00f,-0.31f,0.00f,1.00f,-1.00f};

    }

   
    public static class LineDatas{

        //1-5
        public static readonly int[] lineWidth=new int[12]{1,1,1,1,1,0,0,0,0,0,0,0};
        public static readonly int[] lineColor=new int[12]{1,1,1,1,1,0,0,0,0,0,0,0};


        public static readonly string[] itemTittles=new string[12]{"Line1","Line2","Line3","Line4","Line5","Line6","Line7","Line8","Line9","Line10","Line11","Line12"};
        public static readonly string[] itemSprites=new string[12]{"fl_b","fl_g","fl_w","fl_y","ny_b","ny_g",
            "ny_w","ny_y","pe_b","pe_g","pe_w","pe_y"};
        //ありなら、なければ””
        public static readonly int[] itemUnlockAt=new int[12]{1,1,3,3,5,10,20,30,50,60,60,60};
        public static readonly int[] itemPrices=new int[12]{10,20,30,40,50,60,70,80,90,100,100,100};

        //風の影響度
        public static readonly float[] graph_1=new float[12]{10.0f,20.0f,30.0f,40.0f,50.0f,60.0f,70.0f,80.0f,90.0f,100.0f,100.0f,100.0f};
        //飛距離
        public static readonly float[] graph_2=new float[12]{100.0f,20.0f,30.0f,40.0f,50.0f,60.0f,70.0f,80.0f,90.0f,100.0f,100.0f,100.0f};
        //強度
        public static readonly float[] graph_3=new float[12]{100.0f,50.0f,30.0f,40.0f,50.0f,60.0f,70.0f,80.0f,90.0f,100.0f,100.0f,100.0f};
        //フッキング
        public static readonly float[] graph_4=new float[12]{10.0f,20.0f,30.0f,40.0f,50.0f,60.0f,70.0f,80.0f,90.0f,100.0f,100.0f,100.0f};
        //感度
        public static readonly float[] graph_5=new float[12]{10.0f,20.0f,30.0f,40.0f,50.0f,60.0f,70.0f,80.0f,90.0f,100.0f,100.0f,100.0f};

        public static readonly Heavy[] heavyCategory=new Heavy[12]{
            Heavy.ul,Heavy.l,Heavy.ml,Heavy.mh,Heavy.h,
            Heavy.ul,Heavy.l,Heavy.ml,Heavy.mh,Heavy.h,
            Heavy.ul,Heavy.l
        };


        //コンタクト;根掛かり回避;ラトル;スプラッシュ;集魚剤  0=false 1=true   

        public static readonly string[] avility=new string[12]{"ライン説明1","ライン説明2","ライン説明1","ライン説明2","ライン説明3","ライン説明4","ライン説明5","ライン説明6","ライン説明7","ライン説明8","ライン説明9","ライン説明10"};

        //個数は必ず１個だけ
        public static readonly int[] itemNumsMax=new int[12]{1,1,1,1,1,1,1,1,1,1,1,1};

    }

    public static class RodsDatas{
        //ロッドの性能もあるがベイトは飛ばないがパワーある



        public static readonly string[] itemTittles=new string[5]{"Rods1","Rods2","Rods3","Rods4","Rods5"};
        public static readonly string[] itemSprites=new string[5]{"rod1","rod2","rod3","rod4","rod5"};
        //ありなら、なければ””
        public static readonly int[] itemUnlockAt=new int[5]{1,1,3,3,5};
        public static readonly int[] itemPrices=new int[5]{100,200,300,400,500};

        //硬さ テーパー　感度に比例
        public static readonly float[] graph_1=new float[5]{0.0f,0.0f,0.0f,0.0f,0.0f};
        //飛距離
        public static readonly float[] graph_2=new float[5]{100.0f,0.0f,0.0f,0.0f,0.0f};
        //強度
        public static readonly float[] graph_3=new float[5]{0.0f,0.0f,0.0f,0.0f,0.0f};
        //フッキング
        public static readonly float[] graph_4=new float[5]{0.0f,0.0f,0.0f,0.0f,0.0f};
        //感度
        public static readonly float[] graph_5=new float[5]{0.0f,0.0f,0.0f,0.0f,0.0f};
        public static readonly Heavy[]  heavyCategory=new Heavy[5]{
            Heavy.ul,Heavy.l,Heavy.ml,Heavy.mh,Heavy.h
        };


        //コンタクト;根掛かり回避;ラトル;スプラッシュ;集魚剤  0=false 1=true   

        public static readonly string[] avility=new string[5]{"FBF支給品のロッドです","軽いルアーに最適","ロッド説明4","ロッド説明5","ロッド説明6"};

        //個数は必ず１個だけ
        public static readonly int[] itemNumsMax=new int[5]{1,1,1,1,1};

    }

    public static class RigDatas{
        
        //ノーマルジグヘッド　ノーシンカー　ノーシンカーバーブド　　ジグヘッドバーブド　ヘビーバーブド　 ラバジ
        public static readonly string[] itemTittles=new string[6]{"Jig-N","Foock","Foock-Ex","Jig-E","Jig-P","Rubber"};
        public static readonly string[] itemSprites=new string[6]{"rig_Jighead3","rig_foock1","rig_foock2","Rig_Jighead","Rig_Jighead2","Rig_RubberJig"};

        public static readonly string[] RigID=new string[6]{"_NJ","_NS","_NSV","_JH","_JW","_RJ"};


        public static readonly Heavy[]  heavyCategory=new Heavy[6]{
            Heavy.m,Heavy.ul,Heavy.ul,Heavy.ml,Heavy.h,Heavy.xh
        };

        //ありなら、なければ””
        public static readonly int[] itemPrices=new int[6]{1,10,15,12,15,20};

        //TOP,MID,BTM
        public static readonly int[] rangePivot=new int[6]{1,0,0,1,2,2};

        //0.0f-1.0f
        public static readonly float[] rangeSize=new float[6]{0.5f,0.5f,0.5f,0.5f,0.45f,0.33f};


        //ノーマルジグヘッド　ノーシンカー　ノーシンカーバーブド　　ジグヘッドバーブド　ヘビーバーブド　 ラバジ

        //アピール　ソフトと＋して100二なる
        public static readonly float[] graph_5=new float[6]{
            5.0f,50.0f,50.0f,10.0f,
            10.0f,20.0f};
        
        //飛距離   ソフトと＋して100二なる
        public static readonly float[] graph_2=new float[6]{
            40.0f,10.0f,10.0f,50.0f,
            60.0f,70.0f};
        
        //適合サイズ　ソフトと＋して100二なる
        public static readonly float[] graph_3=new float[6]{
            0.0f,0.0f,0.0f,5.0f,
            10.0f,20.0f};




        //カバー耐性 0-100
        public static readonly float[] graph_1=new float[6]{
            20.0f,20.0f,50.0f,60.0f,
            60.0f,70.0f};
        
        //フッキング
        public static readonly float[] graph_4=new float[6]{
            40.0f,50.0f,35.0f,30.0f,
            30.0f,20.0f};
        

        // コンタクト;根掛かり回避;ラトル;スプラッシュ;集魚剤;反射板
        public static readonly string[] avilitys=new string[6]{
            "000000","000000","010000","010000",
            "010000","010000"};
    }


  
    public enum SoftLureType{Curly,Shad,Streight,Zari};
    public static class SoftLureDatas{


        //ノーマルジグヘッド　ノーシンカー　ノーシンカーバーブド　　ジグヘッドバーブド　ヘビーバーブド　 ラバジ
        public static readonly int[] itemUnlockAt=new int[15]{
            1,1,3,3,
            5,10,20,
            30,50,60,80,
            90,100,100,100};


        //０は最初から持っている。　全て同じにする。購入したら全てに使える
        //最大５個まで
        public static readonly string[] AvaillableRig=new string[15]{
            "0;1;2;3;4;5","0;1;2;3;4;5", "0;1;2;3;4;5", "0;1;2;3;4;5",
            "0;3;4;5","0;3;4;5", "0;3;4;5",
            "0;1;2;3","0;1;2;3", "0;1;2;3", "0;1;2;3",
            "0;1;2;3;4;5","0;1;2;3;4;5", "0;1;2;3;4;5", "0;1;2;3;4;5",};
        public static readonly int[] itemNumsMax = new int[15]{2,2,2,2,2,2,2,2,2,3,3,3,2,2,2};
        public static readonly string[] itemTittles = new string[15]{"Curly wm","Curly y","Curly g","Curly w","S-tail mb","S-tail w","S-tail p","S-tail by","Worm bg","Worm pr","Worm wm","Worm ns","Craw n","Craw sp","Craw ns"};
        public static readonly string[] itemSprites = new string[15]{"Curly1","Curly2","Curly3","Curly4","Shad1","Shad2","Shad3","Shad4","Streight1","Streight2","Streight3","Streight4","Zari1","Zari2","Zari3"};
        public static readonly int[] itemPrices = new int[15]{15,15,15,15,20,20,20,20,12,12,12,12,20,20,20};
        public static readonly string[] avilitys = new string[15]{"000000","000000","000000","000000","000010","000010","000010","000010","000000","000000","000000","000000","000010","000010","000010"};
        public static readonly float[] colorRange = new float[15]{-0.63f,0.35f,-0.41f,1.00f,-1.00f,1.00f,0.37f,0.39f,-0.70f,0.26f,0.00f,0.00f,0.00f,0.00f,0.00f};


        public static readonly float[] graph_2 = new float[15]{20.00f,20.00f,20.00f,20.00f,5.00f,5.00f,05.00f,5.00f,10.00f,10.00f,10.00f,10.00f,30.00f,30.00f,30.00f};
        public static readonly float[] graph_3 = new float[15]{0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,0.00f,10.00f,10.00f,10.00f,10.00f,10.00f,10.00f,10.00f};
        public static readonly float[] graph_5 = new float[15]{35.00f,35.00f,35.00f,35.00f,25.00f,25.00f,25.00f,25.00f,25.00f,25.00f,25.00f,25.00f,30.00f,30.00f,30.00f};

        public static readonly SoftLureType[] itemType=new SoftLureType[15]{
            SoftLureType.Curly,SoftLureType.Curly,SoftLureType.Curly,SoftLureType.Curly,
            SoftLureType.Shad,SoftLureType.Shad,SoftLureType.Shad,SoftLureType.Shad,
            SoftLureType.Streight,SoftLureType.Streight,SoftLureType.Streight,SoftLureType.Streight,
            SoftLureType.Zari,SoftLureType.Zari,SoftLureType.Zari};
        
       
       
    }
    public static class RigPositionDefines{

        public static bool GetShouldShowBack(int rigNum,SoftLureType lureType){

            switch(lureType){
            case SoftLureType.Curly:
                return shouldShowBack1;
                break;
            case SoftLureType.Shad:
                return shouldShowBack2;
                break;
            case SoftLureType.Streight:
                return shouldShowBack3;
                break;
            case SoftLureType.Zari:
                return shouldShowBack4;
                break;
            }
            return false;
        }
        public static Vector3 GetRigPos(int rigNum,SoftLureType lureType){

            switch(lureType){
            case SoftLureType.Curly:
                return rigPos1[rigNum];
                break;
            case SoftLureType.Shad:
                return rigPos2[rigNum];
                break;
            case SoftLureType.Streight:
                return rigPos3[rigNum];
                break;
            case SoftLureType.Zari:
                return rigPos4[rigNum];
                break;
            }
            return Vector3.zero;
        }
        public static Vector3 GetRigRot(int rigNum,SoftLureType lureType){
            switch(lureType){
            case SoftLureType.Curly:
                return rigRot1[rigNum];
                break;
            case SoftLureType.Shad:
                return rigRot2[rigNum];
                break;
            case SoftLureType.Streight:
                return rigRot3[rigNum];
                break;
            case SoftLureType.Zari:
                return rigRot4[rigNum];
                break;
            }
            return Vector3.zero;
        }
        //カーリータイプ 
        //ノーマルジグヘッド　ノーシンカー　ノーシンカーバーブド　　ジグヘッドバーブド　ジグヘッドヘビーバーブド　
        public static readonly Vector3[] rigPos1=new Vector3[6]{
            new Vector3(-111.0f,-25.69998f,0.0f),
            new Vector3(-111.0f,-1.14f,0.0f),
            new Vector3(-68.28f,-45.39f,0.0f),
            new Vector3(-121.5f,-29.6f,0.0f),
            new Vector3(-112.25f,-12.7f,0.0f),
            new Vector3(-112.25f,-27.91f,0.0f)};
        public static readonly Vector3[] rigRot1=new Vector3[6]{
            new Vector3(0.0f,0.0f,0.0f),
            new Vector3(0.0f,0.0f,0.0f),
            new Vector3(0.0f,0.0f,1.0f),
            new Vector3(0.0f,0.0f,0.3f),
            new Vector3(0.0f,0.0f,0.3f),
            new Vector3(0.0f,0.0f,0.3f)};
        public static readonly bool shouldShowBack1=true;
        //シャッドタイプ 
        public static readonly Vector3[] rigPos2=new Vector3[6]{
            new Vector3(-93.6f,11.6f,0.0f),
            new Vector3(-93.6f,32.3f,0.0f),
            new Vector3(-50.52f,2.99f,0.0f),
            new Vector3(-84.2f,15.06f,0.0f),
            new Vector3(-84.2f,21.12f,0.0f),
            new Vector3(-84.2f,3.8f,0.0f)};
        public static readonly Vector3[] rigRot2=new Vector3[6]{
            new Vector3(0.0f,0.0f,-1.100006f),
            new Vector3(0.0f,0.0f,-1.100006f),
            new Vector3(0.0f,0.0f,-1.100006f),
            new Vector3(0.0f,0.0f,-2.700012f),
            new Vector3(0.0f,0.0f,0.3f),
            new Vector3(0.0f,0.0f,0.3f)};
        public static readonly bool shouldShowBack2=true;
        //ストレートタイプ 
        public static readonly Vector3[] rigPos3=new Vector3[6]{
            new Vector3(-113.3f,-3.1f,0.0f),
            new Vector3(-110.48f,22.6f,0.0f),
            new Vector3(-73.98f,3.8f,0.0f),
            new Vector3(-120.62f,10.53f,0.0f),
            new Vector3(-120.8f,4.85f,0.0f),
            new Vector3(-109.48f,-7.8f,0.0f)};
        public static readonly Vector3[] rigRot3=new Vector3[6]{
            new Vector3(0.0f,0.0f,0.0f),
            new Vector3(0.0f,0.0f,0.0f),
            new Vector3(0.0f,0.0f,-6.630005f),
            new Vector3(0.0f,0.0f,-7.890015f),
            new Vector3(0.0f,0.0f,1.1f),
            new Vector3(0.0f,0.0f,1.1f)};
        public static readonly bool shouldShowBack3=true;
        //ザリタイプ 
        public static readonly Vector3[] rigPos4=new Vector3[6]{
            new Vector3(-106.6f,-53.91f,0.0f),
            new Vector3(-106.6f,-35.48f,0.0f),
            new Vector3(-86.08f,-65f,0.0f),
            new Vector3(-115.6f,-51.3f,0.0f),
            new Vector3(-97.5f,-37.5f,0.0f),
            new Vector3(-97.5f,-59.91f,0.0f)};
        public static readonly Vector3[] rigRot4=new Vector3[6]{
            new Vector3(0.0f,0.0f,13.0f),
            new Vector3(0.0f,0.0f,13.0f),
            new Vector3(0.0f,0.0f,13f),
            new Vector3(0.0f,0.0f,9.700001f),
            new Vector3(0.0f,0.0f,9.700001f),
            new Vector3(0.0f,0.0f,13.7f)};
        public static readonly bool shouldShowBack4=false;
    }
}

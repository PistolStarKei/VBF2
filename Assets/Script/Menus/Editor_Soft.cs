using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Editor_Soft : MonoBehaviour {

   

    public bool ValidateTittle(string str){
        foreach(string str2 in itemTittles){
            if(str2==str){
                return false;
            }
        }
        return true;
    }
    public UIInput input;

    public List<string>  itemTittles=new List<string>();
    public List<string> itemSprites=new List<string>();


    public List<int> itemNumsMax=new List<int>();
    public List<int> itemUnlockAt=new List<int>();
    public List<int> itemPrices=new List<int>();

    public List<float> graph_1=new List<float>();
    public List<float> graph_2=new List<float>();
    public List<float> graph_3=new List<float>();
    public List<float> graph_4=new List<float>();
    public List<float> graph_5=new List<float>();

    public List<string> avility=new List<string>();
    public List<float> colorRange=new List<float>();

    public int dataRange=48;
    void Start()
    {
        
        if(!ES2.Exists(dataFile+"?tag=itemTittles")){
            if(useNumbering){

                itemTittles.AddRange(new string[dataRange]);
                ES2.Save(itemTittles,dataFile+"?tag=itemTittles");
                itemSprites.AddRange(new string[dataRange]);
                ES2.Save(itemSprites,dataFile+"?tag=itemSprites");



                itemNumsMax.AddRange(new int[dataRange]);
                ES2.Save(itemNumsMax,dataFile+"?tag=itemNumsMax");

                itemUnlockAt.AddRange(new int[dataRange]);
                ES2.Save(itemUnlockAt,dataFile+"?tag=itemUnlockAt");

                itemPrices.AddRange(new int[dataRange]);
                ES2.Save(itemPrices,dataFile+"?tag=itemPrices");

                graph_1.AddRange(new float[dataRange]);
                graph_2.AddRange(new float[dataRange]);
                graph_3.AddRange(new float[dataRange]);
                graph_4.AddRange(new float[dataRange]);
                graph_5.AddRange(new float[dataRange]);
                ES2.Save(graph_1,dataFile+"?tag=graph_1");
                ES2.Save(graph_2,dataFile+"?tag=graph_2");
                ES2.Save(graph_3,dataFile+"?tag=graph_3");
                ES2.Save(graph_4,dataFile+"?tag=graph_4");
                ES2.Save(graph_5,dataFile+"?tag=graph_5");

                avility.AddRange(new string[dataRange]);
                ES2.Save(avility,dataFile+"?tag=avility");
                colorRange.AddRange(new float[dataRange]);
                ES2.Save(colorRange,dataFile+"?tag=colorRange");
            }else{
                SaveData();
            }

        }
        Debug.Log("Start 1");
        LoadData();
        Debug.Log("Start 2");
        UpdateData(currentData);
        Debug.Log("Start 3");
    }

    public void LoadData(){
        itemTittles=ES2.LoadList<string>(dataFile+"?tag=itemTittles");
        itemSprites=ES2.LoadList<string>(dataFile+"?tag=itemSprites");
        itemNumsMax=ES2.LoadList<int>(dataFile+"?tag=itemNumsMax");
        itemUnlockAt=ES2.LoadList<int>(dataFile+"?tag=itemUnlockAt");
        itemPrices=ES2.LoadList<int>(dataFile+"?tag=itemPrices");
        graph_1=ES2.LoadList<float>(dataFile+"?tag=graph_1");
        graph_2=ES2.LoadList<float>(dataFile+"?tag=graph_2");
        graph_3=ES2.LoadList<float>(dataFile+"?tag=graph_3");
        graph_4=ES2.LoadList<float>(dataFile+"?tag=graph_4");
        graph_5=ES2.LoadList<float>(dataFile+"?tag=graph_5");
        avility=ES2.LoadList<string>(dataFile+"?tag=avility");
        colorRange=ES2.LoadList<float>(dataFile+"?tag=colorRange");
        numbering=new int[colorRange.Count];

    }

    public void SaveBtn(){
        AffectData();
        SaveData();

        UpdateData(currentData);
    }
    public void SaveData(){
        ES2.Save(itemTittles,dataFile+"?tag=itemTittles");
        ES2.Save(itemSprites,dataFile+"?tag=itemSprites");

        ES2.Save(itemNumsMax,dataFile+"?tag=itemNumsMax");
        ES2.Save(itemUnlockAt,dataFile+"?tag=itemUnlockAt");

        ES2.Save(itemPrices,dataFile+"?tag=itemPrices");
        ES2.Save(graph_1,dataFile+"?tag=graph_1");
        ES2.Save(graph_2,dataFile+"?tag=graph_2");
        ES2.Save(graph_3,dataFile+"?tag=graph_3");
        ES2.Save(graph_4,dataFile+"?tag=graph_4");
        ES2.Save(graph_5,dataFile+"?tag=graph_5");

        ES2.Save(avility,dataFile+"?tag=avility");
        ES2.Save(colorRange,dataFile+"?tag=colorRange");


    }
    public UISprite sp;
    public UIInput maxHas;
    public UIInput price;
    public UIInput unlockAt;
    public UISlider[] graphVals;

    public UILabel numLbl;
    void UpdateData(int cur){

        Debug.Log("Start 2-1"+itemTittles.Count);


        numLbl.text="No."+(cur+1).ToString();
        input.value=itemTittles[cur];
        Debug.Log("Start 2-2");
        sp.spriteName=itemSprites[cur];
        Debug.Log("Start 2-2");
        maxHas.value=itemNumsMax[cur].ToString();
        price.value=itemPrices[cur].ToString();
        unlockAt.value=itemUnlockAt[cur].ToString();

        //コンタクト;根掛かり回避;ラトル;スプラッシュ;集魚剤;反射板  0=false 1=true   
        //"0101000"

        setCK(avility[cur]);
        //0 =-1  0.5=0 1.0=1.0
        colorRangeSL.value=HenkanColToRange(colorRange[cur]);
        graphVals[0].value=graph_1[cur]/100.0f;
        graphVals[1].value=graph_2[cur]/100.0f;
        graphVals[2].value=graph_3[cur]/100.0f;
        graphVals[3].value=graph_4[cur]/100.0f;
        graphVals[4].value=graph_5[cur]/100.0f;

        Debug.Log("Start 2-3");

        // 5 50
        // 2 80
        //3 30
    }



    public string  GetCK(){
        string dat="";
        for(int i=0;i<avilTs.Length;i++){
            if(i==4){
                dat+=avilTs[i].value?"1":"0";
            }else{
                dat+="0";
            }
           
        }

        return dat;
    }
    public UIToggle[] avilTs;
    public void setCK(string data){
        bool[] dat=new bool[6]{false,false,false,false,false,false};
        if(data!=""){
            dat=PSGameUtils.StringToBoolArray(data);
            if(dat.Length!=avilTs.Length){
                Debug.LogError("not match ");
                dat=new bool[6]{false,false,false,false,false,false};
            }
        }
        for(int i=0;i<avilTs.Length;i++){
            avilTs[i].value=dat[i];
        }
    }

    public float HenkanColToRange(float vals){
        float val=0.0f;
        if(vals>=0.0f){
            //0.5-1.0
            val=0.5f+(vals/2.0f);
        }else{
            //0.0f - 0.5f
            //-1 0
            val=0.5f-(Mathf.Abs(vals)/2.0f);
        }

        return val;
    }
    public float HenkanRangeToCol(float vals){
        float val=0.0f;
        if(vals>=0.5f){
            val=(vals*2.0f)-1.0f;
        }else{
            val=-1.0f+(vals*2.0f);
        }

        return val;
    }

    public void Test(){
    }
    public UISlider colorRangeSL;

    public float[] valInRig=new float[5];
    void AffectData(){
        if(ValidateTittle(input.value))itemTittles[currentData]=input.value;
        itemSprites[currentData]=sp.spriteName;

        itemNumsMax[currentData]=int.Parse(maxHas.value);
        itemPrices[currentData]=int.Parse(price.value);
        itemUnlockAt[currentData]=int.Parse(unlockAt.value);
        graph_1[currentData]=(graphVals[0].value)*100.0f;
        graph_2[currentData]=(graphVals[1].value)*100.0f;
        graph_3[currentData]=(graphVals[2].value)*100.0f;
        graph_4[currentData]=(graphVals[3].value)*100.0f;
        graph_5[currentData]=(graphVals[4].value)*100.0f;
        colorRange[currentData]= HenkanRangeToCol(colorRangeSL.value);
        avility[currentData]=GetCK();

    }

    public bool useNumbering=false;
    public int[] numbering=new int[3];
    public void GenData(){
        string datas="";
        datas+=Generator("itemTittles",itemTittles);
        Debug.Log(datas);
        datas+=Environment.NewLine;

        datas+=Generator("itemSprites",itemSprites);
        Debug.Log(datas);
        datas+=Environment.NewLine;

        datas+=GeneratorInt("itemNumsMax",itemNumsMax);
        Debug.Log(datas);
        datas+=Environment.NewLine;

        datas+=GeneratorInt("itemUnlockAt",itemUnlockAt);
        Debug.Log(datas);
        datas+=Environment.NewLine;

        datas+=GeneratorInt("itemPrices",itemPrices);
        Debug.Log(datas);
        datas+=Environment.NewLine;


        datas+=GeneratorFloat("graph_2",graph_2);
        Debug.Log(datas);
        datas+=Environment.NewLine;

        datas+=GeneratorFloat("graph_3",graph_3);
        Debug.Log(datas);
        datas+=Environment.NewLine;


        datas+=GeneratorFloat("graph_5",graph_5);
        Debug.Log(datas);
        datas+=Environment.NewLine;

        datas+=Generator("avility",avility);
        Debug.Log(datas);
        datas+=Environment.NewLine;

        datas+=GeneratorFloat("colorRange",colorRange);
        Debug.Log(datas);
        datas+=Environment.NewLine;



    }

    string GeneratorFloat(string paramName,List<float> data){
        string str="public static readonly float[] "+paramName;
        if(data.Count<=0){
            str+=" = new float[1]";
            return str;
        }
        str+=" = new float["+data.Count+"]{";

        if(useNumbering){
            int i=1;
            for(int e=0;e<data.Count;e++){


                if(i==data.Count){
                    str+=""+data[Array.IndexOf(numbering,i-1)].ToString("F2")+"f};";
                }else{
                    str+=""+data[Array.IndexOf(numbering,i-1)].ToString("F2")+"f,";
                }

                i++;
            }
        }else{
            int i=1;
            foreach(float strd in data){
                if(i==data.Count){
                    str+=""+strd.ToString("F2")+"f};";
                }else{
                    str+=""+strd.ToString("F2")+"f,";
                }

                i++;
            }
        }

        return str;

    }


    string GeneratorInt(string paramName,List<int> data){
        string str="public static readonly int[] "+paramName;
        if(data.Count<=0){
            str+=" = new int[1]";
            return str;
        }
        str+=" = new int["+data.Count+"]{";


        if(useNumbering){
            int i=1;
            for(int e=0;e<data.Count;e++){


                if(i==data.Count){

                    str+=""+data[Array.IndexOf(numbering,i-1)]+"};";
                }else{
                    str+=""+data[Array.IndexOf(numbering,i-1)]+",";
                }

                i++;
            }
        }else{
            int i=1;
            foreach(int strd in data){
                if(i==data.Count){
                    str+=""+strd+"};";
                }else{
                    str+=""+strd+",";
                }

                i++;
            }
        }



        return str;

    }


    string Generator(string paramName,List<Constants.Heavy> data){
        string str="public static readonly Heavy[] "+paramName;
        if(data.Count<=0){
            str+=" = new Heavy[1]";
            return str;
        }
        str+=" = new Heavy["+data.Count+"]{";


        if(useNumbering){
            int i=1;
            for(int e=0;e<data.Count;e++){


                if(i==data.Count){
                    str+="Heavy."+data[Array.IndexOf(numbering,i-1)].ToString()+"};";
                }else{
                    str+="Heavy."+data[Array.IndexOf(numbering,i-1)].ToString()+",";
                }

                i++;
            }
        }else{
            int i=1;
            foreach(Constants.Heavy strd in data){
                if(i==data.Count){
                    str+="Heavy."+strd.ToString()+"};";
                }else{
                    str+="Heavy."+strd.ToString()+",";
                }

                i++;
            }
        }


        return str;

    }

    string Generator(string paramName,List<string> data){
        string str="public static readonly string[] "+paramName;
        if(data.Count<=0){
            str+=" = new string[1]";
            return str;
        }
        str+=" = new string["+data.Count+"]{";

        if(useNumbering){
            int i=1;
            for(int e=0;e<data.Count;e++){


                if(i==data.Count){
                    str+="\""+data[Array.IndexOf(numbering,i-1)]+"\"};";
                }else{
                    str+="\""+data[Array.IndexOf(numbering,i-1)]+"f\",";
                }

                i++;
            }
        }else{
            int i=1;
            foreach(string strd in data){
                if(i==data.Count){
                    str+="\""+strd+"\"};";
                }else{
                    str+="\""+strd+"\",";
                }

                i++;
            }
        }


        return str;

    }

    public void clearDatas(){
        ES2.Delete(dataFile);
    }

  

    public int currentData=0;
    public UIButton pre;
    public UIButton next;
    public void NextData(){
        if(currentData<itemTittles.Count-1){
            next.enabled=true;
            if(currentData==itemTittles.Count-2)next.enabled=false;
            currentData++;
            UpdateData(currentData);
        }else{
            next.enabled=false;
        }
        if(currentData>0)pre.enabled=true;
    }
    public void PreData(){
        if(currentData>=1){
            pre.enabled=true;
            if(currentData==1)pre.enabled=false;
            currentData--;
            UpdateData(currentData);
        }else{
            pre.enabled=false;
        }
        if(currentData<itemTittles.Count-1)next.enabled=true;
    }


    public string dataFile="Dat.txt";
}

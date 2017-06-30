using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaderGraph : MonoBehaviour {

    public WMG_Axis_Graph graph;
    public WMG_Radar_Graph conf;

   List<string> tittleData=new List<string>();

    public UILabel[] tittleLabels;

    public void InitRader(string[] tittleData,Transform parent){
        this.tittleData.Clear();
        this.tittleData.AddRange(tittleData);
        SetLabels(this.tittleData);
        if( graph.lineSeries.Count==0)conf.refreshGraph();
    }

    public void SetRaderData(float[] data){
        Debug.Log("SetRaderData"+data.Length);
        if(tittleData.Count!=data.Length){
            return;
        }
        dataNums.Clear();
        for(int i=0;i<data.Length;i++){
            if(data[i]<0.0f)data[i]=0.0f;
            if(data[i]>100.0f)data[i]=100.0f;
        }

        dataNums.AddRange(data);
        SetNums(dataNums,null);

    }
    List<float> dataNums=new List<float>();
    List<float> dataNums2=new List<float>();
    public void SetRaderData(float[] data,float[] data2){

        if(tittleData.Count!=data2.Length){
            return;
        }
        if(data==null && data2==null){
            return;
        }

        dataNums.Clear();
        dataNums.AddRange(data);
        dataNums2.Clear();
        dataNums2.AddRange(data2);
        SetNums(dataNums,dataNums2);

    }



    void SetLabels(List<string> tittleData){
        int num=0;
        foreach(UILabel lb in tittleLabels){
            lb.text="";
        }
        for(int i=0;i<tittleData.Count;i++){
            tittleLabels[i].text=tittleData[i];
        }
    }

    void SetNums(List<float> data,List<float> data2){
       
        if(data!=null){
            //Debug.Log("SetNums 2"+graph.lineSeries.Count+""+(0 + conf.numGrids));
            WMG_Series aSeries = graph.lineSeries[0 + conf.numGrids].GetComponent<WMG_Series>();

            aSeries.pointValues = graph.GenRadar(data, conf.offset.x, conf.offset.y, conf.degreeOffset);
        }

        if(data2!=null){
            graph.lineSeries[1 + conf.numGrids].gameObject.SetActive(true);
            WMG_Series aSeries = graph.lineSeries[1 + conf.numGrids].GetComponent<WMG_Series>();

            aSeries.pointValues = graph.GenRadar(data2, conf.offset.x, conf.offset.y, conf.degreeOffset);
        }else{
            graph.lineSeries[1 + conf.numGrids].gameObject.SetActive(false);
        }
    }



}

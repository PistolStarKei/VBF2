using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public enum Info_IC{Normal,Warning}
public class InfoLauncher : MonoBehaviour {

	// Use this for initialization
    public UIGrid grid;
    string poolName = "GUI_Effects";

    public Transform info;
    public int[] positions;
    List<InfoItem> items=new List<InfoItem>();
    public void UpdateGrid(){
        if(items.Count>0){
            int i=0;
            foreach(InfoItem ite in items){
                ite.transform.localPosition=new Vector3(0.0f,positions[i],0.0f);
                i++;
            }
        }
    }
    public void Despawn(InfoItem trans){
        items.Remove(trans);
        UpdateGrid();
        PoolManager.Pools[poolName].Despawn(trans.transform);
       
    }
    public void DespawAll(){
        PoolManager.Pools[poolName].DespawnAll();
        items.Clear();
    }

    Transform current ;
    public void AddInfo(string info){
        if(items.Count>=positions.Length){
            return;
        }
        current = PoolManager.Pools[poolName].Spawn(this.info);
        current.transform.parent=grid.transform;
        current.gameObject.layer=LayerMask.NameToLayer("GUI");
        current.transform.localRotation=Quaternion.identity;
        current.transform.localPosition=new Vector3(0.0f,positions[items.Count],0.0f);
        current.transform.localScale=Vector3.one;

        current.gameObject.GetComponent<InfoItem>().SetItems(info,"ic_infos",this,4.0f);
        items.Add(current.gameObject.GetComponent<InfoItem>());
        UpdateGrid();
    }

}

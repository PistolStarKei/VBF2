using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public class ItemListDynamic : MonoBehaviour {

    void Start(){
        ShowIfNot(btmIndecater,false);
        ShowIfNot(topIndecater,false);
    }
    public Transform listContainer;
    public void AffectGrid(){
        listContainer.gameObject.GetComponent<UIGrid>().Reposition();
    }
    public UIScrollView view;
    public GameObject topIndecater;
    public GameObject btmIndecater;
    public List<LureItemContainer> items=new List<LureItemContainer>();

    public int itemCount=0;
    void FixedUpdate(){
        
        //0 top 1 btm
        if(view.gameObject.activeSelf && itemCount>10){
            if( view.verticalScrollBar.value<=0.1f){
                //top 下がまだある
                ShowIfNot(topIndecater,false);
                ShowIfNot(btmIndecater,true);
            }else{
                ShowIfNot(topIndecater,true);
                if(view.verticalScrollBar.value>=0.9f){
                    //btm
                    ShowIfNot(btmIndecater,false);
                }else{
                    ShowIfNot(btmIndecater,true);
                }
            }
        }else{
            ShowIfNot(btmIndecater,false);
            ShowIfNot(topIndecater,false);
        }

    }
    void ShowIfNot(GameObject go,bool isShow){
        if(go.activeSelf!=isShow)NGUITools.SetActive(go,isShow);
    }

   
    string poolName = "GUI_Effects";
    public Transform Row;
    public void DespawnRow(int i){
        if(items.Count>i){
            items[i].isUsed=false;
            PoolManager.Pools[poolName].Despawn(items[i].gameObject.transform);
        }
    }
    public void DespawAll(){
        PoolManager.Pools[poolName].DespawnAll();
        items.Clear();
    }
    Transform current ;
    public void SpawnRow(){
        current = PoolManager.Pools[poolName].Spawn(Row);
        current.transform.parent=listContainer;
        current.gameObject.layer=LayerMask.NameToLayer("GUI");
        current.transform.localRotation=Quaternion.identity;
        current.transform.localPosition=Vector3.zero;
        current.transform.localScale=Vector3.one;
        items.Add(current.gameObject.GetComponent< LureItemContainer>());
    }
}

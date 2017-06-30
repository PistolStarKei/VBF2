using UnityEngine;
using System.Collections;

public class SetRange : MonoBehaviour {
    public void Show(){
        if(!gameObject.activeSelf) NGUITools.SetActive(gameObject,true);
    }
    public void Hide(){
        if(gameObject.activeSelf)NGUITools.SetActive(gameObject,false);
    }
    public UIScrollBar rangeSlider;
    public void SetRangeValue(LureRangePivot pivot,float percent){
        switch(pivot){
        case LureRangePivot.TOP:
            rangeSlider.value=0.0f;
            break;
        case LureRangePivot.MID:
            rangeSlider.value=0.5f;
            break;
        case LureRangePivot.BTM:
            rangeSlider.value=1.0f;
            break;
        }
        rangeSlider.barSize=percent;
    }

}

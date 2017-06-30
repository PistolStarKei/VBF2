using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum GUIColor{Base1,Base2,Base3,Accent1,Accent2,Accent3,LabelBase1,LabelBase2,LabelAccent1,LabelAccent2};
public class GUIColors : PS_SingletonBehaviour<GUIColors>  {


    public void Onyes(bool isYes){
        Debug.Log("OnYes"+isYes);
    }
    public bool needToSingleton=false;
    void DestroyAll(){
        foreach (Transform childTransform in gameObject.transform) Destroy(childTransform.gameObject);
        Destroy(gameObject);
    }

	void Awake(){
        if(needToSingleton){
            if(this != Instance)
            {
                DestroyAll();
                return;
            }
        }
        DontDestroyOnLoad(this.gameObject);
		cols.Add(gui_BaseColor1);
		cols.Add(gui_BaseColor2);
		cols.Add(gui_BaseColor3);
		cols.Add(gui_AccentColor1);
		cols.Add(gui_AccentColor2);
        cols.Add(gui_AccentColor3);
		cols.Add(gui_LabelBaseColor1);
		cols.Add(gui_LabelBaseColor2);
		cols.Add(gui_LabelAccentColor1);
		cols.Add(gui_LabelAccentColor2);
	}
	List<Color> cols=new List<Color>();

	public Color GetColor(GUIColor col){
		if((int)col<cols.Count){
			return cols[(int)col];
		}else{
			return Color.white;
		}

	}
	public Color gui_BaseColor1;
	public Color gui_BaseColor2;
	public Color gui_BaseColor3;
	public Color gui_AccentColor1;
	public Color gui_AccentColor2;
    public Color gui_AccentColor3;
	public Color gui_LabelBaseColor1;
	public Color gui_LabelBaseColor2;

	public Color gui_LabelAccentColor1;
	public Color gui_LabelAccentColor2;

    public Color gray;



    public Color GUI_Tab_Selected;
    public Color GUI_Tab_UnSelected;
    public Color GUI_Tab_Selected_Lb;
    public Color GUI_Tab_UnSelected_Lb;

    public Color GUI_List_Selected;
    public Color GUI_List_Availlable;
    public Color GUI_List_UnAvaillable;

    public Color GUI_SPAvility_None;
    public Color GUI_Scroll_Accent;
    public Color GUI_Scroll_Accent2;

    public Color MatchColor_Minus;
    public Color  MatchColor_Plus;
}

using UnityEngine;
using System.Collections;

public class SetNumsGUI : MonoBehaviour {

    public void Show(bool isShow){
        if(isShow!=gameObject.activeSelf)NGUITools.SetActive(gameObject,isShow);
    }

    public UISprite[] nums;
    public void SetNums(int num,int max){
        SetNumContents(max);
            int num2=0;
            foreach(UISprite sp in nums){
                if(num2<num){
                    sp.enabled=true;
                    sp.color=Color.white;
                }else{
                    if(num2>max-1){
                        sp.enabled=false;
                        sp.color=GUIColors.Instance.gray;
                    }else{
                        sp.enabled=true;
                        sp.color=GUIColors.Instance.gray;
                    }

                }
                num2++;
            }
    }

    void SetNumContents(int max){
        if(max<=0){
            Show(false);
        }else{
            int num=0;
            int[] size5=new int[2]{55,37};
            float add=40.0f;
            Show(true);
            switch(max){
                case 1:
                    size5=new int[2]{210,37};
                    add=101.0f;
                    foreach(UISprite sp in nums){
                        sp.width=size5[0];
                        sp.height=size5[1];
                        sp.transform.localPosition=new Vector3(-80.0f+(add*num),0.0f,0.0f);
                        num++;
                    }
                    break;
                case 2:
                    size5=new int[2]{115,37};
                    add=101.0f;

                    foreach(UISprite sp in nums){
                        sp.width=size5[0];
                        sp.height=size5[1];
                        sp.transform.localPosition=new Vector3(-80.0f+(add*num),0.0f,0.0f);
                        num++;
                    }
                    break;
                case 3:
                    size5=new int[2]{80,37};
                    add=69.0f;

                    foreach(UISprite sp in nums){
                        sp.width=size5[0];
                        sp.height=size5[1];
                        sp.transform.localPosition=new Vector3(-80.0f+(add*num),0.0f,0.0f);
                        num++;
                    }
                    break;
                case 4:
                    size5=new int[2]{64,37};
                    add=50.0f;
                   
                    foreach(UISprite sp in nums){
                        sp.width=size5[0];
                        sp.height=size5[1];
                        sp.transform.localPosition=new Vector3(-80.0f+(add*num),0.0f,0.0f);
                        num++;
                    }
                    break;
                case 5:
                    foreach(UISprite sp in nums){
                        sp.width=size5[0];
                        sp.height=size5[1];
                        sp.transform.localPosition=new Vector3(-80.0f+(add*num),0.0f,0.0f);
                        num++;
                    }
                    break;
            }
        }
    }
        
        
    public void SetNums(int num,int has,int equip,int max){
        if(num==(has-equip)){
            SetNums(num,max);
            return;
        }
        //５　　2(3 Lost) 1 ５
        //W B P P P 
        //5 2(3 Lost) 0 5
        //W W P P P

        SetNumContents(max);
        int num2=0;
        foreach(UISprite sp in nums){
            if(num2<num){
                if(num2<has){
                    if(num2<(has-equip)){
                        sp.enabled=true;
                        sp.color=Color.white;

                    }else{
                        sp.enabled=true;
                        sp.color=new Color(24.0f/255.0f,147.0f/255.0f,206.0f/255.0f);
                    }

                }else{
                    //ロストしている
                    sp.enabled=true;
                    sp.color=Color.white;
                    sp.color=new Color(1.0f,0.0f,185.0f/255.0f);
                }

            }else{
                if(num2>max-1){
                    sp.enabled=false;
                    sp.color=GUIColors.Instance.gray;
                }else{
                    sp.enabled=true;
                    sp.color=GUIColors.Instance.gray;
                }

            }
            num2++;
        }
    }

}

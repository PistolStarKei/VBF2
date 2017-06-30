using UnityEngine;
using System.Collections;

public class BassAnimeEvenet : MonoBehaviour {

    public void OnJumped(){
       
        if(bassAI){
            bassAI.OnJumped();
        }else{
            bassAItest.OnJumped();
        }
    }

    public void OnJumpTop(){
        if(bassAI){
            bassAI.OnJumpTop();
        }else{
            bassAItest.OnJumpTop();
        }
    }
    public void Jump(){
        if(Random.value<=0.5f){

            animator.SetTrigger("jump1");
        }else{
            animator.SetTrigger("jump2");
        }
    }
    public BassAI bassAI;
    public BassAITest bassAItest;
    public Animator animator;

    public void OneShotAnime(string name){

        animator.SetTrigger(name);
    }
    public void StartKubifuri(bool isOn){
        if(animator.GetBool("kubifuri")==isOn)return;
        if(isOn){
            animator.SetBool("kubifuri",true);
        }else{
            animator.SetBool("kubifuri",false);
        }
    }
    void Update() {
       
        animator.SetFloat("speed",speed);
    }
    public void SetSpeed(float speed){
        this.speed=speed;
    }
    float speed=0.0f;
}

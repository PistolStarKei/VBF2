using UnityEngine;
using System.Collections;
using PathologicalGames;

public class FollowingTargetHamon : MonoBehaviour {

    public ParticleScaler scaler;
    public ParticleSystem sys;
    void SetHamonScale(float scale){
        if(scaler.particleScale!=scale)scaler.particleScale=scale;
    }
    void SetHamonRot(float rot){
        if(sys.startRotation!=rot)sys.startRotation=rot;
    }

    public void Dead(){
        PoolManager.Pools["Non_GUI_Effects"].Despawn(transform);
    }
    public Transform target;
    public float yOffset=0.25f;
    public float ScaleOffset=0.25f;
    public void Follow(Transform target,float offset){
        this.offset=offset;
        this.target=target;
        SetHamonScale(ScaleOffset*target.lossyScale.x);
        transform.parent=target.transform;
        transform.localScale=Vector3.one;
        transform.localRotation=Quaternion.identity;
        transform.localPosition=new Vector3(0.0f,0.0f+yOffset,0.0f);
        time=0.0f;
        StartCoroutine(CheckIfAlive ());
    }
    float time=0.0f;
    public float mintimeToDead=0.0f;
    Vector3 vec;
    float offset=0.3f;
    IEnumerator CheckIfAlive ()
    {

        if(target==null)yield break;
        float time=Time.timeSinceLevelLoad;
        Debug.Log("CheckIfAlive");
        while(true)
        {
            yield return null;
            vec.x=target.transform.position.x;
            vec.y=0.0f+yOffset;
            vec.z=target.transform.position.z;
            transform.position=vec;
            SetHamonRot(  0.0172f* transform.eulerAngles.y);
            if(target.transform.position.y+offset<WaterPlane.Instance.transform.position.y)
            {
                Debug.Log("たまサイドう");
                if(Time.timeSinceLevelLoad-time> mintimeToDead){
                    Invoke("Dead",1.0f);

                    yield break;
                }

            }
        }
    }

}

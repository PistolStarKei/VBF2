using UnityEngine;
using System.Collections;
using PathologicalGames;

[RequireComponent(typeof(ParticleSystem))]
public class PS_AutoDestructShuriken : MonoBehaviour {

    public float deadIn=3.0f;
    void OnEnable()
    {
        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive ()
    {
        float time=Time.timeSinceLevelLoad;
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            if(!GetComponent<ParticleSystem>().IsAlive(true))
            {
                PoolManager.Pools["Non_GUI_Effects"].Despawn(transform);
                yield break;
            }
            if(Time.timeSinceLevelLoad-time>deadIn){
                PoolManager.Pools["Non_GUI_Effects"].Despawn(transform);
                yield break;
            }
        }
    }
}

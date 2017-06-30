using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kalagaan
{
    /// Compute collision
    /// require a VertExmotionSensor component
    //[RequireComponent(typeof(VertExmotionSensor))]
    public class VertExmotionColliderBase : MonoBehaviour
    {

        [System.Serializable]
        public class CollisionZone
        {
            public Vector3 positionOffset = Vector3.zero;
            public float radius = 1f;
            [HideInInspector]
            public Vector3 collisionVector = Vector3.zero;
            [HideInInspector]
            public RaycastHit[] hits;
        }


        [HideInInspector]
        public string className = "VertExmotionCollider";

        ///Layer mask for physic interactions
        /// 
        public LayerMask m_layerMask;

        ///Smooth factor
        /// 0 : no smooth
        /// 1 : realistic reaction with inertia
        /// 10 : low reaction to physic
        public float m_smooth = 1f;

        ///List of CollisionZone
        ///add several zone to fit mesh volume
        public List<CollisionZone> m_collisionZones = new List<CollisionZone>();
        float m_collisionScaleFactor = 1f;
        public List<Collider> m_ignoreColliders = new List<Collider>();

        VertExmotionSensorBase m_sensor;





        void Start()
        {
            m_sensor = GetComponentInParent<VertExmotionSensorBase>();
            if (m_sensor == null)
            {
                enabled = false;
                Debug.LogError("VertExmotion collider must be a child of a sensor");
            }

            //check on larger sphere to get better precision for unity 4
            if (Application.unityVersion.StartsWith("4"))
                m_collisionScaleFactor = 100f;
        }


        /// <summary>
        /// Ignore collider from colliding with collision zone
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="ignore"></param>
        public void IgnoreCollision( Collider collider, bool ignore )
        {
            if (ignore && !m_ignoreColliders.Contains(collider))
                m_ignoreColliders.Add(collider);

            if( !ignore && m_ignoreColliders.Contains(collider))
                m_ignoreColliders.Remove(collider);
        }


        void FixedUpdate()
        {
            Vector3 target = Vector3.zero;
            float count = 0;            
            
            for (int i = 0; i < m_collisionZones.Count; ++i)
            {
                UpdateCollisionZone(m_collisionZones[i]);
                if (m_collisionZones[i].collisionVector != Vector3.zero)
                {
                    target += m_collisionZones[i].collisionVector;
                    count++;
                }
            }

            if (count > 1)
                target /= count;

            //set target
            m_smooth = Mathf.Clamp(m_smooth, 0, 10f);
            if (m_smooth > 0f)
                m_sensor.m_collision = Vector3.Lerp(m_sensor.m_collision, target, Time.deltaTime * (10f / m_smooth));
            else
                m_sensor.m_collision = target;

        }






        public void UpdateCollisionZone(CollisionZone cz)
        {
            //return;

            Vector3 hitNormal = Vector3.zero;
            float hitDist = 0;
            int count = 0;
            float radius = cz.radius * VertExmotionBase.GetScaleFactor(transform);

            Vector3 collisionCenter = transform.TransformPoint(cz.positionOffset);// transform.position + transform.rotation * cz.positionOffset;

            cz.collisionVector = Vector3.zero;

            if (Physics.CheckSphere(collisionCenter, radius, m_layerMask.value))
            {
                cz.collisionVector = Vector3.zero;

                //				//test
                //				Collider[] hitColliders = Physics.OverlapSphere(collisionCenter, m_collisionZones[0].radius, m_layerMask);
                //				int j = 0;
                //				while (j < hitColliders.Length)
                //				{
                //					Debug.Log("collide " + hitColliders[j].name);
                //					hitColliders[j].ClosestPointOnBounds( transform.position );
                //					j++;
                //				}


                for (int a = 0; a < 6; ++a)//WIP : check collision on each axis
                //int a = 0;
                {
                    Vector3 dir = Vector3.zero;
                    switch (a)
                    {
                        case 0: dir = m_sensor.transform.forward; break;
                        case 1: dir = -m_sensor.transform.forward; break;
                        case 2: dir = m_sensor.transform.up; break;
                        case 3: dir = -m_sensor.transform.up; break;
                        case 4: dir = m_sensor.transform.right; break;
                        case 5: dir = -m_sensor.transform.right; break;
                    }


                    //cz.hits = Physics.SphereCastAll(collisionCenter - dir * radius / 2f, radius * m_collisionScaleFactor, dir, radius / 2f, m_layerMask.value, QueryTriggerInteraction.Collide);
                    cz.hits = Physics.SphereCastAll(collisionCenter - dir * radius, radius * m_collisionScaleFactor, dir, radius, m_layerMask.value, QueryTriggerInteraction.Collide);



                    for (int i = 0; i < cz.hits.Length; ++i)
                    {
                        //Debug.Log("collision "+Vector3.Distance (cz.hits [i].point, collisionCenter));

                        //Debug.DrawLine(cz.hits[i].collider.ClosestPointOnBounds(transform.position), transform.position);

                        if (m_ignoreColliders.Contains(cz.hits[i].collider))
                            continue;

                        Vector3 hitPos = cz.hits[i].point;

                        float debugoffset = .01f;
#if KVTM_DEBUG
                        Debug.DrawLine(cz.hits[i].collider.ClosestPointOnBounds(transform.position) + Vector3.up * a * debugoffset, transform.position + Vector3.up * a * debugoffset, Color.blue);
#endif
                        //Vector3 hitPos = cz.hits [i].collider.ClosestPointOnBounds(transform.position);

                        //if (Vector3.Distance (cz.hits [i].point, collisionCenter) < radius  )
                        if (Vector3.Distance(hitPos, collisionCenter) < radius)
                        {
                            /*
                            hitNormal = cz.hits[i].normal.normalized;
                            //hitNormal += (collisionCenter-cz.hits [i].point ).normalized;
                            hitNormal += (collisionCenter - hitPos).normalized;
                            hitNormal.Normalize();
                            */

                            hitNormal = -(hitPos - collisionCenter).normalized;


                            //if (Vector3.Dot ((transform.position - hits [i].point).normalized, hits [i].normal) >= 0)
                            if (Vector3.Dot((collisionCenter - hitPos).normalized, -dir) > 0)
                            {
                                //hitDist += (radius - Vector3.Distance(hitPos, collisionCenter));
                                hitDist += (radius - Vector3.Distance(hitPos, collisionCenter)) / 6f;
                                count++;

                            }/*
                            else
                            {
                                if (cz.hits[i].collider as SphereCollider == null)
                                {
                                    hitNormal = (-dir + cz.hits[i].normal).normalized;
                                    hitDist = radius;
                                }
                                else
                                {
                                    hitNormal = cz.hits[i].normal.normalized;
                                    hitDist = radius - Vector3.Distance(hitPos, collisionCenter);
                                }
                            }*/
                            //count++;
                        }/*
					else
					{
						hitDist = cz.radius;
						hitNormal = (-m_sensor.transform.forward + cz.hits[i].normal).normalized;
					}*/
                    }

                    if (count > 0)
                    {
                        cz.collisionVector += hitNormal.normalized * hitDist * 2f;
                    }/*
                    else
                    {
                        cz.collisionVector += hitNormal.normalized * radius * 2f;
                    }*/
                }

                //cz.collisionVector /= 6f;


            }
            else
            {
                cz.collisionVector = Vector3.zero;
            }
        }



        /*
        void OnDrawGizmos()
        {
            for (int id = 0; id < m_collisionZones.Count; ++id)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position + m_collisionZones[id].positionOffset, m_collisionZones[id].radius);
                Gizmos.color = Color.red;

                if (m_collisionZones[id].hits != null)
                    for (int i = 0; i < m_collisionZones[id].hits.Length; ++i)
                    {
                        Gizmos.DrawSphere(m_collisionZones[id].hits[i].point, .01f);
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(m_collisionZones[id].hits[i].point, m_collisionZones[id].hits[i].normal);
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(m_collisionZones[id].hits[i].point, m_collisionZones[id].hits[i].point + (transform.position + m_collisionZones[id].positionOffset - m_collisionZones[id].hits[i].point).normalized);
                    }

            }

        }*/
    }
}

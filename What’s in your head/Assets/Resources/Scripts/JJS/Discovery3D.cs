using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discovery3D : MonoBehaviour
{
    public bool gizmoOn;

    [SerializeField]
    [Range(0f, 50f)]
    private float _discoveryRadius;

    [Range(0f, 180f)]
    [SerializeField] private float _viewAngle;

    [Range(-180f, 180f)]
    [SerializeField] private float _viewDirection;

    [SerializeField] private LayerMask viewTargetMask;

    private Collider[] hitColliders;
    private RaycastHit[] rayHits;
    private Ray[] ray = new Ray[3];

    public Collider[] HitColliders { get { return hitColliders; } }

    public List<GameObject> targetObj;

    void Update()
    {
        DiscoveryTargetBool3D();
    }
    void OnDrawGizmos()
    {   
        if(gizmoOn)
            OnDrawGizmosRay();
    }

    void OnDrawGizmosRay()
    {
        Vector3 originPos = transform.position;
        Vector3 RightDir = AngleToDirY(-_viewAngle + _viewDirection);
        Vector3 LeftDir = AngleToDirY(_viewAngle + _viewDirection);
        Vector3 lookDir = transform.forward;

        ray[0].origin = transform.position;
        ray[0].direction = LeftDir;
        ray[1].origin = transform.position;
        ray[1].direction = RightDir;
        ray[2].origin = transform.position;
        ray[2].direction = lookDir;

        Gizmos.DrawWireSphere(transform.position, _discoveryRadius);
        Debug.DrawRay(ray[0].origin, ray[0].direction * _discoveryRadius, Color.red);
        Debug.DrawRay(ray[1].origin, ray[1].direction * _discoveryRadius, Color.red);
        Debug.DrawRay(ray[2].origin, ray[2].direction * _discoveryRadius, Color.green);

        //RightDir = AngleToDirY(-_viewAngle + _viewDirection);
        //LeftDir = AngleToDirY(_viewAngle + _viewDirection);
        //lookDir = transform.forward;

        //ray[0].origin = transform.position;
        //ray[0].direction = LeftDir;
        //ray[1].origin = transform.position;
        //ray[1].direction = RightDir;
        //ray[2].origin = transform.position;
        //ray[2].direction = lookDir;

        //Debug.DrawRay(ray[0].origin, ray[0].direction * _discoveryRadius, Color.red);
        //Debug.DrawRay(ray[1].origin, ray[1].direction * _discoveryRadius, Color.red);
        //Debug.DrawRay(ray[2].origin, ray[2].direction * _discoveryRadius, Color.green);
    }

    private Vector3 AngleToDirY(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.y) * Mathf.Deg2Rad;//호도법 계산식
        return new Vector3(Mathf.Sin(-radian), 0, Mathf.Cos(-radian));
    }

    private Vector3 AngleToDirX(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.x) * Mathf.Deg2Rad;//호도법 계산식
        return new Vector3(0, Mathf.Sin(radian), Mathf.Cos(radian));
    }

    public bool DiscoveryTargetBool3D()
    {
        Vector3 originPos = transform.position;
        int count = 0;
        hitColliders = Physics.OverlapSphere(transform.position, _discoveryRadius, viewTargetMask);
        if (hitColliders.Length!=0)
        {
            targetObj.Clear();
        }
        for (int index = 0; index < hitColliders.Length; index++)
        {
            if (hitColliders[index] != null)
            {
                Vector3 targetPos = hitColliders[index].transform.position;
                Vector3 dir = (targetPos - originPos).normalized;//(타겟의 위치값 - 오브젝트의 위치값).백터값
                Vector3 lookDir = AngleToDirY(_viewDirection);

                // float angle = Vector3.Angle(lookDir, dir)
                // 아래 두 줄은 위의 코드와 동일하게 동작함. 내부 구현도 동일
                float dot = Vector3.Dot(lookDir, dir);
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (angle <= _viewAngle)
                {
                    count++;
                    {
                        targetObj.Add(hitColliders[index].gameObject);
                        Debug.DrawLine(originPos, targetPos, Color.red);
                    }
                }
            }
        }

        if (count == 0)
        {
            return false;
        }
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Effect
{
    public class MotionTrail : MonoBehaviour
    {
        [Header("�ܻ� ����")] [SerializeField] int trailCount = 50;
        [Header("�ܻ� �����ִ� �ð�")] [SerializeField] float trailRemainTime = 0.15f;
        [Header("�ܻ� ��ȯ �ð� ����")] [SerializeField] float trailSpawnTime = 0.01f;
        [Header("�ܻ� �޽� ������")] [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
        [Header("������ ���׸���")] [SerializeField] Material mat;

        GameObject TrailContainer;
        readonly Queue<GameObject> objQueue = new();

        int curSpawnIndex = 0;
        Mesh curMesh;

        WaitForSeconds remainWs;
        WaitForSeconds spawnWs;

        

        private void Awake()
        {
            remainWs = new(trailRemainTime);
            spawnWs = new(trailSpawnTime);
            TrailContainer = new("TrailContainer");
            for (int i= 0 ; i<trailCount ; ++i)
            {
                GameObject go = new("Trail" + i);
                go.transform.parent = TrailContainer.transform;
                go.SetActive(false);
                go.AddComponent<MeshRenderer>();
                go.GetComponent<MeshRenderer>().sharedMaterial = mat;
                go.AddComponent<MeshFilter>();
                objQueue.Enqueue(go);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(nameof(MakeTrail));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            objQueue.Clear();
            for (int i=0 ; i<trailCount ; ++i)
            {
                GameObject go = TrailContainer.transform.GetChild(i).gameObject;
                go.SetActive(false);
                objQueue.Enqueue(go);
            }
        }

        // �ܻ� ����
        IEnumerator MakeTrail()
        {
            while (curSpawnIndex < trailCount)
            {
                if (objQueue.Count != 0)
                {
                    GameObject go = objQueue.Dequeue();
                    go.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    go.SetActive(true);

                    if(curMesh == null)
                        curMesh = new();
                    skinnedMeshRenderer.BakeMesh(curMesh);
                    go.GetComponent<MeshFilter>().mesh = curMesh;
                    StartCoroutine(DeleteTrail(go));
                    yield return spawnWs;
                    if (++curSpawnIndex >= trailCount)
                        curSpawnIndex = 0;
                }
                else
                    yield return null;
            }
        }

        // �ܻ� ����
        IEnumerator DeleteTrail(GameObject trail)
        {
            yield return remainWs;
            trail.SetActive(false);
            objQueue.Enqueue(trail);
            yield break;
        }
    }
}

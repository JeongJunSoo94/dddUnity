using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS.BT
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }
        [HideInInspector] public State state = State.Running;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public ObjectInfo objectInfo;
        //[HideInInspector] public Blackboard blackboard;
        //[HideInInspector] public aiŬ���� agent;
        [TextArea] public string description;

        public State Update()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state == State.Failure || state == State.Success)
            {
                OnStop();
                started = false;
            }
            return state;
        }

        public void Abort()
        {
            BehaviourTree.Traverse(this, (node) => {
                node.started = false;
                node.state = State.Running;
                node.OnStop();
            });
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}

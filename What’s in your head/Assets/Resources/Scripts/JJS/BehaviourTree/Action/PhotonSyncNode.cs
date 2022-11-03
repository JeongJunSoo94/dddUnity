using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JJS;
using Photon.Pun;

namespace JJS.BT
{
    public class PhotonSyncNode : ActionNode
    {
        public bool local;
        public int syncChange;
        protected override void OnStart()
        {

            if (objectInfo.photonView.IsMine)
            {
                if (local)
                {
                    objectInfo.syncIndex = syncChange;
                }
            }

        }
   
        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (objectInfo.syncIndex== syncChange)
            {
                return State.Success;
            }
            return State.Failure;
        }

    }
}

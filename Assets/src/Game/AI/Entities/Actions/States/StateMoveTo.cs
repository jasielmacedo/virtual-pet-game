using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Game.AI.FSM;
using Game.AI.FSM.BasicStates;

namespace Game.AI.Entities.Actions.States
{
    public class StateMoveTo : MoveState
    {
        public override void EnterState()
        {
            if (Owner.Params.ContainsKey("destination"))
            {
                Owner.SetWorldDestination((Vector3)Owner.Params["destination"]);
                base.EnterState();
            }
            else
            {
                Owner.ChangeState<StateExecute>();
            }
        }

        public override void ExitState()
        {
            base.ExitState();

            if (Owner.Params.ContainsKey("destination"))
            {
                Owner.Params.Remove("destination");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.AI.FSM;

namespace Game.AI.Entities.Actions.States
{
    public class StateIdle : FSMState
    {
        public override string Id { get { return "idle"; } }

        public override void Tick(float deltaTime)
        {
            Owner.ChangeState<StateMoveTo>();
        }
    }
}

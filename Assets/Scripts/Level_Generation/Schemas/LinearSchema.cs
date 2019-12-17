using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Level_Generation.Schemas
{
    public class LinearSchema : LevelSchema
    {
        #region Metadata

        [SerializeField]
        protected Direction primaryDirection;

        #endregion

        #region Generation Methods

        protected virtual void Step()
        {
            if (resolvingConflict)
            {
                ResolveConflictStep();
            }
            else if (!done)
            {
                BuildStep();
            }
        }

        #endregion

        #region MonoBehaviour

        protected override void Awake()
        {
            directionMask = new List<Direction>();
            directionMask.Add(primaryDirection);
            directionMask.Add(Directionf.GetRightDirection(primaryDirection));
            base.Awake();
        }

        protected void FixedUpdate()
        {
            if(!done) Step();
        }

        #endregion
    }
}

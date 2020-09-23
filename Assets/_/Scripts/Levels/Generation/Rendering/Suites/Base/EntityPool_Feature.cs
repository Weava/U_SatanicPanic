using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Base
{
    /// <summary>
    /// Special Entity pool that is always a required singleton
    /// </summary>
    public class EntityPool_Feature : MonoBehaviour
    {
        public List<SuiteEntity> entities = new List<SuiteEntity>();

        #region Meta Properties

        [HideInInspector]
        public int smallestEntitySize
        {
            get
            {
                return entities.OrderBy(o => o.entitySize).Select(s => s.entitySize).First();
            }
        }

        #endregion Meta Properties
    }
}
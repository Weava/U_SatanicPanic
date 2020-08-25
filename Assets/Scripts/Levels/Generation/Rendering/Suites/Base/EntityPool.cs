using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Base
{
    /// <summary>
    /// A collection of suite entities that fulfill a similar role in the suite generation process
    /// </summary>
    public class EntityPool : MonoBehaviour
    {
        /// <summary>
        /// Only one entity from this pool can be used at once
        /// </summary>
        public bool singleton;

        /// <summary>
        /// Requires that at least once entity is used in here.
        /// </summary>
        public bool required;

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

        #endregion
    }
}

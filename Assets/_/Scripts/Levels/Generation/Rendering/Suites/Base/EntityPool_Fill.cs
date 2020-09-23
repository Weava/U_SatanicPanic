using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Base
{
    /// <summary>
    /// Container for partial entities for filling a suite's remaining scaffold pieces
    /// </summary>
    public class EntityPool_Fill : MonoBehaviour
    {
        [Header("Floor")]
        public SuiteEntity_Scaffold floor_main;
        public SuiteEntity_Scaffold floor_connector;
        public SuiteEntity_Scaffold floor_column;

        [Header("Wall")]
        public SuiteEntity_Scaffold wall_main;
        public SuiteEntity_Scaffold wall_connector;

        [Header("Ceiling")]
        public SuiteEntity_Scaffold ceiling_main;
        public SuiteEntity_Scaffold ceiling_connector;
        public SuiteEntity_Scaffold ceiling_column;
    }
}

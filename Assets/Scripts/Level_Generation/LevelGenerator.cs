using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Level_Generation
{
    /// <summary>
    /// Manage a set of level schemas to produce the full level
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        protected bool done = false;
        protected bool breakUpdate = false;

        [SerializeField]
        protected string seed;

        protected LevelSchema currentSchema;

        [SerializeField]
        protected List<LevelSchema> schemaChain;

        protected List<LevelSchema> schemaChainProgress;
        protected List<LevelSchema> schemaInstances = new List<LevelSchema>();

        #region Monobehaviour

        private void Start()
        {
            InitRandomSeed();
            InitLevelGeneration();
        }

        private void FixedUpdate()
        {
            if(!breakUpdate)
                {
                if (currentSchema != null || schemaChainProgress.Count > 0)
                {
                    if (currentSchema.IsDoneGenerating())
                    {
                        if (schemaChainProgress.Count > 0)
                        {
                            InitNextSchema();
                        }
                        else
                        {
                            currentSchema = null; //Done generating
                            done = true;
                        }
                    }
                } else if (done)
                {
                    Debug.Log("Done generating level.");
                    breakUpdate = true;
                }
            }
        }

        #endregion

        #region Getters and Setters

        public Bounds GetBoundsOfLevel()
        {
            return schemaInstances.GetBoundsOfLevel(); //TODO: List of schemas do not have references to schemaRooms, null instead, fix this
        }

        #endregion

        #region Private Methods

        private void InitRandomSeed()
        {
            if (seed != "")
            { Random.InitState(ConvertSeed(seed)); }
            else
            { Random.InitState((int)DateTime.Now.Ticks); }
        }

        private void InitLevelGeneration()
        {
            schemaChainProgress = schemaChain;

            if(currentSchema == null)
            {
                if(schemaChain.Count > 0)
                {
                    var selectedSchema = schemaChainProgress[0];
                    schemaChainProgress.RemoveAt(0);
                    currentSchema = Instantiate(selectedSchema, new Vector3(), new Quaternion());
                    currentSchema.SetLevelGenerator(this);
                    schemaInstances.Add(currentSchema);
                }
                else
                {
                    throw new Exception("Schema chain must have at least one schema");
                }             
            }
        }

        private void InitNextSchema()
        {
            var nextSchema = schemaChainProgress[0];
            schemaChainProgress.RemoveAt(0);

            var rootRoom = currentSchema.GetRootForNextSchema();
            
            nextSchema.SetRootRoom(rootRoom);
            currentSchema = Instantiate(nextSchema, new Vector3(), new Quaternion());
            currentSchema.SetLevelGenerator(this);
            schemaInstances.Add(currentSchema);
        }

        private int ConvertSeed(string seed)
        {
            var temp = "";

            foreach (char c in seed)
            {
                temp += ((int)c).ToString();
            }

            return (int)long.Parse(temp);
        }

        #endregion
    }
}

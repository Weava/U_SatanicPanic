using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Rendering.Suites.Base;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Suites
{
    /// <summary>
    /// A suite that has a collection of entities and attempt to use them to form the room's geometry
    /// </summary>
    public class Suite_Basic : Suite
    {
        public List<EntityPool> entityPools = new List<EntityPool>();

        //[HideInInspector]

        public override bool ValidateRoom(LevelRoom room)
        {
            var cellCount = CellCollection.GetByRoom(room.roomId).Count;

            //A required entity pool wouldn't even fit
            if (entityPools.Any(x => cellCount < x.smallestEntitySize && x.required)) return false;

            return true;
        }

        public override bool Build(LevelRoom room)
        {
            foreach (var entityPool in entityPools)
            {
                var success = false;

                foreach (var entity in entityPool.entities.OrderByDescending(o => o.roomType))
                {
                    success = RenderEntity(room, entity);
                    if (success && entityPool.singleton)
                    { break; }
                    RollbackRenderContainer();
                }

                if (!success && entityPool.required) return false;
            }

            BuildFill(room);

            return true;
        }
    }
}
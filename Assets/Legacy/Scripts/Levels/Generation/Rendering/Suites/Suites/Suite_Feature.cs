using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Rendering.Suites.Base;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Suites
{
    /// <summary>
    /// Suite that contains a distinctive feature, usually an objective room.
    /// </summary>
    public class Suite_Feature : Suite
    {
        public EntityPool_Feature featurePool;

        public List<EntityPool> entityPools = new List<EntityPool>();

        public override bool ValidateRoom(LevelRoom room)
        {
            var cellCount = CellCollection.GetByRoom(room.roomId).Count;

            if (cellCount < featurePool.smallestEntitySize) return false;

            //A required entity pool wouldn't even fit
            if (entityPools.Any(x => cellCount < x.smallestEntitySize && x.required)) return false;

            return true;
        }

        public override bool Build(LevelRoom room)
        {
            var success = false;

            ResetRenderContainer(room);

            foreach (var entity in featurePool.entities)
            {
                success = RenderEntity(room, entity);
                if (success) { break; }
                RollbackRenderContainer();
            }

            foreach (var entityPool in entityPools)
            {
                foreach (var entity in entityPool.entities)
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
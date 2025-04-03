using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace DestroyRift.assets
{
    public class RiftEvent: ModSystem
    {
        ICoreServerAPI sapi;
        IWorldAccessor world;
        private Dictionary<int, Rift> Rifts;

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);

            sapi = api;
            world = api.World;
            api.Event.RegisterGameTickListener(detectPlayer, 2000);
            
        }

        private void detectPlayer(float dt)
        {
            Rifts = sapi.ModLoader.GetModSystem<ModSystemRifts>().riftsById;
            Rift curRift = null;

            foreach (IServerPlayer plr in sapi.World.AllOnlinePlayers){

                if (plr.ConnectionState != EnumClientState.Playing) continue;

                EntityPos plrPos = plr.Entity.Pos;
                var invName = plr.InventoryManager.GetInventoryName("character");
                IInventory invId = plr.InventoryManager.GetInventory(invName);

                foreach (Rift rifts in Rifts.Values)
                {
                    if (rifts.Position.DistanceTo(plrPos.XYZ) <= 2f + rifts.Size)
                    {
                        curRift = rifts;
                        var size = rifts.Size;
                    }
                    else
                    {
                        continue;
                    }
                }

                ItemSlot Item = invId.ElementAt<ItemSlot>(6);
                

                if (curRift == null) continue;

                if (Item.Itemstack.Item.Code == "game:clothes-neck-gear-amulet-temporal")
                {
                    
                    if (curRift.Size > 0)
                    {
                        for (int i = 0; i < world.Rand.Next(10, 20); i++)
                        {

                            String entityName;
                            switch (world.Rand.Next(4))
                            {
                                case 1:
                                    entityName = "normal";
                                    break;
                                case 2:
                                    entityName = "deep";
                                    break;
                                case 3:
                                    entityName = "tainted";
                                    break;
                                default:
                                    entityName = "normal";
                                    break;
                            }
                            EntityProperties drifter = sapi.World.GetEntityType(new AssetLocation("game:drifter-" + entityName));
                            Entity entity = world.ClassRegistry.CreateEntity(drifter);
                            if (entity != null)
                            {
                                entity.ServerPos.X = curRift.Position.X + world.Rand.Next(8);
                                entity.ServerPos.Y = curRift.Position.Y;
                                entity.ServerPos.Z = curRift.Position.Z + world.Rand.Next(8);
                                entity.Pos.SetFrom(entity.ServerPos);
                                world.SpawnEntity(entity);
                            }
                        }
                        curRift.DieAtTotalHours = 0;
                        return;
                    }

                }

            }
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;


namespace ZMapDesigner
{
    public static class GenWorldMapBiomes
    {
        public static void PlaceBiome(BiomeDef biome, WorldMapSettings settings)
        {
            WorldGrid worldGrid = Find.WorldGrid;

            List<int> usedTiles = new List<int>();
            List<int> edgeTiles = new List<int>();


            List<int> tmpTiles = new List<int>();
            List<Tile> validTiles = worldGrid.tiles.Where(t => settings.AllowedOn(t)).ToList();

            int placed = 0;

            if (validTiles.Count == 0)
            {
                Log.Message("No valid tiles found for " + biome.defName);
                return;
            }

            int placements = 1 + (int)(validTiles.Count * Math.Min(settings.commonality, 1));

            for (int i = 0; i < placements && placed < placements; i++)
            {
                // try to find a starting point
                //int tileID = Rand.Range(0, worldGrid.TilesCount);
                //Tile tile = worldGrid[tileID];
               
                Tile tile = validTiles.RandomElement();
                int tileID = worldGrid.tiles.FindIndex(x => x == tile);

                usedTiles.Clear();
                edgeTiles.Clear();

                // start patch
                // roll patch size once per patch
                int totalCount = settings.NumberOfTiles();

                tile.biome = biome;
                usedTiles.Add(tileID);
                edgeTiles.Add(tileID);
                placed++;

                while (edgeTiles.Count > 0 && usedTiles.Count() < totalCount)
                {
                    tmpTiles.Clear();
                    tileID = edgeTiles.RandomElement();
                    worldGrid.GetTileNeighbors(tileID, tmpTiles);

                    // remove neighbors that already have biome, or that aren't valid targets
                    tmpTiles.RemoveAll(t => worldGrid[t].biome == biome || !settings.AllowedOn(worldGrid[t]));            
                        
                    if (tmpTiles.Count > 0)
                    {
                        foreach (int id in tmpTiles)
                        {
                            tile = worldGrid[id];
                            tile.biome = biome;
                            usedTiles.Add(id);
                            edgeTiles.Add(id);
                            placed++;
                        }
                    }
                    // remove tile, it's no longer a valid edge tile
                    else
                    {
                        edgeTiles.Remove(tileID);
                    }

                }
                
            }
        }
    }
}

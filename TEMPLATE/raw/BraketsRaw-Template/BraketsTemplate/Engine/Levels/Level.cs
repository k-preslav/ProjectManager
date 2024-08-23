using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace BraketsEngine
{
    public class Level
    {
        public string Name;

        private List<Sprite> LevelSprites = new List<Sprite>();
        private Tileset tileset;

        public Level(string name)
        {
            this.Name = name;
        }

        public void ClearSprites() 
        {
            foreach (var sp in LevelSprites)
            {
                Globals.ENGINE_Main.RemoveSprite(sp);
            }
            LevelSprites.Clear();
        }

        public static async Task<Level> CreateFromData(string name, string data)
        {
            DateTime loadStart = DateTime.Now;
            
            Level level = new Level(name);
            try
            {
                Debug.Log($"Loading level: {name}", level);
                Globals.STATUS_Loading = true;

                await level.LoadData(data);
            }
            catch (Exception ex)
            {
                Debug.Error($"Failed to load Level Data. \n EX: {ex}", level);
            }

            TimeSpan loadTime = DateTime.Now - loadStart;

            await Task.Delay(50);
            Globals.STATUS_Loading = false;
            Debug.Log($"Loaded level successfully: {level.Name}, {loadTime.TotalMilliseconds.ToString("0.000")}ms", level);

            return level;
        }

        private async Task LoadData(string _d)
        {
            string[] split = _d.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int typeIndex = Array.IndexOf(split, "*type*") + 1;
            int endTypeIndex = Array.IndexOf(split, "*end_type*");

            int scaleIndex = Array.IndexOf(split, "*scale*") + 1;
            int endScaleIndex = Array.IndexOf(split, "*end_scale*");

            int defIndex = Array.IndexOf(split, "*def*") + 1;
            int endDefIndex = Array.IndexOf(split, "*end_def*");

            int dataIndex = Array.IndexOf(split, "*data*") + 1;
            int endDataIndex = Array.IndexOf(split, "*end_data*");

            string type = (typeIndex < endTypeIndex && typeIndex > 0) ? split[typeIndex] : null;

            string scale = (scaleIndex < endScaleIndex && scaleIndex > 0) ? split[scaleIndex] : null;

            string[] definitions = (defIndex < endDefIndex && defIndex > 0)
                ? split.Skip(defIndex).Take(endDefIndex - defIndex).ToArray()
                : new string[0];

            string[] data = (dataIndex < endDataIndex && dataIndex > 0)
                ? split.Skip(dataIndex).Take(endDataIndex - dataIndex).ToArray()
                : new string[0];

            if (type == "tilemap")
            {
                await LoadTilemap(definitions, data, float.Parse(scale));
            }
        }

        private async Task LoadTilemap(string[] definitions, string[] data, float scale)
        {
            await Task.Run(() =>
            {
                tileset = new Tileset();

                foreach (var def in definitions)
                {
                    string[] defSplit = def.Split(",");

                    if (defSplit.Length >= 2)
                    {
                        string associatedChar = defSplit[0].Trim();
                        string textureName = defSplit[1].Trim();

                        tileset.TileData.Add(
                            new Dictionary<string, string>()
                            {
                                { "tileCh", associatedChar },
                                { "textureName", textureName }
                            }
                        );
                    }
                }

                int x = 0;
                int y = 0;

                foreach (var line in data)
                {
                    foreach (string ch in line.Split(","))
                    {
                        if (ch == "0")
                        {
                            x++;
                            continue;
                        }

                        string tileTextureName = "default_texture";

                        foreach (var tilesetData in tileset.TileData)
                        {
                            if (tilesetData.TryGetValue("tileCh", out string tileChar) && tileChar == ch.Trim())
                            {
                                tileTextureName = tilesetData["textureName"];
                                break;
                            }
                        }

                        Tile tile = new Tile(new Vector2(x, y), tileTextureName);
                        tile.ApplyPosition(scale);

                        LevelSprites.Add(tile);
                        x++;
                    }
                    x = 0;
                    y++;
                }
            });
        }
    }
}

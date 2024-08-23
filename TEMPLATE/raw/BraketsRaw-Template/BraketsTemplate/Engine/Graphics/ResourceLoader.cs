using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FontStashSharp;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BraketsEngine;

public enum ResourceType
{
    Texture,
    Font,
    Level,
    Sound,
    Song
}

public struct BTexture
{
    public string name;
    public Texture2D tex;
}

public struct BFont
{
    public string name;
    public FontSystem fontSystem;
}

public class BSound
{
    public string name;
    private SoundEffect sound;

    public BSound(string name, SoundEffect sound)
    {
        this.name = name;
        this.sound = sound;
    }

    public void Play(float volume)
    {
        var soundInstance = sound.CreateInstance();
        soundInstance.Volume = volume / 100.0f;
        soundInstance.Play();
    }
}

public class BSong
{
    public string name;
    public bool repeat = true;
    private Song song;

    public BSong(string name, Song song, bool repeat) 
    {
        this.name = name;
        this.song = song;
        this.repeat = repeat;
    }

    public void Play() 
    {
        MediaPlayer.Stop();
        MediaPlayer.Play(song);
    }
    public void Pause() => MediaPlayer.Pause();
    public void Stop() => MediaPlayer.Stop();
    public void SetVolume(float value) => MediaPlayer.Volume = value / 100.0f;
}

public static class ResourceManager
{
    private static List<BTexture> _textures = new List<BTexture>();
    private static List<Level> _levels = new List<Level>();
    private static List<BFont> _fonts = new List<BFont>();
    private static List<BSound> _sounds = new List<BSound>();
    private static List<BSong> _songs = new List<BSong>();

    private static int failedLoads = 0;

    public static async void Load(ResourceType type, string filename)
    {
        string path = Globals.Args_Path + "/content/";
        if (type is ResourceType.Texture)
        {
            path += $"images/{filename}.png";

            try
            {
                _textures.Add(
                    new BTexture{
                        name = filename.Trim(),
                        tex = Texture2D.FromFile(Globals.ENGINE_GraphicsDevice, path)
                    }
                );
            }
            catch (Exception ex)
            {
                Debug.Error($"[ContentLoader.Load] Failed to load texture! \n EX: {ex}");
            }
        }
        else if (type is ResourceType.Font)
        {
            path += $"fonts/{filename}.ttf";
            try
            {
                FontSystem _fontSys = new FontSystem();
                _fontSys.AddFont(File.ReadAllBytes(path));

                _fonts.Add(
                    new BFont{
                        name = filename,
                        fontSystem = _fontSys
                    }
                );
            }
            catch (Exception ex)
            {
                Debug.Error($"[ContentLoader.Load] Failed to load font! \n EX: {ex}");
            }

        }
        else if (type is ResourceType.Level)
        {
            path += $"levels/{filename}.level";
            
            try
            {
                using StreamReader reader = new(path);
                string levelData = await reader.ReadToEndAsync();
                
                Level level = await Level.CreateFromData(filename, levelData);
                _levels.Add(level);
            }
            catch (Exception ex)
            {
                Debug.Error($"[ContentLoader.Load] Failed to load level! \n EX: {ex}");
            }
        }
        else if (type is ResourceType.Sound)
        {
            path += $"sounds/{filename}.wav";
            
            try
            {       
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    _sounds.Add(
                        new BSound (
                            name: filename,
                            sound: SoundEffect.FromStream(fs)
                        )
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.Error($"[ContentLoader.Load] Failed to load sound! \n EX: {ex}");
            }
        }
        else if (type is ResourceType.Song)
        {
            path += $"songs/{filename}.ogg";
            
            try
            {
                _songs.Add(
                    new BSong (
                        name: filename,
                        song: Song.FromUri(filename, new Uri(Path.GetFullPath(path))),
                        repeat: true
                    )
                );
            }
            catch (Exception ex)
            {
                Debug.Error($"[ContentLoader.Load] Failed to load song! \n EX: {ex}");
            }
        }
        // TODO: Implement other content types
    }

    public static Texture2D GetTexture(string name)
    {
        foreach (var tex in _textures)
        {
            if (tex.name.Trim() == name.Trim())
            {
                failedLoads = 0;
                return tex.tex;
            }
        }
        failedLoads++;

        if (failedLoads < 5)
        {
            Load(ResourceType.Texture, name);
            return GetTexture(name);
        }
        else
        {
            Debug.Fatal($"FAILED TO LOAD TEXTURE '{name}'!");
        }

        return null;
    }

    public static SpriteFontBase GetFont(string name, int size)
    {
        foreach (var font in _fonts)
        {
            if (font.name == name)
            {
                failedLoads = 0;
                return font.fontSystem.GetFont(size);
            }
        }

        failedLoads++;

        if (failedLoads < 5)
        {
            Load(ResourceType.Font, name);
            return GetFont(name, size);
        }
        else
        {
            Debug.Fatal($"FAILED TO LOAD FONT '{name}'!");
        }

        return null;
    }

    public static Level GetLevel(string name)
    {
        foreach (var level in _levels)
        {
            if (level.Name == name)
            {
                failedLoads = 0;
                return level;
            }
        }

        Debug.Fatal($"FAILED TO LOAD LEVEL '{name}'!");
        return null;
    }
    public static void UnloadLevel(string name)
    {
        foreach (var level in _levels.ToList())
        {
            if (level.Name == name)
            {
                level.ClearSprites();
                _levels.Remove(level);
            }
        }   
    }
    public static async void ReloadLevel(string name)
    {
        await Task.Run(() => {
            UnloadLevel(name);});
        Load(ResourceType.Level, name);
    }

    public static BSound GetSound(string name)
    {
        foreach (var sound in _sounds)
        {
            if (sound.name == name)
            {
                failedLoads = 0;
                return sound;
            }
        }

        failedLoads++;
        if (failedLoads < 5)
        {
            Load(ResourceType.Sound, name);
            return GetSound(name);
        }
        else Debug.Fatal($"FAILED TO LOAD SOUND '{name}'!");
        
        return null;
    }

    public static BSong GetSong(string name)
    {
        foreach (var song in _songs)
        {
            if (song.name == name)
            {
                failedLoads = 0;
                return song;
            }
        }

        failedLoads++;
        if (failedLoads < 5)
        {
            Load(ResourceType.Song, name);
            return GetSong(name);
        }
        else Debug.Fatal($"FAILED TO LOAD SONG '{name}'!");

        return null;
    }
    // TODO: Implement other content types
}
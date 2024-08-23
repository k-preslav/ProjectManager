using System.Threading.Tasks;
using System.Transactions;
using BraketsEngine;
using Breach;
using ImGuiNET;
using Test;

public class GameManager
{
    Player p;

    public async void Start()
    {                   
        Globals.Camera.Center();

        LoadingScreen.Initialize();
        LoadingScreen.Show();

        p = new Player();
        new Cursor();  

        await LoadingScreen.Hide();                
    }

    public void Update(float dt)
    {
        
    }

    internal void Stop()
    {

    }
}

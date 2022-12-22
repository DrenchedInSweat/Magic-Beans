using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Characters.BaseStats;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private CharacterStatsSo[] baseStats;
    public CharacterStatsSo[] Stats { get; private set; }
    public static StatsManager Instance { get; private set; }

    private const string Root = "SavedGames";
    private const string FileName = "Save_";

#if UNITY_EDITOR
    [SerializeField] private bool loadDataOnAwake;
#endif

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        //Copy the stats...  (Only needed here for if not starting from menu OR we want a "stats" page.
        
        ResetStats();
        
#if UNITY_EDITOR
        if(loadDataOnAwake)
            LoadGame(0);
#endif
    }

    public void ResetStats()
    {
        int k = baseStats.Length;
        Stats = new CharacterStatsSo[k];
        for (int i = 0; i < k; ++i)
        {
            Stats[i] = (CharacterStatsSo)baseStats[i].Clone();
        }
        
    }

    //May make sense to move this stuff
    public void SaveGame(int fileNum)
    {
        if (!Directory.Exists(Root))
        {
            Directory.CreateDirectory(Root);
        }
        
        print("Saving file");
        
        StreamWriter sw = new StreamWriter(Root + "/" + FileName + fileNum + ".txt");
        sw.Flush();
        sw.WriteLine("Seed goes here");
        
        for (int i = 0; i < Stats.Length; ++i)
        {
            CharacterStatsSo cs = Stats[i];
            if(!cs.HasBeenModified) continue;
            print("Saving: " + i+','+cs.Name+','+cs.MoveSpeed+','+cs.MaxSpeed+','+cs.JumpForce+','+cs.MaxJumps+','+cs.MaxHealth+','+cs.ContactDamage);
            sw.WriteLine(i+","+cs.Name+','+cs.MoveSpeed+','+cs.MaxSpeed+','+cs.JumpForce+','+cs.MaxJumps+','+cs.MaxHealth+','+cs.ContactDamage); // Needs to be double quotes for some reason (Only the first)
        }
        sw.Close();
    }
    
    //May need to be async...

    public void LoadGame(int fileNum)
    {
        ResetStats();

        string loc = Root + "/" + FileName + fileNum + ".txt";
        if (!File.Exists(loc)) return; // Let's hope I don't need this... 
        
        StreamReader sr = new StreamReader(loc);
        
        print("Seed: " + sr.ReadLine());

        while (!sr.EndOfStream)
        {
            string [] data = sr.ReadLine().Split(',');
            Stats[Convert.ToInt32(data[0])].SetStats(data[1], Convert.ToSingle(data[2]), Convert.ToSingle(data[3]), Convert.ToSingle(data[4]),
                Convert.ToInt32(data[5]), Convert.ToSingle(data[6]), Convert.ToSingle(data[7]));
            print("New stats: " +  Stats[Convert.ToInt32(data[0])].MaxJumps);
        }
        
        sr.Close();
    }

}

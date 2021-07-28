using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Linq;

public class TimeSave : MonoBehaviour
{
    static public float bestTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SaveTimer()
    {
        string path = Application.dataPath + "/timersave.txt";

        string levelName = "level " + SceneManager.GetActiveScene().buildIndex;
        float  timer     = Player.Instance.timer;

        if (!File.Exists(path))
        {
            string startingFile = "level 1\n999\nlevel 2\n999\nlevel 3\n999\nlevel 4\n999\nlevel 5\n999\nlevel 6\n999\n<end>";

            File.WriteAllText(path, startingFile);
        }

        string[] lines = File.ReadAllLines(path);

        int cnt = 0;

        foreach (string line in lines)
        {
            if (line.Contains(levelName))
            {
                string nextLine = lines[cnt + 1];
                nextLine.Remove(nextLine.Length - 1);
                if (float.Parse(nextLine) > timer)
                {
                    lines[cnt + 1] = timer.ToString();
                    bestTime = timer;
                }

                else
                    bestTime = float.Parse(nextLine);
            }
            cnt++;
        }

        File.WriteAllLines(path, lines);
    }
}

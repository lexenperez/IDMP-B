using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private List<TimeSpan> records = new List<TimeSpan>();
    private float currentBossTime = 0;

    [SerializeField] private GameObject recordsObj;

    public GameObject timerText;
    public GameObject endCanvas;
    public GameObject endText;
    private TextAsset recordText;
    public string timerTag;
    public string endCanvasTag;
    public string endTextTag;
    public string bossTag;
    private string recordPath = "/Resources/records.txt";

    private int currentBoss = 0;
    public GameObject[] bosses;
    private bool endScreenShown = false;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;

        foreach (string t in Directory.GetFiles(Application.dataPath + "/Scenes/Bosses"))
        {
            if (t.EndsWith(".unity"))
            {
                records.Add(TimeSpan.FromSeconds(0));
            }
        }
        recordText = Resources.Load<TextAsset>("records");
        if (recordText == null)
        {
            Debug.Log("Create new");
            File.Create(Application.dataPath + recordPath);
        }
        else
        {
            LoadRecords();
        }
        SetRecords();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("Menu"))
        {
            SetRecords();
        }
        else
        {
            Debug.Log("Loaded " + scene.name);
            Setup();
            if (endCanvas) endCanvas.SetActive(false);
        }

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SaveRecords();
    }

    void Setup()
    {
        endCanvas = GameObject.FindGameObjectWithTag(endCanvasTag);
        timerText = GameObject.FindGameObjectWithTag(timerTag);
        endText = GameObject.FindGameObjectWithTag(endTextTag);
        bosses = GameObject.FindGameObjectsWithTag(bossTag);
        
    }

    void LoadRecords()
    {
        string recordTxt = recordText.text;
        string[] s = recordTxt.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries); ;
        for (int i = 0; i < s.Length; i++)
        {
            if (!(i >= records.Count))
            {
                records[i] = TimeSpan.ParseExact(s[i], "mm':'ss':'ff", CultureInfo.InvariantCulture);
            }

        }

    }

    void SetRecords()
    {
        string s = "";
        int c = 1;
        foreach(TimeSpan ts in records)
        {
            string toSet = ts.ToString("mm':'ss':'ff");
            //Debug.Log(ts);
            if (ts.TotalSeconds == 0) toSet = "Not yet tried";
            s += string.Format("Boss {0}: ", c) + toSet + "\n";
            c++;
        }
        recordsObj.GetComponent<TextMeshProUGUI>().text = s;
    }

    void SaveRecords()
    {
        
        string path = Application.dataPath + recordPath;
        Debug.Log("saving to " + path);
        StreamWriter writer = new StreamWriter(path, false);
        foreach(TimeSpan record in records)
        {
            Debug.Log(record);
            writer.WriteLine(record.ToString("mm':'ss':'ff"));
        }
        writer.Flush();
        writer.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (!endScreenShown)
        {
            if (bosses.Length != 0)
            {
                if (AllBossesDead())
                {
                    FinishFight();
                    endScreenShown = true;
                    Debug.Log("Bosses all dead");

                }
                else
                {
                    currentBossTime += Time.deltaTime;
                    timerText.GetComponent<TextMeshProUGUI>().text = "Timer: " + TimeSpan.FromSeconds(currentBossTime).ToString("mm':'ss':'ff");
                }
            }

        }

    }
    
    bool AllBossesDead()
    {
        //if (bosses.Length == 0) return false;
        foreach (GameObject ob in bosses)
        {
            if (ob != null) return false;
        }
        return true;
    }

    public void ResetCurrentTimer()
    {
        currentBossTime = 0f;
    }

    public void FinishFight()
    {
        timerText.GetComponent<TextMeshProUGUI>().text = "";
        endCanvas.SetActive(true);

        bool newRec = UpdateRecord(currentBoss);
        endText.GetComponent<TextMeshProUGUI>().text = EndText(TimeSpan.FromSeconds(currentBossTime).ToString("mm':'ss':'ff"), currentBoss, newRec);
        //endText.GetComponent<TextMeshProUGUI>().text = "This Time: " + TimeSpan.FromSeconds(currentBossTime).ToString("mm':'ss':'ff") + "\n";


    }

    private bool UpdateRecord(int boss)
    {
        if (boss <= records.Count)
        {
            TimeSpan ts = records[boss];
            //Debug.Log(TimeSpan.Compare(TimeSpan.FromSeconds(currentBossTime), ts));
            if (TimeSpan.Compare(TimeSpan.FromSeconds(currentBossTime), ts) == -1 || ts.TotalSeconds == 0)
            {
                records[boss] = TimeSpan.FromSeconds(currentBossTime);
                return true;
            }
        }
        return false;
    }

    private string EndText(string currTime, int boss, bool newRecord)
    {
        string s = "";
        s += string.Format("Boss {0} Defeated!\n", boss);
        s += "Time: " + currTime + "\n";
        if (newRecord)
        {
            s += "New Record!\n";
        }
        s += "Best Time: " + records[boss].ToString("mm':'ss':'ff");
        return s;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CTFSim : MonoBehaviour {

    public bool loaded = false;
    private bool paused = false;
    private bool over = false;
    private ResultScript rs;

    private int PlayersPerTeam;
    private float GameDuration;
    private string Brain;

    public List<GoalScript> goals;
    public List<int> scores = new List<int>();
    public List<CTFPlayer> players = new List<CTFPlayer>();

    public AI playerPrefab;
    public Human humanPrefab;
    public Material blueMaterial;
    public Material redMaterial;

    public float time = 0f;
    public float resetTime = 0f;
    public float spawnRadius = 10f;
    public bool hum = false;

    public NeuralNet nnbrain = null;

	// Use this for initialization
	void Awake () {

        if (FindObjectsOfType<CTFSim>().Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
	}

    public void Init(int players, float time, string aibrain, bool human)
    {
        PlayersPerTeam = players;
        GameDuration = time;
        Brain = aibrain;
        hum = human;
    }

    public void StartGame()
    {
        StartCoroutine(LoadScene("Maps/TrainingWorld/TrainingWorld"));
    }

    public void LoadMap(Map map)
    {
        goals = map.goals;
        scores.Add(0);
        scores.Add(0);
        int i = 0;

        if (hum)
        {
            Vector3 offset = new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * spawnRadius;
            Human newPlayer = Instantiate<Human>(humanPrefab, goals[0].transform.position + offset, Quaternion.identity);
            if (nnbrain != null)
            {
                newPlayer.InitBrain(0, blueMaterial, this, nnbrain);
            }
            else
            {
                newPlayer.Init(0, blueMaterial, this);
            }
            ThrdCam tcam = Camera.main.gameObject.AddComponent<ThrdCam>();
            tcam.target = newPlayer.body;
            tcam.transform.SetParent(newPlayer.transform);
            newPlayer.SetCam(tcam);
            players.Add(newPlayer);
            map.hs.SetPlayer(players[0]);
            i++;
        }
        else
        {
            Camera.main.gameObject.AddComponent<FrstCam>();
            Camera.main.transform.position = goals[0].transform.position + Vector3.up * 20f;
        }

        for (; i < PlayersPerTeam; i++)
        {
            float ang = Mathf.PI * 2 * (float)i / (float)PlayersPerTeam;
            Vector3 offset = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * spawnRadius;
            AI newPlayer = Instantiate<AI>(playerPrefab, goals[0].transform.position + offset, Quaternion.identity);
            if (nnbrain != null)
            {
                newPlayer.InitBrain(0, blueMaterial, this, nnbrain);
            }
            else
            {
                newPlayer.Init(0, blueMaterial, this);
            }
            players.Add(newPlayer);
        }

        for (int j = 0; j < PlayersPerTeam; j++)
        {
            float ang = Mathf.PI * 2 * (float)j / (float)PlayersPerTeam + Mathf.PI;
            Vector3 offset = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang)) * spawnRadius;
            AI newPlayer = Instantiate<AI>(playerPrefab, goals[1].transform.position + offset, Quaternion.identity);
            if (nnbrain != null)
            {
                newPlayer.InitBrain(1, redMaterial, this, nnbrain);
            }
            else
            {
                newPlayer.Init(1, redMaterial, this);
            }
            players.Add(newPlayer);
        }

        loaded = true;
        over = false;
        paused = false;
        time = 0f;
        resetTime = 0f;
        rs = map.rs;
    }

    public void Score(int team)
    {
        scores[team]++;
    }

    public string GetTime()
    {
        float newTime = GameDuration - time;
        float seconds = Mathf.FloorToInt(newTime % 60f);
        string secs = "" + seconds;
        if (seconds < 10)
        {
            secs = "0" + secs;
        }
        return Mathf.FloorToInt(newTime / 60f) + ":" + secs;
    }

    public void UnloadMap()
    {
        loaded = false;
        over = false;
        paused = false;
        goals = null;
        rs.EndResult();
        rs = null;
        if (hum)
        {
            nnbrain = players[0].GetComponent<Human>().GenSets(10000);
        }
        scores.Clear();
        players.Clear();
        time = 0f;
        resetTime = 0f;
        StartCoroutine(LoadScene("Maps/Menu/Menu"));
    }

    IEnumerator LoadScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Update is called once per frame
    void Update () {
        if (over)
        {
            resetTime += Time.deltaTime;
            if (resetTime >= 5f)
            {
                UnloadMap();
            }
        }
        else if (paused)
        {

        }
        else if (loaded)
        {
            time += Time.deltaTime;
            if (time >= GameDuration)
            {
                over = true;
                time = GameDuration;
                string reso = " Team Wins!";
                if (scores[0] > scores[1])
                {
                    reso = "Blue" + reso;
                }
                else if (scores[1] > scores[0])
                {
                    reso = "Red" + reso;
                }
                else
                {
                    reso = "Tie!";
                }
                rs.PublishResult(reso);
            }
        }
	}
}

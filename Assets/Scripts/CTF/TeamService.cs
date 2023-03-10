using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Team
{
    public Team(int teamID)
    {
        TeamID = teamID;
        Spawnpads = new List<Spawnpad>();
    }

    public int TeamID {get; private set;}
    public List<Spawnpad> Spawnpads {get; private set;}

    public override string ToString() => $"Team {TeamID}";
}

public class TeamService : MonoBehaviour
{
    public static TeamService Instance {get; private set;}
    private List<Team> teams;

    bool DoesTeamExist(int TeamID)
    {
        return TeamID >= 0 && TeamID < teams.Count;
    }

    int CreateTeam()
    {
        int newTeamID = teams.Count; // -1 == no team
        Team newTeam = new Team(newTeamID);
        teams.Add(newTeam);
        return newTeamID;
    }

    Team GetTeam(int TeamID)
    {
        return teams[TeamID];
    }

    bool IsPlayerOnTeam(Agent A)
    {
        return A.TeamID != -1;
    }

    bool ArePlayersOnSameTeam(Agent A, Agent B)
    {
        return IsPlayerOnTeam(A) && A.TeamID == B.TeamID;
    }


    public void SpawnAgent(Agent agent)
    {
        int TeamID = agent.TeamID;
        if (!DoesTeamExist(TeamID))
        {
            return;
        }

        if (!agent.Character)
        {
            agent.LoadCharacter();
        }

        Team team = GetTeam(TeamID);
        int numSpawnpads = team.Spawnpads.Count;
        Assert.IsTrue(numSpawnpads > 0);
        Spawnpad randomSpawnpad = team.Spawnpads[Random.Range(0, numSpawnpads-1)];
        Vector3 spawn_position = randomSpawnpad.GetSpawnPosition();
        agent.Character.transform.position = spawn_position;
    }


    private bool DoSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return false;
        }
        else
        {
            Instance = this;
            return true;
        }
    }

    void Awake()
    {
        if (!DoSingleton()) return;

        teams = new List<Team>();
        CreateTeam(); // TeamID: 0
        CreateTeam(); // TeamID: 1

        var allSpawnpads = Object.FindObjectsOfType<Spawnpad>();
        for (int i = 0; i < allSpawnpads.Length; i++)
        {
            int TeamID = allSpawnpads[i].TeamID;
            if (!DoesTeamExist(TeamID))
            {
                continue;
            }
            GetTeam(TeamID).Spawnpads.Add(allSpawnpads[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

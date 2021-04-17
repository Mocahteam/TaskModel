using System;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    [Serializable]
    public class RawParticipant
    {
        public string profil = "";
        public string role = "";
    }

    [Serializable]
    public class RawWorkingSession
    {
        public string duration = "";
        public string organisation = "";
        public List<RawParticipant> participants = new List<RawParticipant>();
    }

    [Serializable]
    public class RawObservation
    {
        public string content = "";
        public List<string> decisions = new List<string>();
    }

    [Serializable]
    public class RawCompetency
    {
        public int type = 0;
        public int id = 0;
        public string details = "";
    }


    [Serializable]
    public class Task
    {
        public string id = "";
        public string objective = "";
        public int complexity = 0;
        public List<string> artefacts = new List<string>();
        public List<RawObservation> observations = new List<RawObservation>();
        public Dictionary<int, RawWorkingSession> workingSessions = new Dictionary<int, RawWorkingSession>();
        public List<RawCompetency> competencies = new List<RawCompetency>();
        public string production = "";
        public List<int> antecedents = new List<int>();
        public List<int> subtasks = new List<int>();

        public Task(string defaultId)
        {
            id = defaultId;
        }
    }

    public List<Task> scenario = new List<Task>();

    public GameObject contentUI;

    public GameObject taskNamePrefab;
    public GameObject taskObjectivePrefab;
    public GameObject taskComplexityPrefab;
    public GameObject taskArtefactPrefab;
    public GameObject taskObservationPrefab;
    public GameObject taskWorkingSessionPrefab;
    public GameObject taskCompetencyPrefab;
    public GameObject taskProductionPrefab;
    public GameObject taskAntecedentPrefab;
    public GameObject taskSubTaskPrefab;
}

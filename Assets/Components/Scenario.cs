using System;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    [Serializable]
    public class RawComplexity
    {
        public int pos;
        public int complexity;
        public RawComplexity(int pos, int complexity)
        {
            this.pos = pos;
            this.complexity = complexity;
        }
    }

    [Serializable]
    public class RawArtefact
    {
        public int pos;
        public string artefact;
        public RawArtefact(int pos, string artefact)
        {
            this.pos = pos;
            this.artefact = artefact;
        }
    }

    [Serializable]
    public class RawObservation
    {
        public int pos;
        public bool viewState;
        public string content;
        public List<string> decisions;
        public RawObservation(int pos, bool viewState, string content)
        {
            this.pos = pos;
            this.viewState = viewState;
            this.content = content;
            decisions = new List<string>();
        }

        public void addRawDecision(string decision)
        {
            decisions.Add(decision);
        }
    }

    [Serializable]
    public class RawParticipant
    {
        public bool viewState = true;
        public string profil;
        public string role;
        public RawParticipant(string profil, string role)
        {
            this.profil = profil;
            this.role = role;
        }
    }

    [Serializable]
    public class RawWorkingSession
    {
        public int pos;
        public bool viewState;
        public string id;
        public string duration;
        public string organisation;
        public List<RawParticipant> participants;

        public RawWorkingSession(int pos, bool viewState, string id, string duration, string organisation)
        {
            this.pos = pos;
            this.viewState = viewState;
            this.id = id;
            this.duration = duration;
            this.organisation = organisation;
            participants = new List<RawParticipant>();
        }

        public void addParticipant(string profil, string role)
        {
            participants.Add(new RawParticipant(profil, role));
        }
    }

    [Serializable]
    public class RawCompetency
    {
        public int pos;
        public bool viewState;
        public int type;
        public int id;
        public string details;

        public RawCompetency(int pos, bool viewState, int type, int id, string details)
        {
            this.pos = pos;
            this.viewState = viewState;
            this.type = type;
            this.id = id;
            this.details = details;
        }
    }

    [Serializable]
    public class RawProduction
    {
        public int pos;
        public bool viewState;
        public string production;

        public RawProduction(int pos, bool viewState, string production)
        {
            this.pos = pos;
            this.viewState = viewState;
            this.production = production;
        }
    }

    [Serializable]
    public class RawAntecedent
    {
        public int pos;
        public int antecedent;

        public RawAntecedent(int pos, int antecedent)
        {
            this.pos = pos;
            this.antecedent = antecedent;
        }
    }

    [Serializable]
    public class RawSubTask
    {
        public int pos;
        public int subTask;

        public RawSubTask(int pos, int subTask)
        {
            this.pos = pos;
            this.subTask = subTask;
        }
    }

    [Serializable]
    public class RawTask
    {
        public string id = "";
        public string objective = "";
        public bool objectiveViewState = false; // false means visible
        public List<RawComplexity> rawComplexities = new List<RawComplexity>(); 
        public List<RawArtefact> rawArtefacts = new List<RawArtefact>();
        public List<RawObservation> rawObservations = new List<RawObservation>();
        public List<RawWorkingSession> rawWorkingSessions = new List<RawWorkingSession>();
        public List<RawCompetency> rawCompetencies = new List<RawCompetency>(); 
        public List<RawProduction> rawProductions = new List<RawProduction>(); 
        public List<RawAntecedent> rawAntecedents = new List<RawAntecedent>(); 
        public List<RawSubTask> rawSubTasks = new List<RawSubTask>();


        public RawTask(string defaultId)
        {
            id = defaultId;
        }
    }

    [Serializable]
    public class RawScenario
    {
        public List<RawTask> tasks = new List<RawTask>();
    }

    public RawScenario rawScenario;

    public GameObject contentUI;

    public GameObject taskNamePrefab;
    public GameObject taskObjectivePrefab;
    public GameObject taskComplexityPrefab;
    public GameObject taskArtefactPrefab;
    public GameObject taskObservationPrefab;
    public GameObject taskDecisionPrefab;
    public GameObject taskWorkingSessionPrefab;
    public GameObject taskParticipantPrefab;
    public GameObject taskCompetencyPrefab;
    public GameObject taskProductionPrefab;
    public GameObject taskAntecedentPrefab;
    public GameObject taskSubTaskPrefab;
}

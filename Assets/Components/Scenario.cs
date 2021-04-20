using System;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    [Serializable]
    public class RawDescriptor
    {
        public bool viewState = true;
    }

    [Serializable]
    public class RawComplexity: RawDescriptor
    {
        public int complexity;
        public RawComplexity(int complexity)
        {
            this.complexity = complexity;
        }
    }

    [Serializable]
    public class RawArtefact : RawDescriptor
    {
        public string artefact;
        public RawArtefact(string artefact)
        {
            this.artefact = artefact;
        }
    }

    [Serializable]
    public class RawObservation : RawDescriptor
    {
        public string content;
        public List<string> decisions;
        public RawObservation(bool viewState, string content)
        {
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
        public string profil;
        public string role;
        public RawParticipant(string profil, string role)
        {
            this.profil = profil;
            this.role = role;
        }
    }

    [Serializable]
    public class RawWorkingSession : RawDescriptor
    {
        public string id;
        public string duration;
        public string organisation;
        public List<RawParticipant> participants;

        public RawWorkingSession(bool viewState, string id, string duration, string organisation)
        {
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
    public class RawCompetency : RawDescriptor
    {
        public int type;
        public int id;
        public string details;

        public RawCompetency(bool viewState, int type, int id, string details)
        {
            this.viewState = viewState;
            this.type = type;
            this.id = id;
            this.details = details;
        }
    }

    [Serializable]
    public class RawProduction : RawDescriptor
    {
        public string production;

        public RawProduction(bool viewState, string production)
        {
            this.viewState = viewState;
            this.production = production;
        }
    }

    [Serializable]
    public class RawAntecedent : RawDescriptor
    {
        public int antecedent;

        public RawAntecedent(int antecedent)
        {
            this.antecedent = antecedent;
        }
    }

    [Serializable]
    public class RawSubTask : RawDescriptor
    {
        public int subTask;

        public RawSubTask(int subTask)
        {
            this.subTask = subTask;
        }
    }

    [Serializable]
    public class Task
    {
        public string id = "";
        public string objective = "";
        public bool objectiveViewState = false; // false means visible
        public List<RawDescriptor> descriptors = new List<RawDescriptor>();

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
    public GameObject taskDecisionPrefab;
    public GameObject taskWorkingSessionPrefab;
    public GameObject taskParticipantPrefab;
    public GameObject taskCompetencyPrefab;
    public GameObject taskProductionPrefab;
    public GameObject taskAntecedentPrefab;
    public GameObject taskSubTaskPrefab;
}

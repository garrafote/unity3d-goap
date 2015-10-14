using System;
using UnityEngine;
using System.Collections;
using PathFinder;
using System.Collections.Generic;
using GOAP.V1;

[ExecuteInEditMode]
public class GOAPTestV1 : MonoBehaviour {

    public bool execute;

    public enum Atoms
    {
        ArmedWithGun,
        ArmedWithBomb,
       
        EnemyVisible,
        NearEnemy,
        EnemyLinedUp,
        EnemyAlive,
        
        WeaponLoaded,
        Alive, 
    }

    public enum Actions
    {
        Scout,
        Approach,
        Aim,
        Shoot,
        Load,
        DetonateBomb,
        Flee,
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (execute)
        {
            execute = false;

            Execute();
        }
	}

    void Execute()
    {
        var planner = new ActionPlanner();
        planner.Clear();

        planner.SetPrecondition(Actions.Scout, Atoms.Alive, true);
        planner.SetPostcondition(Actions.Scout, Atoms.EnemyVisible, true);

        planner.SetPrecondition(Actions.Approach, Atoms.EnemyVisible, true);
        planner.SetPostcondition(Actions.Approach, Atoms.NearEnemy, true);

        planner.SetPrecondition(Actions.Aim, Atoms.EnemyVisible, true);
        planner.SetPrecondition(Actions.Aim, Atoms.WeaponLoaded, true);
        planner.SetPostcondition(Actions.Aim, Atoms.EnemyLinedUp, true);

        planner.SetPrecondition(Actions.Shoot, Atoms.EnemyLinedUp, true);
        planner.SetPostcondition(Actions.Shoot, Atoms.EnemyAlive, false);

        planner.SetPrecondition(Actions.Load, Atoms.ArmedWithGun, true);
        planner.SetPostcondition(Actions.Load, Atoms.WeaponLoaded, true);

        planner.SetPrecondition(Actions.DetonateBomb, Atoms.ArmedWithBomb, true);
        planner.SetPrecondition(Actions.DetonateBomb, Atoms.NearEnemy, true);
        planner.SetPostcondition(Actions.DetonateBomb, Atoms.Alive, false);
        planner.SetPostcondition(Actions.DetonateBomb, Atoms.EnemyAlive, false);
        planner.SetCost(Actions.DetonateBomb, 5);

        planner.SetPrecondition(Actions.Flee, Atoms.EnemyVisible, true);
        planner.SetPostcondition(Actions.Flee, Atoms.EnemyVisible, false);

        Debug.Log(planner.ToString<Actions, Atoms>());

        var state = new WorldState();
        state.Set(Atoms.EnemyVisible,   false);
        state.Set(Atoms.ArmedWithGun,   true);
        state.Set(Atoms.WeaponLoaded,   false);
        state.Set(Atoms.EnemyLinedUp,   false);
        state.Set(Atoms.EnemyAlive,     true);
        state.Set(Atoms.ArmedWithBomb,  true);
        state.Set(Atoms.NearEnemy,      false);
        state.Set(Atoms.Alive,          true);

        Debug.LogFormat("State: {0}", state.ToString<Atoms>());

        var goal = new WorldState();
        goal.Set(Atoms.EnemyAlive, false);

        Debug.LogFormat("Goal: {0}", goal.ToString<Atoms>());

        var search = new Fringe(Graph.PlannerHeuristic);

        var stateNode = new WorldStateNode(planner, state);
        var goalNode = new WorldStateNode(planner, goal);

        var path = search.FindPath(stateNode, goalNode);

        foreach (WorldStateNode node in path)
        {
            Debug.Log(node.State);
        }
    }

}


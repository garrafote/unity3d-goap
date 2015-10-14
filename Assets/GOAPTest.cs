using System;
using UnityEngine;
using System.Collections;
using PathFinder;
using System.Collections.Generic;
using GOAP;

[ExecuteInEditMode]
public class GOAPTest : MonoBehaviour {

    public bool execute;

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

        var scout = new GoapAction();
        planner.SetPrecondition (scout, "Alive", true);
        planner.SetPostcondition(scout, "EnemyVisible", true);

        var approach = new GoapAction();
        planner.SetPrecondition (approach, "EnemyVisible", true);
        planner.SetPostcondition(approach, "NearEnemy", true);
        
        var aim = new GoapAction();
        planner.SetPrecondition (aim, "EnemyVisible", true);
        planner.SetPrecondition (aim, "WeaponLoaded", true);
        planner.SetPostcondition(aim, "EnemyLinedUp", true);

        var shoot = new GoapAction();
        planner.SetPrecondition (shoot, "EnemyLinedUp", true);
        planner.SetPostcondition(shoot, "EnemyAlive", false);

        var load = new GoapAction();
        planner.SetPrecondition (load, "ArmedWithGun", true);
        planner.SetPostcondition(load, "WeaponLoaded", true);

        var detonateBomb = new GoapAction { Cost = 5 };
        planner.SetPrecondition (detonateBomb, "ArmedWithBomb", true);
        planner.SetPrecondition (detonateBomb, "NearEnemy", true);
        planner.SetPostcondition(detonateBomb, "Alive", false);
        planner.SetPostcondition(detonateBomb, "EnemyAlive", false);

        var flee = new GoapAction();
        planner.SetPrecondition (flee, "EnemyVisible", true);
        planner.SetPostcondition(flee, "EnemyVisible", false);

        Debug.Log(planner.ToString());

        var state = new WorldState();
        state[planner.StringToAtom("EnemyVisible")] =   false;
        state[planner.StringToAtom("ArmedWithGun")] =   true;
        state[planner.StringToAtom("WeaponLoaded")] =   false;
        state[planner.StringToAtom("EnemyLinedUp")] =   false;
        state[planner.StringToAtom("EnemyAlive")] =     true;
        state[planner.StringToAtom("ArmedWithBomb")] =  true;
        state[planner.StringToAtom("NearEnemy")] =      false;
        state[planner.StringToAtom("Alive")] =          true;

        Debug.LogFormat("State: {0}", state.ToString(planner.Atoms));

        var goal = new WorldState();
        goal[planner.StringToAtom("EnemyAlive")] = false;

        Debug.LogFormat("Goal: {0}", goal.ToString(planner.Atoms));

        var search = new Fringe(GoapGraph.PlannerHeuristic);

        var graph = new GoapGraph(planner);
        var stateNode = new WorldStateNode(graph, state);
        var goalNode = new WorldStateNode(graph, goal);

        var path = search.FindPath(stateNode, goalNode);

        foreach (WorldStateNode node in path)
        {
            Debug.Log(node.State);
        }
    }

}


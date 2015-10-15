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

        var scout = planner.CreateAction("Scout");
        scout.SetPrecondition ("Alive", true);
        scout.SetPostcondition("EnemyVisible", true);

        var approach =planner.CreateAction("Approach");
        approach.SetPrecondition ("EnemyVisible", true);
        approach.SetPostcondition("NearEnemy", true);
        
        var aim =planner.CreateAction("Aim");
        aim.SetPrecondition ("EnemyVisible", true);
        aim.SetPrecondition ("WeaponLoaded", true);
        aim.SetPostcondition("EnemyLinedUp", true);

        var shoot =planner.CreateAction("Shoot");
        shoot.SetPrecondition ("EnemyLinedUp", true);
        shoot.SetPostcondition("EnemyAlive", false);

        var load =planner.CreateAction("Load");
        load.SetPrecondition ("ArmedWithGun", true);
        load.SetPostcondition("WeaponLoaded", true);

        var detonateBomb =planner.CreateAction("DetonateBomb", 5);
        detonateBomb.SetPrecondition ("ArmedWithBomb", true);
        detonateBomb.SetPrecondition ("NearEnemy", true);
        detonateBomb.SetPostcondition("Alive", false);
        detonateBomb.SetPostcondition("EnemyAlive", false);

        var flee =planner.CreateAction("Flee");
        flee.SetPrecondition ("EnemyVisible", true);
        flee.SetPostcondition("EnemyVisible", false);

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
        var stateNode = new WorldStateNode(graph, state, null);
        var goalNode = new WorldStateNode(graph, goal, null);

        var path = search.FindPath(stateNode, goalNode);

        foreach (WorldStateNode node in path)
        {
            Debug.Log(node.State.ToString(planner.Atoms));
        }
    }

}


using UnityEngine;
using System.Collections;
using GOAP;
using PathFinder;
using UnityEngine.UI;
using System.Text;

public class GOAPInteractive : MonoBehaviour {
    public PopulateContainer actionsContainer;
    public PopulateContainer initialStateContainer;
    public PopulateContainer goalContainer;
    public Text outputText;

    public bool execute;
    // Update is called once per frame
    void Update()
    {
        if (execute)
        {
            execute = false;

            Execute();
        }
    }


    public void Execute()
    {
        var sb = new StringBuilder();

        var planner = new ActionPlanner();
        planner.Clear();
        
        foreach (Transform actionViewTransform in actionsContainer.container)
        {
            var actionView = actionViewTransform.GetComponent<ActionView>();
            if (!actionView) continue;

            var action = planner.CreateAction(actionView.GetName());
            action.Cost = actionView.GetCost();

            foreach (var atom in actionView.GetPreconditions())
            {
                action.SetPrecondition(atom.GetName(), atom.GetState());
            }

            foreach (var atom in actionView.GetPostconditions())
            {
                action.SetPostcondition(atom.GetName(), atom.GetState());
            }
        }

        sb.AppendLine(".:: Action PLanner ::.");
        sb.AppendLine(planner.ToString());

        var state = new WorldState();

        foreach (Transform atom in initialStateContainer.container)
        {
            var atomView = atom.GetComponent<AtomView>();
            if (atomView) state[planner.StringToAtom(atomView.GetName())] = atomView.GetState();
        }

        sb.AppendLine(".:: Initial State ::.");
        sb.AppendFormat("{0}\n\n", state.ToString(planner.Atoms));

        var goal = new WorldState();

        foreach (Transform atom in goalContainer.container)
        {
            var atomView = atom.GetComponent<AtomView>();
            if (atomView) goal[planner.StringToAtom(atomView.GetName())] = atomView.GetState();
        }

        sb.AppendLine(".:: Goal ::.");
        sb.AppendFormat("{0}\n\n", goal.ToString(planner.Atoms));

        var search = new Fringe(GoapGraph.PlannerHeuristic);

        var graph = new GoapGraph(planner);
        var stateNode = new WorldStateNode(graph, state, null);
        var goalNode = new WorldStateNode(graph, goal, null);

        var path = search.FindPath(stateNode, goalNode);

        var padding = 0;
        foreach (WorldStateNode node in path)
        {
            if (node.Action == null) continue;
            padding = System.Math.Max(node.Action.Name.Length, padding);
        }

        var format = "{0," + padding + "}: {1}\n";

        sb.AppendLine(".:: Action Plan ::.");
        foreach (WorldStateNode node in path)
        {
            sb.AppendFormat(format, node.Action == null ? "" : node.Action.Name, node.State.ToString(planner.Atoms));
        }

        outputText.text = sb.ToString();

    }

    public void Sample01() {

        ClearAll();

        var scout = CreateAction("Scout");
        scout.SetPrecondition("Alive", true);
        scout.SetPostcondition("EnemyVisible", true);

        var approach = CreateAction("Approach");
        approach.SetPrecondition("EnemyVisible", true);
        approach.SetPostcondition("NearEnemy", true);

        var aim = CreateAction("Aim");
        aim.SetPrecondition("EnemyVisible", true);
        aim.SetPrecondition("WeaponLoaded", true);
        aim.SetPostcondition("EnemyLinedUp", true);

        var shoot = CreateAction("Shoot");
        shoot.SetPrecondition("EnemyLinedUp", true);
        shoot.SetPostcondition("EnemyAlive", false);

        var load = CreateAction("Load");
        load.SetPrecondition("ArmedWithGun", true);
        load.SetPostcondition("WeaponLoaded", true);

        var detonateBomb = CreateAction("DetonateBomb", 5);
        detonateBomb.SetPrecondition("ArmedWithBomb", true);
        detonateBomb.SetPrecondition("NearEnemy", true);
        detonateBomb.SetPostcondition("Alive", false);
        detonateBomb.SetPostcondition("EnemyAlive", false);

        var flee = CreateAction("Flee");
        flee.SetPrecondition("EnemyVisible", true);
        flee.SetPostcondition("EnemyVisible", false);

        CreateAtom(initialStateContainer, "EnemyVisible", false);
        CreateAtom(initialStateContainer, "ArmedWithGun", true);
        CreateAtom(initialStateContainer, "WeaponLoaded", false);
        CreateAtom(initialStateContainer, "EnemyLinedUp", false);
        CreateAtom(initialStateContainer, "EnemyAlive", true);
        CreateAtom(initialStateContainer, "ArmedWithBomb", true);
        CreateAtom(initialStateContainer, "NearEnemy", false);
        CreateAtom(initialStateContainer, "Alive", true);

        CreateAtom(goalContainer, "EnemyAlive", false);

        outputText.text = "FPS Sample!";
    }

    public void ClearAll()
    {
        actionsContainer.GetComponent<PopulateContainer>().Clear();
        initialStateContainer.GetComponent<PopulateContainer>().Clear();
        goalContainer.GetComponent<PopulateContainer>().Clear();


        outputText.text = "GOAP Cleared!";
    }

    private ActionView CreateAction(string name, int cost = 1)
    {
        var actionView = actionsContainer.AddPrefabToContainerReturn().GetComponent<ActionView>();

        actionView.SetName(name);
        actionView.SetCost(cost);

        return actionView;
    }

    private AtomView CreateAtom(PopulateContainer container, string atom, bool status)
    {
        var atomView = container.AddPrefabToContainerReturn().GetComponent<AtomView>();
        
        atomView.SetName(atom);
        atomView.SetStatus(status);

        return atomView;
    }
}

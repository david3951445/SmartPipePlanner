namespace SmartPipePlanner.Core.Search;

public class Node(State state, Node? parent, double gCost, double hCost, Pipe? action)
{
    public State State { get; } = state;
    public Node? Parent { get; } = parent;
    public double GCost { get; } = gCost;
    public double HCost { get; } = hCost;
    public double FCost => GCost + HCost;
    public Pipe? Action { get; } = action;
}

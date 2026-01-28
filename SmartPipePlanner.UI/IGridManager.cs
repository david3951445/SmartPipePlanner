using SmartPipePlanner.Core;

namespace SmartPipePlanner.UI;

public interface IGridManager
{
    Grid GetGrid();
    void SetPipes(Pipe[] pipes);
}
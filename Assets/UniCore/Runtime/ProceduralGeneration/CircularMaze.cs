namespace KarenKrill.UniCore.ProceduralGeneration
{
    public class CircularMaze
    {
        public CircularMazeCell[][] Cells { get; set; }
        public CircularMazeCell ExitCell { get; set; }

        public CircularMaze(CircularMazeCell[][] cells, CircularMazeCell exitCell)
        {
            Cells = cells;
            ExitCell = exitCell;
        }
    }
}

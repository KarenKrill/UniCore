using System.Collections.Generic;

namespace KarenKrill.UniCore.ProceduralGeneration
{
    public class CircularMazeCell
    {
        public IReadOnlyList<bool> FrontWalls => _frontWalls;
        public bool LeftWall { get; private set; } = true;
        public bool RightWall { get; private set; } = true;
        public bool BackWall { get; private set; } = true;
        public int Level { get; private set; }
        public int Cell { get; private set; }

        public CircularMazeCell(int level, int cell, int frontWallsCount)
        {
            Level = level;
            Cell = cell;
            _frontWalls = new List<bool>(frontWallsCount);
            for (int i = 0; i < frontWallsCount; i++)
            {
                _frontWalls.Add(true);
            }
        }
        public void BreakFrontWall(int wallIndex) => _frontWalls[wallIndex] = false;
        public void BreakLeftWall() => LeftWall = false;
        public void BreakRightWall() => RightWall = false;
        public void BreakBackWall() => BackWall = false;

        private readonly List<bool> _frontWalls;
    }
}

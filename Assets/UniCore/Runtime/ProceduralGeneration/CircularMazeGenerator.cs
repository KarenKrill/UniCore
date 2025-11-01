using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KarenKrill.UniCore.ProceduralGeneration
{
    using Logging;
    using PathFinding.DepthFirstSearch;

    public class CircularMazeGenerator
    {
        public static int CellsOnLevel(int level) => level == 0 ? 1 : (int)Mathf.Pow(2, Mathf.Floor(Mathf.Log(level + 1, 2)) + 2);

        public CircularMazeGenerator(ILogger logger)
        {
            _logger = logger;
        }
        public CircularMaze Generate(int levels)
        {
            // Caching
            int[] cellsOnLevelCache = new int[levels];
            for(int level = 0; level < levels; level++)
            {
                cellsOnLevelCache[level] = CellsOnLevel(level);
            }

            // Nodes & Cells creation:
            CircularMazeCell[][] cells = new CircularMazeCell[levels][];
            TreeNode<CircularMazeCell>[][] cellNodes = new TreeNode<CircularMazeCell>[levels][];
            for (int level = 0; level < levels; level++)
            {
                int cellsOnLevel = cellsOnLevelCache[level];
                int cellsOnNextLevel = level + 1 < levels ? cellsOnLevelCache[level + 1] : cellsOnLevel;
                int nextLevelCellsPerCell = cellsOnNextLevel / cellsOnLevel;
                int wallsOnCell = nextLevelCellsPerCell;
                cellNodes[level] = new TreeNode<CircularMazeCell>[cellsOnLevel];
                cells[level] = new CircularMazeCell[cellsOnLevel];
                for (int cellIndex = 0; cellIndex < cellsOnLevel; cellIndex++)
                {
                    var cell = new CircularMazeCell(level, cellIndex, wallsOnCell);
                    cellNodes[level][cellIndex] = new(cell);
                    cells[level][cellIndex] = cell;
                }
            }

            // Center cell inner walls destroing (center cell only have front walls):
            var centerCell = cells[0][0];
            centerCell.BreakLeftWall();
            centerCell.BreakRightWall();
            centerCell.BreakBackWall();

            TreeNode<CircularMazeCell> NextUnvisitedNodeStrategy(IEnumerable<TreeNode<CircularMazeCell>> unvisitedNodes)
            {
                return unvisitedNodes.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
            }
            void DebugIterationCallback(TreeNode<CircularMazeCell> currCell)
            {
                if (currCell.Data is CircularMazeCell cell)
                {
                    var neighbours = string.Join(',', currCell.Neighbours.Select(node =>
                    {
                        if (node.Data is CircularMazeCell cellNode)
                        {
                            return $"L{cellNode.Level}C{cellNode.Cell}({(node.IsVisited ? "V" : "U")})";
                        }
                        else return "NoMazeCell";
                    }));
                    _logger.Log($"Added L{cell.Level}C{cell.Cell} Neighbours: {neighbours}");
                }
            }
            // Tree building:
            Tree<CircularMazeCell> tree = new(cellNodes[0][0], NextUnvisitedNodeStrategy);//, DebugIterationCallback);
            var rootNode = tree.RootNode;
            void DebugBindNodes(TreeNode<CircularMazeCell> first, TreeNode<CircularMazeCell> second)
            {
                var cell = first.Data;
                var neighbourCell = second.Data;
                var cellNeighbours = first.Neighbours.Select(node => node.Data);
                var neighbourNeighbours = second.Neighbours.Select(node => node.Data);
                if (neighbourNeighbours.Where(c => c.Level == cell.Level && c.Cell == cell.Cell).Any())
                {
                    _logger.LogWarning($"BindWith:L{cell.Level}C{cell.Cell} already neigh of L{neighbourCell.Level}C{neighbourCell.Cell}");
                }
                if (cellNeighbours.Where(c => c.Level == neighbourCell.Level && c.Cell == neighbourCell.Cell).Any())
                {
                    _logger.LogWarning($"BindWith:L{neighbourCell.Level}C{neighbourCell.Cell} already neigh of L{cell.Level}C{cell.Cell}");
                }
                first.BindWith(second);
            }
            for (int cell = 0; cell < cellsOnLevelCache[1]; cell++)
            {
                DebugBindNodes(rootNode, cellNodes[1][cell]);
            }
            for (int level = 1; level < levels; level++)
            {
                int cellsOnLevel = cellNodes[level].Length;
                bool isNotLastLevel = level + 1 < levels;
                int cellsOnNextLevel = isNotLastLevel ? cellNodes[level + 1].Length : 0;
                bool isNextLevelExtended = isNotLastLevel && cellsOnNextLevel > cellsOnLevel;
                int cellsPerPrevLevelCell = cellsOnNextLevel / cellsOnLevel;
                for (int cell = 0; cell < cellsOnLevel; cell++)
                {
                    var cellNode = cellNodes[level][cell];
                    var nextCellNode = cellNodes[level][(cell + 1) % cellsOnLevel];
                    //_logger.Log($"Bind1 L{cellNode.Data.Level}C{cellNode.Data.Cell} with L{nextCellNode.Data.Level}C{nextCellNode.Data.Cell}");
                    DebugBindNodes(cellNode, nextCellNode);
                    if (isNotLastLevel)
                    {
                        if (isNextLevelExtended)
                        {
                            var nextLevelCellsOffset = cell * cellsPerPrevLevelCell;
                            for (int i = 0; i < cellsPerPrevLevelCell; i++)
                            {
                                //_logger.Log($"Bind2 L{cellNode.Data.Level}C{cellNode.Data.Cell} with L{level + 1}C{nextLevelCellsOffset + i}");
                                DebugBindNodes(cellNode, cellNodes[level + 1][nextLevelCellsOffset + i]);
                            }
                        }
                        else
                        {
                            //_logger.Log($"Bind3 L{cellNode.Data.Level}C{cellNode.Data.Cell} with L{level + 1}C{cell}");
                            DebugBindNodes(cellNode, cellNodes[level + 1][cell]);
                        }
                    }
                }
            }

            var dfsPath = tree.DepthFirstSearch();

            _logger.Log(string.Join("->", dfsPath.Select(cell => $"L{cell.Level}C{cell.Cell}")));

            // Walls destroing:
            for (int i = 1; i < dfsPath.Count; i++)
            {
                var prevCell = dfsPath[i - 1];
                var cell = dfsPath[i];
                int cellsOnPrevLevel = cellsOnLevelCache[prevCell.Level];
                int cellsOnLevel = cellsOnLevelCache[cell.Level];
                int levelCellsPerCell = cellsOnLevel > cellsOnPrevLevel ? cellsOnLevel / cellsOnPrevLevel : cellsOnPrevLevel / cellsOnLevel;
                if (cell.Level != prevCell.Level)
                {
                    if (cell.Level > prevCell.Level)
                    {
                        cell.BreakBackWall();
                        prevCell.BreakFrontWall(cell.Cell % levelCellsPerCell);
                    }
                    else
                    {
                        cell.BreakFrontWall(prevCell.Cell % levelCellsPerCell);
                        prevCell.BreakBackWall();
                    }
                }
                else if (cell.Cell == prevCell.Cell + 1 || (cell.Cell == 0 && prevCell.Cell > 1))
                {
                    cell.BreakLeftWall();
                    prevCell.BreakRightWall();
                }
                else
                {
                    cell.BreakRightWall();
                    prevCell.BreakLeftWall();
                }
            }

            var uniquePath = new List<CircularMazeCell>();
            foreach (var node in dfsPath)
            {
                if (!uniquePath.Contains(node))
                {
                    uniquePath.Add(node);
                }
            }

            return new(cells, uniquePath[^1]);
        }

        private readonly ILogger _logger;
    }
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace KarenKrill.UniCore.PathFinding.DepthFirstSearch
{
    public class TreeNode<T>
    {
        public T Data { get; private set; }
        public bool IsVisited { get; private set; } = false;
        public List<TreeNode<T>> Neighbours { get; private set; } = new();

        public TreeNode(T data)
        {
            Data = data;
        }
        public void BindWith(TreeNode<T> neighbour)
        {
            Neighbours.Add(neighbour);
            neighbour.Neighbours.Add(this);
        }
        public void Visit() => IsVisited = true;
    }

    public delegate TreeNode<T> NextUnvisitedNodeStrategy<T>(IEnumerable<TreeNode<T>> unvisitedNodes);

    public class Tree<T>
    {
        public TreeNode<T> RootNode { get; private set; }

        public Tree(T data, NextUnvisitedNodeStrategy<T>? nextUnvisitedNodeStrategy = null, Action<TreeNode<T>>? debugDfsIteration = null)
        {
            RootNode = new(data);
            _nextUnvisitedNodeStrategy = nextUnvisitedNodeStrategy;
            _debugDfsIteration = debugDfsIteration;
        }
        public Tree(TreeNode<T> rootNode, NextUnvisitedNodeStrategy<T>? nextUnvisitedNodeStrategy = null, Action<TreeNode<T>>? debugDfsIteration = null)
        {
            RootNode = rootNode;
            _nextUnvisitedNodeStrategy = nextUnvisitedNodeStrategy;
            _debugDfsIteration = debugDfsIteration;
        }
        public void DepthFirstSearch(TreeNode<T> currCell, ref List<T> values)
        {
            currCell.Visit();
            _debugDfsIteration?.Invoke(currCell);
            TreeNode<T> nextCell;
            do
            {
                values.Add(currCell.Data);
                nextCell = GetNextUnvisitedCell(currCell);
                if (nextCell != null)
                {
                    DepthFirstSearch(nextCell, ref values);
                }
            }
            while (nextCell != null);
        }
        public List<T> DepthFirstSearch()
        {
            List<T> path = new();
            DepthFirstSearch(RootNode, ref path);
            return path;
        }

#warning Unremoved debug feature
        private readonly Action<TreeNode<T>>? _debugDfsIteration;
        private readonly NextUnvisitedNodeStrategy<T>? _nextUnvisitedNodeStrategy;

        private IEnumerable<TreeNode<T>> GetUnvisitedCells(TreeNode<T> cell)
        {
            foreach (var neighbour in cell.Neighbours)
            {
                if (!neighbour.IsVisited)
                {
                    yield return neighbour;
                }
            }
        }
        private TreeNode<T> GetNextUnvisitedCell(TreeNode<T> node)
        {
            var unvisitedCells = GetUnvisitedCells(node);
            return _nextUnvisitedNodeStrategy?.Invoke(unvisitedCells) ?? unvisitedCells.FirstOrDefault();
        }
    }
}

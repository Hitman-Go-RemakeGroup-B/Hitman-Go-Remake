using System.Collections.Generic;
using System.Linq;
using BT;
using UnityEngine;

public class BaseEntity
{
    protected Controller _controller;
    public Controller controller { get => _controller; set => _controller = value; }
    protected Touch _touch;

    protected Vector2Int NodeToDir(Node from, Node to)
    { return (to.GridCoordinate - from.GridCoordinate); }

    public BaseEntity(Controller controller)
    { _controller = controller; }

    /// <param name="dirX">the direction to the endNode in the x axis</param>
    /// <param name="dirY">the direction to the endNode in the y axis</param>
    /// <returns>true if the direction is not possible </returns>
    protected virtual bool WrongDirection(int dirX, int dirY)
    {
        // this is going to check if you can move to that direction
        // rook = if abs(x) != abs(y) return false
        // bishop = if abs(x) == abs(y) return false
        // knight = if abs(x) + abs(y) == 3 return false
        // pawn = if dir == _controller.Dir return false
        Vector2Int dir = new(dirX, dirY);
        if (dir != _controller.OldDir)
            return false;

        return true;
    }

    /// <summary>
    /// gives the first direction it can find
    /// </summary>
    /// <returns>succsess once a direction has been found</returns>
    public virtual BT_Node.Status NextDirection()
    {
        if (_controller.IsImmobile)
        {
            return BT_Node.Status.Success;
        }

        //_controller.PossibleNodes.Clear();

        if (_controller.Index >= _controller.PossibleDirections.list.Count)
        {
            _controller.Index = 0;
            _controller.DirIndex = 0;
            _controller.OldDir = Vector2Int.zero;
            return BT_Node.Status.Running;
        }

        Vector2Int[] directions = _controller.PossibleDirections.list[_controller.Index].vector2Ints;
        if (_controller.DirIndex >= directions.Length || directions[_controller.DirIndex] == _controller.OldDir)
        {
            _controller.Index++;
            _controller.DirIndex = 0;
            return BT_Node.Status.Running;
        }

        if (directions[_controller.DirIndex] == _controller.OldDir)
        {
            _controller.DirIndex++;
            return BT_Node.Status.Running;
        }

        _controller.Dir = directions[_controller.DirIndex];
        _controller.DirIndex++;

        return BT_Node.Status.Failure;
    }

    public virtual BT_Node.Status FindDirection()
    {
        Node node = null;

        foreach (Line connection in _controller.CurrentNode.Connections)
        {
            if (connection == null) continue;

            Vector2Int directionToEndNode = NodeToDir(_controller.CurrentNode, connection.EndNode);

            if (WrongDirection(directionToEndNode.x, directionToEndNode.y))
                continue;

            node = connection.EndNode;

            _controller.PossibleNodeDirections.Add(node);
        }
        if (node == null)
            return BT_Node.Status.Failure;

        return BT_Node.Status.Success;
    }

    public virtual BT_Node.Status ChooseDirection()
    {
        if (Input.touchCount <= 0)
            return BT_Node.Status.Running;

        Vector3 touchPos;
        _touch = Input.GetTouch(0);

        if (_touch.phase != TouchPhase.Began)
            return BT_Node.Status.Running;

        Ray ray = Camera.main.ScreenPointToRay(_touch.position);

        Plane plane;
        Node hitNode = null;
        float distance = 99f;

        foreach (Node node in _controller.PossibleNodeDirections)
        {
            if (node == null) continue;

            plane = new(Vector3.up, node.transform.position);
            float dist = 0;

            if (!plane.Raycast(ray, out dist))
                continue;

            touchPos = ray.GetPoint(dist);
            touchPos.y = 0;

            float distanceToNode = (touchPos - node.transform.position).magnitude;

            if (distanceToNode > 5f)
                continue;

            if (distanceToNode < distance)
            {
                distance = distanceToNode;
                hitNode = node;
            }
        }

        if (hitNode == null)
            return BT_Node.Status.Running;

        _controller.Dir = NodeToDir(_controller.CurrentNode, hitNode);
        return BT_Node.Status.Success;
    }

    /// <summary>
    /// finds all the possible nodes the entity can move twoards
    /// </summary>
    /// <param name="controller"></param>
    /// <returns>the status of the process</returns>
    public virtual BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        Node node = null;

        foreach (Line connection in from.Connections)
        {
            if (connection == null) continue;

            Vector2Int directionToEndNode = NodeToDir(from, connection.EndNode);

            if (WrongDirection(directionToEndNode.x, directionToEndNode.y)
                || (_controller.Dir != directionToEndNode
                && _controller.Dir != Vector2Int.zero))
                continue;

            node = connection.EndNode;

            _controller.PossibleNodes.Add(node);
        }

        if (node == null)
            return BT_Node.Status.Failure;

        return BT_Node.Status.Success;
    }

    /// <summary>
    /// finds the next node to go twoards inside FoundPath
    /// </summary>
    /// <param name="from">the node where you are from</param>
    /// <param name="direction">the direction you want to go to</param>
    /// <returns>success after finding the endone</returns>
    public virtual BT_Node.Status FindEndNodeWhileDistracted()
    {
        _controller.OldDir = Vector2Int.zero;
        Node node = _controller.FoundPath[0];

        _controller.EndNode = node;
        _controller.FoundPath.RemoveAt(0);
        if (_controller.FoundPath.Count <= 0)
            _controller.IsDistracted = false;

        return BT_Node.Status.Success;
    }

    /// <summary>
    /// trows a raycast towards the last node of all possible nodes
    /// </summary>
    /// <returns>succes if raycast hits the player</returns>
    public virtual BT_Node.Status Raycast()
    {
        // this is good for the pawn, rook and bishop
        // for the knight you have to change the raycast to a OverlapBox
        _controller.OldDir = -_controller.Dir;
        int possibleNodesNum = _controller.PossibleNodes.Count;
        _controller.EndNode = _controller.PossibleNodes[possibleNodesNum - 1];

        Vector3 direction = _controller.EndNode.transform.position - _controller.CurrentNode.transform.position;
        Ray ray = new Ray(_controller.CurrentNode.transform.position, direction.normalized);
        bool raycast = Physics.Raycast(ray, out RaycastHit hit, direction.magnitude);

        Debug.DrawRay(_controller.CurrentNode.transform.position, direction, Color.red, 10);

        if (raycast && hit.transform.TryGetComponent(out PlayerController player))
        {
            //player.Death();

            _controller.EndNode = player.CurrentNode;
            return BT_Node.Status.Success;
        }

        return BT_Node.Status.Failure;
    }

    /// <summary>
    /// finds the nearest node from the pressed
    /// </summary>
    /// <returns>the pressed end node</returns>
    public virtual BT_Node.Status ChooseEndNode()
    {
        if (Input.touchCount <= 0)
            return BT_Node.Status.Running;

        Vector3 touchPos;
        _touch = Input.GetTouch(0);

        if (_touch.phase != TouchPhase.Began)
            return BT_Node.Status.Running;

        Ray ray = Camera.main.ScreenPointToRay(_touch.position);

        Plane plane;
        Node hitNode = null;
        float distance = 99f;

        foreach (Node node in _controller.PossibleNodes)
        {
            if (node == null) continue;

            plane = new(Vector3.up, node.transform.position);
            float dist = 0;

            if (!plane.Raycast(ray, out dist))
                continue;

            touchPos = ray.GetPoint(dist);
            touchPos.y = 0;

            float distanceToNode = (touchPos - node.transform.position).magnitude;

            if (distanceToNode > 5f)
                continue;

            if (distanceToNode < distance)
            {
                distance = distanceToNode;
                hitNode = node;
            }
        }

        if (hitNode == null)
            return BT_Node.Status.Running;

        _controller.EndNode = hitNode;
        return BT_Node.Status.Success;
    }

    /// <summary>
    /// move twoards the controller's endnode
    /// </summary>
    /// <returns> Running while going twoards the endNode and Success when it's done </returns>
    public virtual BT_Node.Status MoveTwoardsEndNode()
    {
        if (_controller.IsImmobile && !_controller.IsKilling || _controller.EndNode == null)
            return BT_Node.Status.Success;

        if (_controller.Timer < _controller.TimeToReachNextNode)
        {
            float deltaTime = _controller.Timer / _controller.TimeToReachNextNode;
            _controller.transform.position = Vector3.Lerp(_controller.StartPos, _controller.EndNode.transform.position, deltaTime);
            _controller.Timer += Time.deltaTime;
            return BT_Node.Status.Running;
        }

        _controller.Timer = 0;
        _controller.DirIndex = 0;
        _controller.transform.position = _controller.EndNode.transform.position;
        _controller.StartPos = _controller.transform.position;
        _controller.CurrentNode = _controller.EndNode;
        _controller.EndNode = null;
        _controller.PossibleNodeDirections.Clear();
        _controller.PossibleNodes.Clear();
        return BT_Node.Status.Success;
    }
}

public class PawnEntity : BaseEntity
{
    int _index = 2;
    public PawnEntity(Controller controller) : base(controller)
    {
    }

    protected override bool WrongDirection(int dirX, int dirY)
    {
        if (Mathf.Abs(dirX) != Mathf.Abs(dirY))
            return false;
        return true;
    }

    public override BT_Node.Status NextDirection()
    {
        return base.NextDirection();
    }

    public override BT_Node.Status FindDirection()
    {
        if (_controller.IsFirstTurn)
            return base.FindDirection();

        return BT_Node.Status.Success;
    }

    public override BT_Node.Status ChooseDirection()
    {
        if (_controller.IsFirstTurn)
            return base.ChooseDirection();

        return BT_Node.Status.Success;
    }

    public override BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        if (!_controller.IsFirstTurn)
            return base.FindPossibleNodes(from, direction);

        Node node = null;

        foreach (Line connection in from.Connections)
        {
            if (connection == null) continue;

            Vector2Int directionToEndNode = NodeToDir(from, connection.EndNode);

            if (WrongDirection(directionToEndNode.x, directionToEndNode.y)
                || (_controller.Dir != directionToEndNode
                && _controller.Dir != Vector2Int.zero))
                continue;

            node = connection.EndNode;

            _controller.PossibleNodes.Add(node);
            _index--;

            if (_index > 0)
            {
                if (FindPossibleNodes(node, directionToEndNode) != BT_Node.Status.Success)
                    continue;
            }

        }

        if (node == null)
            return BT_Node.Status.Failure;

        _controller.IsFirstTurn = false;
        return BT_Node.Status.Success;
    }

    public override BT_Node.Status FindEndNodeWhileDistracted()
    {
        return base.FindEndNodeWhileDistracted();
    }

    public override BT_Node.Status ChooseEndNode()
    {
        if (!_controller.IsFirstTurn)
            _controller.Dir = Vector2Int.zero;
        return base.ChooseEndNode();
    }

    public override BT_Node.Status Raycast()
    {
        return base.Raycast();
    }

    public override BT_Node.Status MoveTwoardsEndNode()
    {
        return base.MoveTwoardsEndNode();
    }
}

public class RookEntity : BaseEntity
{
    public RookEntity(Controller controller) : base(controller)
    {
    }

    protected override bool WrongDirection(int dirX, int dirY)
    {
        if (Mathf.Abs(dirX) != Mathf.Abs(dirY))
            return false;
        return true;
    }

    public override BT_Node.Status NextDirection()
    {
        return base.NextDirection();
    }

    public override BT_Node.Status FindDirection()
    {
        return base.FindDirection();
    }

    public override BT_Node.Status ChooseDirection()
    {
        return base.ChooseDirection();
    }

    public override BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        Node node = null;

        foreach (Line connection in from.Connections)
        {
            if (connection == null) continue;

            Vector2Int directionToEndNode = NodeToDir(from, connection.EndNode);

            if (WrongDirection(directionToEndNode.x, directionToEndNode.y)
                || (_controller.Dir != directionToEndNode
                && _controller.Dir != Vector2Int.zero))
                continue;

            node = connection.EndNode;

            _controller.PossibleNodes.Add(node);

            if (FindPossibleNodes(node, directionToEndNode) == BT_Node.Status.Failure)
                continue;
        }

        if (node == null)
            return BT_Node.Status.Failure;

        return BT_Node.Status.Success;
    }

    public override BT_Node.Status FindEndNodeWhileDistracted()
    {
        Node node = _controller.FoundPath[0];
        Vector2Int direction = NodeToDir(_controller.CurrentNode, node);
        _controller.FoundPath.RemoveAt(0);
        node = GetLastNodeInDirection(node, direction);

        _controller.EndNode = node;

        if (_controller.FoundPath.Count <= 0)
            _controller.IsDistracted = false;

        return BT_Node.Status.Success;

        Node GetLastNodeInDirection(Node from, Vector2Int direction)
        {
            Node node = _controller.FoundPath[0];

            if (NodeToDir(from, node) == direction)
            {
                _controller.FoundPath.RemoveAt(0);
                return GetLastNodeInDirection(node, direction);
            }

            return from;
        }
    }

    public override BT_Node.Status Raycast()
    {
        return base.Raycast();
    }

    public override BT_Node.Status ChooseEndNode()
    {
        return base.ChooseEndNode();
    }

    public override BT_Node.Status MoveTwoardsEndNode()
    {
        return base.MoveTwoardsEndNode();
    }
}

public class BishopEntity : BaseEntity
{
    public BishopEntity(Controller controller) : base(controller)
    {
    }

    protected override bool WrongDirection(int dirX, int dirY)
    {
        if (Mathf.Abs(dirX) == Mathf.Abs(dirY))
            return false;
        return true;
    }

    public override BT_Node.Status NextDirection()
    {
        return base.NextDirection();
    }
    public override BT_Node.Status FindDirection()
    {
        return base.FindDirection();
    }

    public override BT_Node.Status ChooseDirection()
    {
        return base.ChooseDirection();
    }

    public override BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        Node node = null;

        foreach (Line connection in from.Connections)
        {
            if (connection == null) continue;

            Vector2Int directionToEndNode = NodeToDir(from, connection.EndNode);

            if (WrongDirection(directionToEndNode.x, directionToEndNode.y)
                || (_controller.Dir != directionToEndNode
                && _controller.Dir != Vector2Int.zero) || base.WrongDirection(directionToEndNode.x, directionToEndNode.y))
                continue;

            node = connection.EndNode;

            _controller.PossibleNodes.Add(node);

            if (FindPossibleNodes(node, directionToEndNode) == BT_Node.Status.Failure)
                continue;
        }

        if (node == null)
            return BT_Node.Status.Failure;

        return BT_Node.Status.Success;
    }

    public override BT_Node.Status FindEndNodeWhileDistracted()
    {
        Node node = _controller.FoundPath[0];
        Vector2Int direction = NodeToDir(_controller.CurrentNode, node);
        _controller.FoundPath.RemoveAt(0);
        node = GetLastNodeInDirection(node, direction);

        _controller.EndNode = node;

        if (_controller.FoundPath.Count <= 0)
            _controller.IsDistracted = false;

        return BT_Node.Status.Success;

        /// returns the last node that can be reached in the same direction that is given
        Node GetLastNodeInDirection(Node from, Vector2Int direction)
        {
            Node node = _controller.FoundPath[0];

            if (NodeToDir(from, node) == direction)
            {
                _controller.FoundPath.RemoveAt(0);
                return GetLastNodeInDirection(node, direction);
            }

            return from;
        }
    }

    public override BT_Node.Status Raycast()
    {
        return base.Raycast();
    }

    public override BT_Node.Status ChooseEndNode()
    {
        return base.ChooseEndNode();
    }

    public override BT_Node.Status MoveTwoardsEndNode()
    {
        return base.MoveTwoardsEndNode();
    }
}

public class KnightEntity : BaseEntity
{
    private float _timeToReachEndPos;

    private float _timer;

    private int _index = 0;
    Vector3[] _directions = new Vector3[2];
    Vector2Int[] _possibleNodeDirs = new Vector2Int[8] { new(1, -2), new(-1, -2), new(-2, 1), new(-2, -1), new(1, 2), new(-1, 2), new(2, 1), new(2, -1) };

    List<Vector2Int> possibleDirections = new List<Vector2Int>();

    private Node GetNodeFromCoordinate(Vector2Int dir) => _controller.NodeFromCoordinates?.Invoke(dir);

    public KnightEntity(Controller controller) : base(controller)
    {
        _timeToReachEndPos = _controller.TimeToReachNextNode / 2;
        _timer = 0;
    }

    protected override bool WrongDirection(int dirX, int dirY)
    {
        if (Mathf.Abs(dirX) + Mathf.Abs(dirY) == 3)
            return false;
        return true;
    }

    public override BT_Node.Status NextDirection()
    {
        return BT_Node.Status.Success;
    }
    public override BT_Node.Status FindDirection()
    {
        //_controller.PossibleNodeDirections.Clear();
        return BT_Node.Status.Success;
    }

    public override BT_Node.Status ChooseDirection()
    {
        return BT_Node.Status.Success;
    }

    public override BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        if (_controller.IsImmobile)
            return BT_Node.Status.Success;

        var contrCoordinates = _controller.CurrentNode.GridCoordinate;

        foreach (Vector2Int dir in _possibleNodeDirs)
        {
            var node = GetNodeFromCoordinate(contrCoordinates + dir);
            if (node == null)
                continue;
            _controller.PossibleNodes.Add(node);
        }

        if (_controller.PossibleNodes.Count <= 0)
            return BT_Node.Status.Failure;


        return BT_Node.Status.Success;
    }

    public override BT_Node.Status FindEndNodeWhileDistracted()
    {
        return base.FindEndNodeWhileDistracted();
    }

    public override BT_Node.Status ChooseEndNode()
    {
        var baseChoice = base.ChooseEndNode();
        if (baseChoice != BT_Node.Status.Success)
            return baseChoice;

        Vector2Int direction = NodeToDir(_controller.CurrentNode, _controller.EndNode);
        float dirX = Mathf.Abs(direction.x);
        float dirY = Mathf.Abs(direction.y);
        if (dirX > dirY)
        {
            _directions[0] = new(_controller.EndNode.transform.position.x, 0, _controller.transform.position.z);
        }
        else
        {
            _directions[0] = new(_controller.transform.position.x, 0, _controller.EndNode.transform.position.z);
        }
        _directions[1] = _controller.EndNode.transform.position;
        return BT_Node.Status.Success;
    }

    public override BT_Node.Status Raycast()
    {
        _controller.OldDir = -_controller.Dir;
        Vector2Int direction = _controller.Dir;
        int dirX = Mathf.Abs(direction.x);
        int dirY = Mathf.Abs(direction.y);

        for (int i = 0; i < 2; i++)
        {
            Node rayNode = GetNodeFromCoordinate(_controller.CurrentNode.GridCoordinate + direction);

            if (rayNode == null) continue;

            Collider[] overlapBox = Physics.OverlapBox(rayNode.transform.position, Vector3.one);

#if UNITY_EDITOR
            DebugExtensions.DrawBox(rayNode.transform.position, rayNode.transform.rotation, Vector3.one, Color.red, 99f);
#endif
            foreach (Collider col in overlapBox)
            {
                if (col.TryGetComponent(out PlayerController playerController))
                {
                    _controller.EndNode = playerController.CurrentNode;
                    _controller.IsKilling = true;

                    if (dirX > dirY)
                    {
                        _directions[0] = new(_controller.EndNode.transform.position.x, 0, _controller.transform.position.z);
                    }
                    else
                    {
                        _directions[0] = new(_controller.transform.position.x, 0, _controller.EndNode.transform.position.z);
                    }
                    _directions[1] = _controller.EndNode.transform.position;

                    return BT_Node.Status.Success;
                }
            }

            if (dirX < 2)
            {
                direction.x *= -1;
                continue;
            }

            direction.y *= -1;
        }

        return BT_Node.Status.Failure;
    }

    public override BT_Node.Status MoveTwoardsEndNode()
    {
        if (_controller.EndNode == null || _controller.IsImmobile && !(_controller.IsKilling || _controller.IsDistracted))
            return BT_Node.Status.Success;

        if (_index >= _directions.Length)
        {
            _index = 0;
            _controller.StartPos = _controller.transform.position;
            _controller.CurrentNode = _controller.EndNode;
            _controller.EndNode = null;
            _controller.IsKilling = false;
            _controller.PossibleNodeDirections.Clear();
            _controller.PossibleNodes.Clear();
            return BT_Node.Status.Success;
        }

        if (_timer < _timeToReachEndPos)
        {
            float deltaTime = _timer / _timeToReachEndPos;
            _controller.transform.position = Vector3.Lerp(_controller.StartPos, _directions[_index], deltaTime);
            _timer += Time.deltaTime;
            return BT_Node.Status.Running;
        }

        _controller.transform.position = _directions[_index];
        _controller.StartPos = _controller.transform.position;
        _timer = 0;
        _index++;
        return BT_Node.Status.Running;
    }
}
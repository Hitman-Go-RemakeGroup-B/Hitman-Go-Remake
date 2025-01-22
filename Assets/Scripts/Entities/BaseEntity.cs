using BT;
using UnityEngine;

public class BaseEntity
{
    protected Controller _controller;
    protected Touch _touch;

    protected Vector2Int DirToNode(Node from, Node to)
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

        _controller.PossibleNodes.Clear();

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

    /// <summary>
    /// finds all the possible nodes the entity can move twoards
    /// </summary>
    /// <param name="controller"></param>
    /// <returns>the status of the process</returns>
    public virtual BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        _controller.StartPos = _controller.transform.position;
        Node node = null;

        foreach (Line connection in from.Connections)
        {
            if (connection == null) continue;

            Vector2Int directionToEndNode = DirToNode(from, connection.EndNode);

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
        Debug.Log(possibleNodesNum);
        _controller.EndNode = _controller.PossibleNodes[possibleNodesNum - 1];
        _controller.PossibleNodes.Clear();
        Vector3 direction = _controller.EndNode.transform.position - _controller.CurrentNode.transform.position;
        Ray ray = new Ray(_controller.CurrentNode.transform.position, direction.normalized);
        bool raycast = Physics.Raycast(ray, out RaycastHit hit, direction.magnitude);

        Debug.DrawRay(_controller.CurrentNode.transform.position, direction, Color.red, 10);

        if (raycast && hit.transform.TryGetComponent(out PlayerController player))
        {
            //player.Death();
            _controller.PossibleNodes.Clear();
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
        if (Input.touchCount < 0)
            return BT_Node.Status.Running;
        Vector3 touchPos;
        Vector3 touchBox;
        float touchMagnitude;
        _touch = Input.GetTouch(0);

        if (_touch.phase != TouchPhase.Began)
            return BT_Node.Status.Running;

        touchPos = Camera.main.ScreenToWorldPoint(_touch.position);
        touchBox = new(1, 0, 1);
        touchMagnitude = touchPos.x + touchPos.y;

        foreach (Node node in _controller.PossibleNodes)
        {
            Vector3 nodePos = (node.transform.position + touchBox);
            Vector3 nodeNegative = node.transform.position - touchBox;
            float nodeMagnitude = nodePos.x + nodePos.y;
            float nodeNegativeMagnitude = nodeNegative.x + nodeNegative.y;

            if (touchMagnitude <= nodeMagnitude || touchMagnitude >= nodeNegativeMagnitude)
            {
                _controller.EndNode = node;
                _controller.PossibleNodes.Clear();
                return BT_Node.Status.Success;
            }
        }

        return BT_Node.Status.Running;
    }

    /// <summary>
    /// move twoards the controller's endnode
    /// </summary>
    /// <returns> Running while going twoards the endNode and Success when it's done </returns>
    public virtual BT_Node.Status MoveTwoardsEndNode()
    {
        if (_controller.IsImmobile && !_controller.IsKilling)
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
        _controller.CurrentNode = _controller.EndNode;

        return BT_Node.Status.Success;
    }
}

public class PawnEntity : BaseEntity
{
    public PawnEntity(Controller controller) : base(controller)
    {
    }

    protected override bool WrongDirection(int dirX, int dirY)
    {
        if (new Vector2Int(dirX, dirY) == _controller.Dir)
            return false;
        return true;
    }

    public override BT_Node.Status NextDirection()
    {
        return base.NextDirection();
    }

    public override BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        return base.FindPossibleNodes(from, direction);
    }

    public override BT_Node.Status FindEndNodeWhileDistracted()
    {
        return base.FindEndNodeWhileDistracted();
    }

    public override BT_Node.Status ChooseEndNode()
    {
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
    private int _indexConnections;

    private int debug;

    public RookEntity(Controller controller) : base(controller)
    {
    }

    protected override bool WrongDirection(int dirX, int dirY)
    {
        if (!base.WrongDirection(dirX, dirY) || Mathf.Abs(dirX) != Mathf.Abs(dirY))
            return false;
        return true;
    }

    public override BT_Node.Status NextDirection()
    {
        return base.NextDirection();
    }

    public override BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        _controller.StartPos = _controller.transform.position;
        Node node = null;

        foreach (Line connection in from.Connections)
        {
            if (connection == null) continue;

            Vector2Int directionToEndNode = DirToNode(from, connection.EndNode);

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
        Vector2Int direction = DirToNode(_controller.CurrentNode, node);
        _controller.FoundPath.RemoveAt(0);
        node = GetLastNodeInDirection(node, direction);

        _controller.EndNode = node;

        if (_controller.FoundPath.Count <= 0)
            _controller.IsDistracted = false;

        return BT_Node.Status.Success;

        Node GetLastNodeInDirection(Node from, Vector2Int direction)
        {
            Node node = _controller.FoundPath[0];

            if (DirToNode(from, node) == direction)
            {
                _controller.FoundPath.RemoveAt(0);
                return GetLastNodeInDirection(node, direction);
            }

            return from;
        }
    }

    public override BT_Node.Status Raycast()
    {
        Debug.Log(debug);

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
        if (!base.WrongDirection(dirX, dirY) || Mathf.Abs(dirX) == Mathf.Abs(dirY))
            return false;
        return true;
    }

    public override BT_Node.Status NextDirection()
    {
        return base.NextDirection();
    }

    public override BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        _controller.StartPos = _controller.transform.position;
        Node node = null;

        foreach (Line connection in from.Connections)
        {
            if (connection == null) continue;

            Vector2Int directionToEndNode = DirToNode(from, connection.EndNode);

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
        Vector2Int direction = DirToNode(_controller.CurrentNode, node);
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

            if (DirToNode(from, node) == direction)
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
    Node GetNodeFromDirection(Vector2Int dir) => _controller.NodeFromCoordinates?.Invoke(dir);
    public KnightEntity(Controller controller) : base(controller)
    {
        _timeToReachEndPos = _controller.TimeToReachNextNode / 2;
        _timer = 0;
    }

    protected override bool WrongDirection(int dirX, int dirY)
    {
        if (!base.WrongDirection(dirX, dirY) || Mathf.Abs(dirX) + Mathf.Abs(dirY) == 3)
            return false;
        return true;
    }

    public override BT_Node.Status NextDirection()
    {
        return BT_Node.Status.Success;
    }

    public override BT_Node.Status FindPossibleNodes(Node from, Vector2Int direction)
    {
        return BT_Node.Status.Success;
    }

    public override BT_Node.Status FindEndNodeWhileDistracted()
    {
        return base.FindEndNodeWhileDistracted();
    }

    public override BT_Node.Status ChooseEndNode()
    {
        return base.ChooseEndNode();
    }

    public override BT_Node.Status Raycast()
    {
        _controller.OldDir = -_controller.Dir;
        Vector2Int direction = _controller.Dir;
        int x = Mathf.Abs(direction.x);
        int y = Mathf.Abs(direction.y);
       

        for (int i = 0; i < 2; i++)
        {
            Node rayNode = GetNodeFromDirection(_controller.CurrentNode.GridCoordinate + direction);

            if (rayNode == null) continue;

            Collider[] overlapBox = Physics.OverlapBox(rayNode.transform.position, Vector3.one);
            Debug.Log("wow", rayNode.transform);

#if UNITY_EDITOR
            DebugExtensions.DrawBox(rayNode.transform.position, rayNode.transform.rotation, Vector3.one, Color.red, 99f);
#endif
            foreach (Collider col in overlapBox)
            {
                if (col.TryGetComponent(out PlayerController playerController))
                {
                    _controller.EndNode = playerController.CurrentNode;
                    _controller.IsKilling = true;
                    return BT_Node.Status.Success;
                }
            }

            if(x <2)
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
        if (_controller.IsImmobile && !_controller.IsKilling && !_controller.IsDistracted)
            return BT_Node.Status.Success;
        Node endNode = _controller.EndNode;
        Node currentNode = _controller.CurrentNode;
        Vector2Int directionToEndnode = DirToNode(currentNode, endNode);
        Debug.Log("directionToEndNode = " + directionToEndnode);

        Vector3 endPosX = GetNodeFromDirection(new(currentNode.GridCoordinate.x + directionToEndnode.x, currentNode.GridCoordinate.y)).transform.position;
        Vector3 endPosZ = GetNodeFromDirection(new(currentNode.GridCoordinate.x, currentNode.GridCoordinate.y + directionToEndnode.y)).transform.position;
       
        Vector3[] endPositions = new Vector3[2];

        if (Mathf.Abs(directionToEndnode.x) > Mathf.Abs(directionToEndnode.y))
        {
            endPositions[0] = endPosX;
            endPositions[1] = endPosZ;
        }
        else
        {
            endPositions[0] = endPosZ;
            endPositions[1] = endPosX;
        }

        if (!MoveTo(endPositions, 0))
        {
            return BT_Node.Status.Running;
        }

        _controller.CurrentNode = _controller.EndNode;
        return BT_Node.Status.Success;

        bool MoveTo(Vector3[] endPos, int index)
        {
            if (index > endPos.Length - 1)
                return true;

            Vector3 pos = _controller.transform.position;
            if (_controller.Timer < _controller.TimeToReachNextNode)
            {
                float deltaTime = _timer / _timeToReachEndPos;
                _controller.transform.position = Vector3.Lerp(_controller.StartPos, endPos[index], deltaTime);
                _timer += Time.deltaTime;
                return false;
            }
            else
            {
                _timer = 0;
                _controller.transform.position = endPos[index];
                return MoveTo(endPos, index + 1);
            }
        }
    }
}
using System.Collections.Generic;
using BT.Process;

namespace BT
{
    public class BT_Node
    {
        public enum Status
        { Success, Failure, Running }

        public readonly string Name;

        public readonly List<BT_Node> Children = new();
        public int CurrentChild;

        public BT_Node(string _Name)
        {
            Name = _Name;
        }

        public virtual void AddChild(BT_Node _ChildNode)
        {
            Children.Add(_ChildNode);
        }

        public virtual Status Process()
        {
            return Children[CurrentChild].Process();
        }

        public virtual void Reset()
        {
            CurrentChild = 0;
            foreach (var _child in Children)
            {
                _child.Reset();
            }
        }
    }

    public class BehaviourTree : BT_Node
    {
        public BehaviourTree(string _Name) : base(_Name)
        { }

        public override Status Process()
        {
            if (CurrentChild < Children.Count)
            {
                var status = Children[CurrentChild].Process();
                if (status != Status.Success)
                {
                    return status;
                }

                // else
                CurrentChild++;
                return Status.Running;
            }

            return Status.Success;
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void AddChild(BT_Node _ChildNode)
        {
            base.AddChild(_ChildNode);
        }
    }

    public class BT_Sequence : BT_Node
    {
        public BT_Sequence(string _Name) : base(_Name)
        {
        }

        public override Status Process()
        {
            if (CurrentChild >= Children.Count)
            {
                return Status.Success;
            }

            var childStatus = Children[CurrentChild].Process();
            //Debug.Log(childStatus);
            if (childStatus == Status.Failure)
                return Status.Failure;

            if (childStatus == Status.Success)
            {
                CurrentChild++;
            }

            return Status.Running;
        }

        public override void AddChild(BT_Node _ChildNode)
        {
            base.AddChild(_ChildNode);
        }

        public override void Reset()
        {
            base.Reset();
        }
    }

    public class BT_Selector : BT_Node
    {
        public BT_Selector(string _Name) : base(_Name)
        {
        }

        public override void AddChild(BT_Node _ChildNode)
        {
            base.AddChild(_ChildNode);
        }

        public override Status Process()
        {
            if (CurrentChild >= Children.Count)
                return Status.Failure;

            var _ChildProcess = Children[CurrentChild].Process();

            if (_ChildProcess == Status.Failure)
            {
                CurrentChild++;
                return Status.Running;
            }

            return _ChildProcess;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }

    public class BT_Repeater : BT_Node
    {
        public BT_Repeater(string _Name) : base(_Name)
        {
        }

        public override void AddChild(BT_Node _ChildNode)
        {
            base.AddChild(_ChildNode);
        }

        public override Status Process()
        {
            if (CurrentChild >= Children.Count)
            {
                Reset();
                return Status.Running;
            }

            var _ChildProcess = Children[CurrentChild].Process();

            if (_ChildProcess == Status.Failure)
            {
                CurrentChild++;
                return Status.Running;
            }

            if(_ChildProcess == Status.Success)
            {
                Reset();
            }

            return _ChildProcess;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }

    public class BT_Leaf : BT_Node
    {
        public IProcess MyProcess;

        public BT_Leaf(string _Name, IProcess _Process) : base(_Name)
        {
            MyProcess = _Process;
        }

        public override Status Process()
        {
            return MyProcess.Process();
        }

        public override void Reset()
        {
            MyProcess.Reset();
        }

        public override void AddChild(BT_Node _ChildNode)
        {
            base.AddChild(_ChildNode);
        }
    }
}
using System;
using System.Collections.Generic;
using BT.Process;
using UnityEngine;

namespace BT.Decorator
{
    public interface IDecorator
    {
        BT_Node.Status Process();

        public void Reset();
    }

    public class BaseDecorator : IDecorator
    {
        public Controller Controller;

        public BaseDecorator(Controller controller)
        {
            Controller = controller;
        }

        public virtual BT_Node.Status Process()
        {
            return BT_Node.Status.Success;
        }

        public virtual void Reset()
        {
            
        }
    }

    public class CheckDecorator : BaseDecorator
    {
        public delegate BT_Node.Status ProcessAction();

        public List<IProcess> Decorators { get; set; }
        protected ProcessAction _status { get; set; }

        public CheckDecorator(Controller controller, ProcessAction action) : base(controller)
        {
            Inizialize(controller, action);
        }

        public void Inizialize(Controller parameter, ProcessAction action)
        {
            Controller = parameter;
            _status += action;
        }

        public override BT_Node.Status Process()
        {
            if (_status != null)
                return _status.Invoke();
            else return BT_Node.Status.Failure;
        }

        public override void Reset()
        {
        }
    }

    public class FindNodeDecorator : BaseDecorator
    {
        public delegate BT_Node.Status ProcessAction(Node from, Vector2Int direction);
        protected ProcessAction _status;


        public FindNodeDecorator(Controller controller, ProcessAction status) : base(controller)
        {
            _status = status;
        }

        public override BT_Node.Status Process()
        {
            if (_status != null)
                return _status.Invoke(Controller.CurrentNode, Controller.Dir);
            return BT_Node.Status.Failure;
        }

        public override void Reset()
        {

        }
    }
}
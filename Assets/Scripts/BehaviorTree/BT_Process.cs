using System;
using System.Collections.Generic;
using BT.Decorator;

namespace BT.Process
{
    public interface IProcess
    {
        List<IDecorator> Decorators { get; set; }

        void AddDecorator(IDecorator decorator);

        BT_Node.Status Process();

        void Reset();
    }

    public class BaseProcess : IProcess
    {
        protected BT_Node.Status status;

        public List<IDecorator> Decorators { get; set; }

        public BaseProcess(IDecorator decorator)
        {
            AddDecorator(decorator);
            status = BT_Node.Status.Running;
        }

        public BaseProcess(List<IDecorator> _Decorators)
        {
            Decorators = _Decorators;
            status = BT_Node.Status.Running;
        }

        /// <summary>
        /// all decorations processes
        /// </summary>
        /// <returns></returns>
        public virtual BT_Node.Status Process()
        {
            status = BT_Node.Status.Success;
            if (Decorators.Count > 0)
            {
                foreach (var _Decorator in Decorators)
                {
                    var _DecProcess = _Decorator.Process();

                    if (_DecProcess != BT_Node.Status.Success)
                    {
                        return _DecProcess;
                    }
                    _Decorator.Reset();
                }
            }

            return status;
        }

        public virtual void Reset()
        {
            if (Decorators.Count <= 0)
                return;

            foreach (var _decorator in Decorators)
            {
                _decorator.Reset();
            }
        }

        public virtual void AddDecorator(IDecorator _Decorator)
        {
            Decorators ??= new(); // if (decorators == null) decorators = new();
            Decorators.Add(_Decorator);
        }

        internal void AddDecorator(CheckDecorator baseDecorator)
        {
            throw new NotImplementedException();
        }
    }
}
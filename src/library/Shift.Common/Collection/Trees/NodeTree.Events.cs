using System;
using System.ComponentModel;

using Shift.Constant;

namespace Shift.Common.Trees
{
    public partial class NodeTree<T>
    {
        //-----------------------------------------------------------------------------
        // Events

        protected EventHandlerList EventHandlerList { get; private set; }

        protected static object ValidateEventKey { get; } = new object();

        protected static object ClearingEventKey { get; } = new object();

        protected static object ClearedEventKey { get; } = new object();

        protected static object SettingEventKey { get; } = new object();

        protected static object SetDoneEventKey { get; } = new object();

        protected static object InsertingEventKey { get; } = new object();

        protected static object InsertedEventKey { get; } = new object();

        protected static object CuttingEventKey { get; } = new object();

        protected static object CutDoneEventKey { get; } = new object();

        protected static object CopyingEventKey { get; } = new object();

        protected static object CopiedEventKey { get; } = new object();

        protected static object DeepCopyingEventKey { get; } = new object();

        protected static object DeepCopiedEventKey { get; } = new object();

        public event EventHandler<NodeTreeDataEventArgs<T>> Validate
        {
            add => GetCreateEventHandlerList().AddHandler(ValidateEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(ValidateEventKey, value);
        }

        public event EventHandler<NodeTreeDataEventArgs<T>> Setting
        {
            add => GetCreateEventHandlerList().AddHandler(SettingEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(SettingEventKey, value);
        }

        public event EventHandler<NodeTreeDataEventArgs<T>> SetDone
        {
            add => GetCreateEventHandlerList().AddHandler(SetDoneEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(SetDoneEventKey, value);
        }

        public event EventHandler<NodeTreeInsertEventArgs<T>> Inserting
        {
            add => GetCreateEventHandlerList().AddHandler(InsertingEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(InsertingEventKey, value);
        }

        public event EventHandler<NodeTreeInsertEventArgs<T>> Inserted
        {
            add => GetCreateEventHandlerList().AddHandler(InsertedEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(InsertedEventKey, value);
        }

        public event EventHandler Cutting
        {
            add => GetCreateEventHandlerList().AddHandler(CuttingEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(CuttingEventKey, value);
        }

        public event EventHandler CutDone
        {
            add => GetCreateEventHandlerList().AddHandler(CutDoneEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(CutDoneEventKey, value);
        }

        public event EventHandler<NodeTreeNodeEventArgs<T>> Copying
        {
            add => GetCreateEventHandlerList().AddHandler(CopyingEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(CopyingEventKey, value);
        }

        public event EventHandler<NodeTreeNodeEventArgs<T>> Copied
        {
            add => GetCreateEventHandlerList().AddHandler(CopiedEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(CopiedEventKey, value);
        }

        public event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopying
        {
            add => GetCreateEventHandlerList().AddHandler(DeepCopyingEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(DeepCopyingEventKey, value);
        }

        public event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopied
        {
            add => GetCreateEventHandlerList().AddHandler(DeepCopiedEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(DeepCopiedEventKey, value);
        }

        public event EventHandler Clearing
        {
            add => GetCreateEventHandlerList().AddHandler(ClearingEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(ClearingEventKey, value);
        }

        public event EventHandler Cleared
        {
            add => GetCreateEventHandlerList().AddHandler(ClearedEventKey, value);

            remove => GetCreateEventHandlerList().RemoveHandler(ClearedEventKey, value);
        }

        protected EventHandlerList GetCreateEventHandlerList()
        {
            return EventHandlerList ?? (EventHandlerList = new EventHandlerList());
        }

        //-----------------------------------------------------------------------------
        // Validate

        protected virtual void OnValidate(INode<T> node, T data)
        {
            if (!Root.IsTree) throw new InvalidOperationException("This is not a tree");
            if (data is INode<T>) throw new ArgumentException("Object is a node");

            if (!typeof(T).IsClass || data != null)
                if (!DataType.IsInstanceOfType(data))
                    throw new ArgumentException("Object is not a " + DataType.Name);

            var e = (EventHandler<NodeTreeDataEventArgs<T>>) EventHandlerList?[ValidateEventKey];
            e?.Invoke(node, new NodeTreeDataEventArgs<T>(data));

            if (!IsRoot) ToNodeTree(Root).OnValidate(node, data);
        }

        //-----------------------------------------------------------------------------
        // Clear

        protected virtual void OnClearing(ITree<T> tree)
        {
            var e = (EventHandler) EventHandlerList?[ClearingEventKey];
            e?.Invoke(tree, EventArgs.Empty);
        }

        protected virtual void OnCleared(ITree<T> tree)
        {
            var e = (EventHandler) EventHandlerList?[ClearedEventKey];
            e?.Invoke(tree, EventArgs.Empty);
        }

        //-----------------------------------------------------------------------------
        // Set

        protected virtual void OnSetting(INode<T> node, T data)
        {
            OnSettingCore(node, data, true);
        }

        protected virtual void OnSettingCore(INode<T> node, T data, bool raiseValidate)
        {
            if (EventHandlerList != null)
            {
                var e = (EventHandler<NodeTreeDataEventArgs<T>>) EventHandlerList[SettingEventKey];
                e?.Invoke(node, new NodeTreeDataEventArgs<T>(data));
            }

            if (!IsRoot) ToNodeTree(Root).OnSettingCore(node, data, false);

            if (raiseValidate) OnValidate(node, data);
        }

        protected virtual void OnSetDone(INode<T> node, T data)
        {
            OnSetDoneCore(node, data, true);
        }

        protected virtual void OnSetDoneCore(INode<T> node, T data, bool raiseValidate)
        {
            if (EventHandlerList != null)
            {
                var e = (EventHandler<NodeTreeDataEventArgs<T>>) EventHandlerList[SetDoneEventKey];
                e?.Invoke(node, new NodeTreeDataEventArgs<T>(data));
            }

            if (!IsRoot) ToNodeTree(Root).OnSetDoneCore(node, data, false);

            //			if ( raiseValidate ) OnValidate( Node, Data );
        }

        //-----------------------------------------------------------------------------
        // Insert

        protected virtual void OnInserting(INode<T> oldNode, NodeTreeInsertOperation operation, INode<T> newNode)
        {
            OnInsertingCore(oldNode, operation, newNode, true);
        }

        protected virtual void OnInsertingCore(INode<T> oldNode, NodeTreeInsertOperation operation, INode<T> newNode,
            bool raiseValidate)
        {
            if (EventHandlerList != null)
            {
                var e = (EventHandler<NodeTreeInsertEventArgs<T>>) EventHandlerList[InsertingEventKey];
                e?.Invoke(oldNode, new NodeTreeInsertEventArgs<T>(operation, newNode));
            }

            if (!IsRoot) ToNodeTree(Root).OnInsertingCore(oldNode, operation, newNode, false);

            if (raiseValidate) OnValidate(oldNode, newNode.Data);

            if (raiseValidate) OnInsertingTree(newNode);
        }

        protected virtual void OnInsertingTree(INode<T> newNode)
        {
            for (var child = newNode.Child; child != null; child = child.Next)
            {
                OnInsertingTree(newNode, child);

                OnInsertingTree(child);
            }
        }

        protected virtual void OnInsertingTree(INode<T> newNode, INode<T> child)
        {
            OnInsertingTreeCore(newNode, child, true);
        }

        protected virtual void OnInsertingTreeCore(INode<T> newNode, INode<T> child, bool raiseValidate)
        {
            var e = (EventHandler<NodeTreeInsertEventArgs<T>>) EventHandlerList?[InsertingEventKey];
            e?.Invoke(newNode, new NodeTreeInsertEventArgs<T>(NodeTreeInsertOperation.Tree, child));

            if (!IsRoot) ToNodeTree(Root).OnInsertingTreeCore(newNode, child, false);

            if (raiseValidate) OnValidate(newNode, child.Data);
        }

        protected virtual void OnInserted(INode<T> oldNode, NodeTreeInsertOperation operation, INode<T> newNode)
        {
            OnInsertedCore(oldNode, operation, newNode, true);
        }

        protected virtual void OnInsertedCore(INode<T> oldNode, NodeTreeInsertOperation operation, INode<T> newNode,
            bool raiseValidate)
        {
            var e = (EventHandler<NodeTreeInsertEventArgs<T>>) EventHandlerList?[InsertedEventKey];
            e?.Invoke(oldNode, new NodeTreeInsertEventArgs<T>(operation, newNode));

            if (!IsRoot) ToNodeTree(Root).OnInsertedCore(oldNode, operation, newNode, false);

            if (raiseValidate) OnInsertedTree(newNode);
        }

        protected virtual void OnInsertedTree(INode<T> newNode)
        {
            for (var child = newNode.Child; child != null; child = child.Next)
            {
                OnInsertedTree(newNode, child);

                OnInsertedTree(child);
            }
        }

        protected virtual void OnInsertedTree(INode<T> newNode, INode<T> child)
        {
            OnInsertedTreeCore(newNode, child, true);
        }

        protected virtual void OnInsertedTreeCore(INode<T> newNode, INode<T> child, bool raiseValidate)
        {
            var e = (EventHandler<NodeTreeInsertEventArgs<T>>) EventHandlerList?[InsertedEventKey];
            e?.Invoke(newNode, new NodeTreeInsertEventArgs<T>(NodeTreeInsertOperation.Tree, child));

            if (!IsRoot) ToNodeTree(Root).OnInsertedTreeCore(newNode, child, false);
        }

        //-----------------------------------------------------------------------------
        // Cut

        protected virtual void OnCutting(INode<T> oldNode)
        {
            var e = (EventHandler) EventHandlerList?[CuttingEventKey];
            e?.Invoke(oldNode, EventArgs.Empty);

            if (!IsRoot) ToNodeTree(Root).OnCutting(oldNode);
        }

        protected virtual void OnCutDone(INode<T> oldRoot, INode<T> oldNode)
        {
            var e = (EventHandler) EventHandlerList?[CutDoneEventKey];
            e?.Invoke(oldNode, EventArgs.Empty);

            if (!IsTree) ToNodeTree(oldRoot).OnCutDone(oldRoot, oldNode);
        }

        //-----------------------------------------------------------------------------
        // Copy

        protected virtual void OnCopying(INode<T> oldNode, INode<T> newNode)
        {
            OnCopyingCore(oldNode, newNode, true);
        }

        protected virtual void OnCopyingCore(INode<T> oldNode, INode<T> newNode, bool raiseValidate)
        {
            var e = (EventHandler<NodeTreeNodeEventArgs<T>>) EventHandlerList?[CopyingEventKey];
            e?.Invoke(oldNode, new NodeTreeNodeEventArgs<T>(newNode));

            if (!IsRoot) ToNodeTree(Root).OnCopyingCore(oldNode, newNode, false);

            //			if ( raiseValidate ) OnValidate( oldNode, newNode.Data );
        }

        protected virtual void OnCopied(INode<T> oldNode, INode<T> newNode)
        {
            OnCopiedCore(oldNode, newNode, true);
        }

        protected virtual void OnCopiedCore(INode<T> oldNode, INode<T> newNode, bool raiseValidate)
        {
            var e = (EventHandler<NodeTreeNodeEventArgs<T>>) EventHandlerList?[CopiedEventKey];
            e?.Invoke(oldNode, new NodeTreeNodeEventArgs<T>(newNode));

            if (!IsRoot) ToNodeTree(Root).OnCopiedCore(oldNode, newNode, false);
        }

        //-----------------------------------------------------------------------------
        // DeepCopy

        protected virtual void OnDeepCopying(INode<T> oldNode, INode<T> newNode)
        {
            OnDeepCopyingCore(oldNode, newNode, true);
        }

        protected virtual void OnDeepCopyingCore(INode<T> oldNode, INode<T> newNode, bool raiseValidate)
        {
            var e = (EventHandler<NodeTreeNodeEventArgs<T>>) EventHandlerList?[DeepCopyingEventKey];
            e?.Invoke(oldNode, new NodeTreeNodeEventArgs<T>(newNode));

            if (!IsRoot) ToNodeTree(Root).OnDeepCopyingCore(oldNode, newNode, false);
        }

        protected virtual void OnDeepCopied(INode<T> oldNode, INode<T> newNode)
        {
            OnDeepCopiedCore(oldNode, newNode, true);
        }

        protected virtual void OnDeepCopiedCore(INode<T> oldNode, INode<T> newNode, bool raiseValidate)
        {
            var e = (EventHandler<NodeTreeNodeEventArgs<T>>) EventHandlerList?[DeepCopiedEventKey];
            e?.Invoke(oldNode, new NodeTreeNodeEventArgs<T>(newNode));

            if (!IsRoot) ToNodeTree(Root).OnDeepCopiedCore(oldNode, newNode, false);
        }
    }
}
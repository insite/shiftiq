using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Shift.Constant;

namespace Shift.Common.Trees
{
    [Serializable]
    public partial class NodeTree<T> : INode<T>, ITree<T>
    {
        #region Constants

        private const string CannotMoveToChild = "Cannot move to Child";
        private const string CannotMoveToFirst = "Cannot move to First";
        private const string CannotMoveToLast = "Cannot move to Last";
        private const string CannotMoveToNext = "Cannot move to Next";
        private const string CannotMoveToParent = "Cannot move to Parent";
        private const string CannotMoveToPrevious = "Cannot move to Previous";

        private const string ThisIsRoot = "This is a root.";
        private const string ThisIsTree = "This is a tree.";
        private const string ThisIsNotRoot = "This is not a root.";
        private const string ThisIsNotTree = "This is not a tree.";

        #endregion

        #region Fields

        private T _data;

        private NodeTree<T> _parent;
        private NodeTree<T> _previous;
        private NodeTree<T> _next;
        private NodeTree<T> _child;

        private TreeRoot<T> TreeRoot => (TreeRoot<T>)Root;

        #endregion

        #region Properties

        public T Data
        {
            get => _data;

            set
            {
                if (IsRoot) throw new InvalidOperationException( ThisIsRoot );

                OnSetting( this, value );

                _data = value;

                OnSetDone( this, value );
            }
        }

        public virtual IEqualityComparer<T> DataComparer
        {
            get
            {
                if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

                return TreeRoot.DataComparer;
            }

            set
            {
                if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

                TreeRoot.DataComparer = value;
            }
        }

        public virtual Type DataType => typeof( T );
        
        public INode<T> Parent => _parent;

        public INode<T> Previous => _previous;

        public INode<T> Next => _next;

        public INode<T> Child => _child;

        public ITree<T> Tree => (ITree<T>)Root;

        public INode<T> Root
        {
            get
            {
                INode<T> node = this;

                while (node.Parent != null)
                    node = node.Parent;

                return node;
            }
        }

        public INode<T> Top
        {
            get
            {
                if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );
                //if ( this.IsRoot ) throw new InvalidOperationException( ThisIsRoot );
                if (IsRoot) return null;

                INode<T> node = this;

                while (node.Parent.Parent != null)
                    node = node.Parent;

                return node;
            }
        }

        public INode<T> First
        {
            get
            {
                INode<T> node = this;

                while (node.Previous != null)
                    node = node.Previous;

                return node;
            }
        }

        public INode<T> Last
        {
            get
            {
                INode<T> node = this;

                while (node.Next != null)
                    node = node.Next;

                return node;
            }
        }

        public INode<T> LastChild => Child?.Last;

        public bool HasPrevious => Previous != null;

        public bool HasNext => Next != null;

        public bool HasChild => Child != null;

        public bool IsFirst => Previous == null;

        public bool IsLast => Next == null;

        public bool IsTree
        {
            get
            {
                if (!IsRoot) return false;
                return this is TreeRoot<T>;
            }
        }

        public bool IsRoot
        {
            get
            {
                var b = (Parent == null);

                if (b)
                {
                    Debug.Assert( Previous == null );
                    Debug.Assert( Next == null );
                }

                return b;
            }
        }

        public bool HasParent
        {
            get
            {
                //if ( IsRoot ) throw new InvalidOperationException( ThisIsRoot );
                if (IsRoot) return false;
                return Parent.Parent != null;
            }
        }

        public bool IsTop
        {
            get
            {
                //if ( IsRoot ) throw new InvalidOperationException( ThisIsRoot );
                if (IsRoot) return false;
                return Parent.Parent == null;
            }
        }

        public virtual int Version
        {
            get
            {
                var root = Root;

                if (!root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

                return ToNodeTree( root ).Version;
            }

            set
            {
                var root = Root;

                if (!root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

                ToNodeTree( root ).Version = value;
            }
        }

        #endregion

        #region Properties (counting)

        public virtual int BranchCount
        {
            get
            {
                var i = 0;

                for (var node = First; node != null; node = node.Next) i++;

                return i;
            }
        }

        public virtual int BranchIndex
        {
            get
            {
                var i = -1;

                for (INode<T> node = this; node != null; node = node.Previous) i++;

                return i;
            }
        }

        public virtual int Count
        {
            get
            {
                var i = IsRoot ? 0 : 1;

                for (var n = Child; n != null; n = n.Next)
                    i += n.Count;

                return i;
            }
        }

        public virtual int Depth
        {
            get
            {
                var i = -1;

                for (INode<T> node = this; !node.IsRoot; node = node.Parent) i++;

                return i;
            }
        }

        public int DirectChildCount
        {
            get
            {
                var i = 0;

                for (var n = Child; n != null; n = n.Next) i++;

                return i;
            }
        }

        #endregion

        #region Properties (enumeration)

        public virtual IEnumerableCollection<INode<T>> Nodes => All.Nodes;
        public virtual IEnumerableCollection<T> Values => All.Values;

        public IEnumerableCollectionPair<T> All => new AllEnumerator<T>( this );
        public IEnumerableCollectionPair<T> AllChildren => new AllChildrenEnumerator<T>( this );
        public IEnumerableCollectionPair<T> DirectChildren => new DirectChildrenEnumerator<T>( this );
        public IEnumerableCollectionPair<T> DirectChildrenInReverse => new DirectChildrenInReverseEnumerator<T>( this );

        #endregion

        #region Methods (construction and destruction)

        protected NodeTree( ) { }

        ~NodeTree( )
        {
            Dispose( false );
        }
        
        public void Dispose( )
        {
            Dispose( true );

            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if (disposing)
            {
                EventHandlerList?.Dispose( );
            }
        }

        #endregion

        #region Methods (instantiation)

        public static ITree<T> NewTree( )
        {
            return new TreeRoot<T>( );
        }

        public static ITree<T> NewTree( IEqualityComparer<T> comparer )
        {
            return new TreeRoot<T>( comparer );
        }

        protected static INode<T> NewNode( )
        {
            return new NodeTree<T>( );
        }

        protected virtual NodeTree<T> CreateTree( )
        {
            return new TreeRoot<T>( );
        }

        protected virtual NodeTree<T> CreateNode( )
        {
            return new NodeTree<T>( );
        }

        #endregion
        
        #region Methods (add/insert)

        public INode<T> InsertPrevious( T o )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var newNode = CreateNode( );
            newNode._data = o;

            InsertPreviousCore( newNode );

            return newNode;
        }

        public INode<T> InsertNext( T o )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var newNode = CreateNode( );
            newNode._data = o;

            InsertNextCore( newNode );

            return newNode;
        }

        public INode<T> InsertChild( T o )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var newNode = CreateNode( );
            newNode._data = o;

            InsertChildCore( newNode );

            return newNode;
        }

        public INode<T> Add( T o )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            return Last.InsertNext( o );
        }

        public INode<T> AddChild( T o )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            if (Child == null)
                return InsertChild( o );
            else
                return Child.Add( o );
        }

        public void InsertPrevious( ITree<T> tree )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var newTree = ToNodeTree( tree );

            if (!newTree.IsRoot) throw new ArgumentException( ThisIsNotRoot );
            if (!newTree.IsTree) throw new ArgumentException( ThisIsNotTree );

            for (var n = newTree.Child; n != null; n = n.Next)
            {
                var node = ToNodeTree( n );
                var copy = node.CopyCore( );
                InsertPreviousCore( copy );
            }
        }

        public void InsertNext( ITree<T> tree )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var newTree = ToNodeTree( tree );

            if (!newTree.IsRoot) throw new ArgumentException( ThisIsNotRoot );
            if (!newTree.IsTree) throw new ArgumentException( ThisIsNotTree );

            for (var n = newTree.LastChild; n != null; n = n.Previous)
            {
                var node = ToNodeTree( n );
                var copy = node.CopyCore( );
                InsertNextCore( copy );
            }
        }

        public void InsertChild( ITree<T> tree )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var newTree = ToNodeTree( tree );

            if (!newTree.IsRoot) throw new ArgumentException( ThisIsNotRoot );
            if (!newTree.IsTree) throw new ArgumentException( ThisIsNotTree );

            for (var n = newTree.LastChild; n != null; n = n.Previous)
            {
                var node = ToNodeTree( n );
                var copy = node.CopyCore( );
                InsertChildCore( copy );
            }
        }

        public void Add( ITree<T> tree )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            Last.InsertNext( tree );
        }

        public void AddChild( ITree<T> tree )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            if (Child == null)
                InsertChild( tree );
            else
                Child.Add( tree );
        }

        protected virtual void InsertPreviousCore( INode<T> newINode )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!newINode.IsRoot) throw new ArgumentException( ThisIsNotRoot );
            if (newINode.IsTree) throw new ArgumentException( ThisIsTree );

            IncrementVersion( );

            OnInserting( this, NodeTreeInsertOperation.Previous, newINode );

            var newNode = ToNodeTree( newINode );

            newNode._parent = _parent;
            newNode._previous = _previous;
            newNode._next = this;
            _previous = newNode;

            if (newNode.Previous != null)
            {
                var previous = ToNodeTree( newNode.Previous );
                previous._next = newNode;
            }
            else // this is a first node
            {
                var parent = ToNodeTree( newNode.Parent );
                parent._child = newNode;
            }

            OnInserted( this, NodeTreeInsertOperation.Previous, newINode );
        }

        protected virtual void InsertNextCore( INode<T> newINode )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!newINode.IsRoot) throw new ArgumentException( ThisIsNotRoot );
            if (newINode.IsTree) throw new ArgumentException( ThisIsTree );

            IncrementVersion( );

            OnInserting( this, NodeTreeInsertOperation.Next, newINode );

            var newNode = ToNodeTree( newINode );

            newNode._parent = _parent;
            newNode._previous = this;
            newNode._next = _next;
            _next = newNode;

            if (newNode.Next != null)
            {
                var next = ToNodeTree( newNode.Next );
                next._previous = newNode;
            }

            OnInserted( this, NodeTreeInsertOperation.Next, newINode );
        }

        protected virtual void InsertChildCore( INode<T> newINode )
        {
            if (!newINode.IsRoot) throw new ArgumentException( ThisIsNotRoot );
            if (newINode.IsTree) throw new ArgumentException( ThisIsTree );

            IncrementVersion( );

            OnInserting( this, NodeTreeInsertOperation.Child, newINode );

            var newNode = ToNodeTree( newINode );

            newNode._parent = this;
            newNode._next = _child;
            _child = newNode;

            if (newNode.Next != null)
            {
                var next = ToNodeTree( newNode.Next );
                next._previous = newNode;
            }

            OnInserted( this, NodeTreeInsertOperation.Child, newINode );
        }

        protected virtual void AddCore( INode<T> newINode )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );

            var lastNode = ToNodeTree( Last );

            lastNode.InsertNextCore( newINode );
        }

        protected virtual void AddChildCore( INode<T> newINode )
        {
            if (Child == null)
                InsertChildCore( newINode );
            else
            {
                var childNode = ToNodeTree( Child );

                childNode.AddCore( newINode );
            }
        }

        #endregion

        #region Methods (move)

        public bool CanMoveToParent
        {
            get
            {
                if (IsRoot) return false;
                if (IsTop) return false;

                return true;
            }
        }

        public bool CanMoveToPrevious
        {
            get
            {
                if (IsRoot) return false;
                if (IsFirst) return false;

                return true;
            }
        }

        public bool CanMoveToNext
        {
            get
            {
                if (IsRoot) return false;
                if (IsLast) return false;

                return true;
            }
        }

        public bool CanMoveToChild
        {
            get
            {
                if (IsRoot) return false;
                if (IsFirst) return false;

                return true;
            }
        }

        public bool CanMoveToFirst
        {
            get
            {
                if (IsRoot) return false;
                if (IsFirst) return false;

                return true;
            }
        }

        public bool CanMoveToLast
        {
            get
            {
                if (IsRoot) return false;
                if (IsLast) return false;

                return true;
            }
        }

        public void MoveToParent( )
        {
            if (!CanMoveToParent) throw new InvalidOperationException( CannotMoveToParent );

            var parentNode = ToNodeTree( Parent );

            var thisNode = CutCore( );

            parentNode.InsertNextCore( thisNode );
        }

        public void MoveToPrevious( )
        {
            if (!CanMoveToPrevious) throw new InvalidOperationException( CannotMoveToPrevious );

            var previousNode = ToNodeTree( Previous );

            var thisNode = CutCore( );

            previousNode.InsertPreviousCore( thisNode );
        }

        public void MoveToNext( )
        {
            if (!CanMoveToNext) throw new InvalidOperationException( CannotMoveToNext );

            var nextNode = ToNodeTree( Next );

            var thisNode = CutCore( );

            nextNode.InsertNextCore( thisNode );
        }

        public void MoveToChild( )
        {
            if (!CanMoveToChild) throw new InvalidOperationException( CannotMoveToChild );

            var previousNode = ToNodeTree( Previous );

            var thisNode = CutCore( );

            previousNode.AddChildCore( thisNode );
        }

        public void MoveToFirst( )
        {
            if (!CanMoveToFirst) throw new InvalidOperationException( CannotMoveToFirst );

            var firstNode = ToNodeTree( First );

            var thisNode = CutCore( );

            firstNode.InsertPreviousCore( thisNode );
        }

        public void MoveToLast( )
        {
            if (!CanMoveToLast) throw new InvalidOperationException( CannotMoveToLast );

            var lastNode = ToNodeTree( Last );

            var thisNode = CutCore( );

            lastNode.InsertNextCore( thisNode );
        }

        #endregion

        #region Methods (search)

        public virtual INode<T> this[T item]
        {
            get
            {
                if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

                var comparer = DataComparer;

                foreach (var n in All.Nodes)
                    if (comparer.Equals( n.Data, item ))
                        return n;

                return null;
            }
        }

        public virtual bool Contains( INode<T> item )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            return All.Nodes.Contains( item );
        }

        public virtual bool Contains( T item )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            return All.Values.Contains( item );
        }

        public virtual INode<T> Find( T item )
        {
            return All.Find( item );
        }

        public virtual INode<T> Find( Predicate<T> predicate )
        {
            return All.Find( predicate );
        }

        #endregion

        #region Methods (sort)

        public virtual void SortAllChildren( )
        {
            foreach (var node in All.Nodes) node.SortDirectChildren( );
        }

        public virtual void SortAllChildren( Comparison<T> comparison )
        {
            foreach (var node in All.Nodes) node.SortDirectChildren( comparison );
        }

        public virtual void SortAllChildren( IComparer<T> comparer )
        {
            foreach (var node in All.Nodes) node.SortDirectChildren( comparer );
        }

        public virtual void SortDirectChildren( )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            if (DirectChildCount < 2) return;

            var children = new List<INode<T>>( DirectChildren.Nodes );

            children.Sort( );

            SortDirectChildrenCore( children );
        }

        public virtual void SortDirectChildren( Comparison<T> comparison )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            if (DirectChildCount < 2) return;

            var children = new List<INode<T>>( DirectChildren.Nodes );

            children.Sort( ( x, y ) => comparison( x.Data, y.Data ) );

            SortDirectChildrenCore( children );
        }

        public virtual void SortDirectChildren( IComparer<T> comparer )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            if (DirectChildCount < 2) return;

            var children = new List<INode<T>>( DirectChildren.Nodes );

            var nodeComparer = new NodeComparer<T>( comparer );

            children.Sort( nodeComparer );

            SortDirectChildrenCore( children );
        }

        private void SortDirectChildrenCore( List<INode<T>> children )
        {
            _child = null;

            NodeTree<T> previous = null;
            foreach (var iChild in children)
            {
                var child = ToNodeTree( iChild );

                child._parent = this;

                child._previous = null;
                child._next = null;

                if (_child == null)
                {
                    _child = child;
                }
                else
                {
                    if (previous != null)
                    {
                        previous._next = child;
                        child._previous = previous;
                    }
                }

                previous = child;
            }
        }

        #endregion

        #region Methods (copy/cut/delete)

        public ITree<T> Cut( T o )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var n = this[o];
            return n?.Cut( );
        }

        public ITree<T> Copy( T o )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var n = this[o];
            return n?.Copy( );
        }

        public ITree<T> DeepCopy( T o )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var n = this[o];
            return n?.DeepCopy( );
        }

        protected virtual T DeepCopyData( T data )
        {
            if (data == null) { Debug.Assert( true ); return default( T ); }

            // IDeepCopy
            if (data is IDeepCopy deepCopy) return (T)deepCopy.CreateDeepCopy( );

            // ICloneable
            if (data is ICloneable cloneable) return (T)cloneable.Clone( );

            // Copy constructor
            var ctor = data.GetType( ).GetConstructor(
                 BindingFlags.Instance | BindingFlags.Public,
                 null, new[] { typeof( T ) }, null );
            if (ctor != null) return (T)ctor.Invoke( new object[] { data } );
            //return ( T ) Activator.CreateInstance( typeof( T ), new object[] { data } );

            // give up
            return data;
        }

        public bool Remove( T o )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var n = this[o];
            if (n == null) return false;

            n.Remove( );
            return true;
        }

        public void Clear( )
        {
            if (!IsRoot) throw new InvalidOperationException( ThisIsNotRoot );
            if (!IsTree) throw new InvalidOperationException( ThisIsNotTree );

            OnClearing( this );

            _child = null;

            OnCleared( this );
        }

        private NodeTree<T> BoxInTree( NodeTree<T> node )
        {
            if (!node.IsRoot) throw new ArgumentException( ThisIsNotRoot );
            if (node.IsTree) throw new ArgumentException( ThisIsTree );

            var tree = CreateTree( );

            tree.AddChildCore( node );

            return tree;
        }

        public ITree<T> Cut( )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            var node = CutCore( );

            return BoxInTree( node );
        }

        public ITree<T> Copy( )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            if (IsTree)
            {
                var newTree = CopyCore( );

                return newTree;
            }
            else
            {
                var newNode = CopyCore( );

                return BoxInTree( newNode );
            }
        }

        public ITree<T> DeepCopy( )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            if (IsTree)
            {
                var newTree = DeepCopyCore( );

                return newTree;
            }
            else
            {
                var newNode = DeepCopyCore( );

                return BoxInTree( newNode );
            }
        }

        public void Remove( )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            RemoveCore( );
        }

        protected virtual NodeTree<T> CutCore( )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );

            IncrementVersion( );

            OnCutting( this );

            var oldRoot = Root;

            if (_next != null)
                _next._previous = _previous;

            if (Previous != null)
                _previous._next = _next;
            else // this is a first node
            {
                Debug.Assert( Parent.Child == this );
                _parent._child = _next;
                Debug.Assert( Next?.Previous == null );
            }

            _parent = null;
            _previous = null;
            _next = null;

            OnCutDone( oldRoot, this );

            return this;
        }

        protected virtual NodeTree<T> CopyCore( )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );
            if (IsRoot && !IsTree) throw new InvalidOperationException( ThisIsRoot );

            if (IsTree)
            {
                var newTree = CreateTree( );

                OnCopying( this, newTree );

                CopyChildNodes( this, newTree, false );

                OnCopied( this, newTree );

                return newTree;
            }
            else
            {
                var newNode = CreateNode( );

                newNode._data = Data;

                OnCopying( this, newNode );

                CopyChildNodes( this, newNode, false );

                OnCopied( this, newNode );

                return newNode;
            }
        }

        protected virtual NodeTree<T> DeepCopyCore( )
        {
            if (!Root.IsTree) throw new InvalidOperationException( ThisIsNotTree );
            if (IsRoot && !IsTree) throw new InvalidOperationException( ThisIsRoot );

            if (IsTree)
            {
                var newTree = CreateTree( );

                OnCopying( this, newTree );

                CopyChildNodes( this, newTree, true );

                OnCopied( this, newTree );

                return newTree;
            }
            else
            {
                var newNode = CreateNode( );

                newNode._data = DeepCopyData( Data );

                OnDeepCopying( this, newNode );

                CopyChildNodes( this, newNode, true );

                OnDeepCopied( this, newNode );

                return newNode;
            }
        }

        private void CopyChildNodes( INode<T> oldNode, NodeTree<T> newNode, bool bDeepCopy )
        {
            NodeTree<T> previousNewChildNode = null;

            for (var oldChildNode = oldNode.Child; oldChildNode != null; oldChildNode = oldChildNode.Next)
            {
                var newChildNode = CreateNode( );

                newChildNode._data = !bDeepCopy ? oldChildNode.Data : DeepCopyData( oldChildNode.Data );

                //				if ( ! bDeepCopy )
                //					OnCopying( oldChildNode, newChildNode );
                //				else
                //					OnDeepCopying( oldChildNode, newChildNode );

                if (oldChildNode.Previous == null) newNode._child = newChildNode;

                newChildNode._parent = newNode;
                newChildNode._previous = previousNewChildNode;
                if (previousNewChildNode != null) previousNewChildNode._next = newChildNode;

                //				if ( ! bDeepCopy )
                //					OnCopied( oldChildNode, newChildNode );
                //				else
                //					OnDeepCopied( oldChildNode, newChildNode );

                CopyChildNodes( oldChildNode, newChildNode, bDeepCopy );

                previousNewChildNode = newChildNode;
            }
        }

        protected virtual void RemoveCore( )
        {
            if (IsRoot) throw new InvalidOperationException( ThisIsRoot );

            CutCore( );
        }

        #endregion

        #region Methods (versioning)

        public bool HasChanged( int version ) { return Version != version; }

        protected void IncrementVersion( )
        {
            var root = Root;

            if (!root.IsTree) throw new InvalidOperationException( ThisIsNotTree );

            ToNodeTree( root ).Version++;
        }

        #endregion

        #region Methods (type conversion)

        protected static NodeTree<T> ToNodeTree( INode<T> node )
        {
            if (node == null) throw new ArgumentNullException( nameof( node ) );

            return (NodeTree<T>)node;
        }

        protected static NodeTree<T> ToNodeTree( ITree<T> tree )
        {
            if (tree == null) throw new ArgumentNullException( nameof( tree ) );

            return (NodeTree<T>)tree;
        }

        public override string ToString( )
        {
            return Data == null ? string.Empty : Data.ToString( );
        }

        public virtual string ToStringRecursive( )
        {
            var s = string.Empty;

            foreach (var item in All.Nodes)
            {
                var node = (NodeTree<T>)item;
                s += new string( ' ', node.Depth * 4 ) + node + System.Environment.NewLine;
            }

            return s;
        }

        

        #endregion
    }
}
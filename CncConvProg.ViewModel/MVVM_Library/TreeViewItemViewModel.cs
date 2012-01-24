using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CncConvProg.ViewModel.MVVM_Library
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : ViewModelBase
    {
        private string _label;
        public virtual string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged("Label");
            }
        }


        internal TreeViewItemViewModel GetRoot()
        {
            return Parent == null ? this : Parent.GetRoot();
        }

        public event EventHandler OnUpdated;

        protected void RequestUpdate(ViewModelBase caller)
        {
            var handler = OnUpdated;
            if (handler != null)
                handler(caller, EventArgs.Empty);
        }

        public event EventHandler OnItemSelected;

        private void RequestOnSelected(TreeViewItemViewModel caller)
        {
            var handler = OnItemSelected;
            if (handler != null)
                handler(caller, EventArgs.Empty);
        }

        #region Data

        static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

        readonly ObservableCollection<TreeViewItemViewModel> _children;
        readonly TreeViewItemViewModel _parent;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        protected TreeViewItemViewModel(string label, TreeViewItemViewModel parent)
        {
            Label = label;

            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();
        }

        public TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren)
                _children.Add(DummyChild);
        }


        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
        }

        #endregion // Constructors

        #region Presentation Members

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        #endregion // Children

        #region HasLoadedChildren

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        #endregion // HasLoadedChildren

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                //if (value != _isSelected)
                //{
                _isSelected = value;
                if (_isSelected)
                {
                    // se selezionato , apre il parent se esiste e lancia onselected
                    if (Parent != null)
                        Parent.IsExpanded = true;

                    OnSelected(this);
                }
                this.OnPropertyChanged("IsSelected");
                //}
            }
        }

        /// <summary>
        /// Dice al parent , che è stato selezionato
        /// </summary>
        public void OnSelected(TreeViewItemViewModel caller)
        {
            if (Parent != null)
                Parent.OnSelected(caller);
            else
                RequestOnSelected(caller);
        }

        #endregion // IsSelected

        #region LoadChildren

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
        }

        #endregion // LoadChildren

        #region Parent

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members

        internal void ExpandChild()
        {
            IsExpanded = true;
            foreach (var treeViewItemViewModel in Children)
                treeViewItemViewModel.ExpandChild();
        }

        private TreeViewItemViewModel GetNext(TreeViewItemViewModel child)
        {
            if (Children.Contains(child))
            {
                var index = Children.IndexOf(child);

                index++;

                return Children.ElementAtOrDefault(index);
            }

            return null;
        }

        private TreeViewItemViewModel GetPrev(TreeViewItemViewModel child)
        {
            if (Children.Contains(child))
            {
                var index = Children.IndexOf(child);

                index--;

                return Children.ElementAtOrDefault(index);
            }

            return null;
        }



        /// <summary>
        /// Metodo che ritorna il prossimo elemento nell'albero del TreeView
        /// </summary>
        /// <param name="selectedScreen"></param>
        /// <returns></returns>
        internal TreeViewItemViewModel GetNextOrDefault(TreeViewItemViewModel selectedScreen)
        {
            if (selectedScreen == this)
                if (Parent == null) // ovvero se è oggetto radice
                {
                    return Children.FirstOrDefault(); // ritorna il primo dei children
                }
                else
                {
                    // chiedo al parent di darmi il successivo di quest'elemento
                    var nextScreen = Parent.GetNext(this);
                    if (nextScreen != null)
                    {
                        return nextScreen;
                    }
                }

            // prima devo vedere se selectedScreen è questo
            // il problema è che controllo solamente i children.

            foreach (var variable in Children)
            {
                var nextOrDefault = variable.GetNextOrDefault(selectedScreen);
                if (nextOrDefault != null)
                    return nextOrDefault;
            }



            // se arrivo qui, significa che non è stato trovato niente dall'oggetto radice.
            // fino all'ultimo del ramo,
            // sara poi dovere dell'oggetto chiamante cercare nella prossima radice.
            return null;
        }

        internal TreeViewItemViewModel GetPrevOrDefault(TreeViewItemViewModel selectedScreen)
        {
            if (selectedScreen == this)
                if (Parent == null) // ovvero se è oggetto radice
                {
                    return null;
                }
                else
                {
                    // chiedo al parent di darmi il successivo di quest'elemento
                    var prevScreen = Parent.GetPrev(this);

                    // se non c'è nessuno prevScreen ritorno il parent
                    return prevScreen ?? Parent;
                }

            // controllo fra i children ( parte ricorsiva) 

            foreach (var variable in Children)
            {
                //if (variable == selectedScreen)
                //{
                //    var indexOf = variable.Children.IndexOf(selectedScreen);

                //    indexOf--;

                //    if (indexOf == Children.Count)
                //        break;

                //    return variable.Children.ElementAtOrDefault(indexOf);
                //}

                var prev = variable.GetPrevOrDefault(selectedScreen);
                if (prev != null)
                    return prev;

            }

            // se arrivo qui, significa che non è stato trovato niente dall'oggetto radice.
            // fino all'ultimo del ramo,
            // sara poi dovere dell'oggetto chiamante cercare nella  radice precedente.
            return null;

        }


    }
}

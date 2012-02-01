using System;
using System.Collections.Generic;
using CncConvProg.ViewModel.MVVM_Library;
using System.Linq;

namespace CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel
{
    public abstract class EditStageTreeViewItem : TreeViewItemViewModel, IValidable
    {
        public event EventHandler OnSourceUpdated;

        protected void RequestUpdateSource()
        {
            var handler = OnSourceUpdated;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private EditStageTreeViewItem TreeViewParent
        {
            get { return Parent as EditStageTreeViewItem; }
        }

        /*
         * allora di default setto il profilo..
         * 
         * se qualche editstage fà override crea il suo profilo personalizzato.
         */


        /// <summary>
        /// Proprietà che serve per mostrare immagine di aiuto
        /// </summary>
        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }

        }


        //protected virtual List<IEntity2D> UpdatePreview()
        //{
        //    if (TreeViewParent != null)
        //       return TreeViewParent.GetPreview();

        //    else if (EditWorkParent != null)
        //        UpdatePreview();

        //    return;
        //}

        /// <summary>
        /// Se chiedo di aggiornare la preview, se sono elemento dell'albero chiamo elemento padre, 
        /// fino ad arrivare elemento radice per poi aggiornare parent.
        /// </summary>
        protected virtual void UpdatePreview()
        {
            if (TreeViewParent != null)
                TreeViewParent.UpdatePreview();

            else if (EditWorkParent != null)
                EditWorkParent.UpdatePreview();

            return;
        }

        protected EditWorkViewModel EditWorkParent;

        public EditStageTreeViewItem(string label, EditWorkViewModel viewModelEditWorkParent)
            : base(null, false)
        {
            EditWorkParent = viewModelEditWorkParent;

            Label = label;

            OnItemSelected += EditStageTreeViewItem_OnItemSelected;
        }

        public EditStageTreeViewItem(string label, EditStageTreeViewItem parent)
            : base(parent, false)
        {
            Label = label;

            OnItemSelected += EditStageTreeViewItem_OnItemSelected;

        }

        protected override void OnPropertyChanged(string propertyName)
        {
            _stageModified = true;

            if (propertyName != "IsValid")
            {
                    OnPropertyChanged("IsValid");
            }

            SourceUpdated();

            base.OnPropertyChanged(propertyName);
        }

        public void SourceUpdated()
        {
            if (TreeViewParent != null)
                TreeViewParent.SourceUpdated();

            else
                RequestUpdateSource();
        }

        void EditStageTreeViewItem_OnItemSelected(object sender, EventArgs e)
        {
            EditWorkParent.SelectedScreen = sender as EditStageTreeViewItem;
        }

        public EditStageTreeViewItem(string label, EditStageTreeViewItem parent, bool stageValid)
            : base(parent, false)
        {
            Label = label;
            _stageValid = stageValid;
        }

        //protected void RequestUpdatePreview()
        //{
        //    if (TreeViewParent != null)
        //        TreeViewParent.RequestUpdatePreview();
        //    else
        //        _parent.UpdatePreview();
        //}

        /// <summary>
        /// Flag che indica se il stage è stato modificato da ultimo ricalcolo
        /// </summary>
        private bool _stageModified = true;

        /// <summary>
        /// Flag che indica se lo stage è valido
        /// </summary>
        private bool? _stageValid;

        /// <summary>
        /// Se lo stage risulta modificato , ricalcolo il valore di valid e restituisco.
        /// </summary>
        public bool? IsValid
        {
            get
            {
                if (_stageModified || _stageValid == null)
                {
                    _stageValid = ValidateStage();
                }

                return _stageValid;
            }
        }

        public abstract bool? ValidateStage();
        /// <summary>
        /// Se stage è IValid,
        ///     se non è modificato ritorna valore memorizzato.
        ///     se stage è modificato ricalcola il valore
        /// </summary>
        /// <returns></returns>
        /// 

        internal bool IsTreeViewValid()
        {
            var rslt = IsValid;

            if (rslt == false)
                return false;

            foreach (var treeViewItemViewModel in Children.OfType<EditStageTreeViewItem>())
            {
                var s = treeViewItemViewModel.IsTreeViewValid();
                if (!s)
                    return false;
            }

            return true;
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Contornatura
{
    public class ContornaturaTrimmingViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly FresaturaContornatura _contornaturaParametri;

        public ContornaturaTrimmingViewModel(FresaturaContornatura contornaturaParametri, EditStageTreeViewItem treeItemParent)
            : base("Trimming", treeItemParent)
        {
            _contornaturaParametri = contornaturaParametri;



        }

        public Dictionary<byte, string> StartPointLookup
        {
            get
            {
                var lookup = new Dictionary<byte, string>
                                 {
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.UpLeft, "1 - Up-Left"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.UpRight, "2 - Up-Right"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.DownLeft, "3 - Down-Left"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.DownRight, "4 - Down-Right"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.Center, "5 - Center"},

                                 };

                return lookup;

            }
        }

        public byte TrimRectangleStartPoint
        {
            get
            {
                return (byte)_contornaturaParametri.TrimRectangleStartPoint;
            }
            set
            {
                _contornaturaParametri.TrimRectangleStartPoint = (SquareShapeHelper.SquareShapeStartPoint)value;
                OnPropertyChanged("TrimRectangleStartPoint");
            }
        }

        public bool TrimPathAbilited
        {
            get
            {
                return _contornaturaParametri.TrimPathAbilited;
            }
            set
            {
                _contornaturaParametri.TrimPathAbilited = value;
                OnPropertyChanged("TrimPathAbilited");
            }
        }

        public double RectTrimWidth
        {
            get
            {
                return _contornaturaParametri.RectTrimWidth;
            }
            set
            {
                _contornaturaParametri.RectTrimWidth = value;
                OnPropertyChanged("RectTrimWidth");
            }
        }
        public double RectTrimHeight
        {
            get
            {
                return _contornaturaParametri.RectTrimHeight;
            }
            set
            {
                _contornaturaParametri.RectTrimHeight = value;
                OnPropertyChanged("RectTrimHeight");
            }
        }
        public double RectTrimCenterY
        {
            get
            {
                return _contornaturaParametri.RectTrimCenterY;
            }
            set
            {
                _contornaturaParametri.RectTrimCenterY = value;
                OnPropertyChanged("RectTrimCenterY");
            }
        }
        public double RectTrimCenterX
        {
            get
            {
                return _contornaturaParametri.RectTrimCenterX;
            }
            set
            {
                _contornaturaParametri.RectTrimCenterX = value;
                OnPropertyChanged("RectTrimCenterX");
            }
        }
        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get { return ValidatedProperties.All(property => GetValidationError(property) == null); }
        }


        protected string[] ValidatedProperties = {

                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion
    }
}

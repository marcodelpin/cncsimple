﻿using System;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.LatheTool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel.ToolViewModels
{
    public class UtensileTornituraViewModel : ToolTreeViewItemViewModel
    {
        #region IDataErrorInfo Members

        protected override string[] ValidatedProperties
        {
            get
            {
                return new[]
                           {
                               "DiametroEffettivo",
                               "DiametroIngombro",
                               "RaggioInserto",
                           };
            }
        }

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                //case "DiametroEffettivo":
                //    {
                //        error = InputCheck.MaggioreDiZero(DiametroEffettivo);
                //    }
                //    break;

                //case "DiametroIngombro":
                //    {
                //        if (DiametroIngombro < DiametroEffettivo)
                //            error = "Incorrect diameter value ";
                //    }
                //    break;

                //case "RaggioInserto":
                //    {
                //        error = InputCheck.MaggioreOUgualeDiZero(RaggioInserto);
                //    } break;

                //default:
                //    Debug.Fail("Unexpected property : " + propertyName);
                //    break;
            }

            return error;
        }

        #endregion

        private readonly UtensileTornitura _utensileTornitura;

        public UtensileTornituraViewModel(UtensileTornitura fresaTool, TreeViewItemViewModel parent) :
            base(fresaTool, parent)
        {
            _utensileTornitura = fresaTool;


        }

    }
}
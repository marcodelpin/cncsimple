using System;

namespace CncConvProg.Model.MachineControl
{
    /*
     * classe postProcessor immagazzina i vari codici specifici ,,
     * 
     * poi una classe prendere postProcessor, e codice uni e crera codice iso.
     *
     * questa classe sarà dentro macchina , in modo che quando prendo ciclo di fresatura uni e identico sia su tornio che centro
     * 
     * solo che su tornio ho inizializzazione differente.
     * 
     * 
     * il postProcessor va bene sia per centro che per tornio,
     * 
     * sara poi specifica tornio o centro a dire cosa è abilitato o meno..
     * 
     * 26/05/2011
     * 
     * come view utilizzare simile ad editwork
     * 
     * dove a sx c'è albero
     * 
     * tornio
     *      -- macro
     *      -- ecc-
     * centro
     *  -- macro
     *  -- dettagli
     *  
     * 
     * e a dx apparira schede per modifica.
     * 
     * 
     */

    [Serializable]
    public class NumericControl
    {
        public NumericControl()
        {
            #region InitDefaultValue

            CharAxisAbsX = 'X';
            CharAxisIncreX = 'U';

            CharAxisAbsZ = 'Z';
            CharAxisIncreZ = 'W';

            CharAxisAbsY = 'Y';
            CharAxisIncreY = 'V';

            CharAxisAbsC = 'C';
            CharAxisIncreC = 'H';

            CharModalCode = 'G';
            CharMachineCode = 'M';

            CharToolCode = 'T';
            CharFeedCode = 'F';
            CharSpeedCode = 'S';
            CharNumberProgram = 'O';
            CharMaxSpindleRev = 'S';
            CharMinSpindleRev = 'Q';

            // se richiamato senza parametri metterne di default
            MoveRapid = 0;
            MoveWork = 1;
            MoveCW = 2;
            MoveCCW = 3;
            Dwell = 5;
            WorkPlaneXY = 17;
            WorkPlaneXZ = 18;
            WorkPlaneYZ = 19;
            InchCommand = 20;
            MMCommand = 21;
            ReturnToFirstZeroPoint = 28;
            ReturnToSecondZeroPoint = 30;

            RadiusCompesationOff = 40;
            RadiusCompesationLeft = 41;
            RadiusCompesationRigth = 42;
            LengthCompesation = 43;
            LengthCompesationOFF = 8888;
            CharLengthCompensation = 'H';
            CharRadiusCompensation = 'D';

            LimitCode = 50;

            WorldOrigin = 53;
            OriginOne = 54;
            OriginTwo = 55;
            OriginThree = 56;
            OriginFour = 57;
            OriginFive = 58;
            OriginSix = 59;

            Cycle_Finishing = 70;
            Cycle_Roughing_Long = 71;
            Cycle_Z_Simple_Drilling = 81;
            Cycle_Z_Depth_Drilling = 83;

            CharLthTrdPasso = 'F';
            Cycle_Lathe_Threading = 92;

            AbsoluteCoordinate = 90;
            RelativeCoordinate = 91;
            SpeedSync = 96;
            SpeedASync = 97;
            FeedAsync = 98;
            FeedSync = 99;


            //// M Code
            StopProgram = 0;
            OptionalStopProgram = 1;
            ChangeTool = 6;
            SpindleCW = 3;
            SpindleCCW = 4;
            RotaryToolCw = 13;
            RotaryToolCcw = 14;
            SpindleStop = 5;
            CoolantOn = 8;
            CoolantOff = 9;
            SpindleOrientation = 19;
            Cycle_Dwell = 'P';
            CharCycleDrill_OffsetStart = 'R';
            CycleDrill_StepQuantity = 'Q';
            Lathe3Ax_Off = 46;
            Lathe3Ax_On = 45;
            Lathe3AxBrake_Off = 69;
            Lathe3AxBrake_On = 68;

            #endregion
        }

        #region _PROPERTY_

        public string ControlName { get; set; }

        public char CharNumberProgram { get; set; }
        public char CharMaxSpindleRev { get; set; }
        public char CharMinSpindleRev { get; set; }

        public char CharAxisAbsZ { get; set; }
        public char CharAxisIncreZ { get; set; }

        public char CharAxisIncreX { get; set; }
        public char CharAxisAbsX { get; set; }

        public char CharAxisIncreY { get; set; }
        public char CharAxisAbsY { get; set; }

        public char CharAxisIncreC { get; set; }
        public char CharAxisAbsC { get; set; }


        public char CharToolCode { get; set; }

        public char CharFeedCode { get; set; }
        public char CharSpeedCode { get; set; }

        // G Code
        public char CharModalCode { get; set; }

        public float MoveRapid { get; set; }

        public String CmdMoveRapid
        {
            get { return CharModalCode + MoveRapid.ToString(); }
        }

        public float MoveWork { get; set; }

        public String CmdMoveWork
        {
            get { return CharModalCode + MoveWork.ToString(); }
        }

        public float MoveCW { get; set; }

        public String CMD_MoveCW
        {
            get { return CharModalCode + MoveCW.ToString(); }
        }

        public float MoveCCW { get; set; }

        public String CMD_MoveCCW
        {
            get { return CharModalCode + MoveCCW.ToString(); }
        }

        public float Dwell { get; set; }

        public String CMD_Dwell
        {
            get { return CharModalCode + Dwell.ToString(); }
        }

        public float WorkPlaneXZ { get; set; }

        public String CMDG_WorkPlaneXZ
        {
            get { return CharModalCode + WorkPlaneXZ.ToString(); }
        }

        public float WorkPlaneXY { get; set; }

        public String CMDG_WorkPlaneXY
        {
            get { return CharModalCode + WorkPlaneXY.ToString(); }
        }

        public float WorkPlaneYZ { get; set; } // ?


        public float RadiusCompesationOff { get; set; } //G40
        public float RadiusCompesationLeft { get; set; } //G41
        public float RadiusCompesationRigth { get; set; } //G42
        public String CMD_RadiusCompesationOff
        {
            get { return CharModalCode + RadiusCompesationOff.ToString(); }
        }

        public char CharLengthCompensation { get; set; }
        public char CharRadiusCompensation { get; set; }

        public float LengthCompesation { get; set; } //G43
        public String CMD_LengthCompesation
        {
            get { return CharModalCode + LengthCompesation.ToString(); }
        }

        public float LengthCompesationOFF { get; set; } //G49
        public String CMD_LengthCompesationOff
        {
            get { return CharModalCode + LengthCompesationOFF.ToString(); }
        }

        public float WorldOrigin { get; set; }
        public float OriginOne { get; set; }

        public String CMD_OriginOne
        {
            get { return CharModalCode + OriginOne.ToString(); }
        }

        public float OriginTwo { get; set; }
        public float OriginThree { get; set; }
        public float OriginFour { get; set; }
        public float OriginFive { get; set; }
        public float OriginSix { get; set; }


        public float ReturnToFirstZeroPoint { get; set; } // G28
        public String CMD_ReturnToFirstZeroPoint
        {
            get { return CharModalCode + ReturnToFirstZeroPoint.ToString(); }
        }

        public float ReturnToSecondZeroPoint { get; set; } // G30
        public float AbsoluteCoordinate { get; set; } // G90
        public String CMD_AbsoluteCoordinate
        {
            get { return CharModalCode + AbsoluteCoordinate.ToString(); }
        }

        public float RelativeCoordinate { get; set; } // G91
        public String CMD_RelativeCoordinate
        {
            get { return CharModalCode + RelativeCoordinate.ToString(); }
        }


        public float InchCommand { get; set; }
        public float MMCommand { get; set; }
        public float LimitCode { get; set; }

        public string CMD_LimitCode
        {
            get { return CharModalCode + LimitCode.ToString(); }
        }

        public float Cycle_Finishing { get; set; }

        public String CMDG_CycleTurnFinishing
        {
            get { return CharModalCode + Cycle_Finishing.ToString(); }
        }

        public float Cycle_Roughing_Long { get; set; }

        public String CMDG_CycleLongTurnRough
        {
            get { return CharModalCode + Cycle_Roughing_Long.ToString(); }
        }

        public float Cycle_Z_Depth_Drilling { get; set; }
        public float Cycle_Z_Simple_Drilling { get; set; }

        public String CMD_Cycle_Simple_Drilling
        {
            get { return CharModalCode + Cycle_Z_Simple_Drilling.ToString(); }
        }

        public float CycleEnd { get; set; }

        public String CMD_CycleEnd
        {
            get { return CharModalCode + CycleEnd.ToString(); }
        }

        public char Cycle_Dwell { get; set; }
        public char CharCycleDrill_OffsetStart { get; set; }
        public char CycleDrill_StepQuantity { get; set; }
        public float Cycle_Lathe_Threading { get; set; }

        public char CharLthTrdPasso { get; set; }

        public String CMDG_CycleLatheThreading
        {
            get { return CharModalCode + Cycle_Lathe_Threading.ToString(); }
        }


        public float SpeedSync { get; set; }

        public String CMDG_SpeedSync
        {
            get { return CharModalCode + SpeedSync.ToString(); }
        }

        public float SpeedASync { get; set; }

        public String CMDG_SpeedASync
        {
            get { return CharModalCode + SpeedASync.ToString(); }
        }

        public float FeedSync { get; set; }

        public String CMDG_FeedSync
        {
            get { return CharModalCode + FeedSync.ToString(); }
        }

        public float FeedAsync { get; set; }

        public String CMDG_FeedASync
        {
            get { return CharModalCode + FeedAsync.ToString(); }
        }

        ///
        /// M Code
        ///
        public char CharMachineCode { get; set; }

        public float StopProgram { get; set; }

        public String CMDM_StopProgram
        {
            get { return CharMachineCode + StopProgram.ToString(); }
        }

        // per tutte fare questo 

        public float OptionalStopProgram { get; set; }

        public String CMD_OSP
        {
            get { return CharMachineCode + OptionalStopProgram.ToString(); }
        }

        public float RotaryToolCcw { get; set; }

        public String CmdRotaryToolCcw
        {
            get { return CharMachineCode + RotaryToolCcw.ToString(); }
        }

        public float RotaryToolCw { get; set; }

        public String CmdRotaryToolCw
        {
            get { return CharMachineCode + RotaryToolCw.ToString(); }
        }

        public float SpindleCW { get; set; }

        public String CMDM_SpindleCW
        {
            get { return CharMachineCode + SpindleCW.ToString(); }
        }

        public float SpindleCCW { get; set; }

        public String CMDM_SpindleCCW
        {
            get { return CharMachineCode + SpindleCCW.ToString(); }
        }

        public float SpindleStop { get; set; }

        public String CMDM_SpindleStop
        {
            get { return CharMachineCode + SpindleStop.ToString(); }
        }

        public float ChangeTool { get; set; }

        public String CMDM_ChangeTool
        {
            get { return CharMachineCode + ChangeTool.ToString(); }
        }

        public float CoolantOn { get; set; }

        public String CMDM_CoolantOn
        {
            get { return CharMachineCode + CoolantOn.ToString(); }
        }

        public float CoolantOff { get; set; }

        public String CMDM_CoolantOff
        {
            get { return CharMachineCode + CoolantOff.ToString(); }
        }

        public float SpindleOrientation { get; set; }

        public String CMDM_SpindleOrientation
        {
            get { return CharMachineCode + SpindleOrientation.ToString(); }
        }

        public float Lathe3AxBrake_Off { get; set; }

        public String CMDM_Lathe3AxBrake_Off
        {
            get { return CharMachineCode + Lathe3AxBrake_Off.ToString(); }
        }

        public float Lathe3AxBrake_On { get; set; }

        public String CMDM_Lathe3AxBrake_On
        {
            get { return CharMachineCode + Lathe3AxBrake_On.ToString(); }
        }

        public float Lathe3Ax_Off { get; set; }

        public String CMDM_Lathe3Ax_Off
        {
            get { return CharMachineCode + Lathe3Ax_Off.ToString(); }
        }

        public float Lathe3Ax_On { get; set; }

        public String CMDM_Lathe3Ax_On
        {
            get { return CharMachineCode + Lathe3Ax_On.ToString(); }
        }

        #endregion

        internal string GetSpeedCode(string speed)
        {
            return CharSpeedCode + speed;
        }
    }
}
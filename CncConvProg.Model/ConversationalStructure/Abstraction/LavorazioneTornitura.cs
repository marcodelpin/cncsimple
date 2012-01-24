using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.PreviewPathEntity;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.Model.ToolMachine;

namespace CncConvProg.Model.ConversationalStructure.Abstraction
{
    /// <summary>
    /// Magari quando faro lavorazioni per tornio 
    /// faro 2 classi derivate 
    /// - lavorazioni tornio
    /// - lavorazioni centro
    /// </summary>
    [Serializable]
    public abstract class LavorazioneTornitura : Lavorazione
    {
        protected LavorazioneTornitura(Guid faseLavorGuid) : base(faseLavorGuid)
        {
        }
    }
}
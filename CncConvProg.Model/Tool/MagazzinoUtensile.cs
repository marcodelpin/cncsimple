﻿using System;
using System.Linq;
using System.Collections.Generic;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.LatheTool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.Tool
{

    public class DefaultDataFiller
    {
        private readonly MagazzinoUtensile _mag;
        public DefaultDataFiller(MagazzinoUtensile magazzinoUtensile)
        {
            _mag = magazzinoUtensile;

        }
        /*
         * posizioni utensili..
         */

        const string C40 = "C40";
        const string Aisi = "AISI 304";
        const string Alu = "6082";

        private const double VelTaglioInox = 160;
        private const double VelTaglioAlluminio = 600;
        private const double VelTaglioC40 = 250;
        private const double FeedTorniSgr = .3;
        private const double FeedTorniFin = .14;

        /// <summary>f
        /// Setta valori di default su nuovo magazzino creato
        /// </summary>
        public void AddDefaultData()
        {
            AddMaterialMetric(C40, 7.8, GruppoMaterialeType.P);
            AddMaterialMetric(Aisi, 7.8, GruppoMaterialeType.M);
            AddMaterialMetric(Alu, 2.8, GruppoMaterialeType.N);

            AddPrice(C40, "Base", 1);
            AddPrice(Aisi, "Base", 4);
            AddPrice(Alu, "Base", 3.5);

            //Utensili tornitura
            AddTurningTool("CNMG", LavorazioniEnumOperazioni.TornituraSgrossatura, FeedTorniSgr, 3, 1);
            AddTurningTool("CNMG Fron.", LavorazioniEnumOperazioni.TornituraSfacciaturaSgrossatura, FeedTorniSgr, 1, 1);
            AddTurningTool("TNMG", LavorazioniEnumOperazioni.TornituraFinitura, FeedTorniFin, 1, 2);
            AddTurningTool("TNMG Fron.", LavorazioniEnumOperazioni.TornituraSfacciaturaFinitura, FeedTorniFin, 1, 2);
            AddGrooveTool("GX24", LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura, 3, .1, 3);
            AddGrooveTool("GX24 Fin", LavorazioniEnumOperazioni.TornituraScanalaturaFinitura, 3, .1, 3);
            AddThreadTool("266 Insert", LavorazioniEnumOperazioni.TornituraScanalaturaFinitura, 4);

            // Punte 
            AddPuntaSet("HSS", 1, 30, 1, .12);
            AddBareniSet("Bareno", new[] { 10, 20, 30, 40, 50, 60 }, .12);
            AddCentriniSet("HSS", new[] { 1, 3, 5, 10 }, .15);
            AddSvasatoriSet("HSS", new[] { 3, 5, 10, 15, 20, 25 }, .15);
            AddMaschiSet("HSS", new[] { 3, 4, 5, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26 }, .15);
            AddLamatoriSet("HSS", new[] { 5, 8, 11, 14, 17, 20 }, .15);
            AddAlesatoriSet("HSS", new[] { 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, .24);

            // Frese
            AddFresaSpianare("R345", 50, 55, 10, .8, 0);

            AddFresaCandela("R390", 40, 3, .4, 12);
            AddFresaCandela("R390", 20, 3, .3, 15);
            AddFresaCandela("R390", 16, 3, .25, 17);

            AddFresaCandela("WX", 3, 1.5, .08, 3);
            AddFresaCandela("WX", 4, 1.5, .09, 4);
            AddFresaCandela("WX", 5, 1.5, .1, 5);
            AddFresaCandela("WX", 6, 1.5, .12, 6);
            AddFresaCandela("WX", 8, 1.5, .13, 7);
            AddFresaCandela("WX", 10, 1.5, .13, 8);
            AddFresaCandela("WX", 12, 1.5, .13, 9);


        }
       

       

        private void AddGrooveTool(string toolName, LavorazioniEnumOperazioni lavorazioniEnumOperazioni, double toolW, double feed, int pos)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            var larghe = (toolW * 80) / 100;

            var tMm = new UtensileScanalatura(MeasureUnit.Millimeter) { OperazioneTipo = lavorazioniEnumOperazioni, ToolName = toolName, ToolPosition = pos, LarghezzaUtensile = toolW };

            AddParametro(tMm, matC40, VelTaglioC40, feed, larghe);
            AddParametro(tMm, matInoxMm, VelTaglioInox, feed, larghe);
            AddParametro(tMm, matAluMm, VelTaglioAlluminio, feed, larghe);

            _mag.AddOrUpdateTool(tMm);

            var matC40Inc = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluI = GetMaterialByName(Alu, MeasureUnit.Inch);

            var tI = new UtensileScanalatura(MeasureUnit.Inch) { OperazioneTipo = lavorazioniEnumOperazioni, ToolName = toolName, ToolPosition = pos, LarghezzaUtensile = FeedAndSpeedHelper.GetInchFromMm(toolW) };

            AddParametro(tI, matC40Inc, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioC40), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(larghe));
            AddParametro(tI, matInoxI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioInox), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(larghe));
            AddParametro(tI, matAluI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioAlluminio), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(larghe));

            _mag.AddOrUpdateTool(tI);
        }

        /// <summary>
        /// Aggiunge utensile tornitura.
        /// </summary>
        private void AddTurningTool(string toolName, LavorazioniEnumOperazioni lavorazioniEnumOperazioni, double feed, double profPass, int pos)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            var tMm = new UtensileTornitura(MeasureUnit.Millimeter) { OperazioneTipo = lavorazioniEnumOperazioni, ToolName = toolName, ToolPosition = pos };


            AddParametro(tMm, matC40, VelTaglioC40, feed, profPass);
            AddParametro(tMm, matInoxMm, VelTaglioInox, feed, profPass);
            AddParametro(tMm, matAluMm, VelTaglioAlluminio, feed, profPass);

            _mag.AddOrUpdateTool(tMm);

            var matC40Inc = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluI = GetMaterialByName(Alu, MeasureUnit.Inch);

            var tI = new UtensileTornitura(MeasureUnit.Inch) { OperazioneTipo = lavorazioniEnumOperazioni, ToolName = toolName, ToolPosition = pos };

            AddParametro(tI, matC40Inc, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioC40), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(profPass));
            AddParametro(tI, matInoxI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioInox), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(profPass));
            AddParametro(tI, matAluI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioAlluminio), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(profPass));

            _mag.AddOrUpdateTool(tI);
        }

        private static void AddParametro(UtensileTornitura tornitura, Materiale materiale, double vel, double feed, double proPa)
        {
            var par = new ParametroUtensileTornitura(tornitura)
            {
                ModalitaVelocita = ModalitaVelocita.VelocitaTaglio,
                Velocita = vel,
                AvanzamentoSincrono = feed,
                MaterialGuid = (materiale.MaterialeGuid),
                ProfonditaPassata = proPa
            };

            tornitura.AddOrUpdateParametro(par, materiale.MaterialeGuid);

        }
        private static void AddParametro(UtensileScanalatura tornitura, Materiale materiale, double vel, double feed, double larghezza)
        {
            var par = new ParametroUtensileTornituraScanalatura(tornitura)
            {
                ModalitaVelocita = ModalitaVelocita.VelocitaTaglio,
                Velocita = vel,
                AvanzamentoSincrono = feed,
                MaterialGuid = (materiale.MaterialeGuid),
                LarghezzaPassata = larghezza,
            };

            tornitura.AddOrUpdateParametro(par, materiale.MaterialeGuid);

        }
        private void AddBareniSet(string toolName, int[] diaList, double feed)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var p = new Bareno(MeasureUnit.Millimeter) { ToolName = toolName, Diametro = i, ToolPosition = 5 };

                AddParametro(p, matC40, VelTaglioC40, feed, i);
                AddParametro(p, matInoxMm, VelTaglioInox, feed, i);
                AddParametro(p, matAluMm, VelTaglioAlluminio, feed, i);
                _mag.AddOrUpdateTool(p);

            }

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var d = FeedAndSpeedHelper.GetInchFromMm(i);
                var p = new Bareno(MeasureUnit.Inch) { ToolName = toolName, Diametro = Math.Round(d, 5), ToolPosition = 5 };

                feed = FeedAndSpeedHelper.GetInchFromMm(feed);

                AddParametro(p, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioC40), feed, p.Diametro);
                AddParametro(p, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioInox), feed, p.Diametro);
                AddParametro(p, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioAlluminio), feed, p.Diametro);
                _mag.AddOrUpdateTool(p);

            }
        }


        private void AddThreadTool(string toolName, LavorazioniEnumOperazioni lavorazioniEnumOperazioni, int pos)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            var tMm = new UtensileFilettare(MeasureUnit.Millimeter) { OperazioneTipo = lavorazioniEnumOperazioni, ToolName = toolName, ToolPosition = pos };

            AddParametro(tMm, matC40, VelTaglioC40);
            AddParametro(tMm, matInoxMm, VelTaglioInox);
            AddParametro(tMm, matAluMm, VelTaglioAlluminio);

            _mag.AddOrUpdateTool(tMm);

            var matC40Inc = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluI = GetMaterialByName(Alu, MeasureUnit.Inch);

            var tI = new UtensileFilettare(MeasureUnit.Inch) { OperazioneTipo = lavorazioniEnumOperazioni, ToolName = toolName, ToolPosition = pos };

            AddParametro(tI, matC40Inc, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioC40));
            AddParametro(tI, matInoxI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioInox));
            AddParametro(tI, matAluI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioAlluminio));

            _mag.AddOrUpdateTool(tI);
        }

        private static void AddParametro(UtensileFilettare tMm, Materiale matC40, double VelTaglioC40)
        {
            var par = new ParametroUtensileTornitura(tMm)
            {
                ModalitaVelocita = ModalitaVelocita.VelocitaTaglio,
                Velocita = VelTaglioC40,
                MaterialGuid = (matC40.MaterialeGuid),
            };

            tMm.AddOrUpdateParametro(par, matC40.MaterialeGuid);
        }

        /// <summary>
        /// Aggiunge Materiale con valori metrici e poi aggiunge anche i suoi valori da inch..
        /// </summary>
        private void AddMaterialMetric(string nome, double pesoSpecifico, GruppoMaterialeType gruppoMaterialeType)
        {
            var mat = new Materiale(MeasureUnit.Millimeter);
            var matInch = new Materiale(MeasureUnit.Inch);
            mat.Descrizione = matInch.Descrizione = nome;
            mat.PesoSpecifico = pesoSpecifico;
            mat.GruppoMateriale = matInch.GruppoMateriale = gruppoMaterialeType;
            matInch.PesoSpecifico = FeedAndSpeedHelper.DensityDmToInch(pesoSpecifico);

            _mag.AddMateriale(matInch);
            _mag.AddMateriale(mat);
        }

        private Materiale GetMaterialByName(string nomeMat, MeasureUnit measureUnit)
        {
            var ms = _mag.GetMaterials(measureUnit);

            return ms.Where(m => m.Descrizione == nomeMat).FirstOrDefault();
        }

        private const double CambioEuroDollaro = 1.3490;

        private void AddPrice(string nomeMat, string descrizione, double price)
        {
            var matMm = GetMaterialByName(nomeMat, MeasureUnit.Millimeter);

            if (matMm != null)
            {
                var p = new PrezzoMateriale
                {
                    Descrizione = descrizione,
                    Prezzo = price,
                    MaterialeGuid = matMm.MaterialeGuid
                };
                _mag.AddPrezzoMateriale(p);
            }

            var matInch = GetMaterialByName(nomeMat, MeasureUnit.Inch);

            if (matInch != null)
            {
                var p = new PrezzoMateriale
                {
                    Descrizione = descrizione,
                    Prezzo = price / CambioEuroDollaro,
                    MaterialeGuid = matInch.MaterialeGuid
                };
                _mag.AddPrezzoMateriale(p);

            }
        }



        #region Punte Set

        private const double VelTaglioHssInox = 12;
        private const double VelTaglioHssAlluminio = 35;
        private const double VelTaglioHssC40 = 20;

        private void AddMaschiSet(string toolName, IEnumerable<int> diaList, double feedOnD10)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var p = new Maschio(MeasureUnit.Millimeter) { ToolName = toolName, Diametro = i, ToolPosition = 6 };

                var feed = (i * feedOnD10) / 10;

                AddParametro(p, matC40, VelTaglioHssC40, feed, i);
                AddParametro(p, matInoxMm, VelTaglioHssInox, feed, i);
                AddParametro(p, matAluMm, VelTaglioHssAlluminio, feed, i);
                _mag.AddOrUpdateTool(p);
            }

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var d = FeedAndSpeedHelper.GetInchFromMm(i);
                var p = new Maschio(MeasureUnit.Inch) { ToolName = toolName, Diametro = Math.Round(d, 5), ToolPosition = 6 };

                var feed = (i * feedOnD10) / 10;
                feed = FeedAndSpeedHelper.GetInchFromMm(feed);

                AddParametro(p, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssC40), feed, p.Diametro);
                AddParametro(p, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssInox), feed, p.Diametro);
                AddParametro(p, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssAlluminio), feed, p.Diametro);
                _mag.AddOrUpdateTool(p);
            }
        }

        private void AddLamatoriSet(string toolName, IEnumerable<int> diaList, double feedOnD10)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var p = new Lamatore(MeasureUnit.Millimeter) { ToolName = toolName, Diametro = i };

                var feed = (i * feedOnD10) / 10;

                AddParametro(p, matC40, VelTaglioHssC40, feed, i);
                AddParametro(p, matInoxMm, VelTaglioHssInox, feed, i);
                AddParametro(p, matAluMm, VelTaglioHssAlluminio, feed, i);
                _mag.AddOrUpdateTool(p);
            }

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var d = FeedAndSpeedHelper.GetInchFromMm(i);
                var p = new Lamatore(MeasureUnit.Inch) { ToolName = toolName, Diametro = Math.Round(d, 5), ToolPosition = 8 };

                var feed = (i * feedOnD10) / 10;
                feed = FeedAndSpeedHelper.GetInchFromMm(feed);

                AddParametro(p, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssC40), feed, p.Diametro);
                AddParametro(p, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssInox), feed, p.Diametro);
                AddParametro(p, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssAlluminio), feed, p.Diametro);
                _mag.AddOrUpdateTool(p);
            }
        }

        private void AddAlesatoriSet(string toolName, IEnumerable<int> diaList, double feedOnD10)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var p = new Lamatore(MeasureUnit.Millimeter) { ToolName = toolName, Diametro = i, ToolPosition = 10 };

                var feed = (i * feedOnD10) / 10;

                AddParametro(p, matC40, VelTaglioHssC40 / 2, feed, i);
                AddParametro(p, matInoxMm, VelTaglioHssInox / 2, feed, i);
                AddParametro(p, matAluMm, VelTaglioHssAlluminio / 2, feed, i);
                _mag.AddOrUpdateTool(p);
            }

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var d = FeedAndSpeedHelper.GetInchFromMm(i);
                var p = new Lamatore(MeasureUnit.Inch) { ToolName = toolName, Diametro = Math.Round(d, 5), ToolPosition = 10 };

                var feed = (i * feedOnD10) / 10;
                feed = FeedAndSpeedHelper.GetInchFromMm(feed);

                AddParametro(p, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssC40 / 2), feed, p.Diametro);
                AddParametro(p, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssInox / 2), feed, p.Diametro);
                AddParametro(p, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssAlluminio / 2), feed, p.Diametro);
                _mag.AddOrUpdateTool(p);
            }
        }

        private void AddSvasatoriSet(string toolName, IEnumerable<int> diaList, double feedOnD10)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var p = new Svasatore(MeasureUnit.Millimeter) { ToolName = toolName, Diametro = i, ToolPosition = 9 };

                var feed = (i * feedOnD10) / 10;

                AddParametro(p, matC40, VelTaglioHssC40, feed, i);
                AddParametro(p, matInoxMm, VelTaglioHssInox, feed, i);
                AddParametro(p, matAluMm, VelTaglioHssAlluminio, feed, i);
                _mag.AddOrUpdateTool(p);
            }

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var d = FeedAndSpeedHelper.GetInchFromMm(i);
                var p = new Svasatore(MeasureUnit.Inch) { ToolName = toolName, Diametro = Math.Round(d, 5), ToolPosition = 9 };

                var feed = (i * feedOnD10) / 10;

                feed = FeedAndSpeedHelper.GetInchFromMm(feed);

                AddParametro(p, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssC40), feed, p.Diametro);
                AddParametro(p, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssInox), feed, p.Diametro);
                AddParametro(p, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssAlluminio), feed, p.Diametro);
                _mag.AddOrUpdateTool(p);
            }
        }

        private void AddCentriniSet(string toolName, IEnumerable<int> diaList, double feedOnD10)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var p = new Centrino(MeasureUnit.Millimeter) { ToolName = toolName, Diametro = i, ToolPosition = 5 };

                var feed = (i * feedOnD10) / 10;

                AddParametro(p, matC40, VelTaglioHssC40, feed, i);
                AddParametro(p, matInoxMm, VelTaglioHssInox, feed, i);
                AddParametro(p, matAluMm, VelTaglioHssAlluminio, feed, i);
                _mag.AddOrUpdateTool(p);

            }

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            foreach (var i in diaList)
            {
                var d = FeedAndSpeedHelper.GetInchFromMm(i);
                var p = new Punta(MeasureUnit.Inch) { ToolName = toolName, Diametro = Math.Round(d, 5), ToolPosition = 5 };

                var feed = (i * feedOnD10) / 10;

                feed = FeedAndSpeedHelper.GetInchFromMm(feed);

                AddParametro(p, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssC40), feed, p.Diametro);
                AddParametro(p, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssInox), feed, p.Diametro);
                AddParametro(p, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssAlluminio), feed, p.Diametro);
                _mag.AddOrUpdateTool(p);

            }
        }

        private void AddPuntaSet(string drillName, double diameterIni, double diameterFin, double increment, double feedOnD10)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            for (var i = diameterIni; i < diameterFin; i += increment)
            {
                var p = new Punta(MeasureUnit.Millimeter) { ToolName = drillName, Diametro = Math.Round(i, 5), ToolPosition = 7 };

                var feed = (i * feedOnD10) / 10;

                AddParametro(p, matC40, VelTaglioHssC40, feed, i);
                AddParametro(p, matInoxMm, VelTaglioHssInox, feed, i);
                AddParametro(p, matAluMm, VelTaglioHssAlluminio, feed, i);

                _mag.AddOrUpdateTool(p);
            }

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            /*
             * come feed prendo un valori indicativo per punta diametro 10 poi faccio proporzione.
             * 10:f = i:x
             * (i*f )/10 = feedC
             */

            for (var i = FeedAndSpeedHelper.GetInchFromMm(diameterIni); i < FeedAndSpeedHelper.GetInchFromMm(diameterFin); i += FeedAndSpeedHelper.GetInchFromMm(increment))
            {
                var p = new Punta(MeasureUnit.Inch) { ToolName = drillName, Diametro = Math.Round(i, 5), ToolPosition = 7 };

                var feed = (i * feedOnD10) / 10;

                feed = FeedAndSpeedHelper.GetInchFromMm(feed);

                AddParametro(p, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssC40), feed, p.Diametro);
                AddParametro(p, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssInox), feed, p.Diametro);
                AddParametro(p, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioHssAlluminio), feed, p.Diametro);
                _mag.AddOrUpdateTool(p);

            }

        }

        private static void AddParametro(FresaBase punta, Materiale materiale, double vel, double feed, double plungeFeed, double larghPerc = 80, double profPerc = 10)
        {
            var p = punta.CreateParametro() as ParametroFresaBase;

            p.MaterialGuid = materiale.MaterialeGuid;
            p.SetVelocitaTaglio(vel);
            p.SetFeedSync(feed);
            p.SetPlungeFeedSync(plungeFeed);
            p.SetLarghPassataPerc(larghPerc);
            p.SetProfPassataPerc(profPerc);

            punta.AddOrUpdateParametro(p, materiale.MaterialeGuid);
        }

        private static void AddParametro(DrillTool punta, Materiale materiale, double vel, double feed, double step)
        {
            var par = new ParametroPunta(punta);
            par.SetVelocitaTaglio(vel);
            par.SetFeedSync(feed);
            if (!(punta is Bareno))
                par.Step = step;
            punta.AddOrUpdateParametro(par, materiale.MaterialeGuid);
        }
        #endregion

        private const double VelTaglioInoxMill = 100;
        private const double VelTaglioAlluminioMill = 300;
        private const double VelTaglioC40Mill = 150;

        private void AddFresaCandela(string millName, double diameter, double lxD, double feed, int toolPos)
        {
            var plungeFeed = feed / 3;

            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            var fpMm = new FresaCandela(MeasureUnit.Millimeter) { ToolName = millName, Diametro = diameter, Altezza = lxD * diameter, ToolPosition = toolPos };

            AddParametro(fpMm, matC40, VelTaglioC40Mill, feed, plungeFeed, 30, 70);
            AddParametro(fpMm, matInoxMm, VelTaglioInoxMill, feed, plungeFeed, 30, 70);
            AddParametro(fpMm, matAluMm, VelTaglioAlluminioMill, feed, plungeFeed, 30, 70);

            _mag.AddOrUpdateTool(fpMm);

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            var p1 = new FresaCandela(MeasureUnit.Inch) { ToolName = millName, Diametro = FeedAndSpeedHelper.GetInchFromMm(diameter), Altezza = FeedAndSpeedHelper.GetInchFromMm(lxD * diameter), ToolPosition = toolPos };

            feed = FeedAndSpeedHelper.GetInchFromMm(feed);

            AddParametro(p1, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioC40Mill), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(plungeFeed), 30, 70);
            AddParametro(p1, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioInoxMill), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(plungeFeed), 30, 70);
            AddParametro(p1, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioAlluminioMill), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(plungeFeed), 30, 70);

            _mag.AddOrUpdateTool(p1);

        }

        private void AddFresaSpianare(string millName, double diameter, double diameterMax, double altezza, double feed, double plungeFeed)
        {
            var matC40 = GetMaterialByName(C40, MeasureUnit.Millimeter);
            var matInoxMm = GetMaterialByName(Aisi, MeasureUnit.Millimeter);
            var matAluMm = GetMaterialByName(Alu, MeasureUnit.Millimeter);

            var fpMm = new FresaSpianare(MeasureUnit.Millimeter) { ToolName = millName, Diametro = diameter, DiametroIngombroFresa = diameterMax, Altezza = altezza, ToolPosition = 16 };

            AddParametro(fpMm, matC40, VelTaglioC40, feed, plungeFeed);
            AddParametro(fpMm, matInoxMm, VelTaglioInox, feed, plungeFeed);
            AddParametro(fpMm, matAluMm, VelTaglioAlluminio, feed, plungeFeed);

            _mag.AddOrUpdateTool(fpMm);

            var matC40I = GetMaterialByName(C40, MeasureUnit.Inch);
            var matInoxMmI = GetMaterialByName(Aisi, MeasureUnit.Inch);
            var matAluMmI = GetMaterialByName(Alu, MeasureUnit.Inch);

            var p1 = new FresaSpianare(MeasureUnit.Inch) { ToolName = millName, Diametro = FeedAndSpeedHelper.GetInchFromMm(diameter), DiametroIngombroFresa = FeedAndSpeedHelper.GetInchFromMm(diameterMax), Altezza = FeedAndSpeedHelper.GetInchFromMm(altezza), ToolPosition = 16 };

            feed = FeedAndSpeedHelper.GetInchFromMm(feed);

            AddParametro(p1, matC40I, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioC40), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(plungeFeed));
            AddParametro(p1, matInoxMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioInox), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(plungeFeed));
            AddParametro(p1, matAluMmI, FeedAndSpeedHelper.GetInchSpeedCut(VelTaglioAlluminio), FeedAndSpeedHelper.GetInchFromMm(feed), FeedAndSpeedHelper.GetInchFromMm(plungeFeed));

            _mag.AddOrUpdateTool(p1);

        }

    }
    [Serializable]
    public class MagazzinoUtensile
    {
        private readonly List<Utensile> _tools = new List<Utensile>();
        private readonly List<Materiale> _materiali = new List<Materiale>();
        private readonly List<PrezzoMateriale> _prezziMateriale = new List<PrezzoMateriale>();

        public MagazzinoUtensile()
        {
            var de = new DefaultDataFiller(this);

            de.AddDefaultData();
        }

        public IEnumerable<TRet> GetTools<TRet>(MeasureUnit measureUnit) where TRet : Utensile
        {
            var rslt = _tools.OfType<TRet>().Where(t => t.Unit == measureUnit);
            return rslt;
        }



        internal List<Materiale> GetMaterials()
        {
            return _materiali;
        }
        public List<Materiale> GetMaterials(MeasureUnit measureUnit)
        {
            return _materiali.Where(m => m.MeasureUnit == measureUnit).ToList();
        }

        /// <summary>
        /// Salva modifiche utensile , se non esiste lo aggiunge
        /// </summary>
        /// <param name="utensile"></param>
        public void SaveTool(Utensile utensile)
        {
            var saved = false;
            for (int i = 0; i < _tools.Count; i++)
            {
                if (_tools[i].ToolGuid == utensile.ToolGuid)
                {
                    _tools[i] = utensile;
                    saved = true;
                    break;
                }
            }

            if (!saved)
                _tools.Add(utensile);
        }


        public List<Utensile> GetTools(Type type, MeasureUnit measureUnit)
        {
            var t = from utensile in _tools
                    where utensile.GetType() == type
                    where utensile.Unit == measureUnit
                    select utensile;

            return t.ToList();
        }

        internal List<Utensile> GetTools(MeasureUnit measureUnit)
        {
            var t = from utensile in _tools
                    where utensile.Unit == measureUnit
                    select utensile;

            return t.ToList();
        }

        internal void DeletePrezzo(PrezzoMateriale prezzoMaterialeSelezionato)
        {
            if (_prezziMateriale.Contains(prezzoMaterialeSelezionato))
                _prezziMateriale.Remove(prezzoMaterialeSelezionato);
        }

        internal void AddPrezzoMateriale(PrezzoMateriale prezzo)
        {
            if (prezzo != null)
                _prezziMateriale.Add(prezzo);
        }

        internal void DeleteMaterial(Materiale materialeSelezionato)
        {
            if (_materiali.Contains(materialeSelezionato))
                _materiali.Remove(materialeSelezionato);
        }

        internal void AddMateriale(Materiale materiale)
        {
            _materiali.Add(materiale);
        }

        internal IEnumerable<PrezzoMateriale> GetPrezziMateriale(Guid materialeGuid, MeasureUnit measureUnit)
        {
            var p = from prezzoMateriale in _prezziMateriale
                    where prezzoMateriale.Materiale.MeasureUnit == measureUnit
                    where prezzoMateriale.MaterialeGuid == materialeGuid
                    select prezzoMateriale;

            return p;
        }

        internal IEnumerable<PrezzoMateriale> GetPrezziMateriale(MeasureUnit measureUnit)
        {
            return _prezziMateriale.Where(m => m.Materiale.MeasureUnit == measureUnit);
        }

        internal PrezzoMateriale GetPrezzoMateriale(Guid prezzoGuid)
        {
            var p = (from prezzoMateriale in _prezziMateriale
                     where prezzoMateriale.PrezzoGuid == prezzoGuid
                     select prezzoMateriale).FirstOrDefault();

            return p;
        }



        internal void AddOrUpdateTool(Utensile tool)
        {
            if (tool == null) return;

            var t = (from utensile in GetTools(tool.Unit)
                     where utensile.ToolGuid == tool.ToolGuid
                     select utensile).FirstOrDefault();

            if (t != null)
            {
                if (_tools.Contains(t))
                    _tools.Remove(t);
            }

            _tools.Add(tool);
            return;


        }

        internal Utensile GetTool(Guid utensileGuid)
        {
            var p = (from utensile in _tools
                     where utensile.ToolGuid == utensileGuid
                     select utensile).FirstOrDefault();

            return p;
        }

        public void RemoveTool(Utensile t)
        {
            if (_tools.Contains(t))
                _tools.Remove(t);
        }
    }

    
}




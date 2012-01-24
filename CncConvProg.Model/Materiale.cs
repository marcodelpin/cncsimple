using System;

namespace CncConvProg.Model.Tool
{
    [Serializable]
    public class Materiale : IEquatable<Materiale>
    {
        public Guid MaterialeGuid { get; private set; }

        public Materiale(MeasureUnit measureUnit)
        {
            MeasureUnit = measureUnit;
            MaterialeGuid = Guid.NewGuid();
        }

        public GruppoMaterialeType GruppoMateriale { get; set; }

        public string Descrizione { get; set; }
        public double PesoSpecifico { get; set; }
        public readonly MeasureUnit MeasureUnit;

        public bool Equals(Materiale other)
        {
            return other.MaterialeGuid == MaterialeGuid;
        }
    }

    [Serializable]
    public class PrezzoMateriale : IEquatable<PrezzoMateriale>
    {
        public PrezzoMateriale()
        {
            PrezzoGuid = Guid.NewGuid();
        }

        public Materiale Materiale
        {
            get { return Singleton.Data.GetMaterialeFromGuid(MaterialeGuid); }
        }

        public bool Equals(PrezzoMateriale other)
        {
            return other.PrezzoGuid == PrezzoGuid;
        }

        public Guid MaterialeGuid { get; set; }

        public Guid PrezzoGuid { get; private set; }

        public double Prezzo { get; set; }

        public string Descrizione { get; set; }

        public string FormatedPrezzoDescription
        {
            get
            {
                var m = Materiale;

                if (m == null) return string.Empty;

                return Materiale.Descrizione + " " + Descrizione;
            }
        }
    }

    [Serializable]
    public class DettaglioArticoloMateriale
    {
        public Materiale Materiale
        {
            get { return Singleton.Data.GetMaterialeFromGuid(MaterialeGuid); }
        }
        public PrezzoMateriale PrezzoMateriale
        {
            get { return Singleton.Data.GetPrezzoMateriale(PrezzoGuid); }
        }
        public Guid MaterialeGuid { get; set; }
        public Guid PrezzoGuid { get; set; }
        public double Lunghezza { get; set; }
        public SezioneType Sezione { get; set; }
        public double DiametroEsterno { get; set; }
        public double DiametroInterno { get; set; }
        public double LatoMaggiore { get; set; }
        public double LatoMinore { get; set; }
        public double ChiaveEsagono { get; set; }

        /// <summary>
        /// Ritorna peso del dettaglio materiale
        /// </summary>
        /// <returns></returns>
        private double CalcPeso()
        {
            var pesoUnitario = 0.0d;

            if (PrezzoMateriale == null || PrezzoMateriale.Materiale == null) return 0;

            var pesoSpecifico = PrezzoMateriale.Materiale.PesoSpecifico;

            switch (Sezione)
            {
                case SezioneType.Tondo:
                    {
                        var radius = DiametroEsterno / 2;
                        pesoUnitario = ((radius * radius * Math.PI * pesoSpecifico) / 1000000);

                    } break;

                case SezioneType.Piatto:
                    {
                        pesoUnitario = ((LatoMaggiore * LatoMinore) * pesoSpecifico) / 1000000;

                    } break;

                case SezioneType.Quadro:
                    {
                        pesoUnitario = ((LatoMaggiore * LatoMaggiore) * pesoSpecifico) / 1000000;

                    } break;

                case SezioneType.Tubo:
                    {
                        var radiusMax = DiametroEsterno / 2;
                        var radiusMin = DiametroInterno / 2;

                        pesoUnitario = (((radiusMax * radiusMax * Math.PI) - (radiusMin * radiusMin * Math.PI)) * pesoSpecifico) / 1000000;

                    } break;

                case SezioneType.Esagono:
                    {
                        var apotema = ChiaveEsagono / 2;

                        var lato = apotema / 0.866;

                        var area = (lato * 3) * apotema;

                        pesoUnitario = (area * pesoSpecifico) / 1000000;

                    } break;

                default:
                    {

                    } break;
            }

            return pesoUnitario * Lunghezza;

        }

        public double PrezzoUnitario
        {
            get
            {
                if (PrezzoMateriale == null) return 0;

                var temp = CalcPeso() * PrezzoMateriale.Prezzo;

                return temp;
            }
        }

        public double PesoUnitario
        {
            get
            {
                return CalcPeso();
            }
        }


    }

    public enum SezioneType : byte
    {
        Tondo,
        Piatto,
        Tubo,
        Quadro,
        Esagono,
    }


    public enum GruppoMaterialeType
    {
        P,
        M,
        K,
        N,
        H,
        S,
    }


}
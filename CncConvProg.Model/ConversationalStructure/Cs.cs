namespace CncConvProg.Model.ConversationalStructure
{
    public enum SpindleRotation
    {
        Cw,
        Ccw
    }

    public enum ModalitaVelocita
    {
        GiriFissi,
        VelocitaTaglio
    }

    public enum FeedType
    {
        Sync,
        ASync
    }

    public class Cs
    {
    }

    public enum MoveType
    {
        /* 
         * ra 0.4
         * ra 0.8
         * ra 3.2
         * ra 6.3
         * ra 12.5
         * non serve che metta feed 
         * 
         * 20/06/2011
         * 
         * a quanto pare è meglio che metta già il valore di feed..e levo tutto plunge feed e securerapidfeed..
         * 
         * todo : todo  in quanto devo levare questo enume , 
         * per fare cosa elegante penvaso di fare dictionary per legare questo enum, al valore doubvle 
         * 
         * tipo su ionizio ciclo lavorazione facevo oppure anche dopo che ne sono
         * register. ( secuyreRapidm feed , 1000) 
         * register. ( workFeed feed , .2) 
         * e cosi via , cosi la parte della creaziine profilo è libera dal valore del feed. e riesco a stare piu astratto e chjiaro..
         */
        Rapid,
        SecureRapidFeed,
        PlungeFeed,
        Work,
        Cw,
        Ccw,
    }

    public enum AxisAbilited
    {
        Xyz,
        Xy,
        Z,
    }
}
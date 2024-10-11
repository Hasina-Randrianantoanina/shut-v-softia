namespace IsodaqOperations.Entities
{
    public class Mesure
    {
        public DateTime Time { get; set; }
        public int NumeroVoie { get; set; }
        public string NomVoie { get; set; } = string.Empty;
        public float ValeurRelative { get; set; } // ISODAQ => Not NGF
        public string Evenement { get; set; }
        public Mesure() 
        {
            if (Time.Second==0 && Time.Minute == 0 && Time.Hour == 0)
            {
                Evenement = "J";
            }
            else
            {
                if (ValeurRelative >= 0)
                {
                    Evenement = " ";
                }
                else
                {
                    Evenement = "-";
                }
            }

        }

        

    }
}

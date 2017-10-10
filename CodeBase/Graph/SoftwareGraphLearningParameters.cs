namespace CodeBase
{
    // in dieser Klasse werden alle von außen gegebenen Parameter gesammelt.
    public class SoftwareGraphLearningParameters
    {
        public int NumberCategories { get; set; }
        public int NumberNodes { get; set; }

        //Paramter für den Verknüpftheitsgrad der Graphen
        public double IntraConnectivityDegree { get; set; }

        public double InterConnectivityDegree { get; set; }

        public int NumberObservations { get; set; }
        public int NumberLabels { get; set; }

        public int NumberOfGraphs { get; set; }

        public int NumberTrainingIterations { get; set; }
    }
}
namespace CRFBase.Input
{
    public interface ITrainingInputGraphData
    {
        double[,] ObservationToCRFScoreProbability { get; set; }
    }
}
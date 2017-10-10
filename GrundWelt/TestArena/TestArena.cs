using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class Gymnasio<Exercisetype, Resulttype> : Startable
        where Resulttype : BaseExerciseResult<Exercisetype>
    {
        private ComputationInfo computationInfo = new ComputationInfo();

        public IComputationInfo ComputationInfo
        {
            get { return computationInfo; }
            set { computationInfo = value as ComputationInfo; }
        }


        private LinkedList<IAthlete<Exercisetype, Resulttype>> athlitis = new LinkedList<IAthlete<Exercisetype, Resulttype>>();
        private List<Exercisetype> dataset;

        public List<Exercisetype> Dataset
        {
            get { return dataset; }
            set
            {
                dataset = value;
                checkState();
            }
        }

        protected abstract List<Exercisetype> CreateExercise(string dataset);

        public void Register(IAthlete<Exercisetype, Resulttype> athlitis)
        {
            this.athlitis.Add(athlitis);
            checkState();
        }

        public void Run(string dataset, Func<Resulttype, double> resultEvaluation = null)
        {
            Run(CreateExercise(dataset), resultEvaluation);
        }
        public void Run(Exercisetype exercise, Func<Resulttype, double> resultEvaluation = null)
        {
            Run(exercise.ToIEnumerable().ToList(), resultEvaluation);
        }
        private StartableState state;

        public StartableState State
        {
            get { return state; }
            set { state = value; }
        }


        public void Run(List<Exercisetype> dataset, Func<Resulttype, double> resultEvaluation = null)
        {
            Dataset = dataset;
            if (resultEvaluation != null)
                ResultEvaluation = resultEvaluation;

            checkState();

            if (State == StartableState.Online)
                Run();
        }

        private void checkState()
        {
            if (State == StartableState.Running)
                return;

            var at = (athlitis.NotNullOrEmpty());
            var ex = dataset.NotNullOrEmpty();
            var res = ResultEvaluation != null;

            if (at && ex && res)
            {
                State = StartableState.Online;
            }
            else
                State = StartableState.Offline;
        }

        private void Run()
        {
            computationInfo.IsActive = true;
            State = StartableState.Running;

            try
            {
                var results = new List<GymResult<Exercisetype, Resulttype>>();

                double runs = athlitis.Count * dataset.Count;
                computationInfo.Length = 1.0 / runs;
                double counter = 0;
                foreach (var athlit in athlitis)
                {
                    athlit.ComputationInfo = ComputationInfo;
                    var res = new GymResult<Exercisetype, Resulttype>();
                    res.Athlete = athlit;
                    results.Add(res);
                    foreach (var exercise in dataset)
                    {
                        if (State == StartableState.CancelRequested)
                        {
                            PublishResults.Enter(results);
                            computationInfo.IsActive = false;
                            State = StartableState.Online;
                            return;
                        }
                        var startTime = DateTime.Now;
                        var result = athlit.execute(exercise, ResultEvaluation);
                        result.StartTime = startTime;
                        result.EndTime = DateTime.Now;
                        result.Challenge = exercise;
                        res.ExerciseResults.Add(result);
                        counter++;
                        computationInfo.From = counter / runs;
                    }
                }
                PublishResults.Enter(results);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                computationInfo.IsActive = false;
                State = StartableState.Online;
            }
        }

        public void Start()
        {
            if (State == StartableState.Online)
            {
                Run();
            }
        }

        public void Stop()
        {
            State = StartableState.CancelRequested;
        }

        private Func<Resulttype, double> resEvaluation;

        public Func<Resulttype, double> ResultEvaluation
        {
            get { return resEvaluation; }
            set
            {
                resEvaluation = value;
                checkState();
            }
        }


        public MEvent<List<GymResult<Exercisetype, Resulttype>>> PublishResults { get; } = new MEvent<List<GymResult<Exercisetype, Resulttype>>>();

    }
    public interface IAthlete<Exercise, Result> where Result : BaseExerciseResult<Exercise>
    {
        IComputationInfo ComputationInfo { get; set; }
        int Id { get; set; }
        string Name { get; set; }
        Result execute(Exercise data, Func<Result, double> resultEvaluation);

        void Store(string file);

    }
    public abstract class BaseExerciseResult<ExerciseData>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan TimeUsed => EndTime - StartTime;
        public ExerciseData Challenge { get; set; }
    }
    public class GymResult<ExerciseData, ExerciseResultData>
        where ExerciseResultData : BaseExerciseResult<ExerciseData>
    {
        public IAthlete<ExerciseData, ExerciseResultData> Athlete { get; set; }
        public List<ExerciseResultData> ExerciseResults { get; set; } = new List<ExerciseResultData>();
    }
}

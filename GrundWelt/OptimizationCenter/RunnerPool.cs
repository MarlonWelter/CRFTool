using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class RunnerPool<PositionData, ActionData, CheckPointType>
           where PositionData : BasePositionData<PositionData, ActionData, CheckPointType>, Cloneable<PositionData>, IHasFingerprint
        where ActionData : BaseActionData<PositionData, ActionData, CheckPointType>
        where CheckPointType : CheckPoint<PositionData>
    {
        public string Id { get; set; }
        public string StoragePath { get; set; } = @"RunItems/";
        public int MultiEvaluationOptions { get; set; }

        public RunnerPool(ActionHandling<PositionData, ActionData> defaultActionHandling, int multiEvaluationOptions, int id = 0, int basicPopulation = GlobalParameters.RunnerPoolDefaultBasicPopulation)
        {
            MultiEvaluationOptions = multiEvaluationOptions;
            Id = id.ToString();
            if (id == 0)
                Id = Program.Random.Next().ToString();

            StoragePath += Id + "//";

            DefaultActionHandling = defaultActionHandling;
            BasicPopulation = basicPopulation;


            Load();
        }


        private void breed(int count)
        {
            var poplist = volatilePopulation.ToList();
            for (int i = 0; i < count; i++)
            {
                volatilePopulation.AddLast(poplist[Program.Random.Next(poplist.Count)].Breed(poplist[Program.Random.Next(poplist.Count)]));
            }
        }

        private readonly ActionHandling<PositionData, ActionData> DefaultActionHandling;
        public Action<RunItem<PositionData, ActionData, CheckPointType>> OnEndpositionDiscovered { get; set; }

        private int BasicPopulation { get; set; }
        private readonly LinkedList<RunItem<PositionData, ActionData, CheckPointType>> volatilePopulation = new LinkedList<RunItem<PositionData, ActionData, CheckPointType>>();

        private readonly LinkedList<RunItem<PositionData, ActionData, CheckPointType>> corePopulation = new LinkedList<RunItem<PositionData, ActionData, CheckPointType>>();


        public abstract LinkedList<RunItem<PositionData, ActionData, CheckPointType>> RequestRunItems(IEnumerable<GWPosition<PositionData, ActionData>> positions);


        public void RefreshPoolPopulation(double ratio)
        {
            if (ratio <= 0 || ratio > 1)
                return;

            var pop = volatilePopulation.OrderBy(rI => rI.Gold).ToList();
            var substitutions = (int)(ratio * volatilePopulation.Count);
            for (int i = 0; i < substitutions; i++)
            {
                volatilePopulation.Remove(pop[i]);
            }
            volatilePopulation.Each(rI => rI.Gold = 0);

            breed(substitutions);

        }

        private double[] RandomWeights(int size)
        {
            var array = new double[size];

            for (int i = 0; i < size; i++)
            {
                array[i] = Program.Random.NextDouble();
            }

            return array;
        }

        public void Store()
        {
            Directory.CreateDirectory(StoragePath);

            // *clean the pool*
            // protect against an overflow of runItems saved in the past.
            foreach (var file in Directory.GetFiles(StoragePath).ToList())
            {
                File.Delete(file);
            }

            foreach (var item in volatilePopulation.OrderByDescending(rI => rI.Gold).Take(BasicPopulation).ToList())
            {
                using (var writer = new StreamWriter(StoragePath + item.Id + ".txt"))
                {
                    item.Save(writer);
                }
            }
        }
        public void Load()
        {
            try
            {
                foreach (var file in Directory.GetFiles(StoragePath))
                {
                    using (var reader = new StreamReader(file))
                    {
                        var item = RunItem<PositionData, ActionData, CheckPointType>.Load(reader, DefaultActionHandling, MultiEvaluationOptions);
                        item.OnTargetReached = OnEndpositionDiscovered;
                        volatilePopulation.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Post("error loading RunItems for Pool " + Id + ". - " + e.Message, LogCategory.Inconsistency);
            }
        }
    }

}

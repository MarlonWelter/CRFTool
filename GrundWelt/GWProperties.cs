using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    //public static class Properties
    //{
    //    public readonly static Random Random = new Random();
    //    public static GWProperty CreateRandomProperty(int length)
    //    {
    //        var proptypes = Enum.GetValues(typeof(PropertyType)).Cast<PropertyType>().ToArray();
    //        var choice = proptypes[Random.Next(proptypes.Length)];
    //        var prop = default(GWProperty);

    //        var baseProps = new double[length];
    //        for (int i = 0; i < length; i++)
    //        {
    //            baseProps[i] = Random.NextDouble();
    //        }
    //        var weight = Random.NextDouble();

    //        switch (choice)
    //        {
    //            case PropertyType.Manhattan:
    //                prop = new ManhattanDistProp(baseProps, weight);
    //                break;
    //            case PropertyType.Euclid:
    //                prop = new EuclidianDistProp(baseProps, weight);
    //                break;
    //            case PropertyType.EuclidSquare:
    //                prop = new SquareEuclidDistProp(baseProps, weight);
    //                break;
    //            default:
    //                break;
    //        }
    //        return prop;
    //    }
    //}

    //enum PropertyType
    //{
    //    Manhattan,
    //    Euclid,
    //    EuclidSquare
    //}

    //public class ManhattanDistProp : GWProperty
    //{
    //    public ManhattanDistProp(double[] baseProps, double weight = 0)
    //        : base(baseProps)
    //    {
    //        Weight = weight;
    //    }
    //    public override double Score(double[] baseproperties)
    //    {
    //        var dist = 0.0;
    //        for (int i = 0; i < BaseProperties.Length; i++)
    //        {
    //            dist += Math.Abs(BaseProperties[i] - baseproperties[i]);
    //        }
    //        return -dist;
    //    }

    //    protected override GWProperty ProtClone()
    //    {
    //        return new ManhattanDistProp(BaseProperties, Weight);
    //    }
    //}
    //public class EuclidianDistProp : GWProperty
    //{
    //    public EuclidianDistProp(double[] baseProps, double weight = 0)
    //        : base(baseProps)
    //    {
    //        Weight = weight;
    //    }
    //    public override double Score(double[] baseproperties)
    //    {
    //        var dist = 0.0;
    //        for (int i = 0; i < BaseProperties.Length; i++)
    //        {
    //            dist += (BaseProperties[i] - baseproperties[i]) * (BaseProperties[i] - baseproperties[i]);
    //        }
    //        dist = Math.Sqrt(dist);
    //        return -dist;
    //    }

    //    protected override GWProperty ProtClone()
    //    {
    //        return new EuclidianDistProp(BaseProperties, Weight);
    //    }
    //}
    //public class SquareEuclidDistProp : GWProperty
    //{
    //    public SquareEuclidDistProp(double[] baseProps, double weight = 0)
    //        : base(baseProps)
    //    {
    //        Weight = weight;
    //    }
    //    public override double Score(double[] baseproperties)
    //    {
    //        var dist = 0.0;
    //        for (int i = 0; i < BaseProperties.Length; i++)
    //        {
    //            dist += (BaseProperties[i] - baseproperties[i]) * (BaseProperties[i] - baseproperties[i]);
    //        }
    //        return -dist;
    //    }

    //    protected override GWProperty ProtClone()
    //    {
    //        return new SquareEuclidDistProp(BaseProperties, Weight);
    //    }
    //}
}

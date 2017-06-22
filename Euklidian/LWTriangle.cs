using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class LWTriangle : IGWObject
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public double Height { get; set; }
        public double LengthLong { get; set; }
        public double LengthShort { get; set; }
        public double WidthLong { get; set; }
        public double WidthShort { get; set; }
        public double LengthDiff { get { return LengthLong - LengthShort; } }
        public double WidthDiff { get { return WidthLong - WidthShort; } }

        //public LWQuboid QuboidOne()
        //{
        //    { return new LWQuboid(WidthShort, LengthLong, Height); }
        //}
        //public LWQuboid QuboidTwo()
        //{
        //    { return new LWQuboid(WidthLong, LengthShort, Height); }
        //}
    }

}

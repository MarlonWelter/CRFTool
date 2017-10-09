
using System;
using System.Collections.Generic;

namespace CodeBase
{

    [Serializable]
    public class GWPackage : IHas<IPackageLogic>
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }

        public GWPackage()
        {
            this.TypeId(mitId);
        }
        public GWPackage(Guid mitId)
        {
            GWId = mitId;
            this.TypeId(mitId);
        }
        public GWPackage(double width, double length, double height)
            : this(width, length, height, Guid.NewGuid())
        {
        }
        public GWPackage(double width, double length, double height, bool canTurnSide, bool canTurnBack)
        {
            this.CanTurnSide(canTurnSide);
            this.CanTurnBack(canTurnBack);
            this.Length(length);
            this.Width(width);
            this.Height(height);
            this.TypeId(mitId);
        }
        public GWPackage(double width, double length, double height, Guid dbGuid)
        {
            this.Length(length);
            this.Width(width);
            this.Height(height);
            GWId = dbGuid;
            this.TypeId(mitId);
        }
        public GWPackage(IHas<IPackageLogic> package)
            : this(package.Logic)
        {
        }
        public GWPackage(IPackageLogic logic)
        {
            this.Logic.TakeSimpleValues(logic);
        }


        public int TourstopPosition { get; set; }
        public string TypeName { get; set; }



        public override string ToString()
        {
            return this.Name();
        }

        private GWPackageLogic logic = new GWPackageLogic();
        public IPackageLogic Logic
        {
            get { return logic; }
        }

    }


}

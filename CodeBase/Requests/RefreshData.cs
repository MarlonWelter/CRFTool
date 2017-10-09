
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class RefreshData : IHas<IRequestLogic<RefreshData>>
    {

        private Guid mitId = Guid.NewGuid();
        public Guid GWId { get { return mitId; } set { mitId = value; } }


        private IRequestLogic<RefreshData> logic = new RequestLogic<RefreshData>();
        public IRequestLogic<RefreshData> Logic
        {
            get { return logic; }
        }
    }

    public class RefreshViewData : IHas<IRequestLogic<RefreshViewData>>
    {

        private Guid mitId = Guid.NewGuid();
        public Guid GWId { get { return mitId; } set { mitId = value; } }


        private IRequestLogic<RefreshViewData> logic = new RequestLogic<RefreshViewData>();
        public IRequestLogic<RefreshViewData> Logic
        {
            get { return logic; }
        }
    }

    public class CommitData : IHas<IRequestLogic<CommitData>>
    {

        private Guid mitId = Guid.NewGuid();
        public Guid GWId { get { return mitId; } set { mitId = value; } }


        private IRequestLogic<CommitData> logic = new RequestLogic<CommitData>();
        public IRequestLogic<CommitData> Logic
        {
            get { return logic; }
        }
    }
}

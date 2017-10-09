using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{

    public class GWRequest<CommandType> : IHas<IRequestLogic<CommandType>>
        where CommandType : GWRequest<CommandType>
    {
        public Guid GWId { get; set; }
        public void Request(RequestRunType runtype = RequestRunType.Default)
        {
            switch (runtype)
            {
                case RequestRunType.Default:
                    DefaultContext<CommandType, bool>.Instance.Enter(this as CommandType);
                    break;
                case RequestRunType.Background:
                    var task = new Task(() => DefaultContext<CommandType, bool>.Instance.Enter(this as CommandType));
                    task.Start();
                    break;
                default:
                    break;
            }
        }

        private RequestLogic<CommandType> logic = new RequestLogic<CommandType>();
        public IRequestLogic<CommandType> Logic
        {
            get { return logic; }
        }
    }

    public class GWRequest<CommandType, ResultType> : IHas<IRequestLogic<CommandType>>
    where CommandType : GWRequest<CommandType, ResultType>
    {
        public Guid GWId { get; set; }
        public ResultType Request(RequestRunType runtype = RequestRunType.Default)
        {
            switch (runtype)
            {
                case RequestRunType.Default:
                    DefaultContext<CommandType, bool>.Instance.Enter(this as CommandType);
                    break;
                case RequestRunType.Background:
                    var task = new Task(() => DefaultContext<CommandType, bool>.Instance.Enter(this as CommandType));
                    task.Start();
                    break;
                default:
                    break;
            }
            return Result;
        }

        public ResultType Result { get; set; }

        private RequestLogic<CommandType> logic = new RequestLogic<CommandType>();
        public IRequestLogic<CommandType> Logic
        {
            get { return logic; }
        }
    }
}

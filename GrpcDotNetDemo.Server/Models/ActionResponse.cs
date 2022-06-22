using System.Collections.Generic;

namespace GrpcDotNetDemo.Server.Models
{
    public class ActionResponse
    {
        public ActionResultType ActionResult { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public object Value { get; set; }

        public ActionResponse(ActionResultType actionResultType)
            : this(actionResultType, null)
        { }

        public ActionResponse(ActionResultType actionResultType, object value)
        {
            ActionResult = actionResultType;
            Value = value;
        }
    }
}

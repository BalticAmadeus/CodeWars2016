using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Game.WebService
{
    [ServiceContract]
    public interface IDebugService
    {
        [OperationContract]
        string CheckMyAccess();

        [OperationContract]
        string GrantMeAccess(string request);

        [OperationContract]
        string RevokeMyAccess(string request);
    }
}

using GameLogic.UserManagement;
using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class CreateObserverReq : BaseReq
    {
        // empty
    }

    [DataContract]
    public class CreateObserverResp : BaseResp
    {
        [DataMember]
        public int ObserverId;

        [DataMember]
        public TeamRole Role;
    }
}
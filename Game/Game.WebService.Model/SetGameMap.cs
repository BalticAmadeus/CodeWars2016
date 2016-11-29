using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class SetGameMapReq : BaseReq
    {
        [DataMember]
        public int GameId;

        [DataMember]
        public EnMapData MapData;
    }

    [DataContract]
    public class SetGameMapResp : BaseResp
    {
        // default
    }
}

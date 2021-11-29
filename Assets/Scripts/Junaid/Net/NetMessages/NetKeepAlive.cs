using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetKeepAlive : NetMessage
{

    public NetKeepAlive() {
        Code = OpCode.KEEP_ALIVE;
    
    }
    public NetKeepAlive(DataStreamReader reader) {
        Code = OpCode.KEEP_ALIVE;
        Deserialize(reader);

    }

    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
    }
    public override void Deserialize(DataStreamReader reader) {

    }


}

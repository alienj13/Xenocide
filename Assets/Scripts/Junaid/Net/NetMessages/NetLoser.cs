using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetLoser : NetMessage
{
    public NetLoser() {
        Code = OpCode.LOSER;
    }
    public NetLoser(DataStreamReader reader) {
        Code = OpCode.LOSER;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
    }
    public override void Deserialize(DataStreamReader reader) {

    }


    public override void RecievedOnClient() {
        NetUtility.C_LOSER?.Invoke(this);

    }
    public override void RecievedOnServer(NetworkConnection cnn) {
        NetUtility.S_LOSER?.Invoke(this, cnn);
    }
}

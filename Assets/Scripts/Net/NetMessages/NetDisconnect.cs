using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetDisconnect : NetMessage
{
    public NetDisconnect() {
        Code = OpCode.CLIENT_DISCONNECT;

    }
    public NetDisconnect(DataStreamReader reader) {
        Code = OpCode.CLIENT_DISCONNECT;
        Deserialize(reader);

    }
    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);

    }
    public override void Deserialize(DataStreamReader reader) {

    }


    public override void RecievedOnClient() {
        NetUtility.C_CLIENT_DISCONNECT?.Invoke(this);

    }
    public override void RecievedOnServer(NetworkConnection cnn) {
        NetUtility.S_CLIENT_DISCONNECT?.Invoke(this, cnn);
    }
}

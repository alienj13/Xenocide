using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetUserName : NetMessage {

    

    public NetUserName() {
        Code = OpCode.USERNAME;

    }
    public NetUserName(DataStreamReader reader) {
        Code = OpCode.USERNAME;
        Deserialize(reader);

    }
    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
    }
    public override void Deserialize(DataStreamReader reader) {
       
    }


    public override void RecievedOnClient() {
        NetUtility.C_USERNAME?.Invoke(this);

    }
    public override void RecievedOnServer(NetworkConnection cnn) {
        NetUtility.S_USERNAME?.Invoke(this, cnn);
    }
}

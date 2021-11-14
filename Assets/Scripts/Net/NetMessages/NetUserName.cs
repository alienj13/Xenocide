using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetUserName : NetMessage {

    
    public FixedString128Bytes PlayerName;
    public NetUserName() {
        Code = OpCode.USERNAME;

    }
    public NetUserName(DataStreamReader reader) {
        Code = OpCode.USERNAME;
        Deserialize(reader);


    }
    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
        writer.WriteFixedString128(PlayerName);
    
    }
    public override void Deserialize(DataStreamReader reader) {
        PlayerName = reader.ReadFixedString128();
    }


    public override void RecievedOnClient() {
        NetUtility.C_USERNAME?.Invoke(this);

    }
    public override void RecievedOnServer(NetworkConnection cnn) {
        NetUtility.S_USERNAME?.Invoke(this, cnn);
    }
}

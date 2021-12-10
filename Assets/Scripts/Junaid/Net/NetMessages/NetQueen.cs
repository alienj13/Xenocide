using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetQueen : NetMessage {
    public int QueenHealth;
  

    public NetQueen() {
        Code = OpCode.QUEEN;

    }
    public NetQueen(DataStreamReader reader) {
        Code = OpCode.QUEEN;
        Deserialize(reader);

    }
    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
        writer.WriteInt(QueenHealth);
    }
    public override void Deserialize(DataStreamReader reader) {
        QueenHealth = reader.ReadInt();
       
    }


    public override void RecievedOnClient() {
        NetUtility.C_QUEEN?.Invoke(this);

    }
    public override void RecievedOnServer(NetworkConnection cnn) {
        NetUtility.S_QUEEN?.Invoke(this, cnn);
    }
}

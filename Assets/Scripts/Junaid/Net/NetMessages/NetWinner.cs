using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetWinner : NetMessage
{
    public int rank;
    public int experience;
    public NetWinner() {
        Code = OpCode.WINNER;
    }
    public NetWinner(DataStreamReader reader) {
        Code = OpCode.WINNER;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
    }
    public override void Deserialize(DataStreamReader reader) {
        rank = reader.ReadInt();
        experience = reader.ReadInt();

    }


    public override void RecievedOnClient() {
        NetUtility.C_WINNER?.Invoke(this);

    }
    public override void RecievedOnServer(NetworkConnection cnn) {
        NetUtility.S_WINNER?.Invoke(this, cnn);
    }
}

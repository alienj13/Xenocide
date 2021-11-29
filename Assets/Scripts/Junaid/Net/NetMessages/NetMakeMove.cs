using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetMakeMove : NetMessage
{

    public int currentX;
    public int currentY;
    public int TargetX;
    public int TargetY;
    public int team;

    public NetMakeMove() {
        Code = OpCode.MAKE_MOVE;

    }
    public NetMakeMove(DataStreamReader reader) {
        Code = OpCode.MAKE_MOVE;
        Deserialize(reader);

    }
    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);
        writer.WriteInt(currentX);
        writer.WriteInt(currentY);
        writer.WriteInt(TargetX);
        writer.WriteInt(TargetY);
        writer.WriteInt(team);
    }
    public override void Deserialize(DataStreamReader reader) {
        currentX = reader.ReadInt();
        currentY = reader.ReadInt();
        TargetX = reader.ReadInt();
        TargetY = reader.ReadInt();
        team = reader.ReadInt();
    }


    public override void RecievedOnClient() {
        NetUtility.C_MAKE_MOVE?.Invoke(this);

    }
    public override void RecievedOnServer(NetworkConnection cnn) {
        NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
    }

}

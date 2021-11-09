using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetStartGame : NetMessage
{
    public NetStartGame() {
        Code = OpCode.START_GAME;

    }
    public NetStartGame(DataStreamReader reader) {
        Code = OpCode.START_GAME;
        Deserialize(reader);

    }
    public override void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)Code);

    }
    public override void Deserialize(DataStreamReader reader) {

    }


    public override void RecievedOnClient() {
        NetUtility.C_START_GAME?.Invoke(this);

    }
    public override void RecievedOnServer(NetworkConnection cnn) {
        NetUtility.S_START_GAME?.Invoke(this, cnn);
    }
}

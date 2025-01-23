using Fusion;
using Google.Protobuf;

public class PlayerData : NetworkBehaviour
{
    private string jwtToken;
    private string nickName;
    private ByteString profileImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetPlayerInfo(string jwtToken,  string nickName, ByteString profileImage)
    {
        this.jwtToken = jwtToken;
        this.nickName = nickName;
        this.profileImage = profileImage;
    }    

    public void GetPlayerInfo()
    {

    }
}

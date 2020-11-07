using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Basic Post Data
/// </summary>
[Serializable]
public abstract class PostData
{
    public string apiKey = Config.APIKey;
}

/// <summary>
/// Match or login post data
/// </summary>
[Serializable]
public class LoginPostData : PostData
{

}

/// <summary>
/// Maybe I don't use maybe i will use // for future
/// </summary>
[Serializable]
public class LoginResponseData
{
    public string playerId;
}

/// <summary>
/// 
/// </summary>
[Serializable]
public abstract class PlayerPostData : PostData
{
    public string playerId;
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class GetMessagePostData : PlayerPostData
{
    public int localMessageCount;
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class GetMessageResponseData
{
    public PrimitiveMessage[] messages;
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class SendMessagePostData : PlayerPostData
{
    public PrimitiveMessage[] messages;
}

/// <summary>
/// Primitive Message Class 
/// </summary>
[Serializable]
public class PrimitiveMessage
{
    public string type;

    public string content;

    public PrimitiveMessage(string type, string content)
    {
        this.type = type;
        this.content = content;
    }
}


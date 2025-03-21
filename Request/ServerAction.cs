namespace Request;

public enum ServerAction
{
    Allow = 1 , Prohibit , Saved , NotSaved , NotDeleted
}

public class ServerResponse
{
    public const int SendProductAndStorage = 1;
    public const int SendProductParty = 2;
    public const int SendStorage = 3;
    public const int SendUserInfo = 4;
    public const int SendProduct = 5;
    public const int SendLocation = 6;
    public const int SendMessageAboutChanges = 7;
    public const int SendDocument = 8;
}

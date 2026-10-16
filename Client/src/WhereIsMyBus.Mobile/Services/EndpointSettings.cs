namespace WhereIsMyBus.Mobile.Services;
public static class EndpointSettings
{
#if DEBUG
    // Android emulator uses 10.0.2.2 to reach the development computer.
    public const string BaseUrl="http://10.0.2.2:5080/";
#else
    // Set this to the public HTTPS API host before a release build.
    public const string BaseUrl="https://api.example.invalid/";
#endif
}

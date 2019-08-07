namespace Assets.RTSCore.Data
{
    public interface IBackwardCompatibilitySerializer
    {
        void OnUnknownElementFound(string uknownName, string value);
    }
}

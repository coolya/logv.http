namespace logv.core.PlugInModel
{
    public interface IPlugIn
    {
        void Register(IApplication app);
        void UnRegister(IApplication app);
    }
}
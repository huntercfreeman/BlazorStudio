namespace BlazorStudio.ClassLib.CommonComponents;

public interface ICommonComponentRenderers
{
    public Type InputFileRendererType { get; }
    public Type InformativeNotificationRendererType { get; }
    public Type ErrorNotificationRendererType { get; }
    public Type FileFormRendererType { get; set; }
}

public interface IInputFileRendererType
{
    
}

public interface IInformativeNotificationRendererType
{
    public string Message { get; set; }
}

public interface IErrorNotificationRendererType
{
    
}

public interface IFileFormRendererType
{
    
}
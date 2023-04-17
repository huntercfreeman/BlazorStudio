namespace BlazorStudio.ClassLib.Parsing.C;

public class EvaluatorResult
{
    public EvaluatorResult(
        Type resultType,
        object resultValue)
    {
        ResultType = resultType;
        ResultValue = resultValue;
    }

    public Type ResultType { get; }
    public object ResultValue { get; }
}
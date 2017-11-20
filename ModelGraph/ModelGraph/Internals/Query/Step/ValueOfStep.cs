
namespace ModelGraph.Internals
{
    internal abstract class ValueOfStep<T> : EvaluateStep
    {
        abstract protected T GetVal();
    }
}

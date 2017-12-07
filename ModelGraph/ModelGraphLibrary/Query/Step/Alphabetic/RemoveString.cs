using System.Text;

namespace ModelGraphSTD
{
    internal class RemoveString : ValueOfString
    {
        internal RemoveString(ComputeStep step) { _step = step; }
        internal override string Text => " - ";

        protected override string GetVal()
        {
            var N = _step.Count;
            var val = _step.Input[0].Evaluate.AsString();
            for (int i = 1; i < N; i++)
            {
                val = val.Replace(_step.Input[i].Evaluate.AsString(), string.Empty);
            }
            return val;
        }
    }
}

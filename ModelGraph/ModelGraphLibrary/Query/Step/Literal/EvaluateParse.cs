using System.Text;

namespace ModelGraphLibrary
{
    /// <summary>
    /// The 
    /// </summary>
    internal class EvaluateParse : EvaluateBase
    {
        private string _text;
        internal int Index1;
        internal int Index2;
        internal EvaluateParse(string text)
        {
            _text = string.IsNullOrWhiteSpace(text) ? string.Empty : text;
            Index1 = Index2 = 0;
        }

        #region Head / Tail  ==================================================
        internal bool IsEmpty => string.IsNullOrEmpty(_text);
        internal bool CanGetHead => Index1 < _text.Length;
        internal bool CanGetTail => Index2 < _text.Length;
        internal char HeadChar => _text[Index1];
        internal char TailChar => _text[Index2];
        internal string HeadToTailString => _text.Substring(Index1, Index2 - Index1);

        internal void AdvanceHead() => Index1++;
        internal void AdvanceTail() => Index2++;
        internal void DecrementTail() => Index2--;
        internal void SetTail() => Index2 = Index1 + 1;
        internal void SetNextHead() => Index1 = Index2;
        internal void SetNextHeadTail() => Index1 = Index2 = (Index1 + 1);

        #endregion

        #region HasUnbalancedQuotes  ==========================================
        internal bool HasUnbalancedQuotes()
        {
            var last = -1;
            var isOn = false;
            for (int i = 0; i < _text.Length; i++)
            {
                var c = _text[i];
                if (c == '"')
                {
                    last = i;
                    isOn = !isOn;
                }
            }
            Index1 = last;
            Index2 = last + 1;

            if (isOn) return true; // has error

            Index1 = Index2 = 0; // no error, reset indices
            return false;
        }
        #endregion

        #region HasUnbancedParens  ============================================
        internal bool HasUnbancedParens()
        {
            var count = 0;
            var first = -1;
            var isOn = false;
            for (int i = 0; i < _text.Length; i++)
            {
                var c = _text[i];
                if (c == '"') isOn = !isOn;
                if (isOn) continue;

                if (c == '(')
                {
                    if (count == 0) first = i;
                    count++;
                }
                if (c == ')')
                {
                    if (first < 0) first = i;
                    count--;
                }
            }
            Index1 = (first >= 0) ? first : 0;
            Index2 = _text.Length;

            if (count != 0) return true; // has error

            Index1 = Index2 = 0; // no error, reset indices
            return false;
        }

        #endregion


        //=====================================================================

        internal override ValueType ValueType => ValueType.IsParseError;
        internal override string Text => _text;

        #region EvaluateBase-v1 (Specialized)  ================================
        internal override void GetValue(ComputeStep step, out bool value) => value = Value.ToBool(false);
        internal override void GetValue(ComputeStep step, out byte value) => value = Value.ToByte(0);
        internal override void GetValue(ComputeStep step, out int value) => value = Value.ToInt32(0);
        internal override void GetValue(ComputeStep step, out long value) => value = Value.ToInt64(0);
        internal override void GetValue(ComputeStep step, out short value) => value = Value.ToInt16(0);
        internal override void GetValue(ComputeStep step, out sbyte value) => value = Value.ToSByte(0);
        internal override void GetValue(ComputeStep step, out uint value) => value = Value.ToUInt32(0);
        internal override void GetValue(ComputeStep step, out ulong value) => value = Value.ToUInt64(0);
        internal override void GetValue(ComputeStep step, out ushort value) => value = Value.ToUInt16(0);
        internal override void GetValue(ComputeStep step, out double value) => value = Value.ToDouble(0);
        internal override void GetValue(ComputeStep step, out string value) => value = Value.ToString(Text);
        #endregion
    }
}

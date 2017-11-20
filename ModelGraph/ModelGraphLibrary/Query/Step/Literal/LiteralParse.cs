using System;
using System.Text;

namespace ModelGraph.Internals
{
    /// <summary>
    /// The 
    /// </summary>
    internal class LiteralParse : EvaluateStep
    {
        private string _text;
        internal int Index1;
        internal int Index2;
        internal LiteralParse(string text)
        {
            _text = string.IsNullOrWhiteSpace(text) ? string.Empty : text;
            Index1 = Index2 = 0;
        }
        internal override ValType ValueType => ValType.IsInvalid;
        internal override string Text => _text;

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

        #region AsValue  ======================================================
        internal override bool AsBool() => throw new NotImplementedException();
        internal override long AsLong() => throw new NotImplementedException();
        internal override double AsDouble() => throw new NotImplementedException();
        internal override string AsString() => throw new NotImplementedException();
        internal override DateTime AsDateTime() => throw new NotImplementedException();
        #endregion
    }
}

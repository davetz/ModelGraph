namespace ModelGraphLibrary
{/*
    Expression trees are used in query where and select clauses.
    They compute values and they qualify specific relational paths.
 */
    /// <summary>
    /// Expression tree step
    /// </summary>
    public abstract class Step
    {
        internal Step[] Inputs;      // a step may have zero or more inputs
        internal NativeType InType;  // expected input native data type
        internal NativeType OutType; // output native data type
        private StepFlag _stepFlags;

        #region Flags  ========================================================
        bool GetFlag(StepFlag flag) => (_stepFlags & flag) != 0;
        void SetFlag(bool val, StepFlag flag) { if (val) _stepFlags |= flag; else _stepFlags &= ~flag; }

        public bool IsWierd { get { return GetFlag(StepFlag.IsWierd); } set { SetFlag(value, StepFlag.IsWierd); } }
        public bool HasParens { get { return GetFlag(StepFlag.HasParens); } set { SetFlag(value, StepFlag.HasParens); } }
        public bool HasNewLine { get { return GetFlag(StepFlag.HasNewLine); } set { SetFlag(value, StepFlag.HasNewLine); } }
        #endregion

        #region Methods  ======================================================
        internal int Count => (Inputs == null) ? 0 : Inputs.Length;

        internal abstract void GetValue(out bool value);
        internal abstract void GetValue(out byte value);
        internal abstract void GetValue(out int value);
        internal abstract void GetValue(out short value);
        internal abstract void GetValue(out long value);
        internal abstract void GetValue(out double value);
        internal abstract void GetValue(out string value);
        #endregion
    }
}

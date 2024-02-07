namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 加算演算子項目 を提供します。
    /// </summary>
    public sealed class PlusItem : OperatorItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public PlusItem() : base("+") { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public PlusItem(string displayValue) : base("+", displayValue) { }
    }
}

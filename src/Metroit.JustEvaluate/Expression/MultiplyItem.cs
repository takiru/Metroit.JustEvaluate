namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 積算演算子項目を提供します。
    /// </summary>
    public sealed class MultiplyItem : OperatorItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public MultiplyItem() : base("*") { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public MultiplyItem(string displayValue) : base("*", displayValue) { }
    }
}

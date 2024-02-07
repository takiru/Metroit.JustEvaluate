namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 減算演算子項目を提供します。
    /// </summary>
    public sealed class MinusItem : OperatorItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public MinusItem() : base("-") { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public MinusItem(string displayValue) : base("-", displayValue) { }
    }
}

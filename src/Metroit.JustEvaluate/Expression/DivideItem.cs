namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 除算演算子項目を提供します。
    /// </summary>
    public sealed class DivideItem : OperatorItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public DivideItem() : base("/") { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public DivideItem(string displayValue) : base("/", displayValue) { }
    }
}

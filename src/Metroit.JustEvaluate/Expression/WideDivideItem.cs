namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 表示文字列が全角の除算演算子項目を提供します。
    /// </summary>
    public sealed class WideDivideItem : WideOperatorItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public WideDivideItem() : base("/", "÷") { }
    }
}

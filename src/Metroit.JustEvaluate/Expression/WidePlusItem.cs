namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 表示文字列が全角の加算演算子項目 を提供します。
    /// </summary>
    public sealed class WidePlusItem : WideOperatorItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public WidePlusItem() : base("+", "＋") { }
    }
}

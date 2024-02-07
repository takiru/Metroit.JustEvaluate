namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 表示文字列が全角の減算演算子項目を提供します。
    /// </summary>
    public sealed class WideMinusItem : WideOperatorItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public WideMinusItem() : base("-", "－") { }
    }
}

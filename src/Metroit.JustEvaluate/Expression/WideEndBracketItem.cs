namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 表示文字列が全角の終了ブラケット項目を提供します。
    /// </summary>
    public sealed class WideEndBracketItem : WideBracketItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public WideEndBracketItem() : base(")", "）") { }
    }
}

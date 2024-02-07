namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 表示文字列が全角の開始ブラケット項目を提供します。
    /// </summary>
    public sealed class WideStartBracketItem : WideBracketItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public WideStartBracketItem() : base("(", "（") { }
    }
}

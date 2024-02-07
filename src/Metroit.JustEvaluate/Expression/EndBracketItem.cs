namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 終了ブラケット項目を提供します。
    /// </summary>
    public sealed class EndBracketItem : BracketItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public EndBracketItem() : base(")") { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public EndBracketItem(string displayValue) : base(")", displayValue) { }
    }
}

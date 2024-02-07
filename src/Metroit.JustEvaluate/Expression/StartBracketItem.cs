namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 開始ブラケット項目を提供します。
    /// </summary>
    public sealed class StartBracketItem : BracketItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public StartBracketItem() : base("(") { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public StartBracketItem(string displayValue) : base("(", displayValue) { }
    }
}

using System.ComponentModel;

namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 表示文字列が全角のブラケット項目を提供します。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class WideBracketItemBase : BracketItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="formulaElement">計算式要素。</param>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public WideBracketItemBase(string formulaElement, string displayValue) : base(formulaElement, displayValue) { }
    }
}

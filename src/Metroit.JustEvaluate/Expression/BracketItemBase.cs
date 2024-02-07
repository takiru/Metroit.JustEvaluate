using System.ComponentModel;

namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// ブラケット項目を提供します。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BracketItemBase : CalcItemBase
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="formulaElement">計算式要素。</param>
        public BracketItemBase(string formulaElement) : base(formulaElement) { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="formulaElement">計算式要素。</param>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public BracketItemBase(string formulaElement, string displayValue) : base(formulaElement, displayValue) { }
    }
}

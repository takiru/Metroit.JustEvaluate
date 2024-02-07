using Metroit.JustEvaluate.Properties;
using System;

namespace Metroit.JustEvaluate
{
    /// <summary>
    /// 計算式のルールに反した計算式要素の追加に失敗した例外を提供します。
    /// </summary>
    public class FormulaRulesException : Exception
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public FormulaRulesException() : base(Resources.FormulaAgainstRules)
        {

        }
    }
}

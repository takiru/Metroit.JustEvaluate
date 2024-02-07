using Metroit.JustEvaluate.Properties;
using System;

namespace Metroit.JustEvaluate
{
    /// <summary>
    /// 計算式の評価に失敗した例外を提供します。
    /// </summary>
    public class CalculationEvaluateFailedException : Exception
    {
        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="innerException">内部例外オブジェクト。</param>
        public CalculationEvaluateFailedException(Exception innerException) : base(Resources.CalculationEvaluateFailed, innerException)
        {

        }
    }
}

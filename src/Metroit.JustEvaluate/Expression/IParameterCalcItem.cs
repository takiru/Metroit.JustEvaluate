namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 埋込パラメーター項目の基本インターフェースを提供します。
    /// </summary>
    public interface IParameterCalcItem : ICalcItem
    {
        /// <summary>
        /// パラメーター名 を取得します。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 表示パラメーター名 を取得します。
        /// </summary>
        string DisplayName { get; }
    }
}

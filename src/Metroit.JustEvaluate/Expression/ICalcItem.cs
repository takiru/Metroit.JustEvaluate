using Newtonsoft.Json;

namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 計算式要素項目の基本インタフェースを提供します。
    /// </summary>
    [JsonObject]
    public interface ICalcItem
    {
        /// <summary>
        /// 計算式要素の実際のオブジェクトのタイプを取得します。
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        string Type { get; }

        /// <summary>
        /// 計算式要素を取得します。
        /// </summary>
        [JsonIgnore]
        string FormulaElement { get; }

        /// <summary>
        /// 計算式要素の表示文字列を取得します。
        /// </summary>
        [JsonIgnore]
        string DisplayValue { get; }
    }
}

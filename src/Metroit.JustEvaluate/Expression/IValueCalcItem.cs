using Newtonsoft.Json;

namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 数値項目の基本インターフェースを提供します。
    /// </summary>
    [JsonObject]
    public interface IValueCalcItem : ICalcItem
    {
        /// <summary>
        /// 値 を取得します。
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        decimal Value { get; set; }
    }
}

using Newtonsoft.Json;

namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 埋込パラメーター項目を提供します。
    /// JSONにシリアライズ/デシリアライズする時、パラメーター名/表示名を必要としません。
    /// </summary>
    [JsonObject]
    public abstract class SlimParameterItemBase : CalcItemBase, IParameterCalcItem
    {
        /// <summary>
        /// 埋込パラメーター名を取得します。
        /// </summary>
        [JsonIgnore]
        public string Name { get; }

        /// <summary>
        /// 埋込パラメーターの表示文字列を取得します。
        /// </summary>
        [JsonIgnore]
        public string DisplayName { get; }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="name">埋込パラメーター名。</param>
        /// <param name="displayName">埋込パラメーターの表示文字列。</param>
        public SlimParameterItemBase(string name, string displayName) : base(name, displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}

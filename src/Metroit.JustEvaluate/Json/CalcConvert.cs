using Newtonsoft.Json;
using System.Linq;

namespace Metroit.JustEvaluate.Json
{
    /// <summary>
    /// 計算式を提供します。
    /// </summary>
    [JsonObject]
    public sealed class CalcConvert
    {
        /// <summary>
        /// 現在の計算式 をシリアライズします。
        /// </summary>
        /// <returns>シリアライズされた文字列。</returns>
        public static string Serialize(CalcExpression expression)
        {
            if (expression.Items.Count() == 0)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(expression);
        }

        /// <summary>
        /// シリアライズされた文字列から計算式を返却します。
        /// </summary>
        /// <param name="value">シリアライズされた文字列。</param>
        /// <returns>計算式。</returns>
        public static CalcExpression Deserialize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new CalcExpression();
            }

            return JsonConvert.DeserializeObject<CalcExpression>(value, new CalcItemConverter());
        }
    }
}

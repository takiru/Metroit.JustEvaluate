using Metroit.JustEvaluate.Expression;
using Metroit.JustEvaluate.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace Metroit.JustEvaluate.Json
{
    /// <summary>
    /// 計算式JSONをデシリアライズするためのコンバーターを提供します。
    /// </summary>
    internal class CalcItemConverter : JsonConverter
    {
        /// <summary>
        /// CalcExpression へのデシリアライズのみ読み込み可能である。
        /// </summary>
        /// <param name="objectType">デシリアライズ対象となる型。</param>
        /// <returns>true:デシリアライズ可能, false:デシリアライズ不可能。</returns>
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(CalcExpression))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// デシリアライズして CalcExpression を求めます。
        /// </summary>
        /// <param name="reader">JsonReader。</param>
        /// <param name="objectType">Type。</param>
        /// <param name="existingValue">object。</param>
        /// <param name="serializer">serializer。</param>
        /// <returns>デシリアライズされた CalcExpression オブジェクト。</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new CalcExpression();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
                if (reader.TokenType != JsonToken.StartArray)
                {
                    continue;
                }

                var items = JArray.Load(reader);

                foreach (var item in items)
                {
                    var typeName = item.SelectToken("Type")?.ToObject<string>();
                    if (typeName == null)
                    {
                        throw new JsonReaderException(Resources.TypeLoadFailed);
                    }

                    var type = Type.GetType(typeName);
                    if (type == null)
                    {
                        // クラス名のみの指定の場合、自身のアセンブリ内に存在するタイプと合致するものとする
                        var selfTypeName = Assembly.GetExecutingAssembly().ExportedTypes.Where(x => x.Name == typeName).FirstOrDefault()?.FullName;
                        if (selfTypeName == null)
                        {
                            throw new JsonReaderException(string.Format(Resources.DeserializationFailed, typeName));
                        }
                        type = Type.GetType(selfTypeName);
                    }

                    var calcItem = JsonConvert.DeserializeObject(item.ToString(), type) as ICalcItem;
                    if (calcItem == null)
                    {
                        throw new JsonReaderException(string.Format(Resources.DeserializationFailed, typeName));
                    }

                    result.Add(calcItem);
                }
            }

            return result;
        }

        /// <summary>
        /// シリアライズには利用しないため、記述なし。
        /// </summary>
        /// <param name="writer">JsonWriter。</param>
        /// <param name="value">object。</param>
        /// <param name="serializer">JsonSerializer。</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            return;
        }
    }
}

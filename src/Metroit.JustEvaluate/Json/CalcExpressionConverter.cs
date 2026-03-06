using Metroit.JustEvaluate.Expression;
using Metroit.JustEvaluate.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Metroit.JustEvaluate.Json
{
    /// <summary>
    /// 計算式JSONをデシリアライズするためのコンバーターを提供します。
    /// </summary>
    internal class CalcExpressionConverter : JsonConverter
    {
        /// <summary>
        /// 自動認識タイプ。
        /// </summary>
        private readonly IEnumerable<Type> _automaticRecognitionTypes = new Type[] { };

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public CalcExpressionConverter() { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="automaticRecognitionTypes">シリアライズ対象としたCalcExpressionに含まれる自動認識タイプ。</param>
        public CalcExpressionConverter(IEnumerable<Type> automaticRecognitionTypes)
        {
            _automaticRecognitionTypes = automaticRecognitionTypes;
        }

        /// <summary>
        /// CalcExpression へのシリアライズ／デシリアライズのみ読み込み可能である。
        /// </summary>
        /// <param name="objectType">シリアライズ／デシリアライズ対象となる型。</param>
        /// <returns>true:シリアライズ／デシリアライズ可能, false:シリアライズ／デシリアライズ不可能。</returns>
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(CalcExpression))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// CalcExpression をシリアライズします。
        /// </summary>
        /// <param name="writer">JsonWriter。</param>
        /// <param name="value">CalcExpression。</param>
        /// <param name="serializer">JsonSerializer。</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var expression = (CalcExpression)value;
            var executingAssembly = Assembly.GetExecutingAssembly();

            var obj = new JObject();
            var items = new JArray();

            foreach (var item in expression.Items)
            {
                var itemObj = new JObject()
                {
                    ["Type"] = GetTypeName(executingAssembly, expression.AutomaticRecognitionTypes,
                                expression.HandleAssemblyQualifiedName, item)
                };

                // Type以外のプロパティをシリアライズ
                var itemToken = JObject.FromObject(item, serializer);
                foreach (var prop in itemToken.Properties())
                {
                    if (prop.Name != "Type")
                    {
                        itemObj[prop.Name] = prop.Value;
                    }
                }

                items.Add(itemObj);
            }

            obj["Items"] = items;
            obj.WriteTo(writer);
        }

        /// <summary>
        /// シリアライズするときに、計算式要素項目のタイプ名を求めます。
        /// </summary>
        /// <param name="executingAssembly">当DLLの実行アセンブリ。</param>
        /// <param name="automaticRecognitionTypes">シリアライズ対象としたCalcExpressionに含まれる自動認識タイプ。</param>
        /// <param name="handleAssemblyQualifiedName">自動認識タイプを完全アセンブリ修飾名としてシリアライズするかどうか。</param>
        /// <param name="item">シリアライズしようとしてる計算式要素項目。</param>
        /// <returns>シリアライズしたタイプ名。</returns>
        private string GetTypeName(Assembly executingAssembly, IEnumerable<Type> automaticRecognitionTypes,
            bool handleAssemblyQualifiedName, ICalcItem item)
        {
            var type = item.GetType();
            if (type.Assembly == executingAssembly)
            {
                return type.Name;
            }

            // AutomaticRecognitionTypes に含まれるクラス
            if (automaticRecognitionTypes.Any(x => x == type))
            {
                if (!handleAssemblyQualifiedName)
                {
                    return type.Name;
                }
            }

            // 異なるアセンブリに含まれるクラスの場合は完全修飾名にアセンブリ名も付与した形とする
            var index = type.AssemblyQualifiedName.IndexOf(",", type.AssemblyQualifiedName.IndexOf(",") + 1);
            return type.AssemblyQualifiedName.Substring(0, index).Trim();
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
            var executingAssembly = Assembly.GetExecutingAssembly();
            var result = new CalcExpression(_automaticRecognitionTypes);

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
                    if (string.IsNullOrEmpty(typeName))
                    {
                        throw new JsonReaderException(Resources.TypeLoadFailed);
                    }

                    var type = GetType(executingAssembly, _automaticRecognitionTypes, typeName);

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
        /// デシリアライズするときに、タイプ名から計算式要素項目のタイプを求めます。
        /// </summary>
        /// <param name="executingAssembly">当DLLの実行アセンブリ。</param>
        /// <param name="automaticRecognitionTypes">自動認識タイプ。</param>
        /// <param name="typeName">タイプ名。</param>
        /// <returns>求められたタイプ。</returns>
        /// <exception cref="JsonReaderException">タイプを見つけることができなかった。</exception>
        private Type GetType(Assembly executingAssembly, IEnumerable<Type> automaticRecognitionTypes, string typeName)
        {
            // 自身DLLのアセンブリ内に同じタイプを持つクラスを持っているときはそのタイプとする
            var selfTypeName = executingAssembly.ExportedTypes
                .Where(x => x.Name == typeName)
                .FirstOrDefault()
                ?.FullName;
            if (selfTypeName != null)
            {
                return Type.GetType(selfTypeName);
            }

            // 完全修飾名によって解決できるタイプならそのタイプとする
            // NOTE: 完全修飾名は Metroit.JustEvaluate.Test.Hoge, Metroit.JustEvaluate.Test という形式。
            var type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            // 自動認識タイプに含まれるタイプ名に合致したらそのタイプとする
            var maybeType = automaticRecognitionTypes.FirstOrDefault(x => x.Name == typeName);
            if (maybeType != null)
            {
                return maybeType;
            }

            throw new JsonReaderException(string.Format(Resources.DeserializationFailed, typeName));
        }
    }
}

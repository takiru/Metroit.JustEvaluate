using System.Reflection;

namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 数値項目を提供します。
    /// </summary>
    public class ValueItem : IValueCalcItem
    {
        /// <summary>
        /// 計算アイテムの実際のオブジェクトのタイプを取得します。
        /// </summary>
        public string Type
        {
            get
            {
                var type = GetType();

                // 同じアセンブリ内クラスの場合はクラス名のみ
                if (type.Assembly == Assembly.GetExecutingAssembly())
                {
                    return type.Name;
                }

                // 異なるアセンブリ内クラスの場合は完全修飾名にアセンブリ名も付与した形を得る
                var index = type.AssemblyQualifiedName.IndexOf(",", type.AssemblyQualifiedName.IndexOf(",") + 1);

                return type.AssemblyQualifiedName.Substring(0, index).Trim();
            }
        }

        /// <summary>
        /// 計算式要素を取得します。
        /// </summary>
        public string FormulaElement => Value.ToString();

        /// <summary>
        /// 計算式要素の表示文字列を取得します。
        /// </summary>
        public virtual string DisplayValue => Value.ToString();

        /// <summary>
        /// 値 を取得または設定します。
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="value">数値。</param>
        public ValueItem(decimal value)
        {
            Value = value;
        }
    }
}

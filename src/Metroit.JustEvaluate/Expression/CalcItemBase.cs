using System.ComponentModel;
using System.Reflection;

namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 計算式要素項目の基本動作を提供します。
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class CalcItemBase : ICalcItem
    {
        /// <summary>
        /// 計算式要素の実際のオブジェクトのタイプを取得します。
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
        public string FormulaElement { get; }

        /// <summary>
        /// 計算式要素の表示文字列を取得します。
        /// </summary>
        public string DisplayValue { get; }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="formulaElement">計算式要素。</param>
        public CalcItemBase(string formulaElement)
        {
            FormulaElement = formulaElement;
            DisplayValue = formulaElement;
        }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="formulaElement">計算式要素。</param>
        /// <param name="displayValue">計算式要素の表示文字列。</param>
        public CalcItemBase(string formulaElement, string displayValue)
        {
            FormulaElement = formulaElement;
            DisplayValue = displayValue;
        }
    }
}

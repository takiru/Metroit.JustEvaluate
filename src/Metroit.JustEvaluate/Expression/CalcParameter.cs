namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 計算式の評価を行うための埋込パラメーター情報を提供します。
    /// </summary>
    public class CalcParameter
    {
        /// <summary>
        /// 埋込パラメーター名を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 埋込パラメーターで利用する数値を取得または設定します。
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="name">埋込パラメーター名。</param>
        internal CalcParameter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="name">埋込パラメーター名。</param>
        /// <param name="value">利用する数値</param>
        internal CalcParameter(string name, decimal value) : this(name)
        {
            Value = value;
        }
    }
}

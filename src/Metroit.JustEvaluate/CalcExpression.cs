using JustEvaluate;
using Metroit.JustEvaluate.Expression;
using Metroit.JustEvaluate.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Metroit.JustEvaluate
{
    /// <summary>
    /// 計算式を提供します。
    /// </summary>
    [JsonObject]
    public sealed class CalcExpression : INotifyPropertyChanged
    {
        /// <summary>
        /// 変更の通知が行われた時に発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 数式検証に利用するテスト値。
        /// </summary>
        private static readonly decimal TestValue = 1;

        /// <summary>
        /// 計算式リストを取得または設定します。
        /// </summary>
        [JsonProperty("Items", Required = Required.Always)]
        private List<ICalcItem> calcItems { get; } = new List<ICalcItem>();

        /// <summary>
        /// 式のアイテムを取得します。
        /// </summary>
        [JsonIgnore]
        public IEnumerable<ICalcItem> Items => calcItems;

        private Dictionary<string, CalcParameter> parameters { get; } = new Dictionary<string, CalcParameter>();

        /// <summary>
        /// パラメーター値を取得します。
        /// </summary>
        [JsonIgnore]
        public ReadOnlyDictionary<string, CalcParameter> Parameters => new ReadOnlyDictionary<string, CalcParameter>(parameters);

        private IEnumerable<Type> _automaticRecognitionTypes = new Type[] { };

        /// <summary>
        /// 自動認識タイプを取得または設定します。
        /// </summary>
        [JsonIgnore]
        public IEnumerable<Type> AutomaticRecognitionTypes
        {
            get => _automaticRecognitionTypes;
            set
            {
                _automaticRecognitionTypes = value ?? throw new ArgumentNullException(nameof(AutomaticRecognitionTypes));
            }
        }

        /// <summary>
        /// 自動認識タイプを完全アセンブリ修飾名として扱うかを取得または設定します。
        /// </summary>
        [JsonIgnore]
        public bool HandleAssemblyQualifiedName { get; set; } = false;

        /// <summary>
        /// 計算式の表示文字列 を取得します。
        /// </summary>
        [JsonIgnore]
        public string DisplayFormula
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var calcItem in calcItems)
                {
                    sb.Append(calcItem.DisplayValue);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 計算式の文字列 を取得します。
        /// </summary>
        [JsonIgnore]
        public string Formula
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var calcItem in calcItems)
                {
                    sb.Append(calcItem.FormulaElement);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        public CalcExpression() { }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="automaticRecognitionTypes">自動認識タイプ。</param>
        public CalcExpression(IEnumerable<Type> automaticRecognitionTypes)
        {
            AutomaticRecognitionTypes = automaticRecognitionTypes;
        }

        /// <summary>
        /// 計算式を有するかどうかを取得します。
        /// </summary>
        /// <returns>true:有する, false:有しない。</returns>
        public bool HasExpression()
        {
            return calcItems.Count > 0;
        }

        /// <summary>
        /// 計算式の要素を末尾に追加します。
        /// </summary>
        /// <param name="item">計算式の要素。</param>
        public void Add(ICalcItem item)
        {
            AddWithNormalizeExpression(item);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayFormula)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Formula)));
        }

        /// <summary>
        /// 指定したコレクションの要素を末尾に追加します。
        /// </summary>
        /// <param name="items">計算式の要素。</param>
        public void AddRange(IEnumerable<ICalcItem> items)
        {
            // 要素反映前にエラーになることなく追加可能か検証する
            var checkExpresion = CalcConvert.Deserialize(CalcConvert.Serialize(this));
            foreach (var item in items)
            {
                AddWithNormalizeExpression(checkExpresion, item);
            }

            foreach (var item in items)
            {
                calcItems.Add(item);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayFormula)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Formula)));
        }

        /// <summary>
        /// 直近の計算式をクリアします。
        /// </summary>
        public void RemoveRecentExpression()
        {
            if (calcItems.Count < 1)
            {
                return;
            }

            var removeItem = calcItems.Last();

            calcItems.Remove(removeItem);
            if (removeItem is IParameterCalcItem)
            {
                RemoveParameter(removeItem.FormulaElement);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayFormula)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Formula)));
        }

        /// <summary>
        /// 指定した削除要素数の計算式をクリアします。
        /// 削除要素数が計算式の要素数より多い時、すべての計算式の要素数を削除します。
        /// </summary>
        /// <param name="count">削除要素数。</param>
        public void RemoveExpression(int count)
        {
            if (calcItems.Count < 1)
            {
                return;
            }

            var deleteCount = calcItems.Count();
            if (deleteCount > count)
            {
                deleteCount = count;
            }

            var removeItems = new ICalcItem[deleteCount];
            calcItems.CopyTo(calcItems.Count - deleteCount, removeItems, 0, deleteCount);

            calcItems.RemoveRange(calcItems.Count() - deleteCount, deleteCount);

            foreach (var removeItem in removeItems.Where(x => x is IParameterCalcItem))
            {
                RemoveParameter(removeItem.FormulaElement);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayFormula)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Formula)));
        }

        /// <summary>
        /// 現在の計算式をクリアします。
        /// </summary>
        public void Clear()
        {
            calcItems.Clear();
            parameters.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayFormula)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Formula)));
        }

        /// <summary>
        /// 計算式に要素の追加が可能かどうかを取得する。
        /// </summary>
        /// <param name="addItem">追加する計算式の要素。</param>
        /// <returns>true:追加可能, false:追加不可能。</returns>
        public bool CanAdd(ICalcItem addItem)
        {
            if (addItem is StartBracketItem startBracketItem)
            {
                return CheckStartBracket(this, startBracketItem, false, false);
            }
            if (addItem is WideStartBracketItem wideStartBracketItem)
            {
                return CheckStartBracket(this, wideStartBracketItem, false, false);
            }
            if (addItem is EndBracketItem endBracketItem)
            {
                return CheckEndBracket(this, endBracketItem, false, false);
            }
            if (addItem is WideEndBracketItem wideEndBracketItem)
            {
                return CheckEndBracket(this, wideEndBracketItem, false, false);
            }
            if (addItem is IValueCalcItem valueCalcItem)
            {
                return CheckValue(this, valueCalcItem, false, false);
            }
            if (addItem is IParameterCalcItem parameterCalcItem)
            {
                return CheckParameter(this, parameterCalcItem, false, false);
            }
            if (addItem is OperatorItemBase operatorItem)
            {
                return CheckOperator(this, operatorItem, false, false);
            }

            return false;
        }

        /// <summary>
        /// 試験的な値を使用して計算式を評価し、現在の計算式の妥当性を検証します。
        /// </summary>
        /// <returns>true:妥当, false:不当。</returns>
        /// <remarks>
        /// ゼロ除算は妥当とみなします。
        /// そのため、<see cref="Evaluate()"/>ではゼロ除算による<see cref="CalculationEvaluateFailedException"/>が発生する可能性があります。
        /// </remarks>
        public bool Validate()
        {
            var parameters = new List<CalcParameter>();

            foreach (var parameter in Parameters)
            {
                parameters.Add(new CalcParameter(parameter.Value.Name, TestValue));
            }

            try
            {
                Evaluate(parameters.ToArray());

                return true;
            }
            catch (CalculationEvaluateFailedException e)
            {
                if (e.InnerException is DivideByZeroException)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 現在の計算式を実行します。
        /// </summary>
        /// <returns>計算結果。</returns>
        public decimal Evaluate()
        {
            return Evaluate(Parameters.Values.ToArray());
        }

        /// <summary>
        /// 計算式の要素を追加し、その時点までの計算式を正常化する。
        /// </summary>
        /// <param name="addItem">追加する計算式の要素。</param>
        private void AddWithNormalizeExpression(ICalcItem addItem)
        {
            AddWithNormalizeExpression(this, addItem);
        }

        /// <summary>
        /// 計算式の要素を追加し、その時点までの計算式を正常化する。
        /// </summary>
        /// <param name="maybeExpression">元の計算式。</param>
        /// <param name="addItem">追加する計算式の要素。</param>
        private void AddWithNormalizeExpression(CalcExpression maybeExpression, ICalcItem addItem)
        {
            if (addItem is StartBracketItem startBracketItem)
            {
                CheckStartBracket(maybeExpression, startBracketItem, true, true);
            }
            if (addItem is WideStartBracketItem wideStartBracketItem)
            {
                CheckStartBracket(maybeExpression, wideStartBracketItem, true, true);
            }
            if (addItem is EndBracketItem endBracketItem)
            {
                CheckEndBracket(maybeExpression, endBracketItem, true, true);
            }
            if (addItem is WideEndBracketItem wideEndBracketItem)
            {
                CheckEndBracket(maybeExpression, wideEndBracketItem, true, true);
            }
            if (addItem is IValueCalcItem valueCalcItem)
            {
                CheckValue(maybeExpression, valueCalcItem, true, true);
            }
            if (addItem is IParameterCalcItem parameterCalcItem)
            {
                CheckParameter(maybeExpression, parameterCalcItem, true, true);
            }
            if (addItem is OperatorItemBase operatorItem)
            {
                CheckOperator(maybeExpression, operatorItem, true, true);
            }
        }

        /// <summary>
        /// 計算式に追加される開始ブラケットをチェックする。
        /// </summary>
        /// <param name="maybeExpression">元の計算式。</param>
        /// <param name="addItem">開始ブラケット。</param>
        /// <param name="addAndNormalize">計算式を追加してノーマライズするかどうか。</param>
        /// <param name="ifRaiseCannotAdd">追加できない時に例外とするかどうか。</param>
        /// <returns>true:妥当, false:不当。</returns>
        private bool CheckStartBracket(CalcExpression maybeExpression, BracketItemBase addItem, bool addAndNormalize, bool ifRaiseCannotAdd)
        {
            // 初回投入は許可
            if (maybeExpression.calcItems.Count == 0)
            {
                if (addAndNormalize)
                {
                    maybeExpression.calcItems.Add(addItem);
                }
                return true;
            }

            var recentItem = maybeExpression.calcItems.Last();

            // 直前が開始ブラケットまたは演算子でないの場合は許可しない
            // NOTE: value, (value)
            if (!(recentItem is StartBracketItem || recentItem is WideStartBracketItem || recentItem is OperatorItemBase))
            {
                if (ifRaiseCannotAdd)
                {
                    throw new FormulaRulesException();
                }
                return false;
            }

            if (addAndNormalize)
            {
                maybeExpression.calcItems.Add(addItem);
            }
            return true;
        }

        /// <summary>
        /// 計算式に追加される終了ブラケットをチェックする。
        /// </summary>
        /// <param name="maybeExpression">元の計算式。</param>
        /// <param name="addItem">終了ブラケット。</param>
        /// <param name="addAndNormalize">計算式を追加してノーマライズするかどうか。</param>
        /// <param name="ifRaiseCannotAdd">追加できない時に例外とするかどうか。</param>
        /// <returns>true:妥当, false:不当。</returns>
        private bool CheckEndBracket(CalcExpression maybeExpression, BracketItemBase addItem, bool addAndNormalize, bool ifRaiseCannotAdd)
        {
            // 初回投入は許可しない
            if (maybeExpression.calcItems.Count == 0)
            {
                if (ifRaiseCannotAdd)
                {
                    throw new FormulaRulesException();
                }
                return false;
            }

            var recentItem = maybeExpression.calcItems.Last();

            // 直前が開始ブラケットまたは演算子の場合は許可しない
            // NOTE: (, (value+
            if (recentItem is StartBracketItem || recentItem is WideStartBracketItem || recentItem is OperatorItemBase)
            {
                if (ifRaiseCannotAdd)
                {
                    throw new FormulaRulesException();
                }
                return false;
            }

            // 開始ブラケットの数 = 終了ブラケットの数の場合は許可しない
            // NOTE: (value+value)
            var startBracketCount = maybeExpression.calcItems
                .Where(x => x is StartBracketItem || x is WideStartBracketItem)
                .Count();
            var endBrackeCount = maybeExpression.calcItems
                .Where(x => x is EndBracketItem || x is WideEndBracketItem)
                .Count();

            if (startBracketCount == endBrackeCount)
            {
                if (ifRaiseCannotAdd)
                {
                    throw new FormulaRulesException();
                }
                return false;
            }

            if (addAndNormalize)
            {
                maybeExpression.calcItems.Add(addItem);
            }

            return true;
        }

        /// <summary>
        /// 計算式に追加される数値をチェックする。
        /// </summary>
        /// <param name="maybeExpression">元の計算式。</param>
        /// <param name="addItem">値。</param>
        /// <param name="addAndNormalize">計算式を追加してノーマライズするかどうか。</param>
        /// <param name="ifRaiseCannotAdd">追加できない時に例外とするかどうか。</param>
        /// <returns>true:妥当, false:不当。</returns>
        private bool CheckValue(CalcExpression maybeExpression, IValueCalcItem addItem, bool addAndNormalize, bool ifRaiseCannotAdd)
        {
            // 初回投入は許可
            if (maybeExpression.calcItems.Count == 0)
            {
                if (addAndNormalize)
                {
                    maybeExpression.calcItems.Add(addItem);
                }
                return true;
            }

            var recentItem = maybeExpression.calcItems.Last();

            // 直前が終了ブラケットの場合は許可しない
            // NOTE: (value)
            if (recentItem is EndBracketItem || recentItem is WideEndBracketItem)
            {
                if (ifRaiseCannotAdd)
                {
                    throw new FormulaRulesException();
                }
                return false;
            }

            if (addAndNormalize)
            {
                // 直前が値またはパラメーターの場合は差し替える
                if (recentItem is IValueCalcItem || recentItem is IParameterCalcItem)
                {
                    maybeExpression.calcItems.Remove(recentItem);
                }
                maybeExpression.calcItems.Add(addItem);
            }

            return true;
        }

        /// <summary>
        /// 計算式に追加されるパラメーターをチェックする。
        /// </summary>
        /// <param name="maybeExpression">元の計算式。</param>
        /// <param name="addItem">パラメーター。</param>
        /// <param name="addAndNormalize">計算式を追加してノーマライズするかどうか。</param>
        /// <param name="ifRaiseCannotAdd">追加できない時に例外とするかどうか。</param>
        /// <returns>true:妥当, false:不当。</returns>
        private bool CheckParameter(CalcExpression maybeExpression, IParameterCalcItem addItem, bool addAndNormalize, bool ifRaiseCannotAdd)
        {
            // 初回投入は許可
            if (maybeExpression.calcItems.Count == 0)
            {
                if (addAndNormalize)
                {
                    maybeExpression.calcItems.Add(addItem);
                    AddParameter(addItem.FormulaElement);
                }
                return true;
            }

            var recentItem = maybeExpression.calcItems.Last();

            // 直前が終了ブラケットの場合は許可しない
            // NOTE: (value)
            if (recentItem is EndBracketItem || recentItem is WideEndBracketItem)
            {
                if (ifRaiseCannotAdd)
                {
                    throw new FormulaRulesException();
                }
                return false;
            }

            if (addAndNormalize)
            {
                // 直前が値またはパラメーターの場合は差し替える
                if (recentItem is IValueCalcItem || recentItem is IParameterCalcItem)
                {
                    maybeExpression.calcItems.Remove(recentItem);
                    if (recentItem is IParameterCalcItem parameterItem)
                    {
                        RemoveParameter(parameterItem.FormulaElement);
                    }

                }
                maybeExpression.calcItems.Add(addItem);
                AddParameter(addItem.FormulaElement);
            }

            return true;
        }

        /// <summary>
        /// 新規に必要となる埋込パラメーターを追加する。
        /// </summary>
        /// <param name="formulaElement">埋込パラメーター名。</param>
        private void AddParameter(string formulaElement)
        {
            // 既に埋込パラメーターが追加済みの場合は追加しない
            if (parameters.Any(x => x.Key == formulaElement))
            {
                return;
            }

            parameters.Add(formulaElement, new CalcParameter(formulaElement));
        }

        /// <summary>
        /// 計算式に不要となる埋込パラメーターを削除する。
        /// </summary>
        /// <param name="formulaElement">埋込パラメーター名。</param>
        private void RemoveParameter(string formulaElement)
        {
            // 計算式全体から、対象の埋込パラメーターが残っている場合は削除しない            
            var parameterExists = calcItems.Any(x =>
            {
                if (!(x is IParameterCalcItem p))
                {
                    return false;
                }
                if (p.FormulaElement == formulaElement)
                {
                    return true;
                }
                return false;
            });

            if (parameterExists)
            {
                return;
            }

            var parameterName = parameters
                .Where(x => x.Key == formulaElement)
                .Select(x => x.Key)
                .FirstOrDefault();
            if (parameterName != null)
            {
                parameters.Remove(parameterName);
            }
        }


        /// <summary>
        /// 計算式に追加される演算子をチェックする。
        /// </summary>
        /// <param name="maybeExpression">元の計算式。</param>
        /// <param name="addItem">演算子。</param>
        /// <param name="addAndNormalize">計算式を追加してノーマライズするかどうか。</param>
        /// <param name="ifRaiseCannotAdd">追加できない時に例外とするかどうか。</param>
        /// <returns>true:妥当, false:不当。</returns>
        private bool CheckOperator(CalcExpression maybeExpression, OperatorItemBase addItem, bool addAndNormalize, bool ifRaiseCannotAdd)
        {
            // 初回投入は許可しない
            if (maybeExpression.calcItems.Count == 0)
            {
                if (ifRaiseCannotAdd)
                {
                    throw new FormulaRulesException();
                }
                return false;
            }

            var recentItem = maybeExpression.calcItems.Last();

            // 直前が開始ブラケットの場合は許可しない
            // NOTE: (
            if (recentItem is StartBracketItem || recentItem is WideStartBracketItem)
            {
                if (ifRaiseCannotAdd)
                {
                    throw new FormulaRulesException();
                }
                return false;
            }

            if (addAndNormalize)
            {
                // 直前が演算子の場合は差し替える
                if (recentItem is OperatorItemBase)
                {
                    maybeExpression.calcItems.Remove(recentItem);
                }
                maybeExpression.calcItems.Add(addItem);
            }

            return true;
        }

        /// <summary>
        /// 現在の計算式を指定されたプロパティセットで実行する。
        /// </summary>
        /// <param name="parameters">パラメーター情報。</param>
        /// <returns>計算結果。</returns>
        private decimal Evaluate(IEnumerable<CalcParameter> parameters)
        {
            var evaluator = new Evaluator(new Parser(), new Builder(new FunctionsRegistry()), new CompiledExpressionsCache());

            try
            {
                if (parameters.Count() > 0)
                {
                    dynamic param = CalcParameterTypeBuilder.CreateNewObject(parameters);
                    return evaluator.Evaluate(Formula, param);
                }

                return evaluator.Evaluate(Formula);

            }
            catch (FormatException e)
            {
                // 数式誤り
                throw new CalculationEvaluateFailedException(e);
            }
            catch (InvalidOperationException e)
            {
                // 変数の値がないか数式誤り
                throw new CalculationEvaluateFailedException(e);
            }
            catch (DivideByZeroException e)
            {
                // 0除算
                throw new CalculationEvaluateFailedException(e);
            }
            catch (OverflowException e)
            {
                // decimal をオーバーフローしている
                throw new CalculationEvaluateFailedException(e);
            }
            catch (Exception e)
            {
                // (, ) の組み合わせがおかしい
                throw new CalculationEvaluateFailedException(e);
            }
        }
    }
}

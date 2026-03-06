using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metroit.JustEvaluate.Expression;
using Metroit.JustEvaluate.Json;

namespace Metroit.JustEvaluate.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var calc = new CalcExpression();
            calc.Add(new DetailHeightItem());
            calc.Add(new DivideItem());
            calc.Add(new StartBracketItem());
            //calc.Add(new ParameterItem("param2", "パラメーター2"));
            calc.Add(new DetailWidthItem());
            calc.Add(new MinusItem());
            calc.Add(new ValueItem(1));
            calc.Add(new EndBracketItem());

            calc.Parameters["DetailHeightItem"].Value = 10;
            calc.Parameters["DetailWidthItem"].Value = 20;

            calc.AutomaticRecognitionTypes = new Type[] {
                typeof(DetailHeightItem),
                typeof(DetailWidthItem)
            };
            calc.HandleAssemblyQualifiedName = false;

            Console.WriteLine($"{calc.DisplayFormula}");
            Console.WriteLine($"{calc.Formula}");
            Console.WriteLine($"{calc.Validate()}");
            Console.WriteLine($"{calc.Evaluate()}");

            var serializedText = CalcConvert.Serialize(calc);
            Console.WriteLine($"{serializedText}");

            var deserializedCalc = CalcConvert.Deserialize(serializedText,
                new Type[] {
                    typeof(DetailHeightItem),
                    typeof(DetailWidthItem)
                });

            Console.ReadLine();
        }

        public class DetailHeightItem : SlimParameterItemBase
        {
            public DetailHeightItem() : base("DetailHeightItem", "作業高")
            {

            }
        }

        public class DetailWidthItem : SlimParameterItemBase
        {
            public DetailWidthItem() : base("DetailWidthItem", "作業幅")
            {

            }
        }
    }
}

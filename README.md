# Metroit.JustEvaluate
Dynamically assemble formulas and run JustEvaluate.  
Assemble expressions with instructions and execute them.  
Works with .NETStandard 2.0 or .NETStandard2.1.

[![NuGet](https://img.shields.io/badge/nuget-v0.1.0-blue.svg)](https://www.nuget.org/packages/Metroit.JustEvaluate/)

# Support operator
|operator|
|---|
| `-` `*` `+` `-` |

# Not Support
- Operator

|operator|
|---|
|`>` `<` `<=` `>=` `=` `<>` `&` `\|` |

- User-Defined Functions

# How to use
```cs
    internal class Program
    {
        static void Main(string[] args)
        {
            var c = new CalcExpression();
            c.Add(new StartBracketItem());
            c.Add(new ParameterItem("Param1", "Display1"));
            c.Add(new PlusItem());
            c.Add(new ValueItem(100));
            c.Add(new EndBracketItem());
            c.Add(new MultiplyItem());
            c.Add(new ParameterItem("Param2", "Display2"));
            c.Add(new MinusItem());
            c.Add(new HogeItem());

            c.Parameters["Param1"].Value = 50;
            c.Parameters["Param2"].Value = 2;
            c.Parameters["Hoge"].Value = 10;

            var r = c.Evaluate();                   // (50 + 100) * 2 - 10
            Console.WriteLine(c.Formula);           // (Param1+100)*Param2-Hoge
            Console.WriteLine(c.DisplayFormula);    // (Display1+100)*Display2-DisplayHoge
            Console.WriteLine(r);                   // 290
            

            var json = CalcConvert.Serialize(c);

            // {"Items":[{"Type":"StartBracketItem"},{"Name":"Param1","DisplayName":"Display1","Type":"ParameterItem"},{"Type":"PlusItem"},{"Type":"ValueItem","Value":100.0},{"Type":"EndBracketItem"},{"Type":"MultiplyItem"},{"Name":"Param2","DisplayName":"Display2","Type":"ParameterItem"},{"Type":"MinusItem"},{"Type":"ConsoleApp1.HogeItem, ConsoleApp1"}]}
            Console.WriteLine(json);

            c = CalcConvert.Deserialize(json);

            Console.ReadLine();
        }
    }

    internal class HogeItem : SlimParameterItemBase
    {
        public HogeItem() : base("Hoge", "DisplayHoge")
        {
        }
    }
```
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Metroit.JustEvaluate.Expression
{
    /// <summary>
    /// 計算式に利用するパラメーターのクラスの生成を提供します。
    /// </summary>
    internal static class CalcParameterTypeBuilder
    {
        private static readonly string TypeName = "CalcParameterType";

        private static readonly string ModuleName = "MainModule";

        /// <summary>
        /// 指定されたプロパティセットを有するオブジェクトを生成します。
        /// </summary>
        /// <param name="propertySets">プロパティセット。</param>
        /// <returns>指定されたプロパティセットを有するオブジェクト。</returns>
        //internal static object CreateNewObject(IEnumerable<IParameterCalcItem> propertySets)
        internal static object CreateNewObject(IEnumerable<CalcParameter> propertySets)
        {
            var myType = CompileResultType(propertySets);
            var newObj = Activator.CreateInstance(myType);

            // プロパティに値を入れる
            var type = newObj.GetType();
            foreach (var propertySet in propertySets)
            {
                var property = type.GetProperty(propertySet.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                if (property == null)
                {
                    continue;
                }
                property.SetValue(newObj, Convert.ChangeType(propertySet.Value, property.PropertyType));
            }

            return newObj;
        }

        /// <summary>
        /// 指定されたプロパティセットを有するタイプを生成します。
        /// </summary>
        /// <param name="propertySets">プロパティセット。</param>
        /// <returns>指定されたプロパティセットを有するタイプ。</returns>
        //private static Type CompileResultType(IEnumerable<IParameterCalcItem> propertySets)
        private static TypeInfo CompileResultType(IEnumerable<CalcParameter> propertySets)
        {
            var tb = GetTypeBuilder();
            tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (var propertySet in propertySets)
            {
                CreateProperty(tb, propertySet.Name, propertySet.Value.GetType());
            }

            return tb.CreateTypeInfo();
        }

        /// <summary>
        /// 生成するクラスのタイプビルダーの取得を行う。
        /// </summary>
        /// <returns></returns>
        private static TypeBuilder GetTypeBuilder()
        {
            var assmName = new AssemblyName(TypeName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assmName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(ModuleName);
            var tb = moduleBuilder.DefineType(TypeName,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);

            return tb;
        }

        /// <summary>
        /// プロパティの生成を行う。
        /// </summary>
        /// <param name="tb">タイプビルダー。</param>
        /// <param name="propertyName">プロパティ名。</param>
        /// <param name="propertyType">プロパティのタイプ。</param>
        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            var fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // プロパティのGetアクセサ
            var propertyGetMethodBuilder = tb.DefineMethod("get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType, Type.EmptyTypes);
            var getIlGen = propertyGetMethodBuilder.GetILGenerator();
            getIlGen.Emit(OpCodes.Ldarg_0);
            getIlGen.Emit(OpCodes.Ldfld, fieldBuilder);
            getIlGen.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(propertyGetMethodBuilder);

            // プロパティのSetアクセサ
            var propertySetMethodBuilder =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                  null, new[] { propertyType });
            var setIlGen = propertySetMethodBuilder.GetILGenerator();
            setIlGen.MarkLabel(setIlGen.DefineLabel());
            setIlGen.Emit(OpCodes.Ldarg_0);
            setIlGen.Emit(OpCodes.Ldarg_1);
            setIlGen.Emit(OpCodes.Stfld, fieldBuilder);
            setIlGen.Emit(OpCodes.Nop);
            setIlGen.MarkLabel(setIlGen.DefineLabel());
            setIlGen.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(propertySetMethodBuilder);
        }
    }
}

using System;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;

public static class DynamicDtoGenerator
{
    public static Type CreateDynamicDto(IDataReader reader, string DTOName)
    {
        var assemblyName = new AssemblyName("DynamicAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        var typeBuilder = moduleBuilder.DefineType(DTOName, TypeAttributes.Public | TypeAttributes.Class);

        // Add properties based on column names from the reader
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var columnName = reader.GetName(i);
            var columnType = reader.GetFieldType(i);
            CreateProperty(typeBuilder, columnName, columnType);
        }

        return typeBuilder.CreateType();
    }

    private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
    {
        var fieldBuilder = typeBuilder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);
        var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

        // The get method
        var getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyName}", MethodAttributes.Public, propertyType, Type.EmptyTypes);
        var getIL = getMethodBuilder.GetILGenerator();
        getIL.Emit(OpCodes.Ldarg_0);
        getIL.Emit(OpCodes.Ldfld, fieldBuilder);
        getIL.Emit(OpCodes.Ret);
        propertyBuilder.SetGetMethod(getMethodBuilder);

        // The set method
        var setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyName}", MethodAttributes.Public, null, new Type[] { propertyType });
        var setIL = setMethodBuilder.GetILGenerator();
        setIL.Emit(OpCodes.Ldarg_0);
        setIL.Emit(OpCodes.Ldarg_1);
        setIL.Emit(OpCodes.Stfld, fieldBuilder);
        setIL.Emit(OpCodes.Ret);
        propertyBuilder.SetSetMethod(setMethodBuilder);
    }
}

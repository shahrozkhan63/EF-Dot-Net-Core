using Alphatech.Services.ProductAPI.Helper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Alphatech.Services.ProductAPI.DBProductContext
{
    public class DatabaseHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly DynamicClassGenerator _classGenerator;

        public DatabaseHelper(ApplicationDbContext context)
        {
            _context = context;
            _classGenerator = new DynamicClassGenerator();
        }
      
        public async Task<List<object>> GenerateDTOFromStoredProcedureAsync(
     string storedProcedureName,
     string className,
     CancellationToken cancellationToken,
     params SqlParameter[] parameters)
        {
            // Fetch the project directory and set the DTO path
            string projectDirectory = Directory.GetCurrentDirectory();
            string dtoFolderPath = Path.Combine(projectDirectory, "Models", "Dto");
            string dtoFilePath = Path.Combine(dtoFolderPath, $"{className}.cs");

            // Ensure the DTO directory exists
            if (!Directory.Exists(dtoFolderPath))
            {
                Directory.CreateDirectory(dtoFolderPath);
            }

            // Remove the existing DTO file, if it exists
            if (File.Exists(dtoFilePath))
            {
                File.Delete(dtoFilePath);
                Console.WriteLine($"Old DTO class {className} deleted.");
            }

            using (var connection = _context.Database.GetDbConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);

                    if (connection.State == ConnectionState.Closed)
                        await connection.OpenAsync(cancellationToken);

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        // Get the schema details to create the DTO
                        var schemaTable = reader.GetSchemaTable();
                        var columnData = new Dictionary<string, Type>();

                        foreach (DataRow row in schemaTable.Rows)
                        {
                            string columnName = row["ColumnName"].ToString();
                            Type columnType = (Type)row["DataType"];
                            columnData.Add(columnName, columnType);
                        }

                        // Generate the DTO class dynamically
                        var classGenerator = new DynamicClassGenerator();
                        classGenerator.GenerateDTOClass(className, columnData);

                        // Dynamically compile the class and get the Type
                        var compiledType = classGenerator.CompileGeneratedDTOClass(className);

                        // Create a list to store results dynamically
                        var results = new List<object>();

                        // Loop through the reader rows and populate the dynamic DTO
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            // Create an instance of the dynamic DTO class
                            var dtoInstance = Activator.CreateInstance(compiledType);

                            // Map the columns to the properties of the DTO
                            foreach (var property in compiledType.GetProperties())
                            {
                                var value = reader[property.Name];
                                if (value != DBNull.Value)
                                {
                                    property.SetValue(dtoInstance, value);
                                }
                            }

                            // Add to the result list
                            results.Add(dtoInstance);
                        }

                        return results;
                    }
                }
            }
        }
        
      


        private async Task<List<Dictionary<string, object>>> ExecuteDynamicStoredProcedureAsync(string storedProcedureName, CancellationToken cancellationToken, params SqlParameter[] parameters)
        {
            var results = new List<Dictionary<string, object>>();

            using (DbConnection connection = _context.Database.GetDbConnection())
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                var dynamicParams = new DynamicParameters();
                foreach (var param in parameters)
                {
                    dynamicParams.Add(param.ParameterName, param.Value, param.DbType, param.Direction);
                }

                var rows = await connection.QueryAsync(storedProcedureName, dynamicParams, commandType: CommandType.StoredProcedure);

                foreach (var row in rows)
                {
                    var rowDictionary = new Dictionary<string, object>();
                    foreach (var prop in (IDictionary<string, object>)row)
                    {
                        rowDictionary.Add(prop.Key, prop.Value);
                    }
                    results.Add(rowDictionary);
                }
            }

            return results;
        }

        public async Task<object> ExecuteStoredProcedureAndGenerateDtoAsync(string storedProcedureName, string dtoName)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    // Explicitly open the connection
                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var dtoType = DynamicDtoGenerator.CreateDynamicDto(reader, dtoName);
                        var dtoList = new List<object>();

                        while (await reader.ReadAsync())
                        {
                            var dto = Activator.CreateInstance(dtoType);

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var property = dtoType.GetProperty(reader.GetName(i));
                                if (property != null && reader.GetValue(i) != DBNull.Value)
                                {
                                    property.SetValue(dto, reader.GetValue(i));
                                }
                            }

                            dtoList.Add(dto);
                        }

                        return dtoList;
                    }
                }
                finally
                {
                    // Ensure the connection is closed after execution
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        public async Task<List<Dictionary<string, object>>> ExecuteDynamicStoredProcedureAsync(string storedProcedureName, params SqlParameter[] parameters)
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                // Open the connection
                if (_context.Database.GetDbConnection().State != ConnectionState.Open)
                {
                    await _context.Database.OpenConnectionAsync();
                    Console.WriteLine("Connection opened successfully.");
                }

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;

                    // Optional: Set command timeout
                    command.CommandTimeout = 60;

                    // Add parameters
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    // Execute reader
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Log exception or handle it
            }
            finally
            {
                // Close the connection only after command execution
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    await _context.Database.CloseConnectionAsync();
                    Console.WriteLine("Connection closed.");
                }
            }

            return results;
        }



    }



}

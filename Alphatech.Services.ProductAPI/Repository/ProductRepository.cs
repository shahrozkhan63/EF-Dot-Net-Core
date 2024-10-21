using Alphatech.Services.ProductAPI.Helper;
using Alphatech.Services.ProductAPI.Models;
using Alphatech.Services.ProductAPI.Models.Dto;
using AutoMapper;
using Dapper;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Alphatech.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;
        private readonly DatabaseHelper _databaseHelper;
        private readonly DynamicClassGenerator _classGenerator;
        public ProductRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _databaseHelper = new DatabaseHelper(db);
            _classGenerator = new DynamicClassGenerator();
        }

        public async Task<ProductDto> CreateUpdateProduct(ProductDto productdto)
        {
            ProductDto returnDto = new();
            try
            {
                Product product = _mapper.Map<ProductDto, Product>(productdto);
                if (product == null)
                {
                    product = new Product();
                }

                if (product.ProductId > 0)
                {
                  
                    _db.Update<Product>(product);
                    
                }
                else
                {
                    _db.Products.Add(product);
                }

                var domain = await _db.Products.Where(x => x.ProductId == product.ProductId).FirstOrDefaultAsync();
                await _db.SaveChangesAsync();
                var domain2 = await _db.Products.Where(x => x.ProductId == product.ProductId).FirstOrDefaultAsync();

                returnDto = _mapper.Map<Product, ProductDto>(product);
            }
            catch (Exception ex)
            {

            }
            return returnDto;
        }

        public async Task<bool> DeleteProduct(int productProductId)
        {
            try
            {
                var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productProductId);

                if (product == null)
                {
                    return false;
                }
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int productProductId)
        {
            ProductDto productsDto = new();
            try
            {
                var domain = await _db.Products.Where(x => x.ProductId == productProductId).FirstOrDefaultAsync();
                productsDto = _mapper.Map<ProductDto>(domain);
            }
            catch (Exception ex)
            {

            }
            return productsDto;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            List<ProductDto> productsDto = new();
            try
            {
                List<Product> domain = await _db.Products.ToListAsync();
                productsDto = _mapper.Map<List<ProductDto>>(domain);
            }
            catch (Exception ex)
            {

            }
            return productsDto;
        }

        public async Task<IEnumerable<object>> GetDynamicProducts()
        {
            try
            {
                // SQL parameters for the stored procedure
                var parameters = new[]
                {
            new SqlParameter("@ProductProductId", SqlDbType.Int) { Value = -1 }
                };

                        // Create a cancellation token
                        var cancellationTokenSource = new CancellationTokenSource();
                        var cancellationToken = cancellationTokenSource.Token;

                        // Call the helper to generate the dynamic DTO and retrieve the data
                        var generatedData = await _databaseHelper.GenerateDTOFromStoredProcedureAsync("sp_GetProducts", "GeneratedProductDTO", cancellationToken, parameters);
            //    );

                return generatedData; // Return the dynamically generated DTOs
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<object>(); // Return an empty list in case of failure
            }
        }


        public async Task<int> TestConnectionAsync()
        {
            int i = 0;
            try
            {
                // Log connection string for debugging
                Console.WriteLine(_db.Database.GetDbConnection().ConnectionString);

                await _db.Database.OpenConnectionAsync();
                Console.WriteLine("Connection opened successfully.");
                i = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await _db.Database.CloseConnectionAsync();
            }
            return i;
        }

        
        public async Task<List<Dictionary<string, object>>> ExecuteDynamicStoredProcedureAsync1(string storedProcedureName, CancellationToken cancellationToken, params SqlParameter[] parameters)
        {
            var results = new List<Dictionary<string, object>>();

            var connection = _db.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                    Console.WriteLine("Connection opened successfully.");
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60; // Set command timeout

                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    if (connection.State != ConnectionState.Open)
                    {
                        throw new InvalidOperationException("Connection was closed unexpectedly.");
                    }

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
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
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Task canceled: {ex.Message}");
                // Log or handle cancellation
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                    Console.WriteLine("Connection closed.");
                }
            }

            return results;
        }

        public Task<ProductDto> CreateProduct(ProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}

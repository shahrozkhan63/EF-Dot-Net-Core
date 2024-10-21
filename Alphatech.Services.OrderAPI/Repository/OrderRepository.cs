using Alphatech.Services.OrderAPI.Helper;
using Alphatech.Services.OrderAPI.Models;
using Alphatech.Services.OrderAPI.Models.Dto;
using Alphatech.Services.OrderAPI.OrderServices;
using AutoMapper;
using Dapper;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Alphatech.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<OrderRepository> _logger;
        private IMapper _mapper;
        private readonly DatabaseHelper _databaseHelper;
        private readonly DynamicClassGenerator _classGenerator;
        public OrderRepository(ApplicationDbContext db, IMapper mapper,ILogger<OrderRepository> logger)
        {
            _db = db;
            _mapper = mapper;
            _databaseHelper = new DatabaseHelper(db);
            _classGenerator = new DynamicClassGenerator();
            _logger = logger;
        }

        public async Task<Order> CreateUpdateOrder(Order order)
        {
            try
            {
                if (order == null)
                {
                    order = new Order();
                }

                if (order.OrderId > 0)
                {
                    _db.Update<Order>(order);
                }
                else
                {
                    _db.Orders.Add(order);
                }

                //var domain = await _db.Orders.Where(x => x.OrderId == order.OrderId).FirstOrDefaultAsync();
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {

            }
            return order;
        }

        public async Task<bool> DeleteOrder(int orderId)
        {
            try
            {
                var order = await _db.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);

                if (order == null)
                {
                    return false;
                }
                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<OrderDto> GetOrderById(int orderId)
        {
            OrderDto ordersDto = new();
            try
            {
                var domain = await _db.Orders.Where(x => x.OrderId == orderId).FirstOrDefaultAsync();
                ordersDto = _mapper.Map<OrderDto>(domain);
            }
            catch (Exception ex)
            {

            }
            return ordersDto;
        }

        public async Task<IEnumerable<OrderDto>> GetOrders()
        {
            List<OrderDto> ordersDto = new();
            try
            {
                List<Order> domain = await _db.Orders.ToListAsync();
                ordersDto = _mapper.Map<List<OrderDto>>(domain);
            }
            catch (Exception ex)
            {

            }
            return ordersDto;
        }

        public async Task<IEnumerable<object>> GetDynamicOrders()
        {
            try
            {
                // SQL parameters for the stored procedure
                var parameters = new[]
                {
            new SqlParameter("@OrderId", SqlDbType.Int) { Value = -1 }
                };

                        // Create a cancellation token
                        var cancellationTokenSource = new CancellationTokenSource();
                        var cancellationToken = cancellationTokenSource.Token;

                        // Call the helper to generate the dynamic DTO and retrieve the data
                        var generatedData = await _databaseHelper.GenerateDTOFromStoredProcedureAsync("sp_GetOrders", "GeneratedOrderDTO", cancellationToken, parameters);
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

        public async Task<int> CreateOrder(Order order)
        {
            int retVal = 0;
            try
            {
                // Add the order to the database context
                _db.Orders.Add(order);

                // Save the changes and get the number of affected rows
                retVal = await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (you can use your logging mechanism here)
                _logger.LogError(ex, "An error occurred while creating the order.");

                // Optionally, rethrow the exception if you want the caller to handle it
                throw;
            }

            // Return the result (number of affected rows)
            return retVal;
        }


        public async Task<int> CreateOrderItem(OrderItem orderItem)
        {
            int retVal = 0;
            try
            {
                if (orderItem == null)
                {
                    orderItem = new OrderItem();
                }

                if (orderItem.OrderId > 0)
                {

                    _db.Update<OrderItem>(orderItem);

                }
                else
                {
                    _db.OrderItems.Add(orderItem);
                }

                var domain = await _db.OrderItems.Where(x => x.OrderId == orderItem.OrderId).FirstOrDefaultAsync();
                retVal=  await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
            return retVal;
        }
    }
}

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;

public class Order
{
    public int ID { get; set; }
    public string Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int ProductID { get; set; }
    public Product Product { get; set; }
}

public class OrdersRepository
{
    private readonly string _connectionString;

    public OrdersRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateOrder(Order order)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            using (var command = new SqlCommand("INSERT INTO Order (Status, CreatedDate, UpdatedDate, ProductID) VALUES (@status, @createdDate, @updatedDate, @productID)", connection))
            {
                command.Parameters.Add("@status", SqlDbType.NVarChar).Value = order.Status;
                command.Parameters.Add("@createdDate", SqlDbType.DateTime).Value = order.CreatedDate;
                command.Parameters.Add("@updatedDate", SqlDbType.DateTime).Value = order.UpdatedDate;
                command.Parameters.Add("@productID", SqlDbType.Int).Value = order.ProductID;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public Order ReadOrder(int id)
    {
        Order order = null;

        using (var connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM Order WHERE ID = @id", connection))
            {
                command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        order = new Order
                        {
                            ID = (int)reader["ID"],
                            Status = (string)reader["Status"],
                            CreatedDate = (DateTime)reader["CreatedDate"],
                            UpdatedDate = (DateTime)reader["UpdatedDate"],
                            ProductID = (int)reader["ProductID"]
                        };
                    }
                }
            }
        }
        return order;
    }

    public void UpdateOrder(Order order)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            using (var command = new SqlCommand("UPDATE Order SET Status = @status, CreatedDate = @createdDate, UpdatedDate = @updatedDate, ProductID = @productID WHERE ID = @id", connection))
            {
                command.Parameters.Add("@id", SqlDbType.Int).Value = order.ID;
                command.Parameters.Add("@status", SqlDbType.NVarChar).Value = order.Status;
                command.Parameters.Add("@createdDate", SqlDbType.DateTime).Value = order.CreatedDate;
                command.Parameters.Add("@updatedDate", SqlDbType.DateTime).Value = order.UpdatedDate;
                command.Parameters.Add("@productID", SqlDbType.Int).Value = order.ProductID;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteOrder(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            using (var command = new SqlCommand("DELETE FROM Order WHERE ID = @id", connection))
            {
                command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public IEnumerable<Order> FetchAllOrders()
    {
        List<Order> orders = new List<Order>();

        using (var connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM Orders", connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            ID = (int)reader["ID"],
                            Status = (string)reader["Status"],
                            CreatedDate = (DateTime)reader["CreatedDate"],
                            UpdatedDate = (DateTime)reader["UpdatedDate"],
                            ProductID = (int)reader["ProductID"]
                        });
                    }
                }
            }
        }

        return orders;
    }

    public IEnumerable<Order> FetchOrders(int? month, string status, int? year, int? productId)
    {
        List<Order> orders = new List<Order>();

        using (var connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand("FetchOrders", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Month", SqlDbType.Int).Value = (object)month ?? DBNull.Value;
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = (object)status ?? DBNull.Value;
                command.Parameters.Add("@Year", SqlDbType.Int).Value = (object)year ?? DBNull.Value;
                command.Parameters.Add("@ProductId", SqlDbType.Int).Value = (object)productId ?? DBNull.Value;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            ID = (int)reader["ID"],
                            Status = (string)reader["Status"],
                            CreatedDate = (DateTime)reader["CreatedDate"],
                            UpdatedDate = (DateTime)reader["UpdatedDate"],
                            ProductID = (int)reader["ProductID"]
                        });
                    }
                }
            }
        }

        return orders;
    }

    public void BulkDeleteOrders(IEnumerable<int> orderIds)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                foreach (int orderId in orderIds)
                {
                    using (SqlCommand command = new SqlCommand("Delete From Orders Where ID = @id", connection, transaction))
                    {
                        try
                        {
                            command.Parameters.Add("@id", SqlDbType.Int).Value = orderId;

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                transaction.Commit();
            }
        }
    }
}
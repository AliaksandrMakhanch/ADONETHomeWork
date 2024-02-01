using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System;

public class Product
{
    public int ID { get; set; }
    public string Description { get; set; }
    public double Weight { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public double Length { get; set; }
}

public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateProduct(Product product)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            using (var command = new SqlCommand("INSERT INTO Product (Description, Weight, Height, Width, Length) VALUES (@description, @weight, @height, @width, @length)", connection))
            {
                command.Parameters.Add("@description", SqlDbType.NVarChar).Value = product.Description;
                command.Parameters.Add("@weight", SqlDbType.Float).Value = product.Weight;
                command.Parameters.Add("@height", SqlDbType.Float).Value = product.Height;
                command.Parameters.Add("@width", SqlDbType.Float).Value = product.Width;
                command.Parameters.Add("@length", SqlDbType.Float).Value = product.Length;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    public Product ReadProduct(int id)
        {
            Product product = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM Product WHERE ID = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Product
                            {
                                ID = (int)reader["ID"],
                                Description = (string)reader["Description"],
                                Weight = (double)reader["Weight"],
                                Height = (double)reader["Height"],
                                Width = (double)reader["Width"],
                                Length = (double)reader["Length"]
                            };
                        }
                    }
                }
            }
            return product;
        }

        public void UpdateProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("UPDATE Product SET Description = @description, Weight = @weight, Height = @height, Width = @width, Length = @length WHERE ID = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = product.ID;
                    command.Parameters.Add("@description", SqlDbType.NVarChar).Value = product.Description;
                    command.Parameters.Add("@weight", SqlDbType.Float).Value = product.Weight;
                    command.Parameters.Add("@height", SqlDbType.Float).Value = product.Height;
                    command.Parameters.Add("@width", SqlDbType.Float).Value = product.Width;
                    command.Parameters.Add("@length", SqlDbType.Float).Value = product.Length;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("DELETE FROM Product WHERE ID = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }



using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Mysql_.netcore_baglantisi
{
    class Program
    {
        public interface IPorductDal
        {//yapmamız gereken interface içerisine hangi metodu kullanacaksam eklemek
            // interface içerisindeki metodun concrate yani işlenmiş halini ise mysql yada mssgl ile dolduracaksın ve bunu product içinde kullancaksınız
            List<Product> GetAllProducts();
            List<Product> Find(string Productname);//gelen kritere göre bilgiyi bulacak kaç ürün bu bilgiyle eşleşiyosa bunu bulacak getirecek
            Product GetProductById(int id); //idyi göndericem producta bana bir id bilgisi getirecek
            int Create(Product p);
            int Delete(int productId);
            int Update(Product p);
            int count();
        }
        public class MySQLProductDal : IPorductDal
        {
            private MySqlConnection GetMysqlConnection()
            {
                string connectionString = @"server=localhost;port=3306;database=northwind;user=root;password=1532blmz;";
                return new MySqlConnection(connectionString);
            }
            public int Create(Product p)
            {
                throw new NotImplementedException();
            }

            public int Delete(int productId)
            {
                throw new NotImplementedException();
            }

            public List<Product> GetAllProducts()
            {
                List<Product> products = null;
                using (var connection = GetMysqlConnection())
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from products";
                        MySqlCommand Command = new MySqlCommand(sql, connection);
                        MySqlDataReader mySqlDataReader = Command.ExecuteReader();
                        products = new List<Product>();
                        while (mySqlDataReader.Read())//her seferinde 1 kayıtı okuycak kaç kayıt varsa o kadar dönecek
                        {
                            products.Add(new Product
                            {
                                ProductId = int.Parse(mySqlDataReader["id"].ToString()),
                                Name = mySqlDataReader["product_name"].ToString(),
                                Price = Double.Parse(mySqlDataReader["list_price"].ToString())
                            });
                            // Console.WriteLine($"name:{mySqlDataReader[3]} price:{mySqlDataReader[6]} ");
                        }
                        mySqlDataReader.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
                return products;
            }

            public Product GetProductById(int id)//kimliğine göre ürün getir
            {
                Product product = null;
                using (var connection = GetMysqlConnection())
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from products where  id=@Productid";

                        MySqlCommand Command = new MySqlCommand(sql, connection);

                        Command.Parameters.Add("@Productid", MySqlDbType.Int32).Value = id;

                        MySqlDataReader reader = Command.ExecuteReader();

                        reader.Read();//bir kere çalışacak çünkü tek bir kayıt bize gelecek

                        if (reader.HasRows)//hasrow bir kayıt geldi demektir
                        {//tek bir kayıt okuyacağım için while ihtiyaç yok
                            product = new Product()//eğer herhangi bilgi getirilmişse procuts nesnesi burayla başlatılacak ve bilgiyi gösterecek 
                                                   //yoksa null değer dönecek başta nul atadık zaten
                            {
                                ProductId = int.Parse(reader["id"].ToString()),
                                Name = reader["product_name"].ToString(),
                                Price = Double.Parse(reader["list_price"]?.ToString())
                            };

                        }
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
                return product;
            }

            public int Update(Product p)
            {
                throw new NotImplementedException();
            }

            public List<Product> Find(string Productname)//aradığın isme göre tüm ürünleri getir
            {
                List<Product> products = null;
                using (var connection = GetMysqlConnection())
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from products where  product_name LIKE @Productname";

                        MySqlCommand Command = new MySqlCommand(sql, connection);

                        Command.Parameters.Add("@Productname", MySqlDbType.String).Value = "%" + Productname + "%";

                        MySqlCommand MySqlCommand = new MySqlCommand(sql, connection);

                        MySqlDataReader reader = Command.ExecuteReader();//readeri oluşturuyoruz

                        products = new List<Product>();//bir liste tanımlıyoruz

                        while (reader.Read())//her seferinde 1 kayıtı okuycak kaç kayıt varsa o kadar dönecek
                        {
                            products.Add(new Product
                            {
                                ProductId = int.Parse(reader["id"].ToString()),
                                Name = reader["product_name"].ToString(),
                                Price = Double.Parse(reader["list_price"].ToString())
                            });
                            // Console.WriteLine($"name:{mySqlDataReader[3]} price:{mySqlDataReader[6]} ");
                        }
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
                return products;//ve bu işlemleri en sonundada geriye göndersin
            }

            public int count()
            {
                int count = 0;
                using (var connection = GetMysqlConnection())
                {
                    try
                    {
                        connection.Open();
                        string sql = "select count(*) from products ";

                        MySqlCommand Command = new MySqlCommand(sql, connection);

                        MySqlCommand MySqlCommand = new MySqlCommand(sql, connection);
                        object result = Command.ExecuteScalar();//geriye tek bir değer dönecekse execute scaller metodunu kullanırım crud işlemi varsa excute nanquery
                        if (result != null)
                        {
                            count = Convert.ToInt32(result);
                        }


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
                return count;
            }

        }

        public class MsSQLProductDal : IPorductDal
        {//mysqldekileri kopyaladım tek farkı connectionstringde mysqlconnectionu sql connection olarak değiştirdim,list<product>larda
            //başında my yazanı sildim sql olarak bıraktım
            private SqlConnection GetMssqlConnection()
            {
                string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=SSPI";
                return new SqlConnection(connectionString);
            }
            public int Create(Product p)
            {
                int result = 0;
                using (var connection = GetMssqlConnection())
                {
                    try
                    {
                        connection.Open();
                        string sql = "insert into products (ProductName,UnitPrice,Discontinued) VALUES (@productname,@unitprice,@discontiud)";
                        SqlCommand command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@productname", p.Name);
                        command.Parameters.AddWithValue("@unitprice", p.Price);
                        command.Parameters.AddWithValue("@discontiud", 1);
                        result = command.ExecuteNonQuery();
                        Console.WriteLine($"{result} adet kayıt eklendi");

                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result;
            }

            public int Delete(int productId)
            {
                int result = 0;
                using (var connection = GetMssqlConnection())
                {
                    try
                    {
                        connection.Open();
                        string sql = "delete from products where ProductId=@productid ";
                        SqlCommand command = new SqlCommand(sql, connection);

                        command.Parameters.AddWithValue("@productid", productId);

                        result = command.ExecuteNonQuery();
                        // Console.WriteLine($"{result} adet kayıt eklendi");

                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result;
            }

            public List<Product> GetAllProducts()
            {
                List<Product> products = null;
                using (var connection = GetMssqlConnection())
                {
                    try
                    {
                        connection.Open();
                        string sql = "select * from products";
                        SqlCommand Command = new SqlCommand(sql, connection);
                        SqlDataReader msSqlDataReader = Command.ExecuteReader();
                        products = new List<Product>();
                        while (msSqlDataReader.Read())//her seferinde 1 kayıtı okuycak kaç kayıt varsa o kadar dönecek
                        {
                            products.Add(new Product
                            {
                                ProductId = int.Parse(msSqlDataReader["ProductID"].ToString()),
                                Name = msSqlDataReader["ProductName"].ToString(),
                                Price = Double.Parse(msSqlDataReader["UnitPrice"].ToString())
                            });
                            // Console.WriteLine($"name:{mySqlDataReader[3]} price:{mySqlDataReader[6]} ");
                        }
                        msSqlDataReader.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
                return products;
            }

            public Product GetProductById(int id)
            {
                throw new NotImplementedException();
            }

            public int Update(Product p)
            {
                int result = 0;
                using (var connection = GetMssqlConnection())
                {
                    try
                    {
                        connection.Open();
                        string sql = "update products set ProductName=@productname, UnitPrice=@unitprice where ProductId=@productid ";
                        SqlCommand command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@productname", p.Name);
                        command.Parameters.AddWithValue("@unitprice", p.Price);
                        command.Parameters.AddWithValue("@productid", p.ProductId);
                        result = command.ExecuteNonQuery();
                        // Console.WriteLine($"{result} adet kayıt eklendi");

                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result;
            }

            public List<Product> Find(string Productname)
            {
                throw new NotImplementedException();
            }

            public int count()
            {
                throw new NotImplementedException();
            }
        }

        public class ProductManager : IPorductDal
        {
            IPorductDal _porductDal;//dışarıdan parametre olarak alıcam
            public ProductManager(IPorductDal porductDal)
            {
                _porductDal = porductDal;
            }

            public int count()
            {
                return _porductDal.count();
            }

            public int Create(Product p)
            {
                return _porductDal.Create(p);
            }

            public int Delete(int productId)
            {
                return _porductDal.Delete(productId);
            }

            public List<Product> Find(string Productname)
            {
                return _porductDal.Find(Productname);
            }

            public List<Product> GetAllProducts()
            {
                return _porductDal.GetAllProducts();
            }

            public Product GetProductById(int id)
            {
                return _porductDal.GetProductById(id);
            }

            public int Update(Product p)
            {
                return _porductDal.Update(p);
            }
        }//mysgele yada msse bağlı olmayan tamamen interfaceye bağlı bir clas
        static void Main(string[] args)
        {
            // GetMysqlConnection();
            // GetAllProducts();
            //var productDal = new MySQLProductDal();
            // var productDal = new MsSQLProductDal();
            var productdal = new ProductManager(new MsSQLProductDal());//veritabanı sorgulamamız buraya neyazarsak o olacak

            //var products = productdal.GetAllProducts();
            //var products = productdal.GetProductById(4);
            //var products = productDal.Find("Gr");
            //Console.WriteLine($"{products.Name}");
            // int count = productdal.count();
            /*  var p = new Product()
              {
                  ProductId=78,
                  Name = "Samsung S10",
                  Price = 800
              };
              int count = productdal.Update(p);
              Console.WriteLine("Güncellenen kayıt sayısı" + count); */
            int result = productdal.Delete(79);
            Console.WriteLine("Silinen kayıt sayısı" + result); 



            /* foreach (var item in products)
             {
                 Console.WriteLine($"{item.ProductId} {item.Name}");
             }*/
        }
    }
}

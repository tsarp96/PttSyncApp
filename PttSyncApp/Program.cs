using Npgsql;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PttSyncApp
{
    class Program
    {
        static string connStr = "User ID=postgres;Password=da8566ffb75a1fa378d5b73ad878829c;Server=keosdesign.net;Port=13876;Database=mahall_test;Integrated Security=true;Pooling=true;";
        public static IDbConnection OpenConnection(string connStr)
        {
            var conn = new NpgsqlConnection(connStr);
            conn.Open();
            return conn;
        }

        public static bool createTable(string connStr) // The code inside in that method will be change according to needs. 
        {
            using var con = new NpgsqlConnection(connStr);
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"CREATE TABLE Neighborhoods(
                      districtId INTEGER REFERENCES Districts(id),
                      id SERIAL PRIMARY KEY, 
                      name VARCHAR(255)
                    );";
            cmd.ExecuteNonQuery();
            return true;
        }

        public static bool insertDistrictsAccordingToCities(List<City> cities){ 
            using var con = new NpgsqlConnection(connStr);
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            var c_index = 1;
            var d_index = 1;
            foreach(var city in cities)
            {
                foreach(var district in city.Districts)
                {
                    string cmd_str = "INSERT INTO Districts(cityid,id,name) VALUES('" + c_index + "'," + "'" + d_index + "'," + "'" + district.Name + "'" + ")";
                    cmd.CommandText = cmd_str;
                    cmd.ExecuteNonQuery();
                    d_index++;
                }
                c_index++;
            }
            return true;
        }  // Single use only

        public static bool insertNeighborhoodssAccordingToDistricts(List<City> cities)
        {
            using var con = new NpgsqlConnection(connStr);
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            var d_index = 1;
            var n_index = 1;
            foreach (var city in cities)
            {
                foreach (var district in city.Districts)
                {
                    foreach(var neighborhood in  district.Neighborhoods)
                    {
                        string cmd_str = "INSERT INTO Neighborhoods(districtid,id,name) VALUES('" + d_index + "'," + "'" + n_index + "'," + "'" + neighborhood.Name + "'" + ")";
                        cmd.CommandText = cmd_str;
                        cmd.ExecuteNonQuery();
                        n_index++;
                    }
                    d_index++;
                }
            }
            return true;
        } // Single use only


        public static bool addCitiesToDatabase(string connStr, List<City> cities) // Single use only
        {
            using var con = new NpgsqlConnection(connStr);
            con.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = @"CREATE TABLE (id SERIAL PRIMARY KEY, 
                    name VARCHAR(255))";
            cmd.ExecuteNonQuery();

            foreach (var city in cities)
            {
                cmd.CommandText = "INSERT INTO Cities(name) VALUES('"+ city.Name +"')";
                cmd.ExecuteNonQuery();
            }
            return true;
        }


        public static int isCityExist(List<City> cities, string cityName)
        {
            int index = 0;
            foreach (var city in cities)
            {
                if(city.Name == cityName)
                {
                    return index;
                }
                index++;
            }
            return -1 ;
        }
        public static int isDestrictExist(List<District> districts, string districtName)
        {
            int index = 0;
            foreach (var district in districts)
            {
                if (district.Name == districtName)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
        static void Main(string[] args)
        {
            // path to your excel file
            string path = "C:\\Users\\TUNA\\source\\repos\\PttSyncApp\\PttSyncApp\\il_ilce_mahalle_postakodu.xlsx";
            FileInfo fileInfo = new FileInfo(path);

            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

            // get number of rows and columns in the sheet
            int rows = worksheet.Dimension.Rows; 
            int columns = worksheet.Dimension.Columns;

            List<City> cities = new List<City> { };

            // loop through the worksheet rows and columns
            for (int i = 2; i <= rows; i++)
            {

                string neighborhoodName = " ", districtName = " ", cityName = " ";

                for (int j = 1; j <= columns; j++)
                {
                    if (j == 1)
                    {
                        cityName = worksheet.Cells[i, j].Value.ToString();
                    }
                    if (j == 2)
                    {
                        districtName = worksheet.Cells[i, j].Value.ToString();
                    }
                    if (j == 4)
                    {
                        neighborhoodName = worksheet.Cells[i, j].Value.ToString();
                    }
                }

                if(cities == null)
                {
                    City city = new City();
                    District district = new District();
                    Neighborhood neighborhood = new Neighborhood();
                    neighborhood.Name = neighborhoodName;
                    district.Name = districtName;
                    district.Neighborhoods.Add(neighborhood);
                    city.Districts.Add(district);
                    city.Name = cityName;
                    cities.Add(city);
                    continue;
                }
                var index = isCityExist(cities, cityName);
                if(index == -1)
                {
                    City city = new City();
                    District district = new District();
                    Neighborhood neighborhood = new Neighborhood();
                    neighborhood.Name = neighborhoodName;
                    district.Name = districtName;
                    district.Neighborhoods.Add(neighborhood);
                    city.Districts.Add(district);
                    city.Name = cityName;
                    cities.Add(city);
                    continue;
                }
                var indexA = isDestrictExist(cities[index].Districts, districtName);
                if(indexA == -1)
                {
                    District district = new District();
                    Neighborhood neighborhood = new Neighborhood();
                    neighborhood.Name = neighborhoodName;
                    district.Name = districtName;
                    district.Neighborhoods.Add(neighborhood);
                    cities[index].Districts.Add(district);
                    continue;
                }
                Neighborhood neighborhood1 = new Neighborhood();
                neighborhood1.Name = neighborhoodName;
                cities[index].Districts[indexA].Neighborhoods.Add(neighborhood1);
            }

            // addCitiesToDatabase(connStr, cities);
            //deneme(connStr);
            //mercimek(cities);
            //patlıcan(cities);

            Console.WriteLine("Finished !");
        }
    }
}

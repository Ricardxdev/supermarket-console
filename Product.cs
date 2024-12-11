using System.Data;

namespace Supermarket
{
    class Product
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public double BuyPrice { get; private set; }
        public double SalePrice { get; private set; }

        public Product(string code, string name, string description, string category, double buyPrice, double salePrice)
        {
            Code = code;
            Name = name;
            Description = description;
            Category = category;
            BuyPrice = buyPrice;
            SalePrice = salePrice;
        }

        public static void GenerateColumns(DataTable dataTable)
        {
            dataTable.Columns.Add("Codigo", typeof(string));
            dataTable.Columns.Add("Nombre", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("Category", typeof(string));
            dataTable.Columns.Add("Costo", typeof(double));
            dataTable.Columns.Add("PVP", typeof(double));
        }
        public void ToRow(DataTable dataTable)
        {
            if (dataTable.Columns.Count == 6)
            {
                dataTable.Rows.Add(Code, Name, Description, Category, BuyPrice, SalePrice);
            }
            else if (dataTable.Columns.Count == 7)
            {
                dataTable.Rows.Add(Code, Name, Description, Category, BuyPrice, SalePrice, " -> Comprar <-");
            }
        }

        public static bool operator ==(Product? a, Product? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Code == b.Code;
        }

        public static bool operator !=(Product a, Product b)
        {
            return !(a == b);
        }
    }
}
using System.Data;

namespace Supermarket
{
    class Client
    {
        public string License { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string Phone { get; private set; }
        public double TotalBilled { get; private set; }

        public Client(string license, string firstName, string lastName, string address, string phone, double totalBilled)
        {
            License = license;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Phone = phone;
            TotalBilled = totalBilled;

        }

        public static void GenerateColumns(DataTable dataTable)
        {
            dataTable.Columns.Add("Cedula", typeof(string));
            dataTable.Columns.Add("Nombre(s)", typeof(string));
            dataTable.Columns.Add("Apellido(s)", typeof(string));
            dataTable.Columns.Add("Telefono", typeof(string));
            dataTable.Columns.Add("Direccion", typeof(string));
            dataTable.Columns.Add("Total Facturado", typeof(double));
        }
        public void ToRow(DataTable dataTable) 
        {
            dataTable.Rows.Add(License, FirstName, LastName, Phone, Address, TotalBilled);
        }

        public static bool operator ==(Client a, Client b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.License == b.License;
        }

        public static bool operator !=(Client a, Client b)
        {
            return !(a == b);
        }
    }
}
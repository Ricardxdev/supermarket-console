namespace Supermarket
{
    class Client
    {
        public int License { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string Phone { get; private set; }
        public double TotalBilled { get; private set; }

        public Client(int license, string firstName, string lastName, string address, string phone, double totalBilled)
        {
            License = license;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Phone = phone;
            TotalBilled = totalBilled;

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
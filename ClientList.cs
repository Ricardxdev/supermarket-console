using System.Data;

namespace Supermarket
{

    class ClientNode
    {
        public Client Client { get; private set; }
        public ClientNode? Next { get; private set; }

        public ClientNode(Client client)
        {
            this.Client = client;
            this.Next = null;
        }

        public Client GetClient()
        {
            return this.Client;
        }
        public ClientNode? GetNext()
        {
            return this.Next;
        }

        public void SetClient(Client client)
        {
            Client = client;
        }

        public void SetNext(ClientNode? next)
        {
            Next = next;
        }

        public void ShowClient()
        {
            Console.WriteLine($"Name: {Client.FirstName} {Client.LastName}, License: {Client.License}, Address: {Client.Address}, Phone: {Client.Phone}, Billed: {Client.TotalBilled}");
        }

    }
    class ClientList
    {
        public ClientNode? Head { get; private set; }
        public ClientNode? Tail { get; private set; }
        private int Size = 0;

        public Client? Get(string license)
        {
            var act = Head;
            while (act != null)
            {
                if (act.Client.License == license)
                {
                    return act.Client;
                }
                act = act.Next;
            }
            return null;
        }

        public void Add(Client client)
        {
            var node = new ClientNode(client);
            if (Head == null)
            {
                Head = node;
                Tail = node;
            }
            else if (Tail != null)
            {
                Tail.SetNext(node);
                Tail = Tail.Next;
            }
            Size++;
        }

        public Client? Update(Client client)
        {
            var act = this.Head;
            while (act != null)
            {
                if (act.Client == client)
                {
                    act.SetClient(client);
                    return client;
                }
                act = act.Next;
            }

            return null;
        }

        public Client? Delete(string license)
        {
            if (Head == null || Tail == null) return null;
            var act = Head;
            var prev = act;
            while (act != null)
            {
                if (act.Client.License == license)
                {
                    if (act.Client == Head.Client)
                    {
                        Head = Head.Next;
                        if (Head == null)
                        {
                            Tail = null;
                        }
                    }
                    else if (act.Client == Tail.Client)
                    {
                        Tail = prev;
                        Tail.SetNext(null);
                    }
                    else
                    {
                        prev.SetNext(act.Next);
                    }

                    Size--;
                    return act.Client;
                }
                prev = act;
                act = act.Next;
            }

            return null;
        }

        public void ShowAll()
        {
            var act = Head;
            while (act != null)
            {
                act.ShowClient();
                act = act.Next;
            }
        }

        public Client? MostBilledClient()
        {
            if (Head == null) return null;
            var act = Head;
            var max = Head;
            while (act != null)
            {
                if (act.Client.TotalBilled > max.Client.TotalBilled)
                {
                    max = act;
                }
                act = act.Next;
            }

            return max.Client;
        }

        public Client? LessBilledClient()
        {
            if (Head == null) return null;
            var act = Head;
            var min = Head;
            while (act != null)
            {
                if (act.Client.TotalBilled < min.Client.TotalBilled)
                {
                    min = act;
                }
                act = act.Next;
            }

            return min.Client;
        }

        public double AverageBilled()
        {
            var act = Head;
            Double acc = 0;

            while (act != null)
            {
                acc += act.Client.TotalBilled;
                act = act.Next;
            }

            return acc / Size;
        }

        public void ForEach(Action<Client> callback)
        {
            var act = Head;
            while (act != null)
            {
                callback(act.Client);
                act = act.Next;
            }
        }

        public DataTable GetDataTable()
        {
            return GetDataTable("");
        }

        public DataTable GetDataTable(string filterLicense)
        {
            var dataTable = new DataTable();
            Client.GenerateColumns(dataTable);

            ForEach((p) =>
            {
                if (filterLicense.Trim() == "" || p.License.Contains(filterLicense))
                {
                    p.ToRow(dataTable);
                }
            });
            return dataTable;
        }
    }
}
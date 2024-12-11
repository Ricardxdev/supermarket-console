namespace Supermarket
{
    using System.Data;
    class BillProduct
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public double Price { get; private set; }
        public int Quantity { get; private set; }
        public double SubTotal {get; private set;}

        public BillProduct(Product product, int quantity)
        {
            Code = product.Code;
            Name = product.Name;
            Price = product.SalePrice;
            Quantity = quantity;
            SubTotal = Price * Quantity;
        }

        public void UpdateQuantity(int quantity)
        {
            Quantity = quantity;
            SubTotal = Price * Quantity;
        }

        public static void GenerateColumns(DataTable dataTable)
        {
            dataTable.Columns.Add("Codigo", typeof(string));
            dataTable.Columns.Add("Nombre", typeof(string));
            dataTable.Columns.Add("Precio", typeof(double));
            dataTable.Columns.Add("Cantidad", typeof(int));
            dataTable.Columns.Add("SubTotal", typeof(double));
            dataTable.Columns.Add("Inc", typeof(string));
            dataTable.Columns.Add("Dec", typeof(string));
            dataTable.Columns.Add("Eliminar", typeof(string));
        }
        public void ToRow(DataTable dataTable) 
        {
            if (dataTable.Columns.Count == 5) 
            {
                dataTable.Rows.Add(Code, Name, Price, Quantity, SubTotal);
            }
            else if (dataTable.Columns.Count == 8)
            {
                dataTable.Rows.Add(Code, Name, Price, Quantity, SubTotal, " | + | ", " | - | ", " | X | ");
            }
        }

        public static bool operator ==(BillProduct? a, BillProduct? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Code == b.Code;
        }

        public static bool operator !=(BillProduct a, BillProduct b)
        {
            return !(a == b);
        }
    }
    class BillProductNode
    {
        public BillProduct BillProduct { get; private set; }
        public BillProductNode? Next { get; private set; }

        public BillProductNode(BillProduct product)
        {
            this.BillProduct = product;
            this.Next = null;
        }

        public BillProduct GetBillProduct()
        {
            return this.BillProduct;
        }
        public BillProductNode? GetNext()
        {
            return this.Next;
        }

        public void SetBillProduct(BillProduct product)
        {
            BillProduct = product;
            BillProduct.UpdateQuantity(product.Quantity);
        }

        public void SetNext(BillProductNode? next)
        {
            Next = next;
        }
    }
    class Bill
    {
        public string ID { get; private set; }
        public Client Client { get; private set; }
        public BillProductNode? Head { get; private set; }
        public BillProductNode? Tail { get; private set; }
        private double Total;
        private int Size = 0;

        public Bill(Client client)
        {
            ID = "paralelepipedo";
            Client = client;
        }

        public BillProduct? Get(string code)
        {
            var act = Head;
            while (act != null)
            {
                if (act.BillProduct.Code == code)
                {
                    return act.BillProduct;
                }
                act = act.Next;
            }
            return null;
        }

        public void Add(BillProduct product)
        {   
            if(product.Quantity <= 0)
            {
                return;
            }
            var pp = Get(product.Code);
            if (pp != null) {
                product.UpdateQuantity(product.Quantity + pp.Quantity);
                Update(product);
                return;
            }

            var node = new BillProductNode(product);
            if (Head == null)
            {
                Head = node;
                Tail = node;
            }
            else
            {
                Tail.SetNext(node);
                Tail = Tail.Next;
            }
            Size++;
        }

        public BillProduct? Update(BillProduct product)
        {
            if(product.Quantity <= 0)
            {
                Delete(product.Code);
                return null;
            }
            var act = this.Head;
            while (act != null)
            {
                if (act.BillProduct == product)
                {
                    act.SetBillProduct(product);
                    return product;
                }
                act = act.Next;
            }

            return null;
        }

        public BillProduct? Delete(string code)
        {
            var act = Head;
            var prev = act;
            while (act != null)
            {
                if (act.BillProduct.Code == code)
                {
                    if (act.BillProduct == Head.BillProduct)
                    {
                        Head = Head.Next;
                        if (Head == null)
                        {
                            Tail = null;
                        }
                    }
                    else if (act.BillProduct == Tail.BillProduct)
                    {
                        Tail = prev;
                        Tail.SetNext(null);
                    }
                    else
                    {
                        prev.SetNext(act.Next);
                    }

                    Size--;
                    return act.BillProduct;
                }
                prev = act;
                act = act.Next;
            }

            return null;
        }

        public double GetTotal()
        {
            Total = 0;
            ForEach((p) => {
                Total += p.Price * p.Quantity;
            });

            return Total;
        }

        public void ForEach(Action<BillProduct> callback)
        {
            var act = Head;
            while (act != null)
            {
                callback(act.BillProduct);
                act = act.Next;
            }
        }

        public DataTable GetDataTable()
        {
            var dataTable = new DataTable();
            BillProduct.GenerateColumns(dataTable);

            ForEach((p) =>
            {
                p.ToRow(dataTable);
            });
            return dataTable;
        }
    }
}
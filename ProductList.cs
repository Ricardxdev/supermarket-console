namespace Supermarket
{
    using System.Collections.Generic;
    using System.Data;

    class ProductNode
    {
        public Product Product { get; private set; }
        public ProductNode? Next { get; private set; }

        public ProductNode(Product product)
        {
            this.Product = product;
            this.Next = null;
        }

        public Product GetProduct()
        {
            return this.Product;
        }
        public ProductNode? GetNext()
        {
            return this.Next;
        }

        public void SetProduct(Product product)
        {
            Product = product;
        }

        public void SetNext(ProductNode? next)
        {
            Next = next;
        }

        public void ShowProduct()
        {
            Console.WriteLine($"Name: {Product.Name}, Code: {Product.Code}, BuyPrice: {Product.BuyPrice}, SalePrice: {Product.SalePrice}, Category: {Product.Category}, Description: {Product.Description}");
        }

    }
    class ProductList
    {
        public ProductNode? Head { get; private set; }
        public ProductNode? Tail { get; private set; }
        private int Size = 0;

        public Product? Get(string code)
        {
            var act = Head;
            while (act != null)
            {
                if (act.Product.Code == code)
                {
                    return act.Product;
                }
                act = act.Next;
            }
            return null;
        }

        public void Add(Product product)
        {
            var node = new ProductNode(product);
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

        public Product? Update(Product product)
        {
            var act = this.Head;
            while (act != null)
            {
                if (act.Product == product)
                {
                    act.SetProduct(product);
                    return product;
                }
                act = act.Next;
            }

            return null;
        }

        public Product? Delete(string code)
        {
            var act = Head;
            var prev = act;
            while (act != null)
            {
                if (act.Product.Code == code)
                {
                    if (act.Product == Head.Product)
                    {
                        Head = Head.Next;
                        if (Head == null)
                        {
                            Tail = null;
                        }
                    }
                    else if (act.Product == Tail.Product)
                    {
                        Tail = prev;
                        Tail.SetNext(null);
                    }
                    else
                    {
                        prev.SetNext(act.Next);
                    }

                    Size--;
                    return act.Product;
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
                act.ShowProduct();
                act = act.Next;
            }
        }

        public void ForEach(Action<Product> callback)
        {
            var act = Head;
            while (act != null)
            {
                callback(act.Product);
                act = act.Next;
            }
        }

        public List<string> GetCategories()
        {
            HashSet<string> categories = new HashSet<string>();
            ForEach((p) => categories.Add(p.Category));

            return new List<string>(categories);
        }

        public ProductList GetProductsByCategory(ProductList list, string category) // TODO: Check functionality
        {
            ForEach(p =>
            {
                if (p.Category == category)
                {
                    list.Add(p);
                }
            });

            return list;
        }

        public DataTable GenerateDataTable()
        {
            var dataTable = new DataTable();
            Product.GenerateColumns(dataTable);
            return dataTable;
        }
        public DataTable FillDataTable(DataTable dataTable)
        {
            return FillDataTable(dataTable, "");
        }

        public DataTable FillDataTable(DataTable dataTable, string filterCode)
        {
            return FillDataTable(dataTable, filterCode, ""); ;
        }
        public DataTable FillDataTable(DataTable dataTable, string filterCode, string filterCategory)
        {
            var trinnedCode = filterCode.Trim();
            var trinnedCategory = filterCategory.Trim();
            ForEach((p) =>
            {
                if (!(trinnedCode == "" || p.Code.Contains(trinnedCode))) return;
                if (!(trinnedCategory == "" || trinnedCategory == "Ninguna" || p.Category.Contains(trinnedCategory))) return;
                p.ToRow(dataTable);
            });
            return dataTable;
        }

        public double GetAverageSalePrice(string category)
        {
            double total = 0;
            int count = 0;
            ForEach((p) =>
            {
                if (p.Category == category)
                {
                    total += p.SalePrice;
                    count++;
                }
            });

            return total / count;
        }

        public ProductNode? GetParent(ProductNode node)
        {
            var act = Head;
            while (act != null)
            {
                if (act.Next == node) return act;
                act = act.Next;
            }

            return null;
        }

        public void Swap(ProductNode prev, ProductNode act)
        {
            var parent = GetParent(prev);
            prev.SetNext(act.Next);
            act.SetNext(prev);
            if (parent == null)
            {
                Head = act;
            }
            else
            {
                parent.SetNext(act);
            }
            prev = act;
            act = act.Next;

            if (act.Next == null)
            {
                Tail = act;
            }
        }

        public bool CheckIsSorted(Func<ProductNode?, ProductNode?, bool> checkCallback)
        {
            var act = Head;
            var prev = act;
            while (act != null)
            {
                if (!checkCallback(prev, act))
                {
                    return false;
                }
                prev = act;
                act = act.Next;
            }

            return true;
        }

        public void Sort(Func<ProductNode?, ProductNode?, bool> checkCallback)
        {
            if (Head == null || Head.Next == null) return;
            var act = Head;
            var prev = (ProductNode?)null;
            while (!CheckIsSorted(checkCallback))
            {
                while (act != null)
                {
                    if (!checkCallback(prev, act))
                    {
                        Swap(prev, act);
                        act = Head;
                        prev = act;
                    }

                    prev = act;
                    act = act.Next;
                }
            }

        }

        public bool CheckIsSortedBySalePriceInDescendingOrder()
        {
            return CheckIsSorted((prev, act) =>
            {
                if (prev is null) return true;
                return prev.Product.SalePrice >= act.Product.SalePrice;
            });
        }

        public void SortBySalePriceInDescendingOrder()
        {
            Sort((prev, act) =>
            {
                if (prev is null) return true;
                return prev.Product.SalePrice >= act.Product.SalePrice;
            });

        }

        public bool CheckIsSortedBySalePriceInIncreasingOrder()
        {
            return CheckIsSorted((prev, act) =>
            {
                if (prev is null) return true;
                return prev.Product.SalePrice <= act.Product.SalePrice;
            });
        }

        public void SortBySalePriceInIncreasingOrder()
        {
            Sort((prev, act) =>
            {
                if (prev is null) return true;
                return prev.Product.SalePrice <= act.Product.SalePrice;
            });
        }

        public bool CheckIsSortedByProfitInDescendingOrder()
        {
            return CheckIsSorted((prev, act) =>
            {
                if (prev is null) return true;
                double ProfitOfPrev = prev.Product.SalePrice * 100 / prev.Product.BuyPrice;
                double ProfitOfAct = act.Product.SalePrice * 100 / act.Product.BuyPrice;

                return ProfitOfPrev >= ProfitOfAct;
            });
        }

        public void SortByProfitInDescendingOrder()
        {
            Sort((prev, act) =>
            {
                if (prev is null) return true;
                double ProfitOfPrev = prev.Product.SalePrice * 100 / prev.Product.BuyPrice;
                double ProfitOfAct = act.Product.SalePrice * 100 / act.Product.BuyPrice;

                return ProfitOfPrev >= ProfitOfAct;
            });
        }

        public bool CheckIsSortedByProfitInIncreasingOrder()
        {
            return CheckIsSorted((prev, act) =>
            {
                if (prev is null) return true;
                double ProfitOfPrev = prev.Product.SalePrice * 100 / prev.Product.BuyPrice;
                double ProfitOfAct = act.Product.SalePrice * 100 / act.Product.BuyPrice;

                return ProfitOfPrev <= ProfitOfAct;
            });
        }

        public void SortByProfitInIncreasingOrder()
        {
            Sort((prev, act) =>
            {
                if (prev is null) return true;
                double ProfitOfPrev = prev.Product.SalePrice * 100 / prev.Product.BuyPrice;
                double ProfitOfAct = act.Product.SalePrice * 100 / act.Product.BuyPrice;

                return ProfitOfPrev <= ProfitOfAct;
            });
        }
    }
}
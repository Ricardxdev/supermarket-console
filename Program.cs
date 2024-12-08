namespace Supermarket
{
    using Terminal.Gui;
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("--------- Products of .dat file ---------");
            var products = new ProductList();
            PreloadProducts(products);
            Console.WriteLine("------- Products of Linked List --------");
            products.ShowAll();

            Console.WriteLine("--------------------------------------------------------------------------");
            products.SortByProfitInDescendingOrder();
            products.ShowAll();

            Console.WriteLine("--------------------------------------------------------------------------");
            products.SortByProfitInIncreasingOrder();
            products.ShowAll();

            Console.WriteLine("--------------------------------------------------------------------------");
            products.SortBySalePriceInDescendingOrder();
            products.ShowAll();

            Console.WriteLine("--------------------------------------------------------------------------");
            products.SortBySalePriceInIncreasingOrder();
            products.ShowAll();

            // Console.WriteLine("--------- Clients of .dat file ---------");
            // var clients = new ClientList();
            // PreloadClients(clients);
            // Console.WriteLine("------- Clients of Linked List --------");
            // clients.ShowAll();

            //Console.WriteLine("------------------  Create Product  ---------------------------");
            //var product1 = new Product("AAA1111", "Torta", "es una torta bro", "postre", 10.34, 100);
            //var product2 = new Product("AAA1112", "Pan", "es un pan bro", "alimento", .863, 3);
            //var product3 = new Product("AAA1113", "Helado", "es un helado bro", "postre", 3, 7.5);
            //products.AddProduct(product1);
            //products.AddProduct(product2);
            //products.AddProduct(product3);
            //
            //products.ShowProducts();
            //
            //Console.WriteLine("------------------  Update Product  ---------------------------");
            //var product4 = new Product("AAA1113", "Helado tuneado", "es un helado tuneado bro", "postre tuneado", 3, 7.5);
            //products.UpdateProduct(product4);
            //products.ShowProducts();
            //
            //Console.WriteLine("------------------   Delete Product ---------------------------");
            //products.DeleteProduct("AAA1113");
            //products.ShowProducts();
            //
            //Console.WriteLine("--------------   Get Deleted Product    -----------------------");
            //Product? p = products.GetProduct("AAA1113");
            //if (p != null) {
            //    (new ProductNode(p)).ShowProduct();
            //} else {
            //    Console.WriteLine("Product 'AAA1113' Not Found");
            //}
            //
            //Console.WriteLine("------------------   Get Product    ---------------------------");
            //p = products.GetProduct("AAA1112");
            //if (p != null) {
            //    (new ProductNode(p)).ShowProduct();
            //} else {
            //    Console.WriteLine("Product 'AAA1112' Not Found");
            //}
            //
            //Console.WriteLine("--------------------------------------------------------------------------");
            //Console.WriteLine();
            //Console.WriteLine("--------------------------------------------------------------------------");
            //
            //
            //Console.WriteLine("------------------  Create Client    ---------------------------");
            //var clients = new ClientList();
            //var client1 = new Client("31668000", "Ricardo", "Martínez", "Av San Juan Bautista", "04123587038", 200);
            //var client2 = new Client("32495697", "Saúl", "García", "Valle Verde", "04148194531", 500);
            //var client3 = new Client("30346561", "Simón", "Aguillón", "Eco-Center", "04127950479", 5000.23);
            //clients.AddClient(client1);
            //clients.AddClient(client2);
            //clients.AddClient(client3);
            //
            //clients.ShowClients();
            //
            //Console.WriteLine("------------------  Update Client    ---------------------------");
            //var client4 = new Client("31668000", "Ricardo", "González", "Calle San José", "04123587038", 200);
            //clients.UpdateClient(client4);
            //clients.ShowClients();
            //
            //Console.WriteLine("------------------   Delete Client   ---------------------------");
            //clients.DeleteClient("31668000");
            //clients.ShowClients();
            //
            //Console.WriteLine("--------------   Get Deleted Client   --------------------------");
            //Client? c = clients.GetClient("31668000");
            //if (c != null) {
            //    new ClientNode(c).ShowClient();
            //} else {
            //    Console.WriteLine("Client '31668000' Not Found");
            //}
            //
            //Console.WriteLine("------------------   Get Client      ---------------------------");
            //c = clients.GetClient("30346561");
            //if (c != null) {
            //    new ClientNode(c).ShowClient();
            //} else {
            //    Console.WriteLine("Client '30346561' Not Found");
            //}
            //
            //Console.WriteLine("--------------------------------------------------------------------------");

            // Application.Init();

            // var window = new Window(){
            //     X = Pos.Center(),
            //     Y = Pos.Center(),
            //     Width = Dim.Fill(),
            //     Height = Dim.Fill()
            // };

            // var label = new Label("skldfjsdjklfklsdfjkskjdf")
            // {
            //     Y = Pos.Center(),
            //     X = Pos.Center(),
            // };

            // var button = new Button("Text")
            // {
            //     X = Pos.Center(),
            //     Y = Pos.Bottom(label),
            // };

            // window.Add(label, button);

            // Application.Top.Add(window);
            // Application.Run();
            // Console.Out.Flush();
        }

        public static void PreloadProducts(ProductList products) {
            foreach (string rawProduct in File.ReadLines("productos.dat"))
            {
                string[] items = rawProduct.Split(',');
                if (items.Length == 6) {
                    var product = new Product(
                        items[0].Trim(),
                        items[1].Trim(),
                        items[2].Trim(),
                        items[3].Trim(),
                        double.Parse(items[4]),
                        double.Parse(items[5])
                    );

                    products.Add(product);
                }
                Console.WriteLine(rawProduct);
            }
        }

        public static void PreloadClients(ClientList clients) {
            foreach (string rawClient in File.ReadLines("clientes.dat"))
            {
                string[] items = rawClient.Split(',');
                if (items.Length == 6) {
                    var client = new Client(
                        Int32.Parse(items[0]),
                        items[1].Trim(),
                        items[2].Trim(),
                        items[3].Trim(),
                        items[4],
                        double.Parse(items[5])
                    );

                    clients.Add(client);
                }
                Console.WriteLine(rawClient);
            }
        }
    }
}


// -> Búsqueda de los datos del cliente por cédula. 
// La factura debe reflejar la cédula, nombre y apellido del comprador
// Si el cliente no está registrado, se le debe registrar en el momento de la compra

// Se introduce el código del producto, se obtienen sus datos (nombre, precio unitario de venta), y se introduce la cantidad a comprar.
// El sistema debe calcular el subtotal por cada producto, el total a pagar y generar la factura.
// Es importante que cada factura tenga un número único y correlativo, comenzando en 0000 y aumentando secuencialmente a medida que se generan nuevas facturas. 
// Los productos deben tener un código alfanumérico compuesto por tres letras seguidas de cuatro números.

// Además, el sistema debe ofrecer opciones de estadísticas y reportes, tales como:

// Listado de productos por categoría.
// Listado de todos los productos ordenados por precio de venta unitario de menor a mayor.
// Listado de todos los productos ordenados de mayor a menor porcentaje de ganancia.
// Cliente con mayor y menor dinero facturado.
// Promedio de precios de venta por categoría.
// Promedio de dinero facturado.

// Ejemplo de una Factura
// Nº 0003
// C.I: 26123123
// Nombre: Luis
// Apellido: Pérez
// --------------------------------------------------------------------------------------
// Cod.
// Producto Nombre Precio Cantidad SubTotal
// Cod1281 Queso blanco 2 5 10
// Cod2211 Jamón de Pavo 4 6.5 26
// Cod1255 Crema Cero 3 5 15
// Cod9922 Vino Blanco 10 15 150
// ----------------------------------------------------------------------------------------
// Total ($) 201
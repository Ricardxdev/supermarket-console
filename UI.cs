namespace Supermarket
{
    using System;
    using System.Data;
    using Terminal.Gui;
    class SupermarketUI
    {
        ProductList Products;
        ClientList Clients;
        public SupermarketUI(ProductList products, ClientList clients)
        {
            Application.Init();

            Products = products;
            Clients = clients;

            //ShowProducts(products);
            //ShowClientForm();
            //OpenPrimaryWindow();

            ShowClients();

            Application.Run();


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
            // SupermarketUI.Out.Flush();
        }

        public void ShowProducts()
        {
            ShowProducts(Products);
        }

        public void ShowProducts(ProductList products)
        {
            Application.Top.RemoveAll();
            MenuBar();
            Application.Refresh();

            var form = new Window("Menu de Busqueda de Clientes por Identificacion.")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var productCodeLabel = new Label("|-- Ingrese el Codigo del Producto --|")
            {
                X = Pos.Center(),
                Y = 2
            };
            var productCodeField = new TextField("")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(productCodeLabel) + 1,
                Width = 40
            };

            // Create a submit button
            var submitButton = new Button("Buscar")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(productCodeField) + 2
            };

            var dataTable = products.GetDataTable();

            // Create TableView
            var tableView = new TableView()
            {
                Text = "Tabla de Productos",
                X = 0,
                Y = Pos.Bottom(submitButton) + 3,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                Table = dataTable
            };

            form.Add(productCodeLabel, productCodeField, submitButton, tableView);
            Application.Top.Add(form);

            // Handle button click event
            submitButton.Clicked += () =>
            {
                var inputCode = productCodeField.Text.ToString().Trim();
                tableView.Table = products.GetDataTable(inputCode);
                tableView.SetNeedsDisplay();
            
            };
            Application.Refresh();
        }

        public void ShowClients()
        {
            Application.Top.RemoveAll();
            MenuBar();
            Application.Refresh();

            var win = new Window("Tabla de Clientes")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var dataTable = Clients.GetDataTable();

            var tableView = new TableView
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                Table = dataTable
            };

            win.Add(tableView);
            Application.Top.Add(win);
            Application.Refresh();
        }

        public void MenuBar()
        {
            // Create a menu bar with items
            var menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem("_Aplicación", new MenuItem[]
            {
                new MenuItem("_Salir", "", () => Application.RequestStop())
            }),
            new MenuBarItem("_Compra", new MenuItem[]
            {
                new MenuItem("_Comprar", "", () => MessageBox.Query(50, 7, "About", "This is a Terminal.Gui app!", "OK"))
            }),
            new MenuBarItem("_Clientes", new MenuItem[]
            {
                new MenuItem("_Buscar Cliente", "", () =>
                {
                    SearchClient();
                }),
                new MenuItem("_Registrar Cliente", "", () =>
                {
                    ShowClientForm();
                }),
                new MenuItem("_Listar clientes", "", () =>
                {
                    ShowClients();
                })
            }),
            new MenuBarItem("_Productos", new MenuItem[]
            {
                new MenuItem("_Añadir producto", "", () =>
                {
                    ShowProductForm();
                }),
                new MenuItem("_Listado por categoría", "", () =>
                {
                    ProductList productsByCategories = new ProductList();
                    foreach (var category in Products.GetCategories()) {
                        Products.GetProductsByCategory(productsByCategories, category);
                    }
                    ShowProducts(productsByCategories);
                }),
                new MenuItem("_Listado por precio(Ascendente)", "", () =>
                {
                    Products.SortBySalePriceInIncreasingOrder();
                    ShowProducts();
                }),
                new MenuItem("_Listado por precio(Descendente)", "", () =>
                {
                    Products.SortBySalePriceInDescendingOrder();
                    ShowProducts();
                }),
                new MenuItem("_Listado por % de ganancia(Ascendente)", "", () =>
                {
                    Products.SortByProfitInIncreasingOrder();
                    ShowProducts();
                }),
                new MenuItem("_Listado por % de ganancia(Descendente)", "", () =>
                {
                    Products.SortBySalePriceInDescendingOrder();
                    ShowProducts();
                })
            }),
            new MenuBarItem("_Reportes", new MenuItem[]
            {
                new MenuItem("_Clientes", "", () =>
                {
                    MessageBox.Query(50, 7, "About", "This is a Terminal.Gui app!", "OK");
                }),
                new MenuItem("_Promedios", "", () =>
                {
                    MessageBox.Query(50, 7, "About", "This is a Terminal.Gui app!", "OK");
                })
            })
            });

            Application.Top.Add(menu);
        }

        public void TestMultipleWindows()
        {
            Application.Init();
            //MenuBar();

            // Create two windows
            var mainWindow = new Window("Main Window")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var secondWindow = new Window("Second Window")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Button to show the second window
            var showButton = new Button("Show Second Window")
            {
                X = Pos.Center(),
                Y = Pos.Center() - 1
            };

            showButton.Clicked += () =>
                {
                    Application.Top.Remove(mainWindow); // Hide main window
                    Application.Top.Add(secondWindow); // Show second window
                    Application.Refresh(); // Refresh UI
                };

            var formButton = new Button("Show Form Window")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(showButton) + 1
            };

            formButton.Clicked += () =>
                {
                    Application.Top.RemoveAll();
                    ShowClientForm();
                    Application.Refresh(); // Refresh UI
                };

            // Button to go back to the main window
            var backButton = new Button("Back to Main Window")
            {
                X = Pos.Center(),
                Y = Pos.Center() + 1
            };

            backButton.Clicked += () =>
                    {
                        Application.Top.Remove(secondWindow); // Hide second window
                        Application.Top.Add(mainWindow); // Show main window
                        Application.Refresh(); // Refresh UI
                    };

            mainWindow.Add(showButton, formButton);
            secondWindow.Add(backButton);

            Application.Top.Add(mainWindow);

            Application.Run();
        }

        public void SearchClient()
        {
            var top = Application.Top;
            top.RemoveAll();
            MenuBar();
            Application.Refresh();

            var form = new Window("Menu de Busqueda de Clientes por Identificacion.")
            {
                X = 0,
                Y = 1, // Leave space for menu bar if needed
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Create labels and text fields for username and password
            var clientIDLabel = new Label("|-- Ingrese la Cedula del Cliente --|")
            {
                X = Pos.Center(),
                Y = 2
            };
            var clientIDField = new TextField("")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(clientIDLabel) + 1,
                Width = 40
            };

            // Create a submit button
            var submitButton = new Button("Submit")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(clientIDField) + 2
            };

            // Handle button click event
            submitButton.Clicked += () =>
            {
                Client? client = Clients.Get(clientIDField.Text.ToString().Trim());
                if (client is not null)
                {
                    MessageBox.Query("Datos del Cliente", $"Nombre(s): {client.FirstName}\nApellido: {client.LastName}\nCedula: {client.License}\nTelefono: {client.Phone}\nDireccion: {client.Address}\nFacturado: {client.TotalBilled}", "OK");
                }
                else
                {
                    int choice = MessageBox.Query("Cliente no Encontrado", "Este Cliente no se encuentra registrado. Desea llenar el formulario de registro?", "Si", "No");
                    if (choice == 0)
                    {
                        ShowClientForm();
                    }
                }
            };

            // Add controls to the window
            form.Add(clientIDLabel, clientIDField, submitButton);

            // Add the window to the application and run it
            top.Add(form);
            Application.Refresh();
        }

        public void ShowClientForm()
        {
            var top = Application.Top;
            top.RemoveAll();
            MenuBar();
            Application.Refresh();

            // Create a main window (form)
            var form = new Window("Formulario de registro de cliente nuevo.")
            {
                X = 0,
                Y = 1, // Leave space for menu bar if needed
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Create labels and text fields for username and password
            var firstNameLabel = new Label("Nombre(s):  ")
            {
                X = 2,
                Y = 2
            };
            var firstNameField = new TextField("")
            {
                X = Pos.Right(firstNameLabel) + 1,
                Y = Pos.Top(firstNameLabel),
                Width = 40
            };

            var lastNameLabel = new Label("Apellido(s):")
            {
                X = 2,
                Y = Pos.Bottom(firstNameLabel) + 1
            };
            var lastNameField = new TextField("")
            {
                X = Pos.Right(lastNameLabel) + 1,
                Y = Pos.Top(lastNameLabel),
                Width = 40
            };

            var licenseLabel = new Label("Cédula:     ")
            {
                X = 2,
                Y = Pos.Bottom(lastNameLabel) + 1
            };
            var licenseField = new TextField("")
            {
                X = Pos.Right(licenseLabel) + 1,
                Y = Pos.Top(licenseLabel),
                Width = 40
            };

            var phoneLabel = new Label("Teléfono:   ")
            {
                X = 2,
                Y = Pos.Bottom(licenseLabel) + 1
            };
            var phoneField = new TextField("")
            {
                X = Pos.Right(phoneLabel) + 1,
                Y = Pos.Top(phoneLabel),
                Width = 40
            };

            var addressLabel = new Label("Dirección:  ")
            {
                X = 2,
                Y = Pos.Bottom(phoneLabel) + 1
            };
            var addressField = new TextField("")
            {
                X = Pos.Right(addressLabel) + 1,
                Y = Pos.Top(addressLabel),
                Width = 40
            };

            // Create a submit button
            var submitButton = new Button("Submit")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(addressField) + 2
            };

            // Handle button click event
            submitButton.Clicked += () =>
            {
                Client client = new Client(
                    licenseField.Text.ToString().Trim(),
                    firstNameField.Text.ToString().Trim(),
                    lastNameField.Text.ToString().Trim(),
                    addressField.Text.ToString().Trim(),
                    phoneField.Text.ToString().Trim(),
                    0
                );
                Clients.Add(client);
                MessageBox.Query("Registro Completado", $"Nombre: {firstNameField.Text}\nApellido: {lastNameField.Text}", "OK");
            };

            // Add controls to the window
            form.Add(firstNameLabel, firstNameField, lastNameLabel, lastNameField, licenseLabel, licenseField, phoneLabel, phoneField, addressLabel, addressField, submitButton);

            // Add the window to the application and run it
            top.Add(form);
            Application.Refresh();
        }

        public void ShowProductForm()
        {
            var top = Application.Top;
            top.RemoveAll();
            MenuBar();
            Application.Refresh();

            // Create a main window (form)
            var form = new Window("Formulario de registro de productos nuevos.")
            {
                X = 0,
                Y = 1, // Leave space for menu bar if needed
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Create labels and text fields for username and password
            var nameLabel = new Label("Nombre:     ")
            {
                X = 2,
                Y = 2
            };
            var nameField = new TextField("")
            {
                X = Pos.Right(nameLabel) + 1,
                Y = Pos.Top(nameLabel),
                Width = 40
            };

            var descriptionLabel = new Label("Descripcion:")
            {
                X = 2,
                Y = Pos.Bottom(nameLabel) + 1
            };
            var descriptionField = new TextField("")
            {
                X = Pos.Right(descriptionLabel) + 1,
                Y = Pos.Top(descriptionLabel),
                Width = 40
            };

            var categoryLabel = new Label("Categoria:  ")
            {
                X = 2,
                Y = Pos.Bottom(descriptionLabel) + 1
            };
            var categoryField = new TextField("")
            {
                X = Pos.Right(categoryLabel) + 1,
                Y = Pos.Top(categoryLabel),
                Width = 40
            };

            var buyPriceLabel = new Label("Costo:      ")
            {
                X = 2,
                Y = Pos.Bottom(categoryLabel) + 1
            };
            var buyPriceField = new TextField("")
            {
                X = Pos.Right(buyPriceLabel) + 1,
                Y = Pos.Top(buyPriceLabel),
                Width = 40
            };

            var salePriceLabel = new Label("P.V.P:      ")
            {
                X = 2,
                Y = Pos.Bottom(buyPriceLabel) + 1
            };
            var salePriceField = new TextField("")
            {
                X = Pos.Right(salePriceLabel) + 1,
                Y = Pos.Top(salePriceLabel),
                Width = 40
            };

            // Create a submit button
            var submitButton = new Button("Submit")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(salePriceField) + 2
            };

            // Handle button click event
            submitButton.Clicked += () =>
            {
                Product product = new Product(
                    "Cod" + GenerateRandomDigits(4),
                    nameField.Text.ToString().Trim(),
                    descriptionField.Text.ToString().Trim(),
                    categoryField.Text.ToString().Trim(),
                    Double.Parse(buyPriceField.Text.ToString().Trim()),
                    Double.Parse(salePriceField.Text.ToString().Trim())
                );
                Products.Add(product);
                MessageBox.Query(50, 7, "Producto Agregado", $"Nombre: {product.Name}\nCodigo: {product.Code}\nPrecio: {product.SalePrice}", "OK");
            };

            // Add controls to the window
            form.Add(nameLabel, nameField, descriptionLabel, descriptionField, categoryLabel, categoryField, buyPriceLabel, buyPriceField, salePriceLabel, salePriceField, submitButton);

            // Add the window to the application and run it
            top.Add(form);
            Application.Refresh();
        }

        static string GenerateRandomDigits(int length)
        {
            Random random = new Random();

            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                // Generate a random digit (0-9) and convert it to a character
                result[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(result);
        }

        public void OpenPrimaryWindow()
        {
            Application.Init();

            // Create the first window
            var mainWindow = new Window("Main Window")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var button = new Button("Go to Second Window")
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };

            button.Clicked += () =>
            {
                // Stop the current window and open the second one
                Application.RequestStop();
                OpenSecondWindow();
            };

            mainWindow.Add(button);
            Application.Top.Add(mainWindow);
            Application.Run();
        }

        public void OpenSecondWindow()
        {
            var secondWindow = new Window("Second Window")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var backButton = new Button("Back to Main Window")
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };

            backButton.Clicked += () =>
            {
                // Stop the second window and return to the main one
                Application.RequestStop();
                OpenPrimaryWindow();
            };

            secondWindow.Add(backButton);
            Application.Top.Add(secondWindow);
            Application.Run();
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
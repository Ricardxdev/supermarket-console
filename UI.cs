namespace Supermarket
{
    using System;
    using System.Data;
    using Terminal.Gui;
    class SupermarketUI
    {
        public enum States
        {
            SearchClient,
            ClientForm,
            Buy,
            Finished
        }
        public Bill? CurrentBill { get; private set; }
        public States BillState { get; private set; }
        ProductList Products;
        ClientList Clients;
        Toplevel Top = new Toplevel();
        public SupermarketUI(ProductList products, ClientList clients)
        {
            Application.Init();

            Products = products;
            Clients = clients;

            //ShowProducts(products);
            //ShowClientForm();
            //OpenPrimaryWindow();

            ShowClients();

            Application.Run(Top);


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

            // Top.Add(window);
            // Application.Run();
            // SupermarketUI.Out.Flush();
        }

        public void Buy()
        {
            if (CurrentBill == null)
            {
                BillState = States.SearchClient;
            }
            if (BillState == States.SearchClient)
            {
                SearchClient((IDField) =>
                {
                    Client? client = Clients.Get(IDField.Text.ToString().Trim());
                    if (client is not null)
                    {
                        CurrentBill = new Bill(client);
                        MessageBox.Query("Mensaje del Sistema", $"Bienvenid@, {client.FirstName} {client.LastName}", "OK");
                        ShowBuyScreen();
                    }
                    else
                    {
                        int choice = MessageBox.Query("Cliente no Encontrado", "Este Cliente no se encuentra registrado. Desea llenar el formulario de registro?", "Si", "No");
                        if (choice == 0)
                        {
                            BillState = States.ClientForm;
                            ShowClientFormFromBuy();
                        }
                    }
                });
            }

            if (BillState == States.ClientForm)
            {
                ShowClientFormFromBuy();
            }

            if (BillState == States.Buy)
            {
                ShowBuyScreen();
            }
        }

        public void ShowClientFormFromBuy()
        {
            ShowClientForm((licenseField, firstNameField, lastNameField, addressField, phoneField) =>
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
                CurrentBill = new Bill(client);
                MessageBox.Query("Registro Completado", $"Bienvenid@, {firstNameField.Text} {lastNameField.Text}", "OK");
                ShowBuyScreen();
            });
        }

        public void ShowBuyScreen()
        {
            BillState = States.Buy;
            Top.RemoveAll();
            MenuBar();

            var win = new Window("Menu de Compra.")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var client = CurrentBill.Client;

            var billIDLabel = new Label("Nro. Factura: ")
            {
                X = 10,
                Y = 2,
                Width = 40
            };
            billIDLabel.Text = billIDLabel.Text.ToString() + CurrentBill.ID;

            var clientIDLabel = new Label("C.I Cliente: ")
            {
                X = 10,
                Y = Pos.Bottom(billIDLabel) + 1,
                Width = 40
            };
            clientIDLabel.Text = clientIDLabel.Text.ToString() + client.License;

            var firstNameLabel = new Label("Nombre(s): ")
            {
                X = 10,
                Y = Pos.Bottom(clientIDLabel) + 1,
                Width = 40,
            };
            firstNameLabel.Text = firstNameLabel.Text.ToString() + client.FirstName;

            var lastNameLabel = new Label("Apellido(s): ")
            {
                X = 10,
                Y = Pos.Bottom(firstNameLabel) + 1,
                Width = 40
            };
            lastNameLabel.Text = lastNameLabel.Text.ToString() + client.LastName;

            var totalLabel = new Label("Total a Pagar: ")
            {
                X = 10,
                Y = Pos.Bottom(lastNameLabel) + 1,
                Width = 40
            };
            totalLabel.Text += CurrentBill.GetTotal().ToString();

            var addProductButton = new Button("Agregar Producto")
            {
                X = Pos.Percent(25) - 8,
                Y = Pos.Bottom(totalLabel) + 1,
                Width = 16
            };

            var buyButton = new Button("Pagar")
            {
                X = Pos.Percent(75) - 5,
                Y = Pos.Bottom(totalLabel) + 1,
                Width = 10
            };

            addProductButton.Clicked += () =>
            {
                ShowProducts((args, dataTable) =>
                {
                    int rowIndex = args.Row;
                    int colIndex = args.Col;

                    if (colIndex == 6)
                    {
                        var selectedRow = dataTable.Rows[rowIndex];
                        var code = selectedRow.Field<string>("Codigo");
                        var p = Products.Get(code);

                        string quantityInput = ShowInputDialog("Cantidad Requerida", "Ingrese la cantidad de productos a agregar:");
                        while (true)
                        {
                            if (!string.IsNullOrEmpty(quantityInput))
                            {
                                if (Int32.Parse(quantityInput) <= 0)
                                {
                                    quantityInput = ShowInputDialog("La Cantidad debe ser positiva", "Ingrese la cantidad de productos a agregar:");
                                }
                                else
                                {
                                    CurrentBill.Add(new BillProduct(Products.Get(code), Int32.Parse(quantityInput)));
                                    break;
                                }
                            }
                            else
                            {
                                MessageBox.Query("Mensaje", "No se ha ingresado una cantidad, se cancela la solicitud.", "OK");
                                break;
                            }
                        }


                        ShowBuyScreen();
                        Application.Refresh();
                    }
                });
            };

            buyButton.Clicked += () => {
                ShowBillDialog(CurrentBill);
            };

            var dataTable = CurrentBill.GetDataTable();

            var tableView = new TableView()
            {
                Text = "Productos a Pagar",
                X = 0,
                Y = Pos.Bottom(addProductButton) + 2,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                Table = dataTable
            };

            tableView.CellActivated += (args) =>
            {
                int rowIndex = args.Row;
                int colIndex = args.Col;
                var selectedRow = dataTable.Rows[rowIndex];
                var code = selectedRow.Field<string>("Codigo");

                BillProduct? bill;
                switch (colIndex)
                {
                    case 5:
                        bill = CurrentBill.Get(code);
                        bill.UpdateQuantity(bill.Quantity + 1);
                        CurrentBill.Update(bill);
                        break;
                    case 6:
                        bill = CurrentBill.Get(code);
                        bill.UpdateQuantity(bill.Quantity - 1);
                        CurrentBill.Update(bill);
                        break;
                    case 7:
                        CurrentBill.Delete(code);
                        break;
                }

                ShowBuyScreen();
                Application.Refresh();
            };

            win.Add(billIDLabel, clientIDLabel, firstNameLabel, lastNameLabel, totalLabel, addProductButton, buyButton, tableView);
            Top.Add(win);
            Application.Refresh();
        }

        public void ShowBillDialog(Bill bill)
        {
            var okButton = new Button("Pagar");
            okButton.Clicked += () => {
                bill.Client.Buy(bill.GetTotal());
                CurrentBill = null;
                BillState = States.SearchClient;
                Clients.Update(bill.Client);
                MessageBox.Query("Compra Finalizada", $"Gracias por su compra {bill.Client.FirstName}, Vuelva pronto.", "OK");
                Buy();
                Application.RequestStop();
            };

            var cancelButton = new Button("Cancelar");
            cancelButton.Clicked += () =>
                {
                    Application.RequestStop();
                };

            // Create the dialog
            var dialog = new Dialog("Confirmacion de Compra", 70, 16, okButton, cancelButton);

            // Add a label for the prompt
            var billCodeLabel = new Label("Nro. Factura: ")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill()
            };
            billCodeLabel.Text += bill.ID;

            var clientCodeLabel = new Label("C.I Cliente: ")
            {
                X = 1,
                Y = Pos.Bottom(billCodeLabel) + 1,
                Width = Dim.Fill()
            };
            clientCodeLabel.Text += bill.Client.License;

            var clientFirstNameLabel = new Label("Nombre(s): ")
            {
                X = 1,
                Y = Pos.Bottom(clientCodeLabel) + 1,
                Width = Dim.Fill()
            };
            clientFirstNameLabel.Text += bill.Client.FirstName;

            var clientLastNameLabel = new Label("Apellido(s): ")
            {
                X = 1,
                Y = Pos.Bottom(clientFirstNameLabel) + 1,
                Width = Dim.Fill()
            };
            clientLastNameLabel.Text += bill.Client.LastName;

            var totalLabel = new Label("Total a Pagar: ")
            {
                X = 1,
                Y = Pos.Bottom(clientLastNameLabel) + 1,
                Width = Dim.Fill()
            };
            totalLabel.Text += bill.GetTotal().ToString();

            dialog.Add(billCodeLabel, clientCodeLabel, clientFirstNameLabel, clientLastNameLabel, totalLabel);

            // Run the dialog modally
            Application.Run(dialog);

        }

        static string ShowInputDialog(string title, string prompt)
        {
            string result = null;

            // Create buttons for the dialog
            var okButton = new Button("OK");
            okButton.Clicked += () => Application.RequestStop();

            var cancelButton = new Button("Cancelar");
            cancelButton.Clicked += () =>
                {
                    result = null; // Clear result if canceled
                    Application.RequestStop();
                };

            // Create the dialog
            var dialog = new Dialog(title, 50, 10, okButton, cancelButton);

            // Add a label for the prompt
            var label = new Label(prompt)
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill()
            };

            // Add a TextField for input
            var textField = new TextField("")
            {
                X = 1,
                Y = Pos.Bottom(label) + 1,
                Width = Dim.Fill() - 2
            };

            dialog.Add(label, textField);

            // Run the dialog modally
            Application.Run(dialog);

            // Retrieve the entered text if OK was pressed
            if (okButton.HasFocus)
            {
                result = textField.Text.ToString();
            }

            return result;
        }

        public void ShowProducts()
        {
            ShowProducts(null);
        }

        public void ShowProducts(Action<TableView.CellActivatedEventArgs, DataTable> onBuy)
        {
            var dataTable = Products.GenerateDataTable();
            dataTable.Columns.Add("   ");
            Products.FillDataTable(dataTable);
            ShowProducts(Products, dataTable, (code, category) =>
            {
                dataTable.Clear();
                Products.FillDataTable(dataTable, code, category);

                return dataTable;
            }, onBuy);
        }

        public void ShowProducts(ProductList products, DataTable dataTable, Func<string, string, DataTable> onSubmit, Action<TableView.CellActivatedEventArgs, DataTable> onCellActivated)
        {
            Top.RemoveAll();
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
                X = Pos.Percent(25) - 19,
                Y = 2,
                Width = 40
            };
            var productCodeField = new TextField("")
            {
                X = Pos.Percent(25) - 20,
                Y = Pos.Bottom(productCodeLabel) + 1,
                Width = 40
            };

            var categoriesBoxLabel = new Label("|-- Filtrar Por --|")
            {
                X = Pos.Percent(75) - 10,
                Y = 2,
                Width = 40
            };

            var categoriesBox = new ComboBox()
            {
                X = Pos.Percent(75) - 15,
                Y = Pos.Bottom(categoriesBoxLabel) + 1,
                Width = 30,
                Height = 7
            };

            var source = new List<string>() { "Ninguna" };
            source.AddRange(products.GetCategories());
            categoriesBox.SetSource(source);

            var submitButton = new Button("Filtrar")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(productCodeField) + 2
            };

            var tableView = new TableView()
            {
                Text = "Tabla de Productos",
                X = 0,
                Y = Pos.Bottom(submitButton) + 3,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                Table = dataTable
            };

            form.Add(productCodeLabel, productCodeField, categoriesBoxLabel, categoriesBox, submitButton, tableView);
            Top.Add(form);

            submitButton.Clicked += () =>
            {
                var inputCode = productCodeField.Text.ToString();
                var inputCategory = categoriesBox.Text.ToString();
                tableView.Table = onSubmit(inputCode, inputCategory);
                tableView.SetNeedsDisplay();
            };

            tableView.CellActivated += (args) =>
            {
                onCellActivated(args, dataTable);
            };

            Application.Refresh();
        }

        public void ShowClients()
        {
            Top.RemoveAll();
            MenuBar();

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
            Top.Add(win);
            Application.Refresh();
        }

        public void MenuBar()
        {
            // Create a menu bar with items
            var menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem("_Aplicación", new MenuItem[]
            {
                new MenuItem("_Salir", "", () => {
                    Application.Shutdown();
                })
            }),
            new MenuBarItem("_Compra", new MenuItem[]
            {
                new MenuItem("_Comprar", "", () => Buy()),
                new MenuItem("_Cancelar Compra", "", () => {
                    CurrentBill = null;
                    BillState = States.SearchClient;
                    Buy();
                })
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
                    var dataTable = Products.GenerateDataTable();
                    Products.FillDataTable(dataTable);
                    ShowProducts(productsByCategories, dataTable,  (code, category) =>
                        {
                            dataTable.Clear();
                            Products.FillDataTable(dataTable, code, category);

                            return dataTable;
                        }, null);
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

            Top.Add(menu);
        }

        public void SearchClient(Action<TextField> onSubmit)
        {
            var top = Top;
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
                onSubmit(clientIDField);
            };

            // Add controls to the window
            form.Add(clientIDLabel, clientIDField, submitButton);

            // Add the window to the application and run it
            top.Add(form);
            Application.Refresh();
        }

        public void SearchClient()
        {
            SearchClient((IDField) =>
            {
                Client? client = Clients.Get(IDField.Text.ToString().Trim());
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
            });
        }

        public void ShowClientForm()
        {
            ShowClientForm((licenseField, firstNameField, lastNameField, addressField, phoneField) =>
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
            });
        }

        public void ShowClientForm(Action<TextField, TextField, TextField, TextField, TextField> onSubmit)
        {
            var top = Top;
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
                onSubmit(licenseField, firstNameField, lastNameField, addressField, phoneField);
            };

            // Add controls to the window
            form.Add(firstNameLabel, firstNameField, lastNameLabel, lastNameField, licenseLabel, licenseField, phoneLabel, phoneField, addressLabel, addressField, submitButton);

            // Add the window to the application and run it
            top.Add(form);
            Application.Refresh();
        }

        public void ShowProductForm()
        {
            var top = Top;
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

        public void NextState()
        {
            switch (BillState)
            {
                case States.SearchClient:
                    BillState = States.ClientForm;
                    break;
                case States.ClientForm:
                    BillState = States.Buy;
                    break;
                case States.Buy:
                    BillState = States.Finished;
                    break;
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
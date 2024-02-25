using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

class Producto
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int CantidadDisponible { get; set; }
}

class Usuario
{
    public string Nombre { get; set; }
    public string CorreoElectronico { get; set; }
    // Puedes agregar más atributos según sea necesario
}

class Inventario
{
    public event Action<string> NivelCriticoAlcanzado;

    public int CantidadDisponible;
    public int NivelCritico;

    public Inventario(int nivelCriticoInicial)
    {
        CantidadDisponible = 0; // Inicializas la cantidad a algún valor
        NivelCritico = nivelCriticoInicial;
    }

    public void ActualizarCantidad(int nuevaCantidad)
    {
        CantidadDisponible = nuevaCantidad;

        if (CantidadDisponible <= NivelCritico)
        {
            NivelCriticoAlcanzado?.Invoke($"¡Alerta! Nivel crítico alcanzado. Cantidad disponible: {CantidadDisponible}");
        }
    }
}

class GestorNotificaciones
{
    public void NotificarUsuario(string mensaje)
    {
        Console.WriteLine($"Notificación: {mensaje}");
        EnviarMensaje(mensaje);
    }

    public static void EnviarMensaje(string mensaje)
    {
        const string accountSid = "ACdad79eab612cd88cded23984d5e6be5b";
        const string authToken = "50fde90d5c11cf8f0b16f7b52d841085";
        const string twilioPhoneNumber = "+12293942404"; // Debes adquirir un número de teléfono Twilio para enviar mensajes
        const string toPhoneNumber = "+50230324209"; // El número al que enviarás el mensaje

        TwilioClient.Init(accountSid, authToken);

        var message = MessageResource.Create(
            body: "Se ha alcanzado el nivel crítico de inventario",
            from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
            to: new Twilio.Types.PhoneNumber(toPhoneNumber)
        );

        Console.WriteLine($"Mensaje enviado a {message.To}: {message.Sid}");
    }
}

class GestorProductos
{
    private List<Producto> productos;
    private string filePath;

    public GestorProductos()
    {
        productos = new List<Producto>();
        filePath = EncontrarRutaProductos();
        CargarProductosDesdeArchivo();
    }

    private string EncontrarRutaProductos()
    {
        // Ruta específica donde debería estar tu archivo
        string rutaArchivo = @"C:\Users\escal\source\repos\Lab_Delegate_MECG_AEEI\Lab_Delegate_MECG_AEEI\productos.txt";

        if (File.Exists(rutaArchivo))
        {
            Console.WriteLine($"La ruta completa del archivo de productos es: {rutaArchivo}");
            return rutaArchivo;
        }
        else
        {
            Console.WriteLine("No se encontró el archivo de productos. Se creará uno nuevo.");
            return rutaArchivo;
        }
    }

    public void AgregarProducto(Producto nuevoProducto)
    {
        productos.Add(nuevoProducto);
        GuardarProductosEnArchivo();
    }

    public void ConsultarProductos(string nombre)
    {
        var productosEncontrados = productos.Where(p => p.Nombre.Contains(nombre)).ToList();

        if (productosEncontrados.Any())
        {
            foreach (var producto in productosEncontrados)
            {
                Console.WriteLine($"Nombre: {producto.Nombre}, Descripción: {producto.Descripcion}, Precio: {producto.Precio}, Cantidad Disponible: {producto.CantidadDisponible}");
            }
        }
        else
        {
            Console.WriteLine("No se encontraron productos con el nombre especificado.");
        }
    }

    public Producto ObtenerProductoPorNombre(string nombre)
    {
        return productos.FirstOrDefault(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
    }

    private void CargarProductosDesdeArchivo()
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] data = line.Split(',');
                if (data.Length == 4)
                {
                    Producto producto = new Producto
                    {
                        Nombre = data[0],
                        Descripcion = data[1],
                        Precio = decimal.Parse(data[2], CultureInfo.InvariantCulture),
                        CantidadDisponible = int.Parse(data[3])
                    };
                    productos.Add(producto);
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("No se encontró el archivo de productos. Se creará uno nuevo.");
        }
    }

    public void GuardarProductosEnArchivo()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var producto in productos)
                {
                    writer.WriteLine($"{producto.Nombre},{producto.Descripcion},{producto.Precio},{producto.CantidadDisponible}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar productos en el archivo: {ex.Message}");
        }
    }
}

class Program
{
    static void Main()
    {
        int opcion;

        GestorProductos gestorProductos = new GestorProductos();
        Inventario inventario = new Inventario(5); // Nivel crítico inicial
        GestorNotificaciones gestorNotificaciones = new GestorNotificaciones();

        // Suscribir el gestor de notificaciones al evento de nivel crítico
        inventario.NivelCriticoAlcanzado += gestorNotificaciones.NotificarUsuario;

        do
        {
            Console.WriteLine("Menú de opciones:");
            Console.WriteLine("1. Registro de Productos");
            Console.WriteLine("2. Consulta de Productos");
            Console.WriteLine("3. Notificación de Niveles Críticos");
            Console.WriteLine("4. Gestión de Inventarios");
            Console.WriteLine("0. Salir");

            Console.Write("Ingrese el número de la opción deseada: ");

            if (int.TryParse(Console.ReadLine(), out opcion))
            {
                switch (opcion)
                {
                    case 1:
                        // Registro de Productos
                        Console.WriteLine("Seleccionaste Registro de Productos");
                        Console.Write("Ingrese el nombre del producto: ");
                        string nombreProducto = Console.ReadLine();
                        Console.Write("Ingrese la descripción del producto: ");
                        string descripcionProducto = Console.ReadLine();
                        Console.Write("Ingrese el precio del producto (usar '.' como separador decimal): ");

                        decimal precioProducto;
                        if (decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out precioProducto))
                        {
                            Console.Write("Ingrese la cantidad disponible del producto: ");
                            if (int.TryParse(Console.ReadLine(), out int cantidadProducto))
                            {
                                Producto nuevoProducto = new Producto
                                {
                                    Nombre = nombreProducto,
                                    Descripcion = descripcionProducto,
                                    Precio = precioProducto,
                                    CantidadDisponible = cantidadProducto
                                };

                                gestorProductos.AgregarProducto(nuevoProducto);
                            }
                            else
                            {
                                Console.WriteLine("Entrada no válida. Por favor, ingrese un número.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Entrada no válida. Por favor, ingrese un número decimal.");
                        }
                        break;

                    case 2:
                        // Consulta de Productos
                        Console.WriteLine("Seleccionaste Consulta de Productos");
                        Console.Write("Ingrese el nombre del producto a buscar: ");
                        string nombreConsulta = Console.ReadLine();
                        gestorProductos.ConsultarProductos(nombreConsulta);
                        break;

                    case 3:
                        // Notificación de Niveles Críticos
                        Console.WriteLine("Seleccionaste Notificación de Niveles Críticos");
                        Console.Write("Ingresa la nueva cantidad en el inventario: ");
                        if (int.TryParse(Console.ReadLine(), out int nuevaCantidad))
                        {
                            inventario.ActualizarCantidad(nuevaCantidad);

                            // Luego de actualizar la cantidad, verifica si es crítico y envía la notificación a Twilio
                            if (nuevaCantidad <= inventario.NivelCritico)
                            {
                                GestorNotificaciones.EnviarMensaje($"¡Alerta! Nivel crítico alcanzado. Cantidad disponible: {nuevaCantidad}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Entrada no válida. Por favor, ingresa un número.");
                        }
                        break;

                    case 4:
                        // Gestión de Inventarios
                        Console.WriteLine("Seleccionaste Gestión de Inventarios");
                        GestionarInventarios(gestorProductos, inventario);
                        break;

                    case 0:
                        Console.WriteLine("Saliendo del programa. ¡Hasta luego!");
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Por favor, ingrese un número válido.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Por favor, ingrese un número válido.");
            }

            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        } while (opcion != 0);
    }

    static void GestionarInventarios(GestorProductos gestorProductos, Inventario inventario)
    {
        Console.WriteLine("Opciones de Gestión de Inventarios:");
        Console.WriteLine("1. Realizar Compra");
        Console.WriteLine("2. Realizar Venta");
        Console.WriteLine("0. Volver al Menú Principal");

        int opcionInventarios;

        if (int.TryParse(Console.ReadLine(), out opcionInventarios))
        {
            switch (opcionInventarios)
            {
                case 1:
                    RealizarCompra(gestorProductos, inventario);
                    break;

                case 2:
                    RealizarVenta(gestorProductos, inventario);
                    break;

                case 0:
                    Console.WriteLine("Volviendo al Menú Principal.");
                    break;

                default:
                    Console.WriteLine("Opción no válida. Por favor, ingrese un número válido.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Por favor, ingrese un número válido.");
        }
    }

    static void RealizarCompra(GestorProductos gestorProductos, Inventario inventario)
    {
        Console.WriteLine("Realizando Compra:");

        Console.Write("Ingrese el nombre del producto a comprar: ");
        string nombreProductoCompra = Console.ReadLine();

        Console.Write("Ingrese la cantidad a comprar: ");
        if (int.TryParse(Console.ReadLine(), out int cantidadCompra) && cantidadCompra > 0)
        {
            Producto productoCompra = gestorProductos.ObtenerProductoPorNombre(nombreProductoCompra);
            if (productoCompra != null)
            {
                productoCompra.CantidadDisponible += cantidadCompra;
                gestorProductos.GuardarProductosEnArchivo();
                Console.WriteLine($"Compra realizada. Nueva cantidad disponible de {productoCompra.Nombre}: {productoCompra.CantidadDisponible}");
            }
            else
            {
                Console.WriteLine($"No se encontró el producto '{nombreProductoCompra}'.");
            }
        }
        else
        {
            Console.WriteLine("Cantidad no válida. Ingrese un número entero positivo.");
        }
    }

    static void RealizarVenta(GestorProductos gestorProductos, Inventario inventario)
    {
        Console.WriteLine("Realizando Venta:");

        Console.Write("Ingrese el nombre del producto a vender: ");
        string nombreProductoVenta = Console.ReadLine();

        Console.Write("Ingrese la cantidad a vender: ");
        if (int.TryParse(Console.ReadLine(), out int cantidadVenta) && cantidadVenta > 0)
        {
            Producto productoVenta = gestorProductos.ObtenerProductoPorNombre(nombreProductoVenta);
            if (productoVenta != null)
            {
                if (productoVenta.CantidadDisponible >= cantidadVenta)
                {
                    productoVenta.CantidadDisponible -= cantidadVenta;
                    gestorProductos.GuardarProductosEnArchivo();
                    Console.WriteLine($"Venta realizada. Nueva cantidad disponible de {productoVenta.Nombre}: {productoVenta.CantidadDisponible}");
                }
                else
                {
                    Console.WriteLine($"No hay suficientes unidades de '{nombreProductoVenta}' disponibles para realizar la venta.");
                }
            }
            else
            {
                Console.WriteLine($"No se encontró el producto '{nombreProductoVenta}'.");
            }
        }
        else
        {
            Console.WriteLine("Cantidad no válida. Ingrese un número entero positivo.");
        }
    }
}

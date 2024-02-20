using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

class Inventario
{
    public event Action<string> NivelCriticoAlcanzado;

    public int cantidadDisponible;
    public int NivelCritico;

    public Inventario(int nivelCriticoInicial)
    {
        cantidadDisponible = 0; // Inicializas la cantidad a algún valor
        NivelCritico = nivelCriticoInicial;
    }

    public void ActualizarCantidad(int nuevaCantidad)
    {
        cantidadDisponible = nuevaCantidad;

        if (cantidadDisponible <= NivelCritico)
        {
            NivelCriticoAlcanzado?.Invoke($"¡Alerta! Nivel crítico alcanzado. Cantidad disponible: {cantidadDisponible}");
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
            body: "Se ha alcanzado el nivel critico de inventario",
            from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
            to: new Twilio.Types.PhoneNumber(toPhoneNumber)
        );

        Console.WriteLine($"Mensaje enviado a {message.To}: {message.Sid}");
    }
}

class Program
{
    static void Main()
    {
        int opcion;

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
                        break;

                    case 2:
                        // Consulta de Productos
                        Console.WriteLine("Seleccionaste Consulta de Productos");
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
}

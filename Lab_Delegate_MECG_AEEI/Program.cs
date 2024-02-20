using System;

class Program
{
    static void Main()
    {
        int opcion;

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

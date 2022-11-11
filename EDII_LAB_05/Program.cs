using ARBOL_AVL;
using EDII_LAB_05.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using RSA;
using Formatting = Newtonsoft.Json.Formatting;
using System.Security.Cryptography.X509Certificates;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace EDII_LAB_05
{
    internal class Program
    {
        public static AVLTree<Ingreso> solicitante = new AVLTree<Ingreso>();

        static void Main(string[] args)
        {
            string ruta = "";
            Console.WriteLine("Ingrese la direccion de archvio");
            ruta = Console.ReadLine();

            var reader = new StreamReader(File.OpenRead(ruta));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var value = line.Split(';');
                if (value[0] == "INSERT")
                {
                    var data = JsonConvert.DeserializeObject<Solicitante>(value[1]);
                    Solicitante trabajar = data;
                    List<string> dupli = trabajar.companies.Distinct().ToList();
                    trabajar.companies = dupli;
                    List<Compania> companias = new List<Compania>();
                    for (int i = 0; i < trabajar.companies.Count; i++)
                    {
                        Compania comp = new Compania();
                        comp.Name = trabajar.companies[i];
                        comp.Libreria.Build(comp.Name + "/" + trabajar.dpi);
                        comp.dpicod = comp.Libreria.Encode(comp.Name + "/" + trabajar.dpi);
                        companias.Add(comp);
                    }
                    Ingreso ingreso = new Ingreso();
                    ingreso.name = trabajar.name;
                    ingreso.dpi = trabajar.dpi;
                    ingreso.address = trabajar.address;
                    ingreso.dateBirth = trabajar.dateBirth;
                    ingreso.companies = companias;
                    string dpicomp = ingreso.dpi;
                    ingreso.recluiter = trabajar.recluiter;
                    solicitante.insert(ingreso, ComparacioDPI);


                }
                else if (value[0] == "PATCH")
                {
                    var data = JsonConvert.DeserializeObject<Solicitante>(value[1]);
                    Solicitante trabajar = data;
                    Ingreso busqueda = new Ingreso();
                    busqueda.name = trabajar.name;
                    busqueda.dpi = trabajar.dpi;
                    if (solicitante.Search(busqueda, ComparacioDPI).name == trabajar.name)
                    {
                        if (trabajar.dateBirth != null)
                        {
                            solicitante.Search(busqueda, ComparacioDPI).dateBirth = trabajar.dateBirth;
                        }
                        if (trabajar.address != null)
                        {
                            solicitante.Search(busqueda, ComparacioDPI).address = trabajar.address;
                        }
                        if (trabajar.companies != null)
                        {
                            List<string> dupli = trabajar.companies.Distinct().ToList();
                            List<Compania> sindupli = new List<Compania>();
                            for (int i = 0; i < dupli.Count; i++)
                            {
                                Compania comp = new Compania();
                                comp.Name = dupli[i];
                                comp.Libreria.Build(comp.Name + "/" + trabajar.dpi);
                                comp.dpicod = comp.Libreria.Encode(comp.Name + "/" + trabajar.dpi);
                                sindupli.Add(comp);
                            }
                            solicitante.Search(busqueda, ComparacioDPI).companies = sindupli;
                        }

                    }
                }
                else if (value[0] == "DELETE")
                {
                    var data = JsonConvert.DeserializeObject<Solicitante>(value[1]);
                    Solicitante trabajar = data;
                    Ingreso ingreso = new Ingreso();
                    ingreso.dpi = trabajar.dpi;
                    List<Ingreso> trabajo = solicitante.getAll();
                    int cant = trabajo.Count();
                    for (int i = 0; i < trabajo.Count; i++)
                    {
                        if (trabajo[i].dpi == ingreso.dpi)
                        {
                            trabajo.RemoveAt(i);
                        }
                    }
                    solicitante = new AVLTree<Ingreso>();
                    int cant2 = trabajo.Count();
                    for (int j = 0; j < trabajo.Count; j++)
                    {
                        solicitante.insert(trabajo[j], ComparacioDPI);
                    }
                }
            }
            string dpi;
            List<string> Companias = new List<string>();
            string companiasreclu = "";
            Console.WriteLine("Escriba el DPI que desea buscar");
            dpi = Console.ReadLine();
            Ingreso solicitudesearch = new Ingreso();
            Ingreso solicitudend = new Ingreso();
            solicitudesearch.dpi = dpi;
            string volver = "Si";
            solicitudend = solicitante.Search(solicitudesearch, ComparacioDPI);
            string rutapublica = "C:\\Users\\Usuario\\OneDrive\\Escritorio\\Publicas";
            string rutaprivada = "C:\\Users\\Usuario\\OneDrive\\Escritorio\\Privadas";
            string rutarecluta = "C:\\Users\\Usuario\\OneDrive\\Escritorio\\Recluta";
            while (volver == "Si")
            {
                Console.WriteLine("RECLUTADOR");
                Console.WriteLine(solicitudend.recluiter);
                Console.WriteLine("COMPANIAS");
                for (int i = 0; i < solicitudend.companies.Count; i++)
                {
                    Console.WriteLine(solicitudend.companies[i].Name);
                }
                Console.WriteLine("Que desea realizar");
                Console.WriteLine("1.Crear llave");
                Console.WriteLine("2.Comprobar identidad");
                string accion = Console.ReadLine();
                string[] files = Directory.GetFiles(rutapublica);
                string[] files2 = Directory.GetFiles(rutaprivada);
                List<int> jprivada = new List<int>();
                List<int> publica = new List<int>();
                List<int> privada = new List<int>();
                string[] Generar = new string[15];
                if (accion == "1")
                {
                    Console.WriteLine("Para que empresa desea crear llave");
                    companiasreclu = Console.ReadLine();
                    string rutapublireclu = rutapublica + "\\" + solicitudend.recluiter + "_" + companiasreclu + ".txt";
                    string rutaprivatereclu = rutaprivada + "\\" + solicitudend.recluiter + "_" + companiasreclu + ".txt";
                    if (File.Exists(rutapublireclu))
                    {
                        Console.WriteLine("Llaves ya creadas");
                        Console.WriteLine("Desea volver Si/No");
                        volver = Console.ReadLine();
                    }
                    else
                    {
                        publica = RSA.RSA.GeneratePublicKey();
                        privada = RSA.RSA.GeneratePrivateKey(publica);
                        publica.RemoveAt(2);
                        string reclu = rutarecluta + "\\" + solicitudend.recluiter + ".txt";
                        string publico = JsonConvert.SerializeObject(publica);
                        string privado = JsonConvert.SerializeObject(privada);
                        Random random = new Random();
                        int valorde = 0;
                        for (int i = 0; i < publica[0]; i++)
                        {
                            int valorle = random.Next(65, 90);
                            int contador = 0;
                            do
                            {
                                contador = 0;
                                for (int j = 0; j < valorde; j++)
                                {
                                    string evaluar = Convert.ToChar(valorle).ToString();
                                    if (Generar[j] == evaluar)
                                    {
                                        contador++;
                                    }
                                }
                                if (contador > 0)
                                {
                                    valorle = random.Next(65, 90);
                                }
                            }
                            while (contador != 0);
                            char letra = Convert.ToChar(valorle);
                            string ingress = letra.ToString();
                            Generar[i] = ingress;
                            valorde++;
                        }
                        string vector = JsonConvert.SerializeObject(Generar);
                        try 
                        {
                            StreamWriter sw = new StreamWriter(rutapublireclu);
                            StreamWriter sw2 = new StreamWriter(rutaprivatereclu);
                            sw.WriteLine(publico);
                            sw2.WriteLine(privado);
                            sw.WriteLine(vector);
                            sw2.WriteLine(vector);
                            sw.Close();
                            sw2.Close();
                        }
                        catch (Exception e)
                        {
                        }
                        Console.WriteLine("Llaves ya creadas");
                        Console.WriteLine("Desea volver Si/No");
                        volver = Console.ReadLine();
                    }
                }
                else if (accion == "2")
                {
                    Console.WriteLine("Que empresa desea comprobar");
                    companiasreclu = Console.ReadLine();
                    Regex regex = new Regex(solicitudend.recluiter + "_" + companiasreclu + ".txt");
                    string[] valor = new string[15];
                    string[] valorprivada = new string[15];
                    foreach (string file2 in files2)
                    {
                        Match match = regex.Match(file2);
                        if (match.Success)
                        {
                            string line;
                            try 
                            {
                                StreamReader sr = new StreamReader(file2);
                                line = sr.ReadLine();
                                int cont = 0;
                                while (line != null) 
                                {
                                    if (cont == 0)
                                    {
                                        jprivada = JsonConvert.DeserializeObject<List<int>>(line);
                                        line = sr.ReadLine();
                                    }
                                    else 
                                    {
                                        valorprivada = JsonConvert.DeserializeObject<string[]>(line);
                                        line = sr.ReadLine();
                                    }
                                    cont++;
                                }
                                sr.Close();
                            }
                            catch 
                            { 
                            }
                        }
                    }
                    Console.WriteLine("Ingrese archivo con llave publica");
                    string rutallavepubli = Console.ReadLine();
                    string line2;
                    List<int> publicafinal = null;
                    try
                    {
                        StreamReader sr = new StreamReader(rutallavepubli);
                        line2 = sr.ReadLine();
                        int cont = 0;
                        while (line2 != null)
                        {
                            if (cont == 0)
                            {
                                publicafinal = JsonConvert.DeserializeObject<List<int>>(line2);
                                line2 = sr.ReadLine();
                                cont++;
                            }
                            else
                            {
                                valor = JsonConvert.DeserializeObject<string[]>(line2);
                                line2 = sr.ReadLine();
                            }
                        }
                        sr.Close();
                    }
                    catch
                    {
                    }
                    Dictionary<int, string> dict = new Dictionary<int, string>();
                    Dictionary<int, string> dict2 = new Dictionary<int, string>();
                    for (int i = 0; i < valor.Length; i++) 
                    {
                        dict.Add(i, valor[i]);
                    }
                    for (int i = 0; i < valorprivada.Length; i++) 
                    {
                        dict2.Add(i, valorprivada[i]);
                    }
                    Console.WriteLine("Ingrese mensaje");
                    string mensaje = Console.ReadLine();
                    int contador = 0;
                    char[] chars = mensaje.ToCharArray();
                    long[] comprobar = new long[chars.Length];
                    for (int i = 0; i < dict.Count; i++) 
                    {
                        for(int j = 0; j < mensaje.Length; j++) 
                        {
                            if (dict[i].Equals(chars[j].ToString())) 
                            {
                                contador++;
                            }
                        }
                    }
                    for (int i = 0; i < comprobar.Length; i++) 
                    {
                        for (int j = 0; j < dict.Count; j++)
                        {
                            if (dict[j].Equals(chars[i].ToString()))
                            {
                                comprobar[i] = j;
                            }
                        }
                    }
                    int conta2 = 0;
                    long [] cifrados = new long[mensaje.Length];
                    if (contador == mensaje.Length)
                    {
                        for (int i = 0; i < comprobar.Length; i++)
                        {
                            long comprobara = comprobar[i];
                            long cifrado = RSA.RSA.Cipher(publicafinal, comprobara);
                            cifrados[i] = cifrado;
                        }
                        long[] decifrados = new long[cifrados.Length];
                        for (int i = 0; i < cifrados.Length; i++) 
                        {
                            long cifrado = cifrados[i];
                            long decifrado = RSA.RSA.Decipher(jprivada, cifrado);
                            decifrados[i] = decifrado;
                        }
                        string[] valores = new string[decifrados.Length];
                        for (int i = 0; i < decifrados.Length; i++)
                        {
                            int valores3 = (int)decifrados[i];
                            valores[i] = dict2[valores3];
                            if (valores[i].Equals(chars[i].ToString())) 
                            {
                                conta2++;
                            }
                        }
                        if (conta2 == mensaje.Length)
                        {
                            Console.WriteLine("Identidad confirmada");
                        }
                        else
                        {
                            Console.WriteLine("Identidad no concordante");
                        }
                    }
                    else 
                    {
                        Console.WriteLine("Ingreso fuera de los parametros");
                    }
                }
                Console.WriteLine("Desea volver Si/No");
                volver = Console.ReadLine();
            }
            Console.ReadKey();
        }
        public static bool ComparacioDPI(Ingreso paciente, string operador, Ingreso paciente2)
        {
            int Comparacion = string.Compare(paciente.dpi, paciente2.dpi);
            if (operador == "<")
            {
                return Comparacion < 0;
            }
            else if (operador == ">")
            {
                return Comparacion > 0;
            }
            else if (operador == "==")
            {
                return Comparacion == 0;
            }
            else return false;
        }
        public static void Serializacion2(List<Ingreso> Lista, string path)
        {
            string solictanteJson = JsonConvert.SerializeObject(Lista.ToArray(), Formatting.Indented);
            File.WriteAllText(path, solictanteJson);
        }

    }
}

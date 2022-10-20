﻿using ARBOL_AVL;
using EDII_LAB_05.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

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
            //            string rutesave;
            Console.WriteLine("Escriba el DPI que desea buscar");
            dpi = Console.ReadLine();
            Ingreso solicitudesearch = new Ingreso();
            Ingreso solicitudend = new Ingreso();
            solicitudesearch.dpi = dpi;
            solicitudend = solicitante.Search(solicitudesearch, ComparacioDPI);
            //           Console.WriteLine("Escriba donde guardar el archivo");
            //           rutesave = Console.ReadLine();
            //           List<Ingreso> solicitantelist = new List<Ingreso>();
            //           solicitantelist.Add(solicitudend);
            //           Serializacion2(solicitantelist, rutesave);
            
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
